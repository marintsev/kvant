using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Rects
{
    public class QuadTree
    {
        public struct Ray
        {
            public double x, y;
            public List<QuadTree> path;
            public int index;
            public Raytraceable obj;
            public Color c, add;
            public bool stop;
        }

        private QuadTree parent;
        private BBox bbox;
        private QuadTree a, b, c, d;
        private List<Raytraceable> objects;

        public QuadTree(BBox bbox_, QuadTree parent_ = null)
        {
            parent = parent_;
            bbox = bbox_;
            a = b = c = d = null;
            objects = null;
        }

        public void AddToList(ref List<Raytraceable> list, Raytraceable o)
        {
            if (list == null)
                list = new List<Raytraceable>();
            list.Add(o);
            Debug.WriteLine("Добавляем объект {0}...", o);
        }

        public QuadTree PeekSubtree(int i)
        {
            switch (i)
            {
                case 0:
                    if (a == null)
                    {
                        a = new QuadTree(new BBox(bbox.Left, bbox.CenterX, bbox.Top, bbox.CenterY), this);
                    }
                    return a;
                case 1:
                    if (b == null)
                    {
                        b = new QuadTree(new BBox(bbox.CenterX, bbox.Right, bbox.Top, bbox.CenterY), this);
                    }
                    return b;
                case 2:
                    if (c == null)
                    {
                        c = new QuadTree(new BBox(bbox.Left, bbox.CenterX, bbox.CenterY, bbox.Bottom), this);
                    }
                    return c;
                case 3:
                    if (d == null)
                    {
                        d = new QuadTree(new BBox(bbox.CenterX, bbox.Right, bbox.CenterY, bbox.Bottom), this);
                    }
                    return d;
            }
            throw new ArgumentException();
        }
        public bool Add(Raytraceable o)
        {
            if (o.IsInsideOf(bbox))
            {
                for (int i = 0; i < 4; i++)
                {
                    var sub = PeekSubtree(i);
                    if (o.IsInsideOf(sub.bbox))
                    {
                        AddToList(ref sub.objects, o);
                        return true;
                    }
                }
                AddToList(ref objects, o);
                return false;
            }
            else if( o.IsCross(bbox))
            {
                AddToList(ref objects, o);
                return false;
            }
            return false;
            //throw new NotImplementedException();
        }

        public static bool IsInside(QuadTree qt, Point p)
        {
            if (qt == null)
                return false;
            return qt.bbox.IsInside(p);
        }

        public void Trace(ref List<Raytraceable> list, Point p)
        {
            if (parent == null || (objects != null && bbox.IsInside(p)))
            {
                list.AddRange(objects);
            }
            if (IsInside(a, p))
                a.Trace(ref list, p);
            else if (IsInside(b, p))
                b.Trace(ref list, p);
            else if (IsInside(c, p))
                c.Trace(ref list, p);
            else if (IsInside(d, p))
                d.Trace(ref list, p);

            /*if (a != null)
                a.Trace(ref list, p);
            if (b != null)
                b.Trace(ref list, p);
            if (c != null)
                c.Trace(ref list, p);
            if (d != null)
                d.Trace(ref list, p);*/

        }
    }
}
