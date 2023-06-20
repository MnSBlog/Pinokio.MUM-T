using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Geometry;

namespace Pinokio.Map
{
    [Flags]
    public enum MapPortType
    {
        None = 0,
        In = 1, // 01
        Out = 2, // 10
        InOut = 3, // 11
    }
    public class MapPort : MapObject
    {
        private Location _location;
        public Location Location { get => _location; }

        public MapPort(uint mapId, uint id, string name, Location location, MapPortType type) : base(mapId, id, name, type)
        {
            _location = location;
        }
        public MapPort(uint mapId, uint id, string name, MapPortType type) : base(mapId, id, name, type)
        {

        }
    }
}
