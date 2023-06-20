//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Pinokio.MapGraph
//{
//    /// <summary>
//    /// Path Finder : Shortest Path Algorithm
//    /// BellmanFordAlgorithm
//    /// </summary>
//    public class BellmanFordAlgorithm : IShortestPathAlgorithm 
//    {
//        private Graph _map;
//        private Dictionary<string, Dictionary<string, double>> _distance;
//        private Dictionary<string, List<MapLink>> _paths;

//        public BellmanFordAlgorithm(Graph graph)
//        {
//            _map = graph;
//        }

//        private void Initialize(string from)
//        {
//            _distance = new Dictionary<string, Dictionary<string, double>>();
//            _paths = new Dictionary<string, List<MapLink>>();

//            var nodeIds = _map.Nodes.Keys.ToArray();
//            for (int i = 0; i < nodeIds.Length; i++)
//            {
//                var fromNodeId = nodeIds[i];
//                _distance.Add(fromNodeId, new Dictionary<string, double>());
//                _paths.Add(fromNodeId, new List<MapLink>());
//                for (int j = 0; j < nodeIds.Length; j++)
//                {
//                    var toNodeId = nodeIds[j];
//                    if (fromNodeId == toNodeId)
//                    {
//                        _distance[fromNodeId].Add(toNodeId, 0.0);
//                    }
//                    else
//                    {
//                        _distance[fromNodeId].Add(toNodeId, double.PositiveInfinity);
//                    }
//                }
//            }

//            var fromNode = _map.Nodes[from];
//            for (int i = 0; i < fromNode.OutLinks.Count; i++)
//            {
//                var link = fromNode.OutLinks[i];
//                _paths[link.ToNode.Id].Add(link);
//            }

//            var linkIds = _map.Links.Keys.ToArray();
//            for (int i = 0; i < linkIds.Length; i++)
//            {
//                var link = _map.Links[linkIds[i]];
//                _distance[link.FromNode.Id][link.ToNode.Id] = link.Weight;
//            }
//        }

//        public Path FindShortestPath(string from, string to)
//        {
//            try
//            {
//                throw new Exception("Not Implemented");
//                Initialize(from);

//                var nodeIds = _map.Nodes.Keys.ToArray();
//                var linkIds = _map.Links.Keys.ToArray();
//                for (int i = 0; i < nodeIds.Length - 1; i++)
//                {
//                    for (int j = 0; j < linkIds.Length; j++)
//                    {
//                        var link = _map.Links[linkIds[j]];

//                        if (_distance[from][link.ToNode.Id] > _distance[from][link.FromNode.Id] + link.Weight)
//                        {
//                            _distance[from][link.ToNode.Id] = _distance[from][link.FromNode.Id] + link.Weight;
//                            _paths[link.ToNode.Id] = new List<MapLink>(_paths[link.FromNode.Id]);
//                            _paths[link.ToNode.Id].Add(link);
//                        }
//                    }
//                }

//                for (int i = 0; i < linkIds.Length; i++)
//                {
//                    var link = _map.Links[linkIds[i]];

//                    var c = _paths[to];
//                    if (_distance[from][link.ToNode.Id] > _distance[from][link.FromNode.Id] + link.Weight)
//                    {
//                        throw new Exception("There is negative cycle in the network");
//                    }
//                }

//                //return _paths[to];
//                return null;
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.ToString());
//                return null;
//            }
            
//        }

//        public Path FindPathExcludingLinks(string from, string to, List<MapLink> excludingLinks)
//        {
//            try
//            {
//                throw new Exception("Not Implemented");
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.ToString());
//                return null;
//            }
//        }

//        public double GetDistance(string from, string to)
//        {
//            try
//            {
//                FindShortestPath(from, to);
//                return _distance[from][to];
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.ToString());
//                return double.PositiveInfinity;
//            }
//        }
//    }
//}