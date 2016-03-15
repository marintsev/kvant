#define CONVERT

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
        public double Right { get { return right; } set { right = value; } }
        public double Top { get { return top; } set { top = value; } }
        public double Bottom { get { return bottom; } set { bottom = value; } }
        public double CenterX { get { return (left + right) / 2; } }
        public double CenterY { get { return (top + bottom) / 2; } }

        public double Width { get { return right - left; } }
        public double Height { get { return top - bottom; } }

        public Point LeftTop
        {
            get
            {
                return new Point(left, top);
            }
        }

        public Point RightTop
        {
            get { return new Point(right, top); }
            set
            {
                var v = value;
                Right = v.x;
                Top = v.y;
            }
        }
        public Point LeftBottom { get { return new Point(left, bottom); } }
        public Point RightBottom
        {
            get { return new Point(right, bottom); }
            set
            {
                var v = value;
                Right = v.x;
                Bottom = v.y;
            }
        }

        public static BBox Infinity = new BBox(double.NegativeInfinity, double.PositiveInfinity, double.PositiveInfinity, double.NegativeInfinity);

        public BBox()
            : this(double.NaN, double.NaN, double.NaN, double.NaN)
        {
        }

        public BBox(Point p1, Point p2)
            : this()
        {
            Feed(p1);
            Feed(p2);
        }

        public void Normalize()
        {
            if (left > right)
            {
                var temp = left;
                left = right;
                right = temp;
            }
            if (bottom > top)
            {
                var temp = bottom;
                bottom = top;
                top = bottom;
            }
        }

        public BBox(double left_, double right_, double top_, double bottom_)
        {
#if CONVERT
            if (left_ > right_)
            {
                var temp = left_;
                left_ = right_;
                right_ = temp;
            }
            if (bottom_ > top_)
            {
                var temp = bottom_;
                bottom_ = top_;
                top_ = bottom_;
            }
#endif
            if (left_ > right_)
                throw new ArgumentException();
            if (bottom_ > top_)
                throw new ArgumentException();

            left = left_;
            right = right_;
            top = top_;
            bottom = bottom_;
        }

        public static BBox FromXYWH(double x, double y, double w, double h)
        {
            return new BBox(
                Math.Min(x, x + w),
                Math.Max(x, x + w),
                Math.Max(y, y + h),
                Math.Min(y, y + h));
        }

        public bool IsNaN()
        {
            return double.IsNaN(left) || double.IsNaN(right) || double.IsNaN(top) || double.IsNaN(bottom);
        }
        public bool IsNull()
        {
            return IsNaN();
        }
        public bool IsEmpty()
        {
            return right <= left || top <= bottom || IsNaN();
        }

        public bool Contains(Point p)
        {
            return IsInside(p);
        }

        public void Move( Point shift )
        {
            left += shift.x;
            right += shift.x;
            top += shift.y;
            bottom += shift.y;
        }

        [Obsolete]
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
            {
            }
            else if (IsNull())
            {
                left = right = p.x;
                top = bottom = p.y;
            }
            else
            {
                if (p.x < left)
                    left = p.x;
                if (p.x > right)
                    right = p.x;
                if (p.y < bottom)
                    bottom = p.y;
                if (p.y > top)
                    top = p.y;
            }
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
            return string.Format("[BBox {0}:{1}, {2}:{3}]", left, right, bottom, top);
        }
    }




}
