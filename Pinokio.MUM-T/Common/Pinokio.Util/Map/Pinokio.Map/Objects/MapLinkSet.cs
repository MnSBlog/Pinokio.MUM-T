using System;
using System.Linq;
using System.Collections.Generic;

using Pinokio.Core;

namespace Pinokio.Map
{
    public class MapLinkSet
    {
        private DistinctList<MapLink> _links;
        private DistinctList<MapNode> _nodes;
        private double _weightSum;
        private Enum _type;
        public DistinctList<MapLink> Links { get => _links; }
        public DistinctList<MapNode> Nodes { get => _nodes; }
        public Enum Type { get => _type; }
        public int Count { get => _links.Count; }

        public MapLinkSet()
        {
            _links = new DistinctList<MapLink>();
            _nodes = new DistinctList<MapNode>();
            _weightSum = 0;
        }

        public MapLinkSet(Enum type)
        {
            _links = new DistinctList<MapLink>();
            _nodes = new DistinctList<MapNode>();
            _weightSum = 0;
            _type = type;
        }

        public void AddLink(MapLink link)
        {
            if (!_links.Contains(link))
            {
                _links.Add(link);
                _weightSum += link.Weight;
            }

            _nodes.Add(link.FromNode);
            _nodes.Add(link.ToNode);
        }

        public void ConnectLink(MapLink nextLink)
        {
            try
            {
                if(_links.Contains(nextLink)) throw new Exception("이미 포함된 링크입니다.");

                if (_links.Count == 0)
                {
                    _nodes.Add(nextLink.ToNode);
                }
                else if (_links.Count > 0)
                {
                    var lastLink = _links.Last();
                    if (lastLink.ToNode != nextLink.FromNode)
                        throw new Exception("합칠 수 없는 링크입니다.");
                }

                _links.Add(nextLink);
                _nodes.Add(nextLink.ToNode);
                _weightSum += nextLink.Weight;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public bool ContainsLink(MapLink link)
        {
            if (_links is null) return false;
            else if (_links.Count == 0) return false;
            else
                return _links.Contains(link);
        }

        public bool ContainsNode(MapNode node)
        {
            if (_nodes is null) return false;
            else if (_nodes.Count == 0) return false;
            else
                return _nodes.Contains(node);
        }

        public bool ContainsNodeName(string nodeName)
        {
            if (_nodes.Count == 0) return false;
            else
            {
                return _nodes.Any(node => node.Name == nodeName);
            }
        }

        public MapLink this[int key]
        {
            get => _links[key];
            set => _links[key] = value;
        }
    }
}