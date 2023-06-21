using System;
using System.Collections.Generic;
using System.Linq;

using Pinokio.Geometry;

namespace Pinokio.Map.Algorithms
{
    /// <summary>
    /// Path Finder : Shortest Path Algorithm
    /// DijkstraAlgorithm
    /// </summary>
    public class TurnDijkstraAlgorithm : ShortestPathAlgorithm
    {
        public TurnDijkstraAlgorithm()
        {
        }

        public override PinokioPath FindShortestPath(PinokioGraph graph, string from, string to, List<MapNode> excludingNodes = null, List<MapLink> excludingLinks = null)
        {
            try
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
                    if (path.WeightSum > link.Weight)
                    {
                        path.Links.Clear();
                        path.Links.Add(link);

                        path.Nodes.Clear();
                        path.Nodes.Add(link.FromNode);
                        path.Nodes.Add(link.ToNode);

                        path.WeightSum = link.Weight;
                    }
                }

                List<MapNode> nodeToCheck = new List<MapNode>();
                nodeToCheck.Add(startNode);

                bool isAllChecked = false;
                while (!isAllChecked)
                {
                    nodeToCheck.Remove(startNode);
                    isChecked[startNode.Name] = true;
                    MapLink lastLink = null;

                    if (paths[(from, startNode.Name)].Links.Count > 0)
                        lastLink = paths[(from, startNode.Name)].Links.Last();

                    for (int i = 0; i < startNode.OutLinks.Count; i++)
                    {
                        var outLink = startNode.OutLinks[i];
                        if (excludingLinks != null && excludingLinks.Contains(outLink)) continue;
                        if (excludingNodes != null && excludingNodes.Contains(outLink.ToNode)) continue;

                        double turningWeight = 0.0;
                        if (lastLink != null)
                        {
                            Vector3 outDirection = outLink.GetDirection(outLink.Length);
                            Vector3 lastDirection = lastLink.GetDirection(lastLink.Length);
                            double degree = Vector3.AngleDegree(outDirection, lastDirection, Vector3.Coordinate.Z);
                            turningWeight += degree / 12.5; // 90'를 8초에 도는 스펙이라고 할때.. 
                        }

                        if (!nodeToCheck.Contains(outLink.ToNode) && !isChecked[outLink.ToNode.Name])
                            nodeToCheck.Add(outLink.ToNode);

                        if (paths[(from, outLink.ToNode.Name)].WeightSum >= paths[(from, startNode.Name)].WeightSum + outLink.Weight + turningWeight)
                        {
                            var newPath = new PinokioPath(paths[(from, startNode.Name)], new Location(outLink.ToNode));
                            newPath.AddLink(outLink); // No Need Destination
                            newPath.WeightSum = paths[(from, startNode.Name)].WeightSum + outLink.Weight + turningWeight;
                            paths[(from, outLink.ToNode.Name)] = newPath;
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

                return paths[(from, to)];

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + $"({from}) --> ({to}) / excluding nodes({excludingNodes?.Count}) / links({excludingLinks?.Count})");
                return null;
            }
        }
    }
}