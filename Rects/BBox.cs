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

        public static BBox Infinity = new BBox(double.NegativeInfinity, double.PositiveInfinity, double.PositiveInfinity, double.NegativeInfinity);

        public BBox()
            : this(0, 0, 0, 0)
        {
        }
        public BBox(double left_, double right_, double top_, double bottom_)
        {
            if (left_ > right_)
                throw new ArgumentException();
            if (bottom_ > top_)
                throw new ArgumentException();
            left = left_;
            right = right_;
            top = top_;
            bottom = bottom_;
        }

        public bool IsEmpty()
        {
            return right <= left || top <= bottom;
        }
        public bool IsInside(Point p)
        {
            return p.x >= left && p.x <= right && p.y >= bottom && p.y <= top;
        }

        public bool IsItInside(BBox bb)
        {
            return bb.IsInsideOf(this);
        }

        public bool IsInsideOf(BBox bb)
        {
            return bb.IsInside(LeftTop) &&
                   bb.IsInside(RightTop) &&
                   bb.IsInside(LeftBottom) &&
                   bb.IsInside(RightBottom);
        }

        public Point LeftTop
        {
            get
            {
                return new Point(left, top);
            }
        }

        public Point RightTop { get { return new Point(right, top); } }
        public Point LeftBottom { get { return new Point(left, bottom); } }
        public Point RightBottom { get { return new Point(right, bottom); } }

        /// <summary>
        /// Этот bbox находится внутри bb?
        /// </summary>
        /// <param name="bb"></param>
        /// <returns></returns>
        public bool Intersects(BBox bb)
        {
            return bb.IsInside(LeftTop) ||
                bb.IsInside(RightTop) ||
                bb.IsInside(LeftBottom) ||
                bb.IsInside(RightBottom) ||
                bb.IsInside(new Point(CenterX, CenterY)) ||
                IsInside(bb.LeftTop) ||
                IsInside(bb.RightTop) ||
                IsInside(bb.LeftBottom) ||
                IsInside(bb.RightBottom) ||
                IsInside(new Point(bb.CenterX, bb.CenterY));
        }

        public void Feed(Point p)
        {
            if (IsInfinity())
                return;
            if (p.x < left)
                left = p.x;
            if (p.x > right)
                right = p.x;
            if (p.y < bottom)
                bottom = p.y;
            if (p.y > top)
                top = p.y;
        }

        public void Feed(BBox bb)
        {
            if (IsInfinity())
                return;
            Feed(bb.LeftTop);
            Feed(bb.RightTop);
            Feed(bb.LeftBottom);
            Feed(bb.RightBottom);
        }

        public bool IsInfinity()
        {
            return double.IsInfinity(left) || double.IsInfinity(right) || double.IsInfinity(top) || double.IsInfinity(bottom);
        }

        public override string ToString()
        {
            return string.Format( "[BBox {0}:{1}, {2}:{3}]", left, right, bottom, top );
        }
    }




}
