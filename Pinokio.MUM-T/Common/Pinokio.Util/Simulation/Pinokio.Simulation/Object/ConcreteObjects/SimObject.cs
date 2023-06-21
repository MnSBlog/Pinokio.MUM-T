using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;
using Pinokio.Geometry;

namespace Pinokio.Simulation
{
    public class SimObject : ConcreteObject
    {
        protected Enum St;
        protected Vector3 Pos;
        protected Vector3 Dir;
        
        public Enum State { get => St; }
        public Vector3 Position { get => Pos; }
        public Vector3 Direction { get => Dir; }

        public SimObject(uint id, Enum type = null) : base(id, type)
        { }
        public SimObject(uint id, string name, Enum type = null) : base(id, name, type)
        { }

        public override void Initialize()
        {
            base.Initialize();
            St = null;
            Pos = Vector3.Zero;
            Dir = Vector3.UnitX;
        }
    }
}
