using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;

namespace Pinokio.Simulation
{
    public class SimLink : AbstractObject
    {
        private SimModel _fromModel;
        private SimModel _toModel;

        public SimLink(uint id, SimModel fromModel, SimModel toModel) : base(id)
        {
            _fromModel = fromModel;
            _toModel = toModel;
        }
    }
}
