using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Rects
{
    public class Scene
    {
        private List<Raytraceable> objects = null;
        private int accuracy;
        private QuadTree quadtree;

        public QuadTree QuadTree { get { return quadtree; } }
        public int Objects { get { return objects.Count; } }

        int curz;

        public Scene(int accuracy_ = 1)
        {
            objects = new List<Raytraceable>();
            accuracy = accuracy_;

            quadtree = new QuadTree(new BBox(0, 1, 1, 0));
            Clear();
        }

        public void Clear()
        {
            curz = 0;
        }
        public void Add(Raytraceable o)
        {
            o.Z = curz++;
            quadtree.Add(o);
            objects.Add(o);
        }

        public Bitmap DrawSubpixel(QuadTree qt, Matrix33 m, int w, int h)
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
                    var c1 = GetColor(qt, p1);
                    var c2 = GetColor(qt, p2);
                    var c3 = GetColor(qt, p3);
                    var c4 = GetColor(qt, p4);
                    var c12 = ColorUtils.Blend(c1, c2, 0.5, 0.5);
                    var c34 = ColorUtils.Blend(c3, c4, 0.5, 0.5);
                    var c1234 = ColorUtils.Blend(c12, c34, 0.5, 0.5);
                    b.SetPixel(x, y, c1234);
                }
            }
            return b;
        }

        public QuadTree CreateTree( Matrix33 m, int w, int h )
        {
            var p1 = (new Pointu(0, 0) * m).ToPoint();
            var p2 = (new Pointu(w, h) * m).ToPoint();
            var bb = new BBox(p1.x, p2.x, p2.y, p1.y);
            var qt = new QuadTree(bb);
            int cnt = 0;
            int added = 0;
            //foreach (var o in quadtree.Tracer(bb))
            foreach (var o in objects )
            {
                if (qt.Add(o) != QuadTree.Culled)
                    added++;
                cnt++;
            }
            Debug.WriteLine("QuadTree: {0}/{1}/{2}", qt, added, cnt);
            return qt;
        }

        public Bitmap Draw(QuadTree qt, Matrix33 m, int w, int h, int z)
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
                return DrawSubpixel(qt, m, w, h);
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
                    var clr = GetColor(qt, p.x, p.y);
                    b.SetPixel(x, y, clr);
                }
            }
            return b;
        }

        public Color GetColor(QuadTree qt, Point p)
        {
            return GetColor(qt, p.x, p.y);
        }

        public Color GetColor(QuadTree qt, double x, double y)
        {
            var ray = new Raytraceable.Ray();
            ray.x = x;
            ray.y = y;
            ray.stop = false;
            ray.c = Color.Transparent;
            var list = new List<Raytraceable>();
            qt.Trace(ref list, new Point(x, y));
            list.Sort((_x, _y) => { return _y.Z - _x.Z; });
            foreach (var obj in list)
            {
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
