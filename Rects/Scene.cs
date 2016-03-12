using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    public class Scene
    {
        private List<Raytraceable> objects = null;

        public Scene()
        {
            objects = new List<Raytraceable>();
        }

        public void Add(Raytraceable o)
        {
            objects.Add(o);
        }

        public Color GetColor(double x, double y)
        {
            Color c = Color.Transparent;
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                var o = objects[i];
                if (o.Trace(x, y, ref c))
                    return c;
            }
            return c;
        }
    }
}
