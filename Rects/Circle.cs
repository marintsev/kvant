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

        public Color Color { get { return color; } }

        public Circle(double x_, double y_, double r_, Color color_)
        {
            cx = x_;
            cy = y_;
            r = r_;
            color = color_;
        }

        public bool IsInside(double x, double y)
        {
            return (x - cx).Sqr() + (y - cy).Sqr() < r.Sqr();
        }
    }
}
