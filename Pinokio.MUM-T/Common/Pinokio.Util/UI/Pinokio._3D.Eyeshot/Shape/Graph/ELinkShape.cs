using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;
using Pinokio.Map;

using devDept.Geometry;
using devDept.Eyeshot.Entities;

namespace Pinokio._3D.Eyeshot
{
    public class ELinkShape : EUniqueShape
    {
        public ELinkShape(uint id, AbstractObject absObj, ViewPort viewPort) : base(id, absObj, viewPort)
        { }

        protected override List<Entity> DrawByShape()
        {
            var link = this.Core as MapLink;
            var entities = new List<Entity>();

            var from = EyeshotHelper.ToPoint3D(DrawSetting.FromPos);
            var to = EyeshotHelper.ToPoint3D(DrawSetting.ToPos);
            switch ((MapLinkType)link.Type)
            {
                case MapLinkType.Straight:
                    var line = new Line(from, to);
                    entities.Add(line);
                    break;
                case MapLinkType.Curved:
                    var arc = DrawArcEntity(from, to, ((Geometry.Arc)link.GeometryObj).DirectionType);
                    entities.Add(arc);
                    break;
            }

            foreach (var ent in entities)
            {
                ent.LineWeightMethod = colorMethodType.byEntity;
                ent.LineWeight = 2;
                ent.ColorMethod = colorMethodType.byEntity;
                ent.Color = DrawSetting.MainColor;
            }

            return entities;
        }

        private EllipticalArc DrawArcEntity(Point3D from, Point3D to, Geometry.DirectionType directionType)
        {
            var crossProduct = (from.X - to.X) * (from.Y - to.Y);
            double centerZ = (from.Z + to.Z) / 2;
            Point3D center = new Point3D(from.X, to.Y, centerZ);
            double r1 = Math.Abs(to.X - from.X), r2 = Math.Abs(to.Y - from.Y);
            if ((crossProduct > 0 && directionType == Geometry.DirectionType.ClockWise) ||
                (crossProduct < 0 && directionType == Geometry.DirectionType.CounterClockWise))
            {
                center = new Point3D(to.X, from.Y, centerZ);
            }
            else if (crossProduct == 0)
            {
                center = new Point3D((from.X + to.X) / 2, (from.Y + to.Y) / 2, centerZ);
                r1 = r2 = Math.Max(Math.Abs(to.X - center.X), Math.Abs(to.Y - center.Y));
            }

            var halfLength = Math.Max(Math.PI * r1, Math.PI * r2);
            var arc = new EllipticalArc(Plane.XY, center, r1, r2, from, to, false);
            if (arc.Length() > halfLength)
                arc = new EllipticalArc(Plane.XY, center, r1, r2, from, to, true);

            return arc;
        }
    }
}
