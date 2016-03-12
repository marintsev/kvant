using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public abstract class Raytraceable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="c"></param>
        /// <returns>true, если столкнулся с непрозрачным объектом</returns>
        public abstract bool Trace(double x, double y, ref Color c);
    }
}
