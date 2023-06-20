using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;
using Pinokio.Geometry;

namespace Pinokio.Simulation
{
    public class SimEntity : SimObject
    {
        public SimEntity() : base(0)
        { }
        public SimEntity(uint id, Enum type = null) : base(id, type)
        { }
        public SimEntity(uint id, string name, Enum type = null) : base(id, name, type)
        { }

        public override void Initialize()
        {
            base.Initialize();
        }

        public virtual void SetState(Enum state)
        {
            this.St = state;
        }

        public virtual void SetPosition(Vector3 pos)
        {
            this.Pos = pos;
        }

        public virtual void SetDirection(Vector3 direction)
        {
            this.Dir = direction;
        }
    }
}
