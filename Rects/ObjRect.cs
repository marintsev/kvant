using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rects
{
    class ObjRect
    {
        private BBox bbox;
        private Color color;

        public BBox BBox { get { return bbox; } set { bbox = value; } }
        public ObjRect(BBox bbox_, Color color_)
        {
            bbox = bbox_;
            color = color_;
        }

        private void AddRect(List<Raytraceable> scene, double x, double y, double w, double h, double l, Color color)
        {
            color = Color.FromArgb(127, 255, 255, 255);
            /*scene.Add(new RTRectangle(x - l / 2, y, l, h, color));
            scene.Add(new RTRectangle(x + w - l / 2, y, l, h, color));
            scene.Add(new RTRectangle(x, y - l / 2, w, l, color));
            scene.Add(new RTRectangle(x, y + h - l / 2, w, l, color));*/

            scene.Add(new RTRectangle(x, y, l, h, color).WithZ(1)); // левая вертикальная
            scene.Add(new RTRectangle(x + w - l, y, l, h, color).WithZ(1)); // правая вертикальная
            scene.Add(new RTRectangle(x, y, w, l, color).WithZ(1));
            scene.Add(new RTRectangle(x, y + h - l, w, l, color).WithZ(1));
        }

        private void AddCircles(List<Raytraceable> scene, double x, double y, double w, double h, Color circ_col, double cr, double sh)
        {
            scene.Add(new RaytraceableCircle(new Circle(x + sh, y + sh, cr, circ_col)).WithZ(2));
            scene.Add(new RaytraceableCircle(new Circle(x + w - sh, y + sh, cr, circ_col)).WithZ(2));
            scene.Add(new RaytraceableCircle(new Circle(x + w - sh, y + h - sh, cr, circ_col)).WithZ(2));
            scene.Add(new RaytraceableCircle(new Circle(x + sh, y + h - sh, cr, circ_col)).WithZ(2));
        }

        private void AddRectangle(List<Raytraceable> scene, double x, double y, double w, double h, Color color)
        {
            var r = new RTRectangle(x, y, w, h, color);
            r.Z = 0;
            scene.Add(r);
            double l = 0.01;

            double cr = l;
            Color circ_col = Color.FromArgb(127, 255, 255, 255);

            AddRect(scene, x, y, w, h, l * 0.5, color);
            AddCircles(scene, x, y, w, h, circ_col, cr, l / 6);

        }

        public List<Raytraceable> EmitObjects()
        {
            var list = new List<Raytraceable>();
            AddRectangle(list, bbox.Left, bbox.Bottom, bbox.Width, bbox.Height, color);
            return list;
        }
    }
}
