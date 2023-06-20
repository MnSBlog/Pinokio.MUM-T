using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinokio.Map.Algorithms
{
    public class YenAlgorithm : KShortestPathAlgorithm
    {
        public YenAlgorithm(ShortestPathAlgorithm spAlgorithm) : base(spAlgorithm)
        { }

        public override List<PinokioPath> FindKShortestPaths(PinokioGraph graph, string from, string to, int K, List<MapNode> excludingNodes, List<MapLink> excludingLinks )
        {
            // Determine the shortest path from the source to the sink
            PinokioPath shortestPath = _spAlgorithm.FindShortestPath(graph, from, to);
            if (shortestPath != null)
            {
                var kShortestPaths = new List<PinokioPath>() { shortestPath };
                // Initialize the set to store the potential kth shorest path
                var potentialPaths = new List<PinokioPath>();

                List<MapNode> thisExcludingNodes = new List<MapNode>(); 
                List<MapLink> thisExcludingLinks = new List<MapLink>(); 
                for (int k = 1; k < K; k++)
                {
                    thisExcludingNodes.AddRange(excludingNodes);
                    thisExcludingLinks.AddRange(excludingLinks);

                    PinokioPath prevPath = kShortestPaths[k - 1];
                    int prevPathLegnth = prevPath.Nodes.Count;
                    
                    // The spur node ranges from the first node to the next to last node in the previous k-shortest path.
                    for (int i = 1; i < prevPathLegnth - 1; i++)
                    {
                        MapNode spurNode = prevPath.Nodes[i];
                        PinokioPath rootPath = prevPath.RangeNode(0, i);

                        if (rootPath != null)
                        {
                            foreach (PinokioPath otherkPath in kShortestPaths)
                            {
                                PinokioPath otherRootPath = otherkPath.RangeNode(0, i);
                                if (rootPath == otherRootPath)
                                {
                                    // Remove the links that are part of the previous shortes paths which share the same root path.
                                    var prevPathNextLink = otherkPath.GetLinkWithNodeIndex(i);
                                    excludingLinks.Add(prevPathNextLink);
                                }
                            }

                            // remove rootPathNode from graph except spurNode
                            List<MapNode> rootPathNodes = rootPath.Nodes.ToList();
                            rootPathNodes.Remove(spurNode);
                            rootPathNodes.ForEach(n => excludingNodes.Add(n));
                        }

                        // Calculate the spur path from the spur node to the sink
                        // Consider also checking if any spurPath found;
                        PinokioPath spurPath = _spAlgorithm.FindShortestPath(graph, spurNode.Name, to, excludingNodes, excludingLinks);
                        if (spurPath != null && spurPath.Links.Count > 0)
                        {
                            // Entire path is made up of the root path and spur path.
                            var totalPath = new PinokioPath(rootPath);
                            totalPath.Merge(spurPath);

                            // Add the potential k-shortest path to the heap.
                            if (!potentialPaths.Contains(totalPath))
                            {
                                potentialPaths.Add(totalPath);
                            }
                        }

                        // Add back the edges and nodes that were removed from the graph.
                        //restore edges to Graph;
                        excludingLinks.Clear();
                        //restore nodes in rootPath to Graph;
                        excludingNodes.Clear();
                    }

                    if (potentialPaths.Count == 0)
                    {
                        // This handles the case of there being no spur paths, or no spur paths left.
                        // This could happen if the spur paths have already been exhausted(added to A),
                        // or there are no spur paths at all - such as when both the source and sink vetices
                        // lie along a "dead end".
                        break;
                    }
                    else
                    {
                        // Sort the potential K-Shortest paths by cost
                        potentialPaths.Sort();
                        // Add the lowest cost path becomes the k-shortest path.
                        kShortestPaths.Add(potentialPaths[0]);
                        // In fact we should rather use shift since we are removing the first element
                        potentialPaths.RemoveAt(0);
                    }
                }

                return kShortestPaths;
            }
            return null;
        }
    }
}