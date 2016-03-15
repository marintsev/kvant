using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

/*
 * Создание нового изображения
 * Создание прямоугольника
 * Удаление прямоугольника
 * Изменение положения или размера прямоугольника
 * Отображении истории изменения изображения (список команд)
 * Отмена единичной команды (Undo)
 * Повторение единичной команды (Redo)
 * Отмена группы команд
 */

namespace Rects
{
    public partial class Form1 : Form
    {
        private Thread t;
        private volatile Image background = null;
        private Object painting = new Object();
        private Scene scene = new Scene();

        private Point center = new Point(0.5, 0.5);
        private Point mouse = new Point(0.5, 0.5);
        private double scale = 256;

        private Matrix33 mProject = Matrix33.Identity();
        private Matrix33 mUniformToClient = Matrix33.Identity();
        private Matrix33 mClientToWindow = Matrix33.Identity();

        private List<ObjRect> objects;

        public Form1()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(Form1_MouseWheel);

            objects = new List<ObjRect>();
        }

        private delegate Color Raytracer(double x, double y);

        private Matrix33 GetMatrix(int width, int height, int iz)
        {
            double z;
            if (iz == 0)
                z = 1;
            else
                z = iz;

            var mi = Matrix33.Identity();
            var mc = mi.Translated(-center);
            var ms = mi.Shrinked(width, height).Scaled(scale, scale);
            var mm = mi.Translated(mouse);
            return mm * ms * mc;
        }

        private Matrix33 GetMatrix()
        {
            return GetMatrix(ClientSize.Width, ClientSize.Height - menuStrip1.Height, 1);
        }

        private Matrix33 GetUniformToClientMatrix()
        {
            var mi = Matrix33.Identity();
            return mi.Scaled(ClientSize.Width, ClientSize.Height - menuStrip1.Height);
        }

        private Matrix33 GetClientToWindowMatrix()
        {
            var mi = Matrix33.Identity();
            return mi.Translated(0, menuStrip1.Height);
        }

        private void UpdateProject()
        {
            mClientToWindow = GetClientToWindowMatrix();
            mUniformToClient = GetUniformToClientMatrix();
            lock (painting)
            {
                mProject = GetMatrix();
            }
        }

        //private bool drawFast = true;
        private int paintingThreads;

        private void Redraw(int width, int height, bool fast = true)
        {
            if (width <= 0 || height <= 0)
                return;
            UpdateProject();
            // TODO: текущая отрисовка должна иногда завершаться
            if (t != null)
                t.Abort();
            lock (painting)
            {
                paintingThreads = 0;
            }
            t = null;
            t = new Thread(() =>
            {
                lock (painting)
                {
                    paintingThreads++;
                }
                int green = new Random().Next(256);

                var m = ConvertMatrix(Coords.Client, Coords.Model);
                //var qt = scene.QuadTree;

                if (fast)
                {
                    var im = m.Inverted();
                    var p1 = (new Pointu(0, 0) * m).ToPoint();
                    var p2 = (new Pointu(width, height) * m).ToPoint();
                    var bb = new BBox(p1.x, p2.x, p2.y, p1.y);
                    var b = scene.DrawFast(scene.QuadTree, bb, im, width, height, 1);
                    lock (painting)
                    {
                        background = b;
                    }
                    Invalidate();
                }
                else
                {
                    var qt = scene.CreateTree(m, width, height);
                    for (int z = 64; z >= 0; z /= 2)
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        var b = scene.Draw(qt, m, width, height, z);
                        //var b = scene.DrawFast(m.Inverted(), width, height, z);
                        sw.Stop();
                        Debug.WriteLine("Draw({0}): {1} ms", z, sw.ElapsedMilliseconds);
                        lock (painting)
                        {
                            background = b;
                        }
                        Invalidate();
                        if (z == 0)
                            break;
                    }
                }
                lock (painting)
                {
                    paintingThreads--;
                }

            });
            t.Start();

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (background == null)
                return;
            e.Graphics.DrawImage(background, 0, menuStrip1.Height, ClientSize.Width, ClientSize.Height - menuStrip1.Height);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            t.Abort();
        }

        private void Redraw(bool fast = true)
        {
            Redraw(ClientSize.Width, ClientSize.Height - menuStrip1.Height, fast);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //Text = string.Format("{0} {1}", Width, Height);
            Redraw();
        }

        private void AddTransparentPattern()
        {
            for (int i = 0; i < 3; i++)
            {
                scene.Add(new RaytraceableCircle(new Circle(0.45 - 0.3, 0.25 + i * 0.2, 0.1, Color.FromArgb(255 / (1 + i), 255, 0, 0))));
                scene.Add(new RaytraceableCircle(new Circle(0.55 - 0.3, 0.25 + i * 0.2, 0.1, Color.FromArgb(255 / (1 + i), 0, 255, 0))));

                scene.Add(new RaytraceableCircle(new Circle(0.45 + 0.3, 0.25 + i * 0.2, 0.1, Color.FromArgb(255 / (1 + i), 0, 255, 0))));
                scene.Add(new RaytraceableCircle(new Circle(0.55 + 0.3, 0.25 + i * 0.2, 0.1, Color.FromArgb(255 / (1 + i), 255, 0, 0))));
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            var r = new Random();
            for (int i = 0; i < 50; i++)
            {
                /*var c = new Circle(r.NextDouble(), r.NextDouble(),
                    r.NextDouble() * 0.1, Color.FromArgb(
                    //255,
                    r.Next(256),
                    r.Next(256), r.Next(256), r.Next(256)
                    ));
                scene.Add(new RaytraceableCircle(c));*/
                objects.Add(new ObjRect(
                    BBox.FromXYWH(r.NextDouble(), r.NextDouble(), r.NextDouble() * 0.2, r.NextDouble() * 0.2),
                    ColorUtils.Random( r)
                    ));
            }
            UpdateScene();
            UpdateProject();
            Redraw();
        }

        private void UpdateScene()
        {
            scene.Clear();
            scene.Add(new RaytraceableGradient(0.5, 0.5, 0.5, 0.5));
            foreach (var o in objects)
                scene.Add(o.EmitObjects());
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                Redraw(false);
            }
            else if (e.KeyChar == (char)Keys.Escape)
                Close();
        }



        public enum Coords
        {
            Client, Window, Uniform, Model
        };

        private bool Convert(ref Point what, Coords from, Coords to, Coords f, Coords t, Matrix33 m)
        {
            if (from == f && to == t)
            {
                what = (what.ToPointu() * m).ToPoint();
                return true;
            }
            else if (to == f && from == t)
            {
                what = (what.ToPointu() * m.Inverted()).ToPoint();
                return true;
            }
            return false;
        }

        private bool ConvertMatrix(ref Matrix33 ret, Coords from, Coords to, Coords f, Coords t, Matrix33 m)
        {
            if (from == f && to == t)
            {
                ret = m;
                return true;
            }
            else if (to == f && from == t)
            {
                ret = m.Inverted();
                return true;
            }
            return false;
        }

        private Matrix33 ConvertMatrix(Coords from, Coords to)
        {
            Matrix33 what = null;
            if (from == to)
                return Matrix33.Identity();
            if (ConvertMatrix(ref what, from, to, Coords.Client, Coords.Uniform, mUniformToClient.Inverted()))
                return what;
            if (ConvertMatrix(ref what, from, to, Coords.Client, Coords.Window, mClientToWindow))
                return what;
            if (ConvertMatrix(ref what, from, to, Coords.Model, Coords.Uniform, mProject))
                return what;
            if (ConvertMatrix(ref what, from, to, Coords.Client, Coords.Model, mProject.Inverted() * mUniformToClient.Inverted()))
                return what;
            if (ConvertMatrix(ref what, from, to, Coords.Uniform, Coords.Window, mClientToWindow * mUniformToClient))
                return what;
            if (ConvertMatrix(ref what, from, to, Coords.Window, Coords.Model, mProject.Inverted() * mUniformToClient.Inverted() * mClientToWindow.Inverted()))
                return what;
            throw new NotImplementedException();
        }

        private Point Convert(Point what, Coords from, Coords to)
        {
            if (from == to)
                return what;
            var m = ConvertMatrix(from, to);
            return (what.ToPointu() * m).ToPoint();
        }

        private Point pMouseHold = Point.Invalid;

        private ObjRect newRectangle = null;
        private Point pStartPoint = Point.Invalid;
        private Point pEndPoint = Point.Invalid;

        private Color clrCurrentRect = Color.FromArgb(127, 255, 255, 255);

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            // TODO: перемещение возможно и в других режимах
            var pMouse = new Point(e.X, e.Y);
            if (mode == Mode.Move)
            {
                pMouseHold = Convert(pMouse, Coords.Window, Coords.Model);
                SetMode(Mode.Move);
            }
            else if (mode == Mode.CreateRectangle)
            {
                pStartPoint = Convert(pMouse, Coords.Window, Coords.Model);
                newRectangle = new ObjRect(new BBox(pStartPoint.x, pStartPoint.x, pStartPoint.y, pStartPoint.y), clrCurrentRect );
                Debug.WriteLine("pStartPoint: {0}", pStartPoint);
                objects.Add(newRectangle);
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            var pu = new Pointu(e.X, e.Y);
            var pp = Convert(pu.ToPoint(), Coords.Window, Coords.Model);
            var pc = Convert(pu.ToPoint(), Coords.Window, Coords.Uniform);
            var p = pu.ToPoint();

            // TODO: status
            Text = string.Format(" Objects: {0}:{1}/{2}, {3}", scene.QuadTree.Count, scene.QuadTree.TotalCount, scene.Objects, pp);

            var pMouse = new Point(e.X, e.Y);
            if (mode == Mode.Move)
            {
                if (!pMouseHold.IsInvalid())
                {
                    var new_center = pMouseHold;
                    var new_mouse = Convert(pMouse, Coords.Window, Coords.Uniform);
                    Debug.WriteLine("New center: {0}", new_center);
                    Debug.WriteLine("New mouse: {0}", new_mouse);
                    lock (painting)
                    {
                        center = new_center;
                        mouse = new_mouse;
                    }
                    // Если идёт отрисовка, то после завершения начать новую.
                    if (paintingThreads == 0)
                        Redraw();
                }
            }
            else if (mode == Mode.CreateRectangle && newRectangle != null)
            {
                pEndPoint = Convert(pMouse, Coords.Window, Coords.Model);
                Debug.WriteLine("pEndPoint: {0}", pEndPoint);
                newRectangle.BBox = new BBox(pStartPoint, pEndPoint);
                UpdateScene();
                Redraw();
            }

            //Text = string.Format("{0} -> {1}, {2}", p, pp, pc);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mode == Mode.Move)
            {
                pMouseHold = Point.Invalid;
                SetLastMode();
            }
            else if (mode == Mode.CreateRectangle)
            {
                var pMouse = new Point(e.X, e.Y);
                pEndPoint = Convert(pMouse, Coords.Window, Coords.Model);
                Debug.WriteLine("pEndPoint: {0}", pEndPoint);
                newRectangle.BBox = new BBox(pStartPoint, pEndPoint);
                newRectangle = null;
                UpdateScene();
                Redraw();
            }
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            //Text = string.Format("{0} {1} {2}", e.X, e.Y - menuStrip1.Height, ClientRectangle.ToString());

            const double rate = 0.3;
            double x = e.Delta * rate / (double)SystemInformation.MouseWheelScrollDelta;
            double factor = (1 + Math.Abs(x));
            if (x < 0)
                factor = 1.0 / factor;
            if (e.Delta == 0)
                return;

            scale *= factor;
            var mp = new Point(e.X, e.Y);
            var new_center = Convert(mp, Coords.Window, Coords.Model);
            var new_mouse = Convert(mp, Coords.Window, Coords.Uniform);
            Debug.WriteLine("New center: {0}", new_center);
            Debug.WriteLine("New mouse: {0}", new_mouse);
            lock (painting)
            {
                center = new_center;
                mouse = new_mouse;
            }
            Redraw();
        }

        private void выделениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMode(Mode.Select);
        }

        private enum Mode { CreateRectangle, Move, Select, Scaling };
        private static Mode defaultMode = Mode.CreateRectangle;
        private Mode mode = defaultMode, lastMode = defaultMode;

        private void SetLastMode()
        {
            SetMode(lastMode);
        }
        private Cursor GetCursor(Mode mode)
        {
            switch (mode)
            {
                case Mode.CreateRectangle:
                    return Cursors.Cross;
                case Mode.Move:
                    return Cursors.SizeAll;
                case Mode.Select:
                    return Cursors.Arrow;
                case Mode.Scaling:
                    return Cursors.UpArrow;
            }
            throw new NotImplementedException();
        }

        private void SetCursor()
        {
            Cursor = GetCursor(mode);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void созданиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMode(Mode.CreateRectangle);
        }

        private void UpdateUI()
        {
            SetCursor();
        }

        private void SetMode(Mode mode_)
        {
            lastMode = mode;
            mode = mode_;
            Debug.WriteLine("{0} -> {1}", lastMode, mode_);
            UpdateUI();
        }
        private void навигацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMode(Mode.Move);
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            SetCursor();
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void масштабированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }


}
