using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;
using Pinokio.Geometry;

using devDept.Geometry;
using devDept.Eyeshot.Entities;

namespace Pinokio._3D.Eyeshot
{
    public class EConveyorShape : EUniqueShape
    {
        public EConveyorShape(uint id, ConcreteObject conObj, ViewPort viewPort) : base(id, conObj, viewPort)
        { }

        protected override List<Entity> DrawByShape()
        {
            var eDrawSetting = DrawSetting as EyeshotDrawSetting;
            var entities = new List<Entity>();
            var convBody = EyeshotCADMart.GetCADByName(eDrawSetting.BlockName);

            var bodySize = new Vector3D(DrawSetting.Width, DrawSetting.Depth, DrawSetting.Height * 0.2);
            var bodyPos = new Vector3D(-DrawSetting.Width / 2, -DrawSetting.Depth / 2, DrawSetting.Height * 0.8);
            foreach (var cbEnt in convBody)
            {
                cbEnt.Scale(bodySize.X, bodySize.Y, bodySize.Z);
                cbEnt.Translate(bodyPos);
                entities.Add(cbEnt);
            }

            var convLeg = EyeshotCADMart.GetCADByName("ConvLegs");

            var legSize = new Vector3D(100, DrawSetting.Depth, DrawSetting.Height * 0.8);
            foreach (var clEnt in convLeg)
            {
                clEnt.Scale(legSize.X, legSize.Y, legSize.Z);
                var clEnt2 = (Entity)clEnt.Clone();

                clEnt.Translate(0, DrawSetting.Width * 3 / 7);
                clEnt2.Translate(0, -DrawSetting.Width * 3 / 7);
                entities.Add(clEnt);
                entities.Add(clEnt2);
            }

            return entities;
        }
    }
}
