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

        public override bool IsInsideOf(BBox bbox)
        {
            return bbox.IsInside(circ.GetPoint(0)) &&
                bbox.IsInside(circ.GetPoint(Math.PI * 0.5)) &&
                bbox.IsInside(circ.GetPoint(Math.PI)) &&
                bbox.IsInside(circ.GetPoint(Math.PI * 1.5));
        }

        public override bool IsCross(BBox bbox)
        {
            return bbox.IsInside(circ.GetPoint(0)) ||
                bbox.IsInside(circ.GetPoint(Math.PI * 0.5)) ||
                bbox.IsInside(circ.GetPoint(Math.PI)) ||
                bbox.IsInside(circ.GetPoint(Math.PI * 1.5)) ||
                (
                circ.IsInside(new Point(bbox.Left, bbox.Top)) ||
                circ.IsInside(new Point(bbox.Right, bbox.Top)) ||
                circ.IsInside(new Point(bbox.Left, bbox.Bottom)) ||
                circ.IsInside(new Point(bbox.Right, bbox.Bottom))
                );
        }
    }


}
