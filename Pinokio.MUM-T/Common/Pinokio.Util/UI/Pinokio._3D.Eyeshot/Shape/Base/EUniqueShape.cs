using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;
using Pinokio._3D;

using devDept.Eyeshot;
using devDept.Eyeshot.Entities;

namespace Pinokio._3D.Eyeshot
{
    /// <summary>
    /// 모든 객체의 크기와 형태가 다른 Shape
    /// Block & BlockReference를 통해 그릴 수 없다.
    /// </summary>
    public class EUniqueShape : EyeshotShape
    {
        public EUniqueShape(uint id, ConcreteObject conObj, ViewPort viewPort) : base(id, conObj, viewPort)
        { }

        public EUniqueShape(uint id, AbstractObject absObj, ViewPort viewPort) : base(id, absObj, viewPort)
        { }

        public EUniqueShape(uint id, ViewPort viewPort) : base(id, viewPort)
        { }

        protected override List<Entity> GetEyeshotEntities()
        {
            var entities = new List<Entity>();
            entities.AddRange(DrawByShape());
            return entities;
        }
    }
}
