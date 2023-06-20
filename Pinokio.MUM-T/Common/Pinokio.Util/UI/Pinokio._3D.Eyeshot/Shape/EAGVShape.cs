using System.Collections.Generic;
using System.Drawing;

using Pinokio.Core;

using devDept.Geometry;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;

namespace Pinokio._3D.Eyeshot
{
    public class EAGVShape : EMovableShape
    {
        public EAGVShape(uint id, ConcreteObject conObj, ViewPort viewPort) : base(id, conObj, viewPort)
        { }

        protected override List<Entity> DrawByShape()
        {
            var entities = new List<Entity>();
            var agvMesh = Mesh.CreateBox(DrawSetting.Width, DrawSetting.Depth, DrawSetting.Height);
            agvMesh.ColorMethod = colorMethodType.byParent;
            agvMesh.EdgeStyle = Mesh.edgeStyleType.Sharp;
            agvMesh.Translate(new Vector3D(-DrawSetting.Width / 2, -DrawSetting.Depth / 2));
            entities.Add(agvMesh);

            var agvHeadMesh = Mesh.CreateBox(DrawSetting.Width / 4, DrawSetting.Depth, DrawSetting.Height * 1.5);
            agvHeadMesh.ColorMethod = colorMethodType.byEntity;
            agvHeadMesh.Color = Color.Yellow;
            agvHeadMesh.EdgeStyle = Mesh.edgeStyleType.Sharp;
            agvHeadMesh.Translate(new Vector3D(DrawSetting.Width / 4, -DrawSetting.Depth / 2));
            entities.Add(agvHeadMesh);

            return entities;
        }
    }
}
