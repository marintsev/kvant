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
        public override bool Trace(ref Ray ray)
        {
            if (circ.IsInside(ray.x, ray.y))
            {
                double f = circ.Color.A / 255.0;
                double g = ray.c.A / 255.0;
                ray.c = ColorUtils.Blend(ray.c, circ.Color, g, 1-g);
                ray.stop = circ.Color.A == 255;
                return true;
            }
            return false;
        }
    }
}
