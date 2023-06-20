using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;

namespace Pinokio.Graph
{
    public class MapAbstractObject : AbstractObject
    {
        private uint _mapId;
        public uint MapId { get => _mapId; }

        public MapAbstractObject(uint mapId, uint id, Enum type = null) : base(id, type)
        {
            _mapId = mapId;
        }

        public MapAbstractObject(uint mapId, uint id, string name, Enum type = null) : base(id, name, type)
        {
            _mapId = mapId;
        }
    }
}
