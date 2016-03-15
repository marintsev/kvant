using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    class ObjRect
    {
        private BBox bbox;
        private Color color;
        private bool hovered, selected;
        private bool dirty = true;

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    Dirty = true;
                }
            }
        }
        public bool Hovered
        {
            get
            {
                return hovered;
            }
            set
            {
                if (hovered != value)
                {
                    hovered = value;
                    Dirty = true;
                }
            }
        }
        public bool Dirty
        {
            get
            {
                return dirty;
            }
            set
            {
                dirty = value;
            }
        }

        public BBox BBox
        {
            get { return bbox; }
            set
            {
                bbox = value;
                Dirty = true;
            }
        }
        public ObjRect(BBox bbox_, Color color_)
        {
            bbox = bbox_;
            color = color_;
            hovered = false;
            selected = false;
            hoveredObjects = 0;
        }

        public enum Subobject { Rect = 1 << 0, LTC = 1 << 1, RTC = 1 << 2, LBC = 1 << 3, RBC = 1 << 4 };

        int hoveredObjects;

        public BBox GetObjectBBox(Subobject which)
        {
            switch (which)
            {
                case Subobject.Rect:
                    return bbox;
                case Subobject.LTC:
                    return GetCircle(0).CalcBBox();
                case Subobject.RTC:
                    return GetCircle(1).CalcBBox();
                case Subobject.LBC:
                    return GetCircle(2).CalcBBox();
                case Subobject.RBC:
                    return GetCircle(3).CalcBBox();
            }
            throw new NotImplementedException();
        }
        public bool CheckObject(Subobject which, Point p)
        {
            var bb = GetObjectBBox(which);
            return bb.Contains(p);
        }

        public int GetHoveredObjects(Point p)
        {
            int sum = 0;
            foreach (Subobject so in typeof(Subobject).GetEnumValues())
            {
                if (CheckObject(so, p))
                    sum |= (int)so;
            }
            return sum;
        }

        public int HoveredObjects
        {
            get
            {
                return hoveredObjects;
            }
            set
            {
                if (hoveredObjects != value)
                    Dirty = true;
                hoveredObjects = value;
            }
        }

        public bool OnHover(Point p)
        {
            HoveredObjects = GetHoveredObjects(p);
            Hovered = HoveredObjects != 0;
            return hovered;
        }

        [Obsolete()]
        public bool OnClick(Point p)
        {
            return Selected = TestSelect(p);
        }
        public bool TestSelect(Point p)
        {
            return bbox.Contains(p);
        }

        private void AddRect(List<Raytraceable> scene, double x, double y, double w, double h, double l, Color color)
        {
            color = Color.FromArgb(127, 255, 255, 255);
            /*scene.Add(new RTRectangle(x - l / 2, y, l, h, color));
            scene.Add(new RTRectangle(x + w - l / 2, y, l, h, color));
            scene.Add(new RTRectangle(x, y - l / 2, w, l, color));
            scene.Add(new RTRectangle(x, y + h - l / 2, w, l, color));*/

            scene.Add(new RTRectangle(x, y, l, h, color).WithZ(1)); // левая вертикальная
            scene.Add(new RTRectangle(x + w - l, y, l, h, color).WithZ(1)); // правая вертикальная
            scene.Add(new RTRectangle(x, y, w, l, color).WithZ(1));
            scene.Add(new RTRectangle(x, y + h - l, w, l, color).WithZ(1));
        }

        private Raytraceable GetCircle(double x, double y, double w, double h, Color circ_col, double cr, double sh, int i)
        {
            if (i == 0)
                return new RaytraceableCircle(new Circle(x + sh, y + sh, cr, circ_col)).WithZ(2);
            else if (i == 1)
                return new RaytraceableCircle(new Circle(x + w - sh, y + sh, cr, circ_col)).WithZ(2);
            else if (i == 2)
                return new RaytraceableCircle(new Circle(x + w - sh, y + h - sh, cr, circ_col)).WithZ(2);
            else if (i == 3)
                return new RaytraceableCircle(new Circle(x + sh, y + h - sh, cr, circ_col)).WithZ(2);
            throw new ArgumentException();
        }

        private Raytraceable GetCircle(int i)
        {
            return GetCircle(bbox.Left, bbox.Bottom, bbox.Width, bbox.Height, GetCircleColor(i), CircleRadius, CircleRadius / 2 - LineWidth / 3, i);
        }
        private void AddCircles(List<Raytraceable> scene)
        {
            for (int i = 0; i < 4; i++)
                scene.Add(GetCircle(i));
        }

        private static Color UnhoveredCircleColor = Color.FromArgb(127, 255, 255, 255);
        private static Color HoveredCircleColor = UnhoveredCircleColor.Enlight();//Color.FromArgb(192, 255, 0, 255);

        private static Subobject[] CircleEnums = { Subobject.LTC, Subobject.RTC, Subobject.LBC, Subobject.RBC };
        private int GetCircleEnum(int i)
        {
            return (int)CircleEnums[i];
        }

        private Color GetCircleColor(int i)
        {
            var hover = (hoveredObjects & GetCircleEnum(i)) != 0;
            return hover ? HoveredCircleColor : UnhoveredCircleColor;
        }

        private double LineWidth
        {
            get
            {
                return 0.01;
            }
        }

        private double CircleRadius
        {
            get
            {
                return LineWidth;
            }
        }


        private void AddRectangle(List<Raytraceable> scene, double x, double y, double w, double h, Color color)
        {
            var mod_color = color;
            if ((hoveredObjects & (int) Subobject.Rect) != 0)
                mod_color = ColorUtils.Enlight(color);

            var r = new RTRectangle(x, y, w, h, mod_color);

            r.Z = 0;
            scene.Add(r);
            double l = LineWidth;

            AddRect(scene, x, y, w, h, l * 0.5, color);

            if (selected)
                AddCircles(scene);

        }

        public List<Raytraceable> EmitObjects()
        {
            var list = new List<Raytraceable>();
            AddRectangle(list, bbox.Left, bbox.Bottom, bbox.Width, bbox.Height, color);
            return list;
        }

        internal void Deselect()
        {
            Selected = false;
        }

        internal void Select()
        {
            Selected = true;
        }

        public void Move(Point p)
        {
            bbox.Move(p);
            Dirty = true;
        }

        internal bool Dehover()
        {
            if (hovered == true)
            {
                Hovered = false;
                return true;
            }
            return false;
        }
    }
}
