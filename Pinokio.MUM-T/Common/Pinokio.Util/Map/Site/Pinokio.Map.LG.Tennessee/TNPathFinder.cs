using System.Collections.Generic;

using Pinokio.Map.Algorithms;

namespace Pinokio.Map.LG.Tennessee
{
    public enum TNLinkType
    {
        None,
        Empty,
        Load,
    }

    public class TNLinkConfig
    {
        private uint _mapId;
        private List<MapLink> _emptyLinks;
        private List<MapLink> _loadLinks;
        private List<MapLinkSet> _linkSets;

        public uint MapId { get => _mapId; }
        public List<MapLink> EmptyLinks { get { return _emptyLinks; } }
        public List<MapLink> LoadLinks { get { return _loadLinks; } }
        public List<MapLinkSet> LinkSets { get => _linkSets; }

        public TNLinkConfig(uint mapId)
        {
            _mapId = mapId;
            _emptyLinks = new List<MapLink>();
            _loadLinks = new List<MapLink>();
            _linkSets = new List<MapLinkSet>();
        }

        public void AddLink(MapLink link, TNLinkType loadType)
        {
            link.SetSubType(loadType);
            switch (loadType)
            {
                case TNLinkType.Empty:
                    _emptyLinks.Add(link);
                    break;
                case TNLinkType.Load:
                    _loadLinks.Add(link);
                    break;
            }
        }

        public void ConstructMap()
        {
            _linkSets.Clear();

            this.ConstructLinkSet(_loadLinks, TNLinkType.Load);
            this.ConstructLinkSet(_emptyLinks, TNLinkType.Empty);
        }

        private void ConstructLinkSet(List<MapLink> links, TNLinkType loadType)
        {
            foreach (MapLink link in links)
            {
                bool isContained = false;
                foreach (var set in _linkSets)
                {
                    if (set.ContainsLink(link))
                    {
                        isContained = true;
                        break;
                    }
                }

                if (!isContained)
                {
                    var newSet = new MapLinkSet(loadType);
                    SearchConnectivity(link, ref newSet);
                    _linkSets.Add(newSet);
                }
            }
        }

        private void SearchConnectivity(MapLink link, ref MapLinkSet set)
        {
            if ((TNLinkType)link.SubType != (TNLinkType)set.Type) return;
            if (set.ContainsLink(link)) return;

            set.AddLink(link);
            foreach (MapLink inLink in link.FromNode.InLinks)
                SearchConnectivity(inLink, ref set);
            foreach (MapLink outLink in link.ToNode.OutLinks)
                SearchConnectivity(outLink, ref set);

            foreach (MapLink inLink in link.ToNode.InLinks)
                SearchConnectivity(inLink, ref set);
            foreach (MapLink outLink in link.ToNode.OutLinks)
                SearchConnectivity(outLink, ref set);
        }

    }
    public class TNPathFinder : PathFinder
    {
        private Dictionary<uint, TNLinkConfig> _linkConfigs;

        public TNPathFinder()
        {
            _linkConfigs = new Dictionary<uint, TNLinkConfig>();
            SPAlgorithm = new TurnDijkstraAlgorithm();
        }

        public void AddLinkConfiguration(TNLinkConfig linkConfig)
        {
            _linkConfigs.Add(linkConfig.MapId, linkConfig);
        }

        public override PinokioPath FindPath(PinokioGraph graph, string fromId, string toId, List<MapNode> excludingNodes, List<MapLink> excludingLinks, PathType type)
        {
            var exLinks = FindExcludingLinks(graph.Id, fromId, toId, type);
            if (excludingLinks != null)
                exLinks.AddRange(excludingLinks);
            return SPAlgorithm.FindShortestPath(graph, fromId, toId, excludingNodes, excludingLinks);
        }

        public override List<PinokioPath> FindKShortestPaths(PinokioGraph graph, string fromId, string toId, int K, PathType type)
        {
            var excludingLinks = FindExcludingLinks(graph.Id, fromId, toId, type);
            return KSPAlgorithm.FindKShortestPaths(new PinokioGraph(graph), fromId, toId, K, new List<MapNode>(), excludingLinks);
        }

        private List<MapLink> FindExcludingLinks(uint mapId, string fromId, string toId, PathType type)
        {
            var excludingLinks = new List<MapLink>();
            if (_linkConfigs.ContainsKey(mapId))
            {
                List<MapLink> loadLinks = _linkConfigs[mapId].LoadLinks;
                List<MapLink> emptyLinks = _linkConfigs[mapId].EmptyLinks;
                List<MapLinkSet> linkSets = _linkConfigs[mapId].LinkSets;
                List<MapLink> otherLinks = new List<MapLink>();
                var loadType = TNLinkType.None;
                switch (type)
                {
                    case PathType.PreDrive: // Empty
                        loadType = TNLinkType.Empty;
                        excludingLinks.AddRange(loadLinks);
                        otherLinks.AddRange(emptyLinks);
                        break;
                    case PathType.MainDrive: // Load
                        loadType = TNLinkType.Load;
                        excludingLinks.AddRange(emptyLinks);
                        otherLinks.AddRange(loadLinks);
                        break;
                }

                foreach (var set in linkSets)
                {
                    if ((TNLinkType)set.Type != loadType) continue;
                    else if (set.ContainsNodeName(fromId) || set.ContainsNodeName(toId))
                    {
                        foreach (var link in set.Links)
                            otherLinks.Remove(link);
                    }
                }

                excludingLinks.AddRange(otherLinks);
            }
            return excludingLinks;
        }
    }
}
