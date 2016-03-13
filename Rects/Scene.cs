using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public class Scene
    {


        private List<Raytraceable> objects = null;

        public Scene()
        {
            objects = new List<Raytraceable>();
        }

        public void Add(Raytraceable o)
        {
            objects.Add(o);
        }

        public Bitmap DrawSubpixel(Matrix33 m, int w, int h)
        {
            var b = new Bitmap(w, h);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    var p1 = new Pointu(x + 0.25, y + 0.25, 1).Multiply(m).ToPoint();
                    var p2 = new Pointu(x + 0.25, y + 0.75, 1).Multiply(m).ToPoint();
                    var p3 = new Pointu(x + 0.75, y + 0.25, 1).Multiply(m).ToPoint();
                    var p4 = new Pointu(x + 0.75, y + 0.75, 1).Multiply(m).ToPoint();
                    var c1 = GetColor(p1);
                    var c2 = GetColor(p2);
                    var c3 = GetColor(p3);
                    var c4 = GetColor(p4);
                    var c12 = ColorUtils.Blend(c1, c2, 0.5, 0.5);
                    var c34 = ColorUtils.Blend(c3, c4, 0.5, 0.5);
                    var c1234 = ColorUtils.Blend(c12, c34, 0.5, 0.5);
                    b.SetPixel(x, y, c1234);
                }
            }
            return b;
        }

        public Bitmap Draw(Matrix33 m, int w, int h, int z)
        {
            // Примерная зависимость:
            //  z   время в мс
            //  64	4
            //  32	3
            //  16	9
            //  8	20
            //  4	73
            //  2	299
            //  1	1108

            //Draw(64): 13 ms
            //Draw(32): 27 ms
            //Draw(16): 60 ms
            //Draw(8): 176 ms
            //Draw(4): 633 ms
            //Draw(2): 2541 ms
            //Draw(1): 10010 ms

            if (z == 0)
            {
                return DrawSubpixel(m, w, h);
            }

            var hs = h / z;
            var ws = w / z;

            var b = new Bitmap(ws, hs);
            for (int y = 0; y < hs; y++)
            {
                for (int x = 0; x < ws; x++)
                {
                    var up = new Pointu(x*z, y*z, 1);
                    var p = up.Multiply(m).ToPoint();
                    var clr = GetColor(p.x, p.y);
                    b.SetPixel(x, y, clr);
                }
            }
            return b;
        }

        public Color GetColor(Point p)
        {
            return GetColor(p.x, p.y);
        }

        public Color GetColor(double x, double y)
        {
            Raytraceable.Ray ray = new Raytraceable.Ray();
            ray.stop = false;
            ray.x = x;
            ray.y = y;
            ray.c = Color.Transparent;

            for (int i = objects.Count - 1; i >= 0; i--)
            {
                var o = objects[i];
                if (o.Trace(ref ray))
                {
                    double f = ray.c.A / 255.0;
                    ray.c = ColorUtils.Blend(ray.c, ray.add, f, 1 - f);
                    if (ray.c.A == 255)
                    {
                        ray.stop = true;
                        return ray.c;
                    }
                }
            }
            return ray.c;
        }
    }
}
