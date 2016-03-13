using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public struct Pointu
    {
        double x, y, t;
        public Pointu(double x_, double y_, double t_ = 1.0)
        {
            x = x_;
            y = y_;
            t = t_;
        }

        public Pointu Multiply(Matrix33 m)
        {
            return new Pointu(
                m.m[0] * x + m.m[1] * y + m.m[2] * t,
                m.m[3] * x + m.m[4] * y + m.m[5] * t,
                m.m[6] * x + m.m[7] * y + m.m[8] * t
                );
        }

        public static Pointu operator *(Pointu p, Matrix33 m)
        {
            return p.Multiply(m);
        }

        public Point ToPoint()
        {
            return new Point(x / t, y / t);
        }

        public override string ToString()
        {
            return string.Format("[{0},{1},{2}]", x,y,t);
        }
    }
}
