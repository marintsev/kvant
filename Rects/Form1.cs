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

        public Form1()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(Form1_MouseWheel);

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
        private void Redraw(int width, int height)
        {
            UpdateProject();
            if (t != null)
                t.Abort();
            t = null;
            t = new Thread(() =>
            {
                int green = new Random().Next(256);

                for (int z = 64; z >= 0; z /= 2)
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    var m = ConvertMatrix(Coords.Client, Coords.Model); //GetMatrix(width, height, z);
                    var b = scene.Draw(m, width, height, z);
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

        private void Redraw()
        {
            Redraw(ClientSize.Width, ClientSize.Height - menuStrip1.Height);
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

        private void AddRect(double x, double y, double w, double h, double l, Color color)
        {
            color = Color.FromArgb(127, 255, 255, 255);
            /*scene.Add(new RTRectangle(x - l / 2, y, l, h, color));
            scene.Add(new RTRectangle(x + w - l / 2, y, l, h, color));
            scene.Add(new RTRectangle(x, y - l / 2, w, l, color));
            scene.Add(new RTRectangle(x, y + h - l / 2, w, l, color));*/

            scene.Add(new RTRectangle(x, y, l, h, color));
            scene.Add(new RTRectangle(x + w-l, y, l, h, color));
            scene.Add(new RTRectangle(x, y, w, l, color));
            scene.Add(new RTRectangle(x, y + h-l, w, l, color));
        }

        private void AddCircles(double x, double y, double w, double h, Color circ_col, double cr, double sh)
        {
            scene.Add(new RaytraceableCircle(new Circle(x + sh, y + sh, cr, circ_col)));
            scene.Add(new RaytraceableCircle(new Circle(x + w - sh, y + sh, cr, circ_col)));
            scene.Add(new RaytraceableCircle(new Circle(x + w - sh, y + h - sh, cr, circ_col)));
            scene.Add(new RaytraceableCircle(new Circle(x + sh, y + h - sh, cr, circ_col)));
        }

        private void AddRectangle(double x, double y, double w, double h, Color color)
        {
            var r = new RTRectangle(x, y, w, h, color);
            scene.Add(r);
            double l = 0.01;

            double cr = l;
            Color circ_col = Color.FromArgb(127, 255, 255, 255);

            AddRect(x, y, w, h, l * 0.5, color);
            AddCircles(x, y, w, h, circ_col, cr, l/6);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var r = new Random();
            scene.Add(new RaytraceableGradient(0.5, 0.5, 0.5, 0.5));
            for (int i = 0; i < 50; i++)
            {
                /*var c = new Circle(r.NextDouble(), r.NextDouble(),
                    r.NextDouble() * 0.1, Color.FromArgb(
                    //255,
                    r.Next(256),
                    r.Next(256), r.Next(256), r.Next(256)
                    ));
                scene.Add(new RaytraceableCircle(c));*/
                AddRectangle(r.NextDouble(), r.NextDouble(),
                    r.NextDouble() * 0.2, r.NextDouble() * 0.2,
                    Color.FromArgb(
                    r.Next(256),
                    //255,
                    r.Next(256), r.Next(256), r.Next(256)
                    ));
            }
            UpdateProject();
            Redraw();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
                Redraw();
            else if (e.KeyChar == (char)Keys.Escape)
                Close();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
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

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //this.Boun
            //var p = PointToScreen(new System.Drawing.Point(e.X, e.Y));
            var pu = new Pointu(e.X, e.Y);
            var pp = Convert(pu.ToPoint(), Coords.Window, Coords.Model);
            var pc = Convert(pu.ToPoint(), Coords.Window, Coords.Uniform);
            var p = pu.ToPoint();
            //Text = string.Format("{0} -> {1}, {2}", p, pp, pc);
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
                throw new NotImplementedException();

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
    }


}
