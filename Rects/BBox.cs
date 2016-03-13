using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public class BBox
    {
        double left, right, top, bottom;

        public double Left { get { return left; } }
        public double Right { get { return right; } }
        public double Top { get { return top; } }
        public double Bottom { get { return bottom; } }
        public double CenterX { get { return (left + right) / 2; } }
        public double CenterY { get { return (top + bottom) / 2; } }
        public BBox(double left_, double right_, double top_, double bottom_)
        {
            left = left_;
            right = right_;
            top = top_;
            bottom = bottom_;
        }

        public bool IsInside(Point p)
        {
            return p.x >= left && p.x <= right && p.y >= bottom && p.y <= top;
        }
    }
}
