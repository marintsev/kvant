using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public class Matrix33
    {
        public double[] m = null;

        public Matrix33()
        {
        }

        public static Matrix33 Identity()
        {
            var m = new Matrix33();
            m.m = new double[9] { 1, 0, 0, 0, 1, 0, 0, 0, 1 };
            return m;
        }

        public static Matrix33 Scale(double sx, double sy)
        {
            var m = new Matrix33();
            m.m = new double[9] { sx, 0, 0, 0, sy, 0, 0, 0, 1 };
            return m;
        }

        public static Matrix33 Translate(double tx, double ty)
        {
            var m = new Matrix33();
            m.m = new double[9] { 1.0, 0, tx, 0, 1.0, ty, 0, 0, 1 };
            return m;
        }

        public static Matrix33 Multiply( Matrix33 a, Matrix33 b )
        {
            var m = new Matrix33();
            m.m = new double[9];
            for(int i=0; i<3; i++)
            {
                for(int j=0;j<3;j++)
                {
                    double s = 0;
                    for( int k=0; k<3;k++)
                    {
                        s += a.m[i * 3 + k] * b.m[k * 3 + j];
                    }
                    m.m[i * 3 + j] = s;
                }
            }
            return m;
        }

        public static Matrix33 operator * ( Matrix33 a, Matrix33 b )
        {
            return Matrix33.Multiply(a, b);
        }
    }
}
