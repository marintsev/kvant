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
        double w, h;
        double cx, cy;

        public RaytraceableGradient(double cx_, double cy_, double w_=1, double h_=1)
        {
            cx = cx_;
            cy = cy_;
            w = w_;
            h = h_;
        }

        public static double Mod( double x, double y )
        {
            double r;
            if( x < 0 )
                r = x - (int)(x / y) * y;
            else
                r = x - (int)(x / y)*y;
            if (r < 0)
                r += y;
            return r;
        }

        public static double Mod2(double x, double y)
        {
            double r;
            if (x < 0)
                r = x - (int)(x / y);
            else
                r = x - (int)(x / y);
            if (r < 0)
                r += y;
            return r;
        }

       

        public static double Magic( double x)
        {
            x = Math.Abs(x);
            var i = (int)x;
            var y = x - i;
            if( i % 2 == 0)
                return y;
            else
                return 1 - y;
        }

        public override bool Trace(ref Ray ray)
        {
            double x = Magic((ray.x-cx)/w);
            double y = Magic((ray.y-cy)/h);
            x = x.Clamp(0, 1);
            y = y.Clamp(0, 1);
            ray.add = Color.FromArgb((int)(x * 255.0), 0, (int)(y * 255.0));
            return true;
        }

        public override bool IsInsideOf(BBox bbox)
        {
            return false;
        }

        public override bool IsCross(BBox bbox)
        {
            return true;
        }

        public override void DrawFast(Graphics g, Matrix33 m)
        {
            var b = new SolidBrush(Color.Black);
            /*var p1 = (new Pointu(x, y) * m).ToPoint();
            var p2 = (new Pointu(x + w, y + h) * m).ToPoint();
            g.FillRectangle(b, (float)Math.Min(p1.x, p2.x), (float)Math.Min(p1.y, p2.y), (float)Math.Abs(p2.x - p1.x), (float)Math.Abs(p2.y - p1.y));*/
            g.FillRectangle(b, g.ClipBounds);
            b.Dispose();
        }
    }
}
