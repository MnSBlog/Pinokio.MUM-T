using System;
using Pinokio.Core;

namespace Pinokio.Map
{
    public class MapNodeSet
    {
        #region Variables
        private uint _id;
        private bool _isTerminal;
        private bool _isConnected;
        private DistinctList<MapNode> _nodes;
        private DistinctList<MapNodeSet> _neighbors;
        private DistinctList<MapNodeSet> _prevSets;
        private DistinctList<MapNodeSet> _nextSets;
        #endregion

        #region Properties
        public uint Id { get { return _id; } }
        public bool IsTerminal { get => _isTerminal; set => _isTerminal = value; }
        public bool IsConnected { get => _isConnected; set => _isConnected = value; }
        public DistinctList<MapNode> Nodes { get { return _nodes; } }
        public DistinctList<MapNodeSet> Neighbors { get { return _neighbors; } }
        public DistinctList<MapNodeSet> PrevSets { get { return _prevSets; } }
        public DistinctList<MapNodeSet> NextSets { get { return _nextSets; } }
        public int Count { get => _nodes.Count; }

        #endregion

        public MapNodeSet(uint id, MapNode node)
        {
            _id = id;
            _isTerminal = false;
            _isConnected = true;
            _nodes = new DistinctList<MapNode>() { node };
            _neighbors = new DistinctList<MapNodeSet>();
            _prevSets = new DistinctList<MapNodeSet>();
            _nextSets = new DistinctList<MapNodeSet>();
        }

        public void AddNeighbor(MapNodeSet other)
        {
            if (!_neighbors.Contains(other))
                _neighbors.Add(other);

            if (!other.Neighbors.Contains(this))
                other.Neighbors.Add(this);
        }

        public MapNode this[int key]
        {
            get => _nodes[key];
            set => _nodes[key] = value;
        }
    }
}