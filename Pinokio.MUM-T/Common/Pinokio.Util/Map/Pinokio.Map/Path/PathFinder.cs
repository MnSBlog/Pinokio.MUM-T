using System.Collections.Generic;

using Pinokio.Core;
using Pinokio.Map.Algorithms;

namespace Pinokio.Map
{
    public class PathFinder
    {
        protected ShortestPathAlgorithmType _spAlgorithmType = ShortestPathAlgorithmType.None;
        protected KShortestPathAlgorithmType _kspAlgorithmType = KShortestPathAlgorithmType.None;

        protected ShortestPathAlgorithm SPAlgorithm;
        protected KShortestPathAlgorithm KSPAlgorithm;

        public virtual void AddGraph(PinokioGraph graph)
        {
            if (_spAlgorithmType == ShortestPathAlgorithmType.Dijkstra)
            {
                var dijkstra = SPAlgorithm as DijkstraAlgorithm;
                dijkstra.AddGraph(graph);
            }
        }

        public void SetShortestPathAlgorithm(ShortestPathAlgorithmType pathFinderType)
        {
            if (_spAlgorithmType == pathFinderType) return;
            else
            {
                _spAlgorithmType = pathFinderType;
                switch (_spAlgorithmType)
                {
                    case ShortestPathAlgorithmType.Dijkstra:
                        SPAlgorithm = new DijkstraAlgorithm();
                        break;
                    case ShortestPathAlgorithmType.TurnDijkstra:
                        SPAlgorithm = new TurnDijkstraAlgorithm();
                        break;
                    //case ShortestPathAlgorithmType.FloydWarshall:
                    //    _pathFinder = new FloydWarshallAlgorithm(_map);
                    //    break;
                    //case ShortestPathAlgorithmType.BellmanFord:
                    //    _pathFinder = new BellmanFordAlgorithm(_map);
                    //    break;
                }

                if (KSPAlgorithm != null)
                {
                    KSPAlgorithm.SetShortestPathAlgorithm(SPAlgorithm);
                }
            }
        }

        public void SetKShortestAlgorithm(KShortestPathAlgorithmType ksAlgorithmType)
        {
            if (SPAlgorithm is null)
            {
                LogHandler.AddLog(LogLevel.Info, "It needs to set shortest algorithm first");
                return;
            }

            if (_kspAlgorithmType == ksAlgorithmType) return;
            else
            {
                _kspAlgorithmType = ksAlgorithmType;
                switch (_kspAlgorithmType)
                {
                    case KShortestPathAlgorithmType.Yen:
                        KSPAlgorithm = new YenAlgorithm(SPAlgorithm);
                        break;
                }
            }
        }

        public virtual PinokioPath FindPath(PinokioGraph graph, string fromId, string toId, List<MapNode> excludingNodes, List<MapLink> excludingLinks, PathType type)
        {
            return SPAlgorithm.FindShortestPath(graph, fromId, toId, excludingNodes, excludingLinks);
        }

        public virtual List<PinokioPath> FindKShortestPaths(PinokioGraph graph, string fromId, string toId, int K, PathType type)
        {
            return KSPAlgorithm.FindKShortestPaths(new PinokioGraph(graph), fromId, toId, K, new List<MapNode>(), new List<MapLink>());
        }

        public virtual List<PinokioPath> FindKShortestPaths(PinokioGraph graph, string fromId, string toId, int K, PathType type, List<MapNode> excludingNodes)
        {
            return KSPAlgorithm.FindKShortestPaths(new PinokioGraph(graph), fromId, toId, K, excludingNodes, new List<MapLink>());
        }

    }
}