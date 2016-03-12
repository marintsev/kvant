using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public class RaytraceableGradient : Raytraceable
    {

        public RaytraceableGradient()
        {

        }
        public override bool Trace(double x, double y, ref Color c)
        {
            c = Color.FromArgb((int)(x * 255.0), 0, (int)(y * 255.0));
            return true;
        }
    }
}
