using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public class RaytraceableCircle : Raytraceable
    {
        Circle circ;
        public RaytraceableCircle(Circle c_)
        {
            circ = c_;
        }
        public override bool Trace(double x, double y, ref Color c)
        {
            if (circ.IsInside(x, y))
            {
                c = circ.Color;
                return true;
            }
            return false;
        }
    }
}
