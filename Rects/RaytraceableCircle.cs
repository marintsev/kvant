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
                ray.add = circ.Color;
                return true;
            }
            return false;
        }
    }

    
}
