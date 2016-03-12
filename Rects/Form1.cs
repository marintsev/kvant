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

        public Form1()
        {
            InitializeComponent();

        }

        private delegate Color Raytracer(double x, double y);

        private void Redraw(int width, int height)
        {
            if (t != null)
                t.Abort();
            t = null;
            t = new Thread(() =>
            {
                int green = new Random().Next(256);

                
                for (int z = 64; z >= 1; z /= 2)
                {
                    var sw = new Stopwatch();
                    sw.Start();     
                    var b = scene.Draw(width, height, z);
                    sw.Stop();
                    Debug.WriteLine("Draw({0}): {1} ms", z, sw.ElapsedMilliseconds);
                    lock (painting)
                    {
                        background = b;
                    }
                    Invalidate();
                    
                }
                
            });
            t.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (background == null)
                return;
            e.Graphics.DrawImage(background, 0, 0, Width, Height);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            t.Abort();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Text = string.Format("{0} {1}", Width, Height);
            Redraw(Width, Height);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var r = new Random();
            scene.Add(new RaytraceableGradient());

            for (int i = 0; i < 3; i++)
            {
                scene.Add(new RaytraceableCircle(new Circle(0.45-0.3, 0.25+i*0.2, 0.1, Color.FromArgb(255/(1+i), 255, 0, 0))));
                scene.Add(new RaytraceableCircle(new Circle(0.55-0.3, 0.25 + i * 0.2, 0.1, Color.FromArgb(255/(1+i), 0, 255, 0))));

                scene.Add(new RaytraceableCircle(new Circle(0.45 + 0.3, 0.25 + i * 0.2, 0.1, Color.FromArgb(255 / (1 + i), 0, 255, 0))));
                scene.Add(new RaytraceableCircle(new Circle(0.55 + 0.3, 0.25 + i * 0.2, 0.1, Color.FromArgb(255 / (1 + i), 255, 0, 0))));
            }

            //scene.Add(new RaytraceableCircle(new Circle(0.25, 0.75, 0.33, Color.FromArgb(127, 0, 255, 0))));
            //scene.Add(new RaytraceableCircle(new Circle(0.75, 0.75, 0.33, Color.FromArgb(127, 255, 0, 0))));

            /*for (int i = 0; i < 100; i++)
            {
                var c = new Circle(r.NextDouble(), r.NextDouble(),
                    r.NextDouble() * 0.1, Color.FromArgb(
                    r.Next(256),
                    r.Next(256), r.Next(256), r.Next(256)
                    ));
                scene.Add(new RaytraceableCircle(c));
            }*/
            Redraw(Width, Height);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
                Redraw(Width, Height);
            else if (e.KeyChar == (char)Keys.Escape)
                Close();
        }
    }
}
