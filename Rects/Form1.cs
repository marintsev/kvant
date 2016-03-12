﻿using System;
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
                var b = new Bitmap(width, height);
                int green = new Random().Next(256);
                var g = Graphics.FromImage(b);

                var sw = new Stopwatch();
                sw.Start();
                int x, y, z;
                z = 64;
                //z = 64;
                Raytracer rt = (cx, cy) =>
                {
                    var color = scene.GetColor(cx, cy);
                    /*if (color.A == 0)
                        return GetColor((double)cx, (double)cy, green);
                    else*/
                    return color;
                };
                for (z = 64; z >= 2; z /= 2)
                {
                    for (y = 0; y < height / z; y++)
                    {
                        double dy = y * z / (double)(height - 1);
                        for (x = 0; x < width / z; x++)
                        {
                            double dx = x * z / (double)(width - 1);
                            var br = new SolidBrush(rt(dx, dy));
                            g.FillRectangle(br, x * z, y * z, z, z);
                        }
                    }
                    g.Flush();
                    lock (painting)
                    {
                        background = (Image)b.Clone();
                    }
                    Invalidate();
                }

                lock (painting)
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            b.SetPixel(x, y, rt(x / (double)width, y / (double)height));
                        }
                    }

                    background = (Image)b.Clone();
                }
                Invalidate();
                sw.Stop();
                Debug.WriteLine("Draw: {0} ms", sw.ElapsedMilliseconds);


            });
            t.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (background == null)
                return;
            e.Graphics.DrawImageUnscaled(background, 0, 0);
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
            for (int i = 0; i < 100; i++)
            {
                var c = new Circle(r.NextDouble(), r.NextDouble(),
                    r.NextDouble() * 0.1, Color.FromArgb(
                    r.Next(256), r.Next(256), r.Next(256)
                    ));
                scene.Add(new RaytraceableCircle(c));
            }
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
