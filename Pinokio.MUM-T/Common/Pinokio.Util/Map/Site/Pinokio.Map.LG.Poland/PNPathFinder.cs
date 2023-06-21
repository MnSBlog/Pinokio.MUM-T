using System.Collections.Generic;

namespace Pinokio.Map.LG.Poland
{
    public class PNConfig
    {
        private uint _mapId;
        private List<MapNode> _destinations;
        public uint MapId { get => _mapId; }
        public List<MapNode> Destinations { get => _destinations; }
        public PNConfig(uint mapId, List<MapNode> destinations)
        {
            _mapId = mapId;
            _destinations = destinations;
        }

    }
    public class PNPathFinder : PathFinder
    {
        private Dictionary<uint, PNConfig> _configs;

        public PNPathFinder()
        {
            _configs = new Dictionary<uint, PNConfig>();
        }

        public void AddConfig(PNConfig config)
        {
            if (!_configs.ContainsKey(config.MapId))
            {
                _configs.Add(config.MapId, config);
            }
        }

        public override PinokioPath FindPath(PinokioGraph graph, string fromId, string toId, List<MapNode> excludingNodes, List<MapLink> excludingLinks, PathType type)
        {
            var exNodes = FindExcludingDestinations(graph.Id, fromId, toId);
            if (excludingNodes != null)
                exNodes.AddRange(excludingNodes);

            return SPAlgorithm.FindShortestPath(graph, fromId, toId, exNodes, excludingLinks);
        }

        public override List<PinokioPath> FindKShortestPaths(PinokioGraph graph, string fromId, string toId, int K, PathType type)
        {
            var excludingNodes = FindExcludingDestinations(graph.Id, fromId, toId);
            return KSPAlgorithm.FindKShortestPaths(new PinokioGraph(graph), fromId, toId, K, excludingNodes, new List<MapLink>());
        }

        private List<MapNode> FindExcludingDestinations(uint mapId, string fromId, string toId)
        {
            var excludingNodes = new List<MapNode>();
            if (_configs.ContainsKey(mapId))
            {
                foreach (var node in _configs[mapId].Destinations)
                {
                    if (node.Name == fromId || node.Name == toId)
                        continue;
                    else
                        excludingNodes.Add(node);
                }
            }
            return excludingNodes;
        }
    }
}
