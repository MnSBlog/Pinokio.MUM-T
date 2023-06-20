using System.Collections.Generic;
using System.Drawing;

using Pinokio.Core;
using Pinokio.Simulation;

using devDept.Geometry;
using devDept.Eyeshot.Entities;


namespace Pinokio._3D.Eyeshot
{
    public class EOHTShape : EMovableShape
    {
        public EOHTShape(uint id, ConcreteObject conObj, ViewPort viewPort) : base(id, conObj, viewPort)
        { }

        protected override List<Entity> DrawByShape()
        {
            var entities = new List<Entity>();
            var ohtMesh = EyeshotCADMart.CreateTriangle(DrawSetting.Width);
            ohtMesh.ColorMethod = colorMethodType.byParent;
            entities.Add(ohtMesh);

            return entities;
        }

        private string _lastState = "Idle";
        public override void UpdateColor()
        {
            var state = ((SimModel)this.Core).State.ToString();
            if (_lastState != state)
            {
                var color = OHTColors.Idle;
                switch (state)
                {
                    case "Loading":
                    case "Unloading":
                        color = OHTColors.Working;
                        break;
                    case "PreDrive":
                        color = OHTColors.PreDriving;
                        break;
                    case "MainDrive":
                        color = OHTColors.MainDriving;
                        break;
                }

                foreach (var ent in this.Entities)
                {
                    ent.Color = color;
                }

                _lastState = state;
            }
        }
    }
}
