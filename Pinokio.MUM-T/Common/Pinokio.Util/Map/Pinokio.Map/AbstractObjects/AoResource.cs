using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;
using Pinokio.Geometry;
using Pinokio.Graph;

namespace Pinokio.Map
{
    public enum ResourceType
    {
        None = 0, Commit, Complete, Stocker, Process, Buffer, WaitingSpot, Port
    }

    public class AoResource : MapAbstractObject
    {
        private Vector3 _position;
        private Vector3 _size;
        private Vector3 _direction;
        private MapNode _inPort;
        private MapNode _outPort;
        private Distribution _tactTime;
        private Distribution _cycleTime;
        private Distribution _loadingTime;
        private Distribution _unloadingTime;
        private List<MapNode> _destinations;
        private Location _location;

        public Vector3 Position { get => _position; }
        public Vector3 Size { get => _size; }
        public Vector3 Direction { get => _direction; }
        public MapNode InPort { get => _inPort; }
        public MapNode OutPort { get => _outPort; }
        public Distribution TactTime { get => _tactTime; }
        public Distribution CycleTime { get => _cycleTime; }
        public Distribution LoadingTime { get => _loadingTime; }
        public Distribution UnloadingTime { get => _unloadingTime; }
        public List<MapNode> Destinations { get => _destinations; }
        public Location Location { get => _location; }
        public AoResource(uint mapId, uint id, string name, ResourceType type) : base(mapId, id, name, type)
        {
            _destinations = new List<MapNode>();
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        public void SetSize(Vector3 size)
        {
            _size = size;
        }

        public void SetDirection(Vector3 direction)
        {
            _direction = direction;
        }

        public void SetInOutPort(MapNode inPort, MapNode outPort)
        {
            _inPort = inPort;
            _outPort = outPort;
        }

        public void SetTactTime(Distribution dist)
        {
            _tactTime = dist;
        }

        public void SetCycleTime(Distribution dist)
        {
            _cycleTime = dist;
        }

        public void SetLoadingTime(Distribution dist)
        {
            _loadingTime = dist;
        }

        public void SetUnloadingTime(Distribution dist)
        {
            _unloadingTime = dist;
        }

        public void SetLocation(Location location)
        {
            _location = location;
            if (location.Node is null)
            {
                this.SetPosition(location.Link.GetPosition(location.Offset));
            }
            else
            {
                this.SetPosition(location.Node.Position);
            }
        }

        public Vector3 GetPosition()
        {
            if (InPort == null && OutPort != null)
                return OutPort.Position;
            else if (InPort != null && OutPort == null)
                return InPort.Position;
            else if (InPort != null && OutPort != null)
                return Vector3.Center(InPort.Position, OutPort.Position);
            else
                return Vector3.Zero;
        }
    }
}