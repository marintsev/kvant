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
        private volatile Bitmap background = null;
        private Object painting = new Object();

        public Form1()
        {
            InitializeComponent();

        }

        private delegate Color Raytracer(double x, double y);

        private Color GetColor(double x, double y, int k)
        {
            return Color.FromArgb((int)(x * 255.0), k, (int)(y * 255.0));
        }

        private void Redraw(int width, int height)
        {
            t = new Thread(() =>
            {
                var b = new Bitmap(width, height);
                int green = new Random().Next(256);
                var g = Graphics.FromImage(b);

                var sw = new Stopwatch();
                sw.Start();
                int x, y, z;
                z = 64;
                //z = 64;
                for (z = 64; z >= 2; z /= 2)
                {
                    for (y = 0; y < height / z; y++)
                    {
                        double dy = y * z / (double)(height - 1);
                        for (x = 0; x < width / z; x++)
                        {
                            double dx = x * z / (double)(width - 1);
                            var br = new SolidBrush(GetColor(dx, dy, green));
                            g.FillRectangle(br, x * z, y * z, z, z);
                        }
                    }
                    g.Flush();
                    Thread.Sleep(200);
                    lock (painting)
                    {
                        background = b;
                    }
                    Invalidate();
                }

                /*lock (painting)
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            b.SetPixel(x, y, GetColor(x / (double)width, y / (double)height, green));
                        }
                    }

                    background = b;
                }
                Invalidate();*/
                sw.Stop();
                Debug.WriteLine("Draw: {0} ms", sw.ElapsedMilliseconds);


            });
            t.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            lock (painting)
            {
                if (background == null)
                    return;
                /*var sw = new Stopwatch();
                sw.Start();*/
                e.Graphics.DrawImageUnscaled(background, 0, 0);
                /*sw.Stop();
                Debug.WriteLine("Elapsed: {0}", sw.ElapsedMilliseconds);*/
            }
            //BackgroundImage
            //e.Graphics.DrawImage()
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            t.Abort();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Text = string.Format("{0} {1}", Width, Height);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Redraw(Width, Height);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
                Redraw(Width, Height);
        }
    }
}
