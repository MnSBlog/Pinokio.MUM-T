using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;

namespace Pinokio.Simulation
{
    public class SimPort : AbstractObject
    {
        private SimModel _fromModel;
        private object _fromObject;
        private SimModel _toModel;
        private object _toObject;
        private SimEntity _entity;

        public SimModel FromModel { get => _fromModel; }
        public object FromObject { get => _fromObject; set => _fromObject = value; }
        public SimModel ToModel { get => _toModel; }
        public object ToObject { get => _toObject; set => _toObject = value; }
        public SimEntity Entity { get => _entity; set => _entity = value; }


        public SimPort(Enum type) : base(0, type)
        {
        }
        public SimPort(Enum type, SimEntity entity) : base(0, type)
        {
            _entity = entity;
        }

        public SimPort(Enum type, SimModel fromModel, SimEntity entity = null) : base(0, type)
        {
            _fromModel = fromModel;
            _entity = entity;
        }

        public SimPort(Enum type, SimModel fromModel, SimModel toModel) : base(0, type)
        {
            _fromModel = fromModel;
            _toModel = toModel;
        }

        public SimPort(Enum type, SimModel fromModel, SimModel toModel, SimEntity entity = null) : base(0, type)
        {
            _fromModel = fromModel;
            _toModel = toModel;
            _entity = entity;
        }
    }
}
