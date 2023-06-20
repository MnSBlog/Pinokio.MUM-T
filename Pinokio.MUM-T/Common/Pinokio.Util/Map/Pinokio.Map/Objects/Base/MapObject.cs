using System;
using System.ComponentModel;

using Pinokio.Core;

namespace Pinokio.Map
{
    public class MapObject : AbstractObject
    {
        private uint _mapId;

        [Browsable(false)]
        public uint MapId { get => _mapId; }

        public MapObject(uint mapId, uint id, Enum type = null) : base(id, type)
        {
            _mapId = mapId;
        }

        public MapObject(uint mapId, uint id, string name, Enum type = null) : base(id, name, type)
        {
            _mapId = mapId;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
