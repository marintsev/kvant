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
        public override bool Trace(ref Ray ray)
        {
            ray.add = Color.FromArgb((int)(ray.x * 255.0), 0, (int)(ray.y * 255.0));
            return true;
        }
    }
}
