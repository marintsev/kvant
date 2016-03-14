using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public class Circle
    {
        double cx, cy, r;
        Color color;

        public Point Center { get { return new Point(cx, cy); } }
        public double Radius { get { return r; } }
        public Color Color { get { return color; } }

        public Circle(double x_, double y_, double r_, Color color_)
        {
            cx = x_;
            cy = y_;
            r = r_;
            color = color_;
        }

        public Point GetPoint(double angle)
        {
            return new Point(cx + Math.Cos(angle) * r,
                cy + Math.Sin(angle) * r);
        }

        public bool IsInside(Point p)
        {
            return IsInside(p.x, p.y);
        }
        public bool IsInside(double x, double y)
        {
            /*if (!(x >= cx - r && x <= cx + r))
                return false;
            if (!(y >= cy - r && y <= cy + r))
                return false;*/
            double dx = x - cx;
            double dy = y - cy;
            return dx * dx + dy * dy <= r * r;
        }
    }
}
