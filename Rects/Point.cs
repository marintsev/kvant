﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public struct Point
    {
        public double x, y;
        public static Point Invalid = new Point(double.NaN, double.NaN);
        public bool IsInvalid()
        {
            return double.IsNaN(x) || double.IsNaN(y);
        }

        public Point(double x_, double y_)
        {
            x = x_;
            y = y_;
        }

        public static Point operator -(Point p)
        {
            return new Point(-p.x, -p.y);
        }

        public static Point operator +(Point p, double v)
        {
            return new Point(p.x + v, p.y + v);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.x - b.x, a.y - b.y);
        }

        public Pointu ToPointu()
        {
            return new Pointu(x, y);
        }

        public override string ToString()
        {
            return string.Format("[{0:0.##},{1:0.##}]", x, y);
        }
    }
}
