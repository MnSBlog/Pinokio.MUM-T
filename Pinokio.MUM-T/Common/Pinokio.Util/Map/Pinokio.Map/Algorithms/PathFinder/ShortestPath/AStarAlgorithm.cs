//using System;
//using System.Collections.Generic;
//using System.Linq;

//using Pinokio.Geometry;

//namespace Pinokio.MapGraph
//{
//    /// <summary>
//    /// Path Finder : Shortest Path Algorithm
//    /// AStarAlgorithm(Not Implemented Yet)
//    /// </summary>
//    public class AStarAlgorithm : IShortestPathAlgorithm
//    {
//        private Graph _map;
//        private Dictionary<string, Dictionary<string, double>> _distance;

//        public AStarAlgorithm(Graph map)
//        {
//            _map = map;
//        }
//        private void Initialize()
//        {
//            _distance = new Dictionary<string, Dictionary<string, double>>();

//            var nodeIds = _map.Nodes.Keys.ToArray();
//            for (int i = 0; i < nodeIds.Length; i++)
//            {
//                var fromNodeId = nodeIds[i];
//                _distance.Add(fromNodeId, new Dictionary<string, double>());
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
//                //Initialize();
//                //Dictionary<string, string> parent = new Dictionary<string, string>();
//                //List<NetworkNode>
//                //List = new List<NetworkNode>();
//                //List<NetworkNode> closeList = new List<NetworkNode>();

//                //INetworkNode currentNode = _graph.Nodes[from];
//                //INetworkNode endNode = _graph.Nodes[to];

//                //bool term = false;
//                //while (!term)
//                //{
//                //    openList.Remove(currentNode);
//                //    closeList.Add(currentNode);

//                //    for (int i = 0; i < currentNode.OutLinks.Count; i++)
//                //    {
//                //        var link = currentNode.OutLinks[i];
//                //        if (_distance[from][link.ToNode.Id] > _distance[from][currentNode.Id] + link.Length)
//                //            _distance[from][link.ToNode.Id] = _distance[from][currentNode.Id] + link.Length;

//                //        if (!openList.Contains(link.ToNode) && !closeList.Contains(link.ToNode))
//                //        {
//                //            openList.Add(link.ToNode);
//                //            parent[link.ToNode.Id] = currentNode.Id;
//                //        }
//                //        else if (openList.Contains(link.ToNode) && _distance[from][link.ToNode.Id] > _distance[from][currentNode.Id] + link.Length)
//                //        {
//                //            // 여기에 들어올 수 있는가?
//                //            _distance[from][link.ToNode.Id] = _distance[from][currentNode.Id] + link.Length;
//                //            parent[link.ToNode.Id] = currentNode.Id;
//                //        }
//                //    }

//                //    if (openList.Count > 0)
//                //    {
//                //        currentNode = openList[0];
//                //        for (int j = 0; j < openList.Count; j++)
//                //        {
//                //            var node = openList[j];
//                //            if (_distance[from][currentNode.Id] + Vector3.Distance(currentNode.PosVec3, endNode.PosVec3) >
//                //                _distance[from][node.Id] + Vector3.Distance(node.PosVec3, endNode.PosVec3))
//                //            {
//                //                // 유클리디안 거리로 비교하는게.. 맞나..?
//                //                currentNode = node;
//                //            }
//                //        }
//                //        if (currentNode.Id == to || closeList.Count == _graph.Nodes.Count || openList.Count == 0)
//                //            term = true;
//                //    } 
//                //    else
//                //        term = true;
//                //}

//                ////while (currentNode.Id != from)
//                ////{
//                ////    path = parentDictionary[currentNode.Id] + " - " + path;
//                ////    currentNode = Network.Instance.Nodes[parentDictionary[currentNode.Id]];
//                ////}
//                //return null;
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