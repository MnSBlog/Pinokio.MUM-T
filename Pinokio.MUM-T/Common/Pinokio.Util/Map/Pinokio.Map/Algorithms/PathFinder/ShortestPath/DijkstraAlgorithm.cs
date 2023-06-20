using System;
using System.Collections.Generic;
using System.Linq;

namespace Pinokio.Map.Algorithms
{
    public class PathSet
    {
        private object _lockObject = new object();
        private uint _graphId;
        private List<string> _checkedFromLocation;
        private Dictionary<(string, string), PinokioPath> _paths;

        public PathSet(uint graphId)
        {
            _graphId = graphId;
            _checkedFromLocation = new List<string>();
            _paths = new Dictionary<(string, string), PinokioPath>();
        }

        public bool IsAlreadyCalculated(string from)
        {
            if (_checkedFromLocation.Contains(from))
                return true;
            else
                return false;
        }

        public PinokioPath this[string from, string to]
        {
            get => _paths[(from, to)];
        }

        public void AddPaths(string from, Dictionary<(string, string), PinokioPath> paths)
        {
            lock (_lockObject)
            {
                if (!_checkedFromLocation.Contains(from))
                {
                    _checkedFromLocation.Add(from);
                    foreach (var tup in paths)
                    {
                        _paths.Add(tup.Key, tup.Value);
                    }
                }
            }
        }
    }
    /// <summary>
    /// Path Finder : Shortest Path Algorithm
    /// DijkstraAlgorithm
    /// </summary>
    public class DijkstraAlgorithm : ShortestPathAlgorithm
    {
        public int Calculate = 0;
        public int Recycle = 0;
        private Dictionary<PinokioGraph, PathSet> _pathSets;
        public DijkstraAlgorithm()
        {
            _pathSets = new Dictionary<PinokioGraph, PathSet>();
        }

        public void AddGraph(PinokioGraph graph)
        {
            if (!_pathSets.ContainsKey(graph))
            {
                _pathSets.Add(graph, new PathSet(graph.Id));
            }
        }

        public override PinokioPath FindShortestPath(PinokioGraph graph, string from, string to, List<MapNode> excludingNodes = null, List<MapLink> excludingLinks = null)
        {
            var pathSet = _pathSets[graph];
            if (!pathSet.IsAlreadyCalculated(from))
            {
                var paths = new Dictionary<(string, string), PinokioPath>();
                var isChecked = new Dictionary<string, bool>();

                // Initialize
                var nodes = graph.Nodes.Values.ToArray();

                for (int i = 0; i < nodes.Length; i++)
                {
                    if (excludingNodes != null && excludingNodes.Contains(nodes[i])) continue;
                    isChecked.Add(nodes[i].Name, false);
                    var newPath = new PinokioPath(new Location(nodes[i])) { WeightSum = double.MaxValue };
                    paths.Add((from, nodes[i].Name), newPath);
                }

                // Initialize Distance
                var startNode = graph.Nodes[from];
                for (int i = 0; i < startNode.OutLinks.Count; i++)
                {
                    var link = startNode.OutLinks[i];
                    if (excludingLinks != null && excludingLinks.Contains(link)) continue;
                    if (excludingNodes != null && excludingNodes.Contains(link.ToNode)) continue;

                    var path = paths[(from, link.ToNode.Name)];
                    // Initialize Path with start node's outlinks
                    path.Links.Add(link);
                    path.Nodes.Add(link.FromNode);
                    path.Nodes.Add(link.ToNode);
                    path.WeightSum = link.Weight;
                }

                List<MapNode> nodeToCheck = new List<MapNode>();
                nodeToCheck.Add(startNode);

                bool isAllChecked = false;
                while (!isAllChecked)
                {
                    nodeToCheck.Remove(startNode);
                    isChecked[startNode.Name] = true;

                    for (int i = 0; i < startNode.OutLinks.Count; i++)
                    {
                        var outLink = startNode.OutLinks[i];
                        if (excludingLinks != null && excludingLinks.Contains(outLink)) continue;
                        if (excludingNodes != null && excludingNodes.Contains(outLink.ToNode)) continue;

                        if (!nodeToCheck.Contains(outLink.ToNode) && !isChecked[outLink.ToNode.Name])
                            nodeToCheck.Add(outLink.ToNode);

                        if (paths[(from, outLink.ToNode.Name)].WeightSum >= paths[(from, outLink.FromNode.Name)].WeightSum + outLink.Weight)
                        {
                            paths[(from, outLink.ToNode.Name)] = new PinokioPath(paths[(from, outLink.FromNode.Name)], new Location(outLink.ToNode));
                            paths[(from, outLink.ToNode.Name)].AddLink(outLink); // No Need Destination
                        }
                    }

                    if (nodeToCheck.Count > 0)
                    {
                        startNode = nodeToCheck.First();
                        for (int i = 0; i < nodeToCheck.Count; i++)
                        {
                            var node = nodeToCheck[i];
                            if (paths[(from, node.Name)].WeightSum < paths[(from, startNode.Name)].WeightSum)
                                startNode = node;
                        }
                    }
                    else
                        isAllChecked = true;
                }

                pathSet.AddPaths(from, paths);

                return paths[(from, to)];
            }

            Recycle++;
            return pathSet[from, to];
        }


    }
}