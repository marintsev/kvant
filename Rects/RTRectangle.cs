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
        double x, y, w, h;
        Color color;

        public RTRectangle(double x_, double y_, double w_, double h_, Color color_)
        {
            x = x_;
            y = y_;
            w = w_;
            h = h_;
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
    }
}
