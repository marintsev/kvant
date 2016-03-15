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
            // TODO: пересечение с гранью
        }

        public override void DrawFast(Graphics g, Matrix33 m)
        {
            var b = new SolidBrush(circ.Color);
            /*var p1 = (new Pointu(x, y) * m).ToPoint();
            var p2 = (new Pointu(x + w, y + h) * m).ToPoint();
            g.FillRectangle(b, (float)Math.Min(p1.x, p2.x), (float)Math.Min(p1.y, p2.y), (float)Math.Abs(p2.x - p1.x), (float)Math.Abs(p2.y - p1.y));*/
            var r = circ.Radius;
            var c = circ.Center;
            var p1 = ((c + (-r)).ToPointu() * m).ToPoint();
            var p2 = ((c + r).ToPointu() * m).ToPoint();
            var w = (float)Math.Abs(p2.x - p1.x);
            var h = (float)Math.Abs(p2.y - p1.y);
            g.FillEllipse(b, (float)p1.x, (float)p1.y, w, h);
            b.Dispose();
        }

        public override BBox CalcBBox()
        {
            return circ.CalcBBox();
        }
    }


}
