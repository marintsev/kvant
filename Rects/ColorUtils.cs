using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public static class ColorUtils
    {
        public static Color Blend(Color c1, Color c2, double f1, double f2)
        {
            double r = c1.R * f1 + c2.R * f2;
            double g = c1.G * f1 + c2.G * f2;
            double b = c1.B * f1 + c2.B * f2;
            double a = c1.A * f1 + c2.A * f2;
            return Color.FromArgb(
                (int)a, (int)r, (int)g, (int)b);
        }

        public static Color Add(Color c1, Color c2)
        {
            int r = (int)c1.R + (int)c2.R;
            return Color.FromArgb(0);
        }

        public static double Clamp(this double x, double min, double max)
        {
            if (x < min)
                return min;
            if (x > max)
                return max;
            return x;
        }

        public static Color Blend(Color c1, Color c2)
        {
            double r1 = c1.R / 255.0;
            double g1 = c1.G / 255.0;
            double b1 = c1.B / 255.0;
            double a1 = c1.A / 255.0;

            double r2 = c2.R / 255.0;
            double g2 = c2.G / 255.0;
            double b2 = c2.B / 255.0;
            double a2 = c2.A / 255.0;

            double a = a1 + a2 * (1 - a1);
            double x = 1.0 / a;
            double r = (r1 * a1 + r2 * a2 * (1 - a1)) * x;
            double g = (g1 * a1 + g2 * a2 * (1 - a1)) * x;
            double b = (b1 * a1 + b2 * a2 * (1 - a1)) * x;

            if (a < 1.0 / 255.0)
                return Color.Transparent;

            r = r.Clamp(0, 1);
            g = g.Clamp(0, 1);
            b = b.Clamp(0, 1);
            a = a.Clamp(0, 1);

            return Color.FromArgb(
                (int)(a * 255.0), (int)(r * 255.0), (int)(g * 255.0), (int)(b * 255.0));
        }

        public static Color Random( Random r )
        {
            if( r == null )
             r = new Random();
            return Color.FromArgb(r.Next(256), r.Next(256), r.Next(256), r.Next(256));
        }

        public static Color RandomOpaque()
        {
            var r = new Random();
            return Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
        }
    }
}
