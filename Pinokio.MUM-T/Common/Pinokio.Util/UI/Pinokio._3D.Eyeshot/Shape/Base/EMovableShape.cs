using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;
using Pinokio.Simulation;
using devDept.Geometry;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;

namespace Pinokio._3D.Eyeshot
{
    public class EMovableShape : EyeshotShape
    {
        public EMovableShape(uint id, ConcreteObject conObj, ViewPort viewPort) : base(id, conObj, viewPort)
        { }

        protected override List<Entity> DrawByBlock()
        {
            var eDrawSetting = DrawSetting as EyeshotDrawSetting;
            var blockRef = new Translating(eDrawSetting.BlockName);
            blockRef.ColorMethod = colorMethodType.byEntity;
            blockRef.Color = DrawSetting.MainColor;
            return new List<Entity> { blockRef };
        }

        public override void Draw()
        {
            var entities = GetEyeshotEntities();

            foreach (var ent in entities)
            {
                ent.EntityData = this;
                //ViewPort.Entities.Add(ent);
            }

            this.SetEyeshotEntities(entities);
        }

        public override void UpdatePosition()
        {
            if (this.Core is SimObject simObj)
            {
                this.SetPosition(simObj.Position);
                this.SetDirection(simObj.Direction);
            }
            UpdateColor();
        }

        public virtual void UpdateColor()
        {
            
        }
    }
}
