using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;
using Pinokio.Geometry;

namespace Pinokio.Simulation
{
    public class SimProduct : AbstractObject
    {
        private List<uint> _steps;
        private Vector3 _size;

        public SimProduct(uint id, string name) : base(id, name)
        { }

        public override void Initialize()
        {
            base.Initialize();
            _steps = new List<uint>();
        }

    }
}
