using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;
using Pinokio._3D;

using devDept.Geometry;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;

namespace Pinokio._3D.Eyeshot
{
    public class EFloorShape : EUniqueShape
    {
        private double _warningSize = 1000; // 1m
        public EFloorShape(uint id, ViewPort viewPort) : base(id, viewPort)
        { }

        protected override List<Entity> DrawByShape()
        {
            this.SetPosition(Geometry.Vector3.Center(DrawSetting.FromPos, DrawSetting.ToPos));
            double width = (DrawSetting.ToPos.X - DrawSetting.FromPos.X) + 10000;
            double depth = (DrawSetting.ToPos.Y - DrawSetting.FromPos.Y) + 10000;
            double height = 1;

            var entities = new List<Entity>();
            var mainFloor = Mesh.CreateBox(width, depth, height);
            mainFloor.ApplyMaterial("White", textureMappingType.Plate, 1, 1);
            mainFloor.Translate(-width/2, -depth/2, -height- 0.1);
            entities.Add(mainFloor);

            Action<Vector3D, Vector3D> AddWarningZone = (size, pos) =>
            {
                var newWarningZone = EyeshotCADMart.CreateBox(size, "Warning", pos);
                entities.Add(newWarningZone);
            };

            var hWarningZoneSize = new Vector3D(_warningSize, depth + 2 * _warningSize, height);
            var hWarningZonePos1 = new Vector3D(-width/2 - _warningSize, -depth/2 - _warningSize, -height - 0.1);
            var hWarningZonePos2 = new Vector3D(width/2, -depth/2 - _warningSize, -height - 0.1);
            AddWarningZone(hWarningZoneSize, hWarningZonePos1);
            AddWarningZone(hWarningZoneSize, hWarningZonePos2);

            var vWarningZoneSize = new Vector3D(width, _warningSize, height);
            var vWarningZonePos1 = new Vector3D(-width/2, -depth /2 - _warningSize, -height - 0.1);
            var vWarningZonePos2 = new Vector3D(-width/2, depth /2, -height - 0.1);
            AddWarningZone(vWarningZoneSize, vWarningZonePos1);
            AddWarningZone(vWarningZoneSize, vWarningZonePos2);

            return entities;
        }
    }
}
