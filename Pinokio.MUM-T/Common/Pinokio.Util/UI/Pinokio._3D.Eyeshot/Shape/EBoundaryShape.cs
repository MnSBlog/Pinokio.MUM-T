using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;

using devDept.Geometry;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;

namespace Pinokio._3D.Eyeshot
{
    public class EBoundaryShape : EMovableShape
    {
        public EBoundaryShape(uint id, ConcreteObject conObj, ViewPort viewPort) : base(id, conObj, viewPort)
        { }

        protected override List<Entity> DrawByShape()
        {
            var boundary = Mesh.CreateBox(DrawSetting.Width, DrawSetting.Depth, 0.0001);
            boundary.ColorMethod = colorMethodType.byParent;
            boundary.EdgeStyle = Mesh.edgeStyleType.None;
            boundary.Translate(new Vector3D(-DrawSetting.Width / 2, -DrawSetting.Depth / 2)); //  (new Vector3D(-AGV_Size.Sensor1 / 2, -sensorSize / 2)); <-- 기존에 Sensor 위치 정할때..

            return new List<Entity>() { boundary };
        }
    }
}
