using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Rects
{
    public class Scene
    {
        private List<Raytraceable> objects = null;
        private int accuracy;

        private QuadTree quadtree = null;
        private BBox bbox = null;
        private bool isDirtyQuadTree = true;

        public QuadTree QuadTree { get { return GetQuadTree(); } }

        private Rects.QuadTree GetQuadTree()
        {
            if (quadtree == null || isDirtyQuadTree)
            {
                bbox = CalcBBox();
                quadtree = CalcQuadTree(bbox);
                Debug.WriteLine("BBox: {0}", bbox);
                isDirtyQuadTree = false;
            }
            return quadtree;
        }

        private BBox CalcBBox()
        {
            var bb = new BBox();
            foreach (var o in objects)
            {
                var bb_o = o.CalcBBox();
                if( !bb_o.IsInfinity() )
                    bb.Feed(bb_o);
            }
            return bb;
        }

        private Rects.QuadTree CalcQuadTree( BBox bb )
        {
            return CreateTree(bb);
        }

        public void SetDirty()
        {
            isDirtyQuadTree = true;
        }
        public int Objects { get { return objects.Count; } }

        int curz;

        public Scene(int accuracy_ = 1)
        {
            objects = new List<Raytraceable>();
            accuracy = accuracy_;

            quadtree = null;
            Clear();
        }

        public void Clear()
        {
            curz = 0;
        }
        public void Add(Raytraceable o)
        {
            o.Z = curz++;
            objects.Add(o);
            isDirtyQuadTree = true;
        }

        public Bitmap DrawSubpixel(QuadTree qt, Matrix33 m, int w, int h)
        {
            var list = new List<Raytraceable>();
            var b = new Bitmap(w, h);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    var p1 = new Pointu(x + 0.25, y + 0.25, 1).Multiply(m).ToPoint();
                    var p2 = new Pointu(x + 0.25, y + 0.75, 1).Multiply(m).ToPoint();
                    var p3 = new Pointu(x + 0.75, y + 0.25, 1).Multiply(m).ToPoint();
                    var p4 = new Pointu(x + 0.75, y + 0.75, 1).Multiply(m).ToPoint();
                    var c1 = GetColor(list, qt, p1);
                    var c2 = GetColor(list, qt, p2);
                    var c3 = GetColor(list, qt, p3);
                    var c4 = GetColor(list, qt, p4);
                    var c12 = ColorUtils.Blend(c1, c2, 0.5, 0.5);
                    var c34 = ColorUtils.Blend(c3, c4, 0.5, 0.5);
                    var c1234 = ColorUtils.Blend(c12, c34, 0.5, 0.5);
                    b.SetPixel(x, y, c1234);
                }
            }
            return b;
        }

        public QuadTree CreateTree(BBox bb)
        {
            var qt = new QuadTree(bb);
            int cnt = 0;
            int added = 0;
            foreach (var o in objects)
            {
                if (qt.Add(o) != QuadTree.Culled)
                    added++;
                cnt++;
            }
            Debug.WriteLine("QuadTree: {0}/{1}/{2}", qt, added, cnt);
            return qt;
        }
        public QuadTree CreateTree(Matrix33 m, int w, int h)
        {
            var p1 = (new Pointu(0, 0) * m).ToPoint();
            var p2 = (new Pointu(w, h) * m).ToPoint();
            var bb = new BBox(p1.x, p2.x, p2.y, p1.y);
            var qt = new QuadTree(bb);
            int cnt = 0;
            int added = 0;
            //foreach (var o in quadtree.Tracer(bb))
            foreach (var o in objects)
            {
                if (qt.Add(o) != QuadTree.Culled)
                    added++;
                cnt++;
            }
            Debug.WriteLine("QuadTree: {0}/{1}/{2}", qt, added, cnt);
            return qt;
        }

        public Bitmap DrawFast(QuadTree qt, BBox bbox, Matrix33 m, int w, int h, int z)
        {
            if (z < 1)
                z = 1;

            var hs = h / z;
            var ws = w / z;

            var sw = new Stopwatch();
            sw.Start();
            var b = new Bitmap(ws, hs);
            var g = Graphics.FromImage(b);
            m = m.Shrinked(z, z);
            foreach (var obj in qt.Tracer(bbox))
            {
                obj.DrawFast(g, m);
            }
            g.Flush();
            sw.Stop();
            Debug.WriteLine("DrawFast: {0} ms", sw.ElapsedMilliseconds);
            return b;
        }

        public Bitmap Draw(QuadTree qt, Matrix33 m, int w, int h, int z)
        {
            if (z == 0)
                return DrawSubpixel(qt, m, w, h);

            var hs = h / z;
            var ws = w / z;

            var b = new Bitmap(ws, hs);
            var list = new List<Raytraceable>();
            for (int y = 0; y < hs; y++)
            {
                for (int x = 0; x < ws; x++)
                {
                    var up = new Pointu(x * z, y * z, 1);
                    var p = up.Multiply(m).ToPoint();
                    var clr = GetColor(list, qt, p.x, p.y);
                    b.SetPixel(x, y, clr);
                }
            }
            return b;
        }

        public Color GetColor(List<Raytraceable> list, QuadTree qt, Point p)
        {
            return GetColor(list, qt, p.x, p.y);
        }

        //private IComparer<int> int_comparer = Comparer<int>.Create((_x, _y) => _y - _x);
        private IComparer<Raytraceable> rt_comparer = Comparer<Raytraceable>.Create((_x, _y) => _y.Z - _x.Z);
        private Comparison<Raytraceable> rtf_comparer = (_x, _y) => { return _y.Z - _x.Z; };

        public Color GetColor(List<Raytraceable> list, QuadTree qt, double x, double y)
        {
            var ray = new Raytraceable.Ray();
            ray.x = x;
            ray.y = y;
            ray.stop = false;
            ray.c = Color.Transparent;

            list.Clear();
            qt.Trace(list, new Point(x, y));
            try
            {
                list.Sort(rt_comparer);
            }
            catch (InvalidOperationException ioe)
            {
                throw ioe.GetBaseException();
            }

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
