using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public class RTRectangle : Raytraceable
    {
        BBox bbox;
        Color color;

        private double x { get { return bbox.Left; } }
        private double y { get { return bbox.Bottom; } }
        private double w { get { return bbox.Right-bbox.Left; } }
        private double h { get { return bbox.Top-bbox.Bottom; } }

        public RTRectangle(double x_, double y_, double w_, double h_, Color color_)
        {
            bbox = new BBox(x_, x_ + w_, y_+h_, y_ );
            color = color_;
        }


        public override bool Trace(ref Raytraceable.Ray ray)
        {
            if (ray.x >= x && ray.x <= x + w && ray.y >= y && ray.y <= y + h)
            {
                ray.add = color;
                return true;
            }
            return false;
        }

        public override bool IsInsideOf(BBox bb)
        {
            return bbox.IsInsideOf(bb);
            /*return bb.IsInside(new Point(x, y)) &&
                bb.IsInside(new Point(x + w, y)) &&
                bb.IsInside(new Point(x + w, y + h)) &&
                bb.IsInside(new Point(x, y + h));*/
        }

        public override bool IsCross(BBox bb)
        {
            /*return bbox.IsInside(new Point(x, y)) ||
                bbox.IsInside(new Point(x + w, y)) ||
                bbox.IsInside(new Point(x + w, y + h)) ||
                bbox.IsInside(new Point(x, y + h)) ||
                IsInsideOf(bbox) ||
                bbox.IsInsideOf(this.bbox);*/
            //return bb.Intersects(bbox);
            return bbox.Intersects(bb);
        }
    }
}
