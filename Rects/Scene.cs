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

        public Bitmap Draw(int w, int h, int z)
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

            var hs = h / z;
            var ws = w / z;
             
            var b = new Bitmap(ws, hs);
            //var g = Graphics.FromImage(b);
            for ( int y = 0; y < hs; y++)
            {
                //double dy = y * z / (double)(h - 1);
                double dy = y / (double)(hs - 1);
                for ( int x = 0; x < ws; x++)
                {
                    //double dx = x * z / (double)(w - 1);
                    double dx = x / (double)(ws - 1);
                    var clr = GetColor(dx, dy);
                    //var br = new SolidBrush();
                    b.SetPixel(x, y, clr);
                    //g.FillRectangle(br, x * z, y * z, z, z);
                }
            }
            //g.Flush();
            return b;

            /*
             for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            b.SetPixel(x, y, rt(x / (double)width, y / (double)height));
                        }
                    }
             */
        }

        public Color GetColor(double x, double y)
        {
            Color c = Color.Transparent;
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                var o = objects[i];
                if (o.Trace(x, y, ref c))
                    return c;
            }
            return c;
        }
    }
}
