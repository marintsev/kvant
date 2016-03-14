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
        public struct Ray
        {
            public double x, y;
            public Color c;
            public Color add;
            public bool stop;
        }
        public int Z { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ray"></param>
        /// <returns>true, если луч попал в объект.</returns>
        public abstract bool Trace(ref Ray ray);
        public abstract bool IsInsideOf(BBox bbox);
        public abstract bool IsCross(BBox bbox);


    }
}
