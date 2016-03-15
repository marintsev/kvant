﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public class RTRectangle : Raytraceable
    {
        public BBox bbox;
        public Color color;

        private double x { get { return bbox.Left; } }
        private double y { get { return bbox.Bottom; } }
        private double w { get { return bbox.Right - bbox.Left; } }
        private double h { get { return bbox.Top - bbox.Bottom; } }

        public RTRectangle(double x_, double y_, double w_, double h_, Color color_)
        {
            bbox = new BBox(x_, x_ + w_, y_ + h_, y_);
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
        }

        public override bool IsCross(BBox bb)
        {
            return bbox.Intersects(bb);
        }

        public override void DrawFast(Graphics g, Matrix33 m)
        {
            var b = new SolidBrush(color);
            var p1 = (new Pointu(x, y) * m).ToPoint();
            var p2 = (new Pointu(x + w, y + h) * m).ToPoint();
            g.FillRectangle(b, (float)Math.Min(p1.x, p2.x), (float)Math.Min(p1.y, p2.y), (float)Math.Abs(p2.x - p1.x), (float)Math.Abs(p2.y - p1.y));
            b.Dispose();
        }

        public override BBox CalcBBox()
        {
            return bbox;
        }
    }
}
