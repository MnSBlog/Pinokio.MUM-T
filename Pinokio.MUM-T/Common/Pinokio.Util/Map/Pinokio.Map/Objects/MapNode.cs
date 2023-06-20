using System;
using System.ComponentModel;
using System.Collections.Generic;

using Pinokio.Core;
using Pinokio.Geometry;

namespace Pinokio.Map
{
    public enum MapNodeType
    {
        Main, Site, SiteInput, SiteOutput, Virtual
    }

    public class MapNode : MapObject 
    {
        #region Private 
        private Vector3 _pos;
        private List<MapLink> _inEdges;
        private List<MapLink> _outEdges;
        
        private List<string> _aliases;
        private List<Polygon> _sweepingVolumes;
        private bool _noRotation;
        #endregion

        #region Properties
        [PinokioCategory("Node", 1)]
        public Vector3 Position { get { return _pos; } }

        [PinokioCategory("Node", 1)]
        public List<MapLink> InLinks { get { return _inEdges; } }

        [PinokioCategory("Node", 1)]
        public List<MapLink> OutLinks { get { return _outEdges; } }

        [Browsable(false)]
        public List<string> AliasNodes { get { return _aliases; } }

        [Browsable(false)]
        public List<Polygon> SweepingVolumes { get { return _sweepingVolumes; } }
        [Browsable(false)]
        public bool NoRotation { get { return _noRotation; } }
        #endregion

        public MapNode(uint mapId, uint id, string name, Vector3 posVec3, MapNodeType nodeType) : base(mapId, id, name, nodeType)
        {
            _pos = posVec3;
            _noRotation = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            _inEdges = new List<MapLink>();
            _outEdges = new List<MapLink>();
            _aliases = new List<string>();
            _sweepingVolumes = new List<Polygon>();
        }

        public void SetNoRotation(bool noRotation)
        {
            _noRotation = noRotation;
        }

        public bool IsVeryClose(MapNode other)
        {
            var thisPos = new Vector2(this.Position.X, this.Position.Y);
            var otherPos = new Vector2(other.Position.X, other.Position.Y);
            if (this.IsVeryClose(otherPos))
                return true;
            else if (other.IsVeryClose(thisPos))
                return true;
            else
                return false;
        }

        public bool IsVeryClose(Vector2 pos)
        {
            foreach (var thisSV in _sweepingVolumes)
            {
                if (thisSV.IsInPolygon(pos))
                    return true;
            }

            return false;
        }

        public bool IsNeighbor(MapNode other)
        {
            foreach (var thisSV in _sweepingVolumes)
            {
                foreach (var otherSV in other.SweepingVolumes)
                {
                    if (thisSV.IsIntersect(otherSV))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class VirtualNode : MapNode
    {
        private string _linkName;
        private double _offset;
        public string LinkName { get { return _linkName; } }
        public double Offset { get { return _offset; } }

        public VirtualNode(uint mapId, uint id, Vector3 pos, string linkName, double offset)
            : base(mapId, id, "v"+id, pos, MapNodeType.Virtual)
        {
            _linkName = linkName;
            _offset = offset;
        }
    }
}
