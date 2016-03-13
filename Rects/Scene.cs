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
        //private List<Raytraceable> objects = null;
        private int accuracy;
        private QuadTree quadtree;

        public Scene(int accuracy_ = 1)
        {
            //objects = new List<Raytraceable>();
            accuracy = accuracy_;

            quadtree = new QuadTree(new BBox(0, 1, 1, 0));
        }

        public void Add(Raytraceable o)
        {
            quadtree.Add(o);
            //objects.Add(o);
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
                    var up = new Pointu(x * z, y * z, 1);
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
            /*var ray = new QuadTree.Ray();
            ray.stop = false;
            ray.x = x;
            ray.y = y;
            ray.path = null;
            ray.obj = null;
            ray.c = Color.Transparent;*/

            var objs = new List<Raytraceable>();
            quadtree.Trace(ref objs, new Point(x, y));
            var ray = new Raytraceable.Ray();
            ray.x = x;
            ray.y = y;
            ray.stop = false;
            ray.c = Color.Transparent;
            //for (int i = 0; i < objs.Count - 1; i++)
            for( int i=objs.Count-1; i>=0; i-- )
            {
                var obj = objs[i];
                if (obj.Trace(ref ray))
                {
                    ray.c = ColorUtils.Blend(ray.c, ray.add);
                    if (255 - ray.c.A < accuracy)
                        ray.stop = true;
                }
                if (ray.stop)
                    return ray.c;
            }
            return ray.c;
        }
    }
}
