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
    public class EyeshotShape : Shape
    {
        protected ViewPort ViewPort;
        private List<Entity> _entities;

        public List<Entity> Entities { get => _entities; }

        public EyeshotShape(uint id, ConcreteObject conObj, ViewPort viewPort) : base(id, conObj)
        {
            ViewPort = viewPort;
        }

        public EyeshotShape(uint id, AbstractObject absObject, ViewPort viewPort) : base(id, absObject)
        {
            ViewPort = viewPort;
        }

        public EyeshotShape(uint id, ViewPort viewPort) : base(id, "")
        {
            ViewPort = viewPort;

        }

        public override void Initialize()
        {
            base.Initialize();
            _entities = new List<Entity>();
        }

        public override void Draw()
        {
            List<Entity> entities = GetEyeshotEntities();

            var tra = new Translation(EyeshotHelper.ToVector3D(this.Pos));
            var rot = new Rotation(this.Radian, Vector3D.AxisZ);
            var combined = tra * rot;
            foreach (var ent in entities)
            {
                ent.TransformBy(combined);
                ent.EntityData = this;
            }
            //entities.Add(EyeshotCADMart.CreateBox(new Vector3D(100, 100, 100), System.Drawing.Color.AliceBlue, EyeshotHelper.ToVector3D(this.Pos)));
            SetEyeshotEntities(entities);
        }

        protected virtual void SetEyeshotEntities(List<Entity> entities)
        {
            _entities = entities;
            ViewPort.Entities.AddRange(_entities);
        }

        protected virtual List<Entity> GetEyeshotEntities()
        {
            var eDrawSetting = DrawSetting as EyeshotDrawSetting;
            var entities = new List<Entity>();
            if (eDrawSetting.DrawByBlock)
            {
                if (!ViewPort.Blocks.Contains(eDrawSetting.BlockName))
                {
                    var newBlock = GenerateEyeshotBlock();
                    ViewPort.Blocks.Add(newBlock);
                }
                entities.AddRange(DrawByBlock());
            }
            else
            {
                entities.AddRange(DrawByShape());
            }

            return entities;
        }

        protected virtual List<Entity> DrawByBlock()
        {
            var eDrawSetting = DrawSetting as EyeshotDrawSetting;
            var blockRef = new BlockReference(eDrawSetting.BlockName);
            return new List<Entity> { blockRef };
        }

        protected virtual List<Entity> DrawByShape()
        {
            return null;
        }

        protected virtual Block GenerateEyeshotBlock()
        {
            var eDrawSetting = DrawSetting as EyeshotDrawSetting;
            List<Entity> entities = DrawByShape();
            if(entities != null)
            {
                var block = new Block(eDrawSetting.BlockName);
                block.Entities.AddRange(entities);
                return block;
            }

            return null;
        }

        public virtual void UpdateColor(object obj)
        {
            
        }
    }
}
