using System.Linq;
using System.Collections.Generic;

using Pinokio.Map.Algorithms;
using Pinokio.Core;

namespace Pinokio.Map
{
    public enum PathType
    {
        Normal,
        Inplace,
        PositionChanged,
        BypassCharger,
        Cycle,
        PreDrive,
        MainDrive,
    }

    public static class PathMaster
    {
        public enum LocationType
        {
            From, To,
        }

        private static Dictionary<uint, PinokioGraph> _graphs = new Dictionary<uint, PinokioGraph>();
        private static PathFinder _pathFinder = new PathFinder();

        public static PathFinder PathFinder { get => _pathFinder; }
        public static Dictionary<uint, PinokioGraph> Graphs { get => _graphs; } 

        #region [Setter]
        public static void AddGraph(PinokioGraph graph)
        {
            if (_graphs.ContainsKey(graph.Id)) return;
            _graphs.Add(graph.Id, graph);

            _pathFinder.AddGraph(graph);
        }

        public static void SetPathFinder(PathFinder pathFinder)
        {
            _pathFinder = pathFinder;
        }
        #endregion

        public static PinokioPath FindPath(uint mapId, string fromNodeName, string toNodeName, List <MapNode> excludingNodes = null, List<MapLink> excludingLinks = null, PathType type = PathType.Normal)
        {
            var path = new PinokioPath(_pathFinder.FindPath(_graphs[mapId], fromNodeName, toNodeName, excludingNodes, excludingLinks, type));
            if (ValidatePath(path, toNodeName))
            {
                path.Type = type;
                return path;
            }
            else
            {
                return null;
            }
        }

        public static PinokioPath FindPath(uint mapId, Location fromLocation, string toNodeName, List <MapNode> excludingNodes = null, List<MapLink> excludingLinks = null, PathType type = PathType.Normal)
        {
            // link에서 ToNode까지의 경로!
            bool onLinkFromLocation = IsLocationOnLink(fromLocation, LocationType.From, out string fromNodeName);

            var path = FindPath(mapId, fromNodeName, toNodeName, excludingNodes, excludingLinks, type);

            if (path != null && onLinkFromLocation)
            {
                path.Links.Insert(0, fromLocation.Link);
            }

            return path;
        }

        public static PinokioPath FindPath(uint mapId, string fromNodeName, Location toLocation, List <MapNode> excludingNodes = null, List<MapLink> excludingLinks = null, PathType type = PathType.Normal)
        {
            bool onLinkToLocation = IsLocationOnLink(toLocation, LocationType.To, out string toNodeName);

            var path = FindPath(mapId, fromNodeName, toNodeName, excludingNodes, excludingLinks, type);

            if (path != null && onLinkToLocation)
                path.AddLink(toLocation.Link);

            return path;
        }

        public static PinokioPath FindPath(uint mapId, Location fromLocation, Location toLocation, List <MapNode> excludingNodes = null, List<MapLink> excludingLinks = null, PathType type = PathType.Normal)
        {
            bool onLinkFromLocation = IsLocationOnLink(fromLocation, LocationType.From, out string fromNodeName);
            bool onLinkToLocation = IsLocationOnLink(toLocation, LocationType.To, out string toNodeName);

            PinokioPath path = null;
            while (path == null)
            {
                if (onLinkFromLocation && onLinkToLocation)
                {
                    if (fromLocation.Link == toLocation.Link &&
                        fromLocation.Offset < toLocation.Offset)
                    {
                        path = new PinokioPath(toLocation);
                        path.Links.Add(toLocation.Link);
                        break;
                    }
                    else if (fromNodeName == toNodeName)
                    {
                        path = new PinokioPath(toLocation);
                    }
                    else
                    {
                        path = FindPath(mapId, fromNodeName, toNodeName, excludingNodes, excludingLinks, type);
                    }
                }
                else
                {
                    path = FindPath(mapId, fromNodeName, toNodeName, excludingNodes, excludingLinks, type);
                }

                if (path is null) return null;
                else if (onLinkFromLocation)
                {
                    path.PushLinkFront(fromLocation.Link);
                }
                if (onLinkToLocation)
                {
                    path.AddLink(toLocation.Link);
                    path.SetDestination(toLocation);
                }
            }
            return path;
        }

        public static bool IsLocationOnLink(Location location, LocationType locationType, out string nodeName)
        {
            if (location.Node is null)
            {
                if (locationType == LocationType.From)
                    nodeName = location.Link.ToNode.Name;
                else // if (locationType == LocationType.To)
                    nodeName = location.Link.FromNode.Name;
                return true;
            }
            else
            {
                nodeName = location.Node.Name;
                return false;
            }
        }

        public static List<PinokioPath> FindKShortestPaths(uint mapId, string fromId, string toId, int K, PathType type = PathType.Normal)
        {
            var paths = _pathFinder.FindKShortestPaths(_graphs[mapId], fromId, toId, K, type);
            foreach (var path in paths.ToList())
            {
                if (ValidatePath(path, toId))
                {
                    path.Type = type;
                }
                else
                {
                    paths.Remove(path);
                }
            }
            return paths;
        }

        public static List<PinokioPath> FindKShortestPaths(uint mapId, string fromId, string toId, int K, List<MapNode> exNodes, PathType type = PathType.Normal)
        {
            var paths = _pathFinder.FindKShortestPaths(_graphs[mapId], fromId, toId, K, type, exNodes);
            foreach (var path in paths.ToList())
            {
                if (ValidatePath(path, toId))
                {
                    path.Type = type;
                }
                else
                {
                    paths.Remove(path);
                }
            }
            return paths;
        }

        public static double GetPathLength(PinokioPath path)
        {
            double length = 0;

            if(path != null && path.Links.Count > 0)
            {
                foreach(var link in path.Links)
                {
                    length += link.Length;
                }

                return length;        
            }

            return length;
        }

        public static double GetDistance(uint mapId, string fromId, string toId, PathType type = PathType.Normal)
        {
            var path = FindPath(mapId, fromId, toId, null, null, type);
            if (path is null) return double.MaxValue;
            else return path.WeightSum;
        }

        public static double GetDistance(uint mapId, Location fromLocation, Location toLocation, PathType type = PathType.Normal)
        {
            var path = FindPath(mapId, fromLocation, toLocation, null, null, type);
            if (path is null) return double.NaN;
            else return path.WeightSum;
        }

        private static bool ValidatePath(PinokioPath path, string toId)
        {
            if (path is null) return false;
            if (path.Links.Count <= 0) return false;
            if (path.Links.Last().ToNode.Name != toId) return false;

            return true;
        }

        public static void CalculateAllPath(uint mapId)
        {
            if (_graphs.ContainsKey(mapId))
            {
                PinokioGraph graph = _graphs[mapId];
                List<MapNode> nodes = graph.Nodes.Values.ToList();
                for (int i = 0; i < nodes.Count; i++)
                {
                    MapNode node = nodes[i];
                    MapNode otherNode = null;
                    if (i < nodes.Count - 1)
                        otherNode = nodes[i + 1];
                    else
                        otherNode = nodes[0];

                    _pathFinder.FindPath(graph, node.Name, otherNode.Name, null, null, PathType.Normal);
                }
            }
        }
    }
}
