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
    }
}
