using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public class Matrix33
    {
        public double[] m;

        public Matrix33(double[] m_ = null)
        {
            m = m_;
        }

        public static Matrix33 Identity()
        {
            return new Matrix33(new double[9] { 1, 0, 0, 0, 1, 0, 0, 0, 1 });
        }

        public static Matrix33 Scale(double sx, double sy)
        {
            return new Matrix33(new double[9] { sx, 0, 0, 0, sy, 0, 0, 0, 1 });
        }

        public static Matrix33 Translate(double tx, double ty)
        {
            var m = new Matrix33();
            m.m = new double[9] { 1.0, 0, tx, 0, 1.0, ty, 0, 0, 1 };
            return m;
        }

        public Matrix33 Translated( double tx, double ty )
        {
            return Translate(tx, ty) * this;
        }

        public Matrix33 Translated(Point p)
        {
            return Translated(p.x, p.y);
        }

        public static Matrix33 Shrink( double atx, double aty )
        {
            return Scale(1.0 / atx, 1.0 / aty);
        }

        public Matrix33 Scaled( double sx, double sy )
        {
            return Scale(sx, sy) * this;
        }

        public Matrix33 Shrinked(double asx, double asy)
        {
            return Shrink(asx, asy) * this;
        }

        public static Matrix33 Multiply(Matrix33 a, Matrix33 b)
        {
            var m = new Matrix33();
            m.m = new double[9];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double s = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        s += a.m[i * 3 + k] * b.m[k * 3 + j];
                    }
                    m.m[i * 3 + j] = s;
                }
            }
            return m;
        }

        public static Matrix33 Invert(Matrix33 x)
        {
            double den = 1.0 / (-(x.m[2] * x.m[4] * x.m[6]) +
                x.m[1] * x.m[5] * x.m[6] +
                x.m[2] * x.m[3] * x.m[7] -
                x.m[0] * x.m[5] * x.m[7] -
                x.m[1] * x.m[3] * x.m[8] +
                x.m[0] * x.m[4] * x.m[8]);
            var m = new Matrix33(
                new double[] { 
                   (-(x.m[5]*x.m[7]) + x.m[4]*x.m[8])*den,
                    (x.m[2]*x.m[7] - x.m[1]*x.m[8])*den,
                    (-(x.m[2]*x.m[4]) + x.m[1]*x.m[5])*den,
                    (x.m[5]*x.m[6] - x.m[3]*x.m[8])*den,
                    (-(x.m[2]*x.m[6]) + x.m[0]*x.m[8])*den,
                    (x.m[2]*x.m[3] - x.m[0]*x.m[5])*den,
                    (-(x.m[4]*x.m[6]) + x.m[3]*x.m[7])*den,
                    (x.m[1]*x.m[6] - x.m[0]*x.m[7])*den,
                    (-(x.m[1]*x.m[3]) + x.m[0]*x.m[4])*den});
            return m;
        }

        public Matrix33 Inverted()
        {
            return Matrix33.Invert(this);
        }

        public static Matrix33 operator *(Matrix33 a, Matrix33 b)
        {
            return Matrix33.Multiply(a, b);
        }
    }
}
