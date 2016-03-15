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

        public BBox BBox { get { return bbox; } }

        public int Count
        {
            get
            {
                if (objects != null)
                    return objects.Count;
                else
                    return 0;
            }
        }

        public int TotalCount
        {
            get
            {
                int x = Count;
                x += a != null ? a.TotalCount : 0;
                x += b != null ? b.TotalCount : 0;
                x += c != null ? c.TotalCount : 0;
                x += d != null ? d.TotalCount : 0;
                return x;
            }
        }

        public bool HaveObject(Raytraceable o)
        {
            return objects.Find((x) => x == o) != null;
        }

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
            //Debug.WriteLine("Добавляем объект {0}...", o);
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

        public const int Culled = 0;
        private const int AddedInside = 1;
        private const int AddedIntersect = 2;

        public int AddInside(Raytraceable o)
        {
            for (int i = 0; i < 4; i++)
            {
                var sub = PeekSubtree(i);
                if (o.IsInsideOf(sub.bbox))
                {
                    var code = sub.AddInside(o);
                    // Поместился целиком в одной из четвертей.
                    return AddedInside;
                }
            }
            // Не помещается ни в одной из четвертей, добавляем.
            AddToList(ref objects, o);
            return AddedInside;
        }

        public int Add(Raytraceable o, bool res = false)
        {
            // Если лежит целиком, то добавляем в наименьшую четверть, куда помещается объект
            if (o.IsInsideOf(bbox))
            {
                return AddInside(o);
            }
            // Если пересекает
            else if (o.IsCross(bbox))
            {
                // то добавляем
                AddToList(ref objects, o);
                return AddedIntersect;
            }
            else
            {
                // Лежит целиком за пределами
                return Culled;
            }
        }

        public static bool IsInside(QuadTree qt, Point p)
        {
            if (qt == null)
                return false;
            return qt.bbox.IsInside(p);
        }

        public static bool IsInside(QuadTree qt, BBox bb)
        {
            if (qt == null)
                return false;
            return qt.bbox.Intersects(bb);
        }

        public IEnumerable<Raytraceable> Tracer(BBox bb)
        {
            if (objects != null)
            {
                //if (parent == null || bbox.Intersects(bb))
                    foreach (var o in objects)
                        yield return o;
            }

            if (IsInside(a, bb))
                foreach (var o in a.Tracer(bb))
                    yield return o;
            if (IsInside(b, bb))
                foreach (var o in b.Tracer(bb))
                    yield return o;
            if (IsInside(c, bb))
                foreach (var o in c.Tracer(bb))
                    yield return o;
            if (IsInside(d, bb))
                foreach (var o in d.Tracer(bb))
                    yield return o;

            yield break;
        }

        public IEnumerable<Raytraceable> Tracer(Point p)
        {
            if (objects != null)
                if (parent == null || bbox.IsInside(p))
                    foreach (var o in objects)
                        yield return o;

            if (IsInside(a, p))
                foreach (var o in a.Tracer(p))
                    yield return o;
            else if (IsInside(b, p))
                foreach (var o in b.Tracer(p))
                    yield return o;
            else if (IsInside(c, p))
                foreach (var o in c.Tracer(p))
                    yield return o;
            else if (IsInside(d, p))
                foreach (var o in d.Tracer(p))
                    yield return o;

            yield break;
        }

        public void TraceAll(List<Raytraceable> list, Point p)
        {
            if (objects != null)
                list.AddRange(objects);

            if (a != null)
                a.Trace(list, p);
            if (b != null)
                b.Trace(list, p);
            if (c != null)
                c.Trace(list, p);
            if (d != null)
                d.Trace(list, p);
        }

        public void Trace(List<Raytraceable> list, Point p)
        {
            if (objects != null)
                if (parent == null || bbox.IsInside(p))
                    list.AddRange(objects);

            if (IsInside(a, p))
                a.Trace(list, p);
            else if (IsInside(b, p))
                b.Trace(list, p);
            else if (IsInside(c, p))
                c.Trace(list, p);
            else if (IsInside(d, p))
                d.Trace(list, p);
        }

        [Obsolete()]
        public void Trace(ref SortedList<int, Raytraceable> list, Point p)
        {
            if (objects != null)
            {
                if (parent == null || bbox.IsInside(p))
                {
                    foreach (var o in objects)
                    {
                        list.Add(o.Z, o);
                    }
                }
            }

            if (IsInside(a, p))
                a.Trace(ref list, p);
            else if (IsInside(b, p))
                b.Trace(ref list, p);
            else if (IsInside(c, p))
                c.Trace(ref list, p);
            else if (IsInside(d, p))
                d.Trace(ref list, p);


        }

        public override string ToString()
        {
            return string.Format("[QT {0}/{1}]", Count, TotalCount);
        }
    }
}
