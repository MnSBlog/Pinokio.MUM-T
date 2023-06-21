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
    public class ENodeShape : EyeshotShape
    {
        public ENodeShape(uint id, AbstractObject absObj, ViewPort viewPort) : base(id, absObj, viewPort)
        {
            if (absObj is MapNode node)
            {
                SetPosition(node.Position);
            }
        }

        protected override List<Entity> DrawByShape()
        {
            var octagon = EyeshotCADMart.CreateOctagon(DrawSetting.Width);
            octagon.ColorMethod = colorMethodType.byEntity;
            octagon.Color = DrawSetting.MainColor;
            return new List<Entity>() { octagon };
        }
    }
}
