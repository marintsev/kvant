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
        /// <param name="ray"></param>
        /// <returns>true, если луч попал в объект.</returns>
        public abstract bool Trace(ref Ray ray);

        
    }
}
