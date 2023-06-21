using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;
using Pinokio.Geometry;

using devDept.Geometry;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;

namespace Pinokio._3D.Eyeshot
{
    public class EInOutConvShape : EConveyorShape
    {
        public EInOutConvShape(uint id, ConcreteObject conObj, ViewPort viewPort) : base(id, conObj, viewPort)
        { }

        protected override List<Entity> DrawByShape()
        {
            var entities = base.DrawByShape();

            var arrow = Mesh.CreateArrow(200, 400, 400, 400, 16, Mesh.natureType.Smooth);
            arrow.EdgeStyle = Mesh.edgeStyleType.None;
            arrow.ColorMethod = colorMethodType.byEntity;
            if (this.Direction == new Vector3(1, 0, 0) || // Right
                this.Direction == new Vector3(0, -1, 0))    // Bottom
                arrow.Color = DrawSetting.MainColor;
            else if (this.Direction == new Vector3(-1, 0, 0) || // Left
                     this.Direction == new Vector3(0, 1, 0))   // Top
                arrow.Color = DrawSetting.SubColor;
            arrow.Translate(0, 0, 1000);
            entities.Add(arrow);

            return entities;
        }
    }
}
