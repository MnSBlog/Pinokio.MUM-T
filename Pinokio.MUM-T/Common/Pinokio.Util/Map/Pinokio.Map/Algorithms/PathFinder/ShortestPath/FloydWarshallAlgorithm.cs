//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Pinokio.MapGraph
//{
//    /// <summary>
//    /// Path Finder : Shortest Path Algorithm
//    /// FloydWarshallAlgorithm
//    /// </summary>
//    public class FloydWarshallAlgorithm : IShortestPathAlgorithm
//    {
//        private Graph _map;
//        private Dictionary<string, Dictionary<string, double>> _distance;
//        private Dictionary<string, Dictionary<string, Path>> _paths;

//        public FloydWarshallAlgorithm(Graph graph)
//        {
//            _map = graph;
//            Initialize();
//        }

//        private void Initialize()
//        {
//            try
//            {
//                //_isChecked = new Dictionary<string, bool>();
//                _distance = new Dictionary<string, Dictionary<string, double>>();
//                _paths = new Dictionary<string, Dictionary<string, Path>>();

//                var nodeIds = _map.Nodes.Keys.ToArray();
//                for (int i = 0; i < nodeIds.Length; i++)
//                {
//                    var fromNodeId = nodeIds[i];
//                    //_distance.Add(fromNodeId, new Dictionary<string, double>());
//                    _paths.Add(fromNodeId, new Dictionary<string, Path>());
//                    //for (int j = 0; j < nodeIds.Length; j++)
//                    //{ 
//                    //    var toNodeId = nodeIds[j];
//                    //    if (fromNodeId == toNodeId)
//                    //        _distance[fromNodeId].Add(toNodeId, 0.0);
//                    //    else
//                    //        _distance[fromNodeId].Add(toNodeId, double.PositiveInfinity);
//                    //}
//                }

//                var linkIds = _map.Links.Keys.ToArray();
//                for (int i = 0; i < linkIds.Length; i++)
//                {
//                    var link = _map.Links[linkIds[i]];
//                    _paths[link.FromNode.Id].Add(link.ToNode.Id, new Path(link)); // Final Node
//                    //_distance[link.FromNode.Id][link.ToNode.Id] = link.Length;
//                }

//                for (int i = 0; i < nodeIds.Length; i++)
//                {
//                    var interNode = nodeIds[i];
//                    for (int j = 0; j < nodeIds.Length; j++)
//                    {
//                        var startNode = nodeIds[j];
//                        for (int k = 0; k < nodeIds.Length; k++)
//                        {
//                            var endNode = nodeIds[k];

//                            //if (_distance[startNode][endNode] > _distance[startNode][interNode] + _distance[interNode][endNode])
//                            //{
//                            //    _distance[startNode][endNode] = _distance[startNode][interNode] + _distance[interNode][endNode];
//                            //    _paths[startNode][endNode] = new List<ILink>(_paths[startNode][interNode]);
//                            //    _paths[startNode][endNode].AddRange(_paths[interNode][endNode]);
//                            //}

//                            if (_paths[startNode][endNode].WeightSum > _paths[startNode][interNode].WeightSum + _paths[interNode][endNode].WeightSum)
//                            {
//                                _paths[startNode][endNode] = new Path(_paths[startNode][interNode], endNode);
//                                _paths[startNode][endNode].Merge(_paths[interNode][endNode]);
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.ToString());
//                return;
//            }
//        }

//        public Path FindShortestPath(string from, string to)
//        {
//            try
//            {
//                return _paths[from][to];
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
