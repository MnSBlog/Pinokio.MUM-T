using System;
using System.Collections.Generic;

using Pinokio.Core;

namespace Pinokio.Map.LG.TP
{
    public class TPWayPoints
    {
        private uint _mapId;
        private Dictionary<string, string> _wayPoints;

        public TPWayPoints(uint mapId)
        {
            _mapId = mapId;
            _wayPoints = new Dictionary<string, string>();
        }

        public void Add(string key, string wayPoint)
        {
            _wayPoints.Add(key, wayPoint);
        }

        public bool ContainsKey(string key)
        {
            return _wayPoints.ContainsKey(key);
        }

        public string this[string key]
        {
            get
            {
                if (_wayPoints.ContainsKey(key))
                    return _wayPoints[key];
                else
                    return null; 
            } 

        }
    }

    public class TPPathFinder : PathFinder
    {
        private Dictionary<uint, TPWayPoints> _wayPoints;
        public TPPathFinder()
        {
            _wayPoints = new Dictionary<uint, TPWayPoints>();
            Initialize();
        }

        private void Initialize()
        {
            var wayPoints = new TPWayPoints(170);
            wayPoints.Add("2202", "2203");
            wayPoints.Add("2204", "2205");
            wayPoints.Add("2208", "2209");
            wayPoints.Add("2210", "2211");
            wayPoints.Add("2212", "2213");
            wayPoints.Add("2214", "2215");
            wayPoints.Add("2216", "2217");
            wayPoints.Add("2218", "2219");
            wayPoints.Add("2222", "2223");
            wayPoints.Add("2224", "2225");
            wayPoints.Add("2226", "2227");
            wayPoints.Add("2228", "2229");
            wayPoints.Add("2230", "2231");
            wayPoints.Add("2232", "2233");
            wayPoints.Add("2070", "2071");
            wayPoints.Add("2072", "2073");
            wayPoints.Add("2075", "2076");
            wayPoints.Add("2077", "2078");
            wayPoints.Add("2080", "2081");
            wayPoints.Add("2082", "2083");
            wayPoints.Add("2085", "2086");
            wayPoints.Add("2087", "2088");
        }

        public override PinokioPath FindPath(PinokioGraph graph, string fromId, string toId, List<MapNode> excludingNode, List<MapLink> excludingLinks, PathType type)
        {
            try
            {
                if (_wayPoints.ContainsKey(graph.Id))
                {
                    if (_wayPoints[graph.Id].ContainsKey(fromId))
                    {
                        var fromNode = graph.Nodes[fromId];
                        var path = base.FindPath(graph, fromId, _wayPoints[graph.Id][fromId], excludingNode, excludingLinks, type);

                        if (toId == _wayPoints[graph.Id][fromId])
                        {
                            return path;
                        }
                        else
                        {
                            var pathToDest = base.FindPath(graph, _wayPoints[graph.Id][fromId], toId, excludingNode, excludingLinks, type);

                            path.Merge(pathToDest);
                            return path;
                        }
                    }
                }

                var originPath = base.FindPath(graph, fromId, toId, excludingNode, excludingLinks, type);
                return originPath;
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Error, e.ToString());
                return null;
            }
        }
    }
}
