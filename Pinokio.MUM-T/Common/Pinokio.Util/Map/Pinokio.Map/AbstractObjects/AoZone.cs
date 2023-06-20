using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;
using Pinokio.Graph;
using Pinokio.Geometry;

namespace Pinokio.Map
{
    public class AoZone : MapAbstractObject, IEquatable<AoZone>
    {
        #region Member Variables
        private bool _isTerminal;
        private bool _isConnected;
        private bool _isOneWay;
        private Vector3 _centerPos;
        private List<MapNode> _nodes;
        private List<MapLink> _links;
        private List<MapPoint> _points;

        private DistinctList<MapNodeSet> _nodeSets;
        private List<AoZone> _neighbors;
        private DistinctList<AoZone> _nextZones;
        private DistinctList<AoZone> _prevZones;
        #endregion

        #region Properties
        public bool IsTerminal { get => _isTerminal; set => _isTerminal = value; }
        public bool IsConnected { get => _isConnected; set => _isConnected = value; }
        public bool IsOneWay { get => _isOneWay; set => _isOneWay = value; }
        public Vector3 CenterPos { get => _centerPos; }
        public List<MapNode> Nodes { get => _nodes; }
        public List<MapLink> Links { get => _links; }
        public List<MapPoint> Points { get => _points; }
        public DistinctList<MapNodeSet> NodeSets { get => _nodeSets; }
        public List<AoZone> Neighbors { get => _neighbors; }
        public DistinctList<AoZone> NextZones { get => _nextZones; }
        public DistinctList<AoZone> PrevZones { get => _prevZones; }
        #endregion

        public AoZone(uint mapId, uint id, Enum type) : base(mapId, id, type)
        { }

        public AoZone(uint mapId, uint id) : base(mapId, id)
        { }

        public override void Initialize()
        {
            base.Initialize();
            _centerPos = Vector3.Zero;
            _isConnected = true;
            _isTerminal = false;
            _isOneWay = false;

            _nodes = new List<MapNode>();
            _links = new List<MapLink>();
            _points = new List<MapPoint>();

            _nodeSets = new DistinctList<MapNodeSet>();

            _neighbors = new List<AoZone>();
            _nextZones = new DistinctList<AoZone>();
            _prevZones = new DistinctList<AoZone>();
        }

        #region [Interface Implementation]
        #region [::IEquatable]
        public override int GetHashCode()
        {
            return _links.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as AoZone);
        }

        public bool Equals(AoZone other)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(other, null)) return false;

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType()) return false;

            // If components counts are not exactly the same, return false.
            if (_links.Count != other.Links.Count) return false;

            if (this.Id != other.Id) return false;

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other)) return true;

            for (int i = 0; i < _links.Count; i++)
            {
                if (!other.ContainsLink(_links[i]))
                    return false;
            }

            return true;
        }
        #endregion [IEquatable::]
        #endregion

        #region Zone Construction
        public virtual void AddLink(MapLink link)
        {
            if (_links.Contains(link)) return;

            _links.Add(link);
            this.AddNode(link.FromNode);
            this.AddNode(link.ToNode);

            this.AddPoints(link.Points.ToList());
            this.AddPoints(new List<MapPoint>() { link.TurnPoint });
        }

        public virtual void AddNode(MapNode node)
        {
            if (_nodes.Contains(node)) return;
            _nodes.Add(node);
        }

        public virtual void AddPoints(List<MapPoint> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                AddPoint(points[i]);
            }
        }

        public virtual void AddPoint(MapPoint point)
        {
            if (_points.Contains(point)) return;
            _points.Add(point);
        }
        #endregion Zone Construction

        public void SetCenterPos(Vector3 pos)
        {
            _centerPos = pos;
        }

        public bool ContainsLink(MapLink item)
        {
            return _links.Contains(item);
        }

        public bool ContainsNode(MapNode node)
        {
            return _nodes.Contains(node);
        }

        public void AddNodeSet(MapNodeSet nodeSet)
        {
            _nodeSets.Add(nodeSet);
            foreach (var node in nodeSet.Nodes)
            {
                if (!_nodes.Contains(node))
                    _nodes.Add(node);
            }
        }

        public void AddNeighbor(AoZone neighbor)
        {
            if (!_neighbors.Contains(neighbor))
            {
                _neighbors.Add(neighbor);
            }
        }
        public void AddRangeNodeSet(DistinctList<MapNodeSet> nodeSets)
        {
            foreach (var nodeSet in nodeSets)
            {
                AddNodeSet(nodeSet);
            }
        }
        #region [Operator Override]
        public static bool operator ==(AoZone section1, AoZone section2)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(section1, null))
            {
                if (Object.ReferenceEquals(section2, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return section1.Equals(section2);
        }

        public static bool operator !=(AoZone section1, AoZone section2)
        {
            return !(section1 == section2);
        }

        public override string ToString()
        {
            return this.Id.ToString();
        }
        #endregion
    }
}