using System;
using System.Linq;
using System.Collections.Generic;

using Pinokio.Core;
using System.IO;

namespace Pinokio.Map
{
    public class PinokioPath : IEquatable<PinokioPath>, IComparable<PinokioPath> // IComparable 추가 weightsum으로 비교
    {
        private MapNode _currentNode;
        private int _nodeIndex = -1;
        private List<MapLink> _links;
        private List<MapNode> _nodes;
        private Location _destination;
        public List<MapPoint> Points { get; set; } 
        public MapNode CurrentNode { get => _currentNode; }
        public int NodeIndex { get => _nodeIndex; }
        public PathType Type { get; set; }
        public List<MapLink> Links { get => _links; }
        public List<MapNode> Nodes { get => _nodes; }
        public double WeightSum { get; set; }
        public Location Destination { get => _destination; }

        #region Constructors
        public PinokioPath(Location destination)
        {
            _destination = destination;
            _links = new List<MapLink>();
            _nodes = new List<MapNode>();
            WeightSum = 0;
        }

        public PinokioPath(PinokioPath otherPath)
        {
            _destination = otherPath.Destination;
            _links = new List<MapLink>(otherPath.Links);
            _nodes = new List<MapNode>(otherPath._nodes);
            WeightSum = otherPath.WeightSum;
        }

        public PinokioPath(PinokioPath otherPath, Location destination)
        {
            _destination = destination;
            _links = new List<MapLink>(otherPath.Links);
            _nodes = new List<MapNode>(otherPath._nodes);
            WeightSum = otherPath.WeightSum;
        }
        #endregion Constructors End

        #region Build Path
        public virtual bool AddLink(MapLink link)
        {
            try
            {
                if (_links.Count > 0)
                {
                    var lastLink = _links.Last();
                    if (lastLink.ToNode != link.FromNode)
                        throw new Exception("추가할 수 없는 링크입니다.");
                }

                _links.Add(link);
                _nodes.Add(link.ToNode);
                WeightSum += link.Weight;
                return true;
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Warn, "M(AddLink);" + e.Message);
                return false;
            }
        }

        public virtual bool PushLinkFront(MapLink link)
        {
            try
            {
                if (_links.Count == 0) { return AddLink(link); }
                else if (link.ToNode != _links[0].FromNode) throw new Exception("추가할 수 없는 링크(link연결이 불가능)");
                else
                {
                    MapNode firstNode = null;
                    for(int i = 0; i < _nodes.Count; i++)
                    {
                        if (_nodes[i] is VirtualNode) continue;

                        firstNode = _nodes[i];
                        break;
                    }

                    if (firstNode is null) throw new Exception("Wrong First Node");
                    else if(link.ToNode != firstNode) throw new Exception("추가할 수 없는 링크(Node연결이 불가능)");
                } 

                _links.Insert(0, link);
                _nodes.Insert(0, link.FromNode);

                WeightSum += link.Weight;
                return true;
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Warn, "M(AddLink);" + e.Message);
                return false;
            }
        }
        #endregion Build Path End

        #region Initialize Path
        public void SetDestination(Location destination)
        {
            _destination = destination;
        }

        public void SetPointPath()
        {
            Points = new List<MapPoint>();
            for (int i = 0; i < Links.Count; i++)
            {
                var link = Links[i];

                for (int j = 0; j < link.Points.Count; j++)
                {
                    Points.Add(link.Points[j]);
                }
            }
        }

        public void SetNodePath(Location location)
        {
            if (location.Node is null && _links.Contains(location.Link))
            {
                if (_nodes[0] != location.Link.ToNode)
                {
                    int index = 0;
                    for (int i = 0; i < _links.Count; i++)
                    {
                        index = i;
                        if (_links[i] == location.Link)
                            break;
                    }

                    if (index > 0)
                        _links.RemoveRange(0, index);
                }
            }

            _nodes = new List<MapNode>();
            _nodeIndex = -1;

            int count = Links.Count;
            if (count > 0)
            {
                if (location.Node is null)
                {
                    if (_links[0].FromNode == _links[1].ToNode && IsArriveAtNode(location,  _links[0].ToNode))
                    {
                        _currentNode = _links[0].ToNode;
                        _nodes.Add(_links[0].ToNode);
                        _nodeIndex = 0;
                    }
                    else if (IsArriveAtNode(location, _links[0].FromNode))
                    {
                        _currentNode = _links[0].FromNode;
                        _nodes.Add(_links[0].FromNode);
                        _nodes.Add(_links[0].ToNode);
                        _nodeIndex = 0;
                    }
                    else if (IsArriveAtNode(location, _links[0].ToNode))
                    {
                        _currentNode = _links[0].ToNode;
                        _nodes.Add(_links[0].ToNode);
                        _nodeIndex = 0;
                    }
                    else if (location.Link == _links[0])
                    {
                        if (location.Link.VirtalNodes.Count > 0)
                        {
                            foreach (VirtualNode vNode in location.Link.VirtalNodes.ToList())
                            {
                                if (Math.Abs(vNode.Offset - location.Offset) <= 350)
                                {
                                    _nodes.Add(vNode);
                                    _currentNode = vNode;
                                    _nodeIndex = 0;
                                }
                                else if (_nodeIndex >= 0)
                                {
                                    _nodes.Add(vNode);
                                }
                            }
                        }

                        _nodes.Add(_links[0].ToNode);
                        if (_nodeIndex < 0)
                        {
                            _currentNode = _links[0].ToNode;
                            _nodeIndex = 0;
                        }
                    }
                    else
                    {
                        throw new Exception("Wrong Location");
                    }
                }

                for (int i = 1; i < count; i++)
                {
                    MapLink link = Links[i];
                    if (_nodes.Last() != link.FromNode) throw new Exception("링크가 연결되지 않습니다.");

                    for (int j = 0; j < link.VirtalNodes.Count; j++)
                    {
                        _nodes.Add(link.VirtalNodes[j]);
                    }

                    _nodes.Add(link.ToNode);
                }
            }
        }
        #endregion Initialize Path End

        #region Check
        public bool IsArriveAtNode(Location location, MapNode node)
        {
            if (location.Node is null)
            {
                if (node is VirtualNode vNode)
                    return (location.Link.Name == vNode.LinkName && Math.Abs(location.Offset - vNode.Offset) <= 350);
                else
                    return (location.Link.FromNode == node && location.Offset <= 350) ||
                    (location.Link.ToNode == node && location.Offset >= location.Link.Length - 350);
            }
            else
            {
                return location.Node == node;
            }
        }

        public int GetNodeIndex(MapNode node, int minLimit = 0)
        {
            for (int i = minLimit; i < _nodes.Count; i++)
            {
                if (_nodes[i] == node) return i;
            }
            return -1;
        }

        public MapLink GetLinkWithNodeIndex(int nodeIndex)
        {
            MapNode prevNode = _nodes[nodeIndex];
            MapNode nextNode = _nodes[nodeIndex + 1];
            foreach (MapLink link in Links)
            {
                if (link.FromNode == prevNode && link.ToNode == nextNode)
                    return link;
            }

            return null;
        }

        public bool ContainsLink(MapLink link)
        {
            return Links.Contains(link);
        }

        public bool ContainsPoint(MapPoint point)
        {
            return Points.Contains(point);
        }

        #endregion Check End

        #region Merge And Split
        public void Merge(PinokioPath otherPath)
        {
            try
            {
                if (otherPath.Links.Count == 0) return;
                else if (this.Links.Count == 0)
                {
                    _links.AddRange(otherPath.Links);
                    _destination = otherPath.Destination;
                    this.WeightSum = otherPath.WeightSum;
                }
                else
                {
                    var thisLastLink = Links[Links.Count - 1];
                    var otherFirstLink = otherPath.Links[0];
                    if (thisLastLink.ToNode == otherFirstLink.FromNode)
                    {
                        for (int i = 0; i < otherPath.Links.Count; i++)
                        {
                            this.AddLink(otherPath.Links[i]); // No Need Destination
                        }
                        _destination = otherPath.Destination;
                    }
                    else
                        throw new Exception("합칠 수 없는 path입니다.");
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public PinokioPath RangeNode(int startIndex, int endIndex)
        {
            if (startIndex < 0) throw new Exception("Wrong start Index of node path");
            else if (endIndex > _nodes.Count - 1) return null;
            else if (startIndex >= endIndex) return null;

            bool isStarted = false;
            PinokioPath newPath = new PinokioPath(new Location(_nodes[endIndex])) { Type = this.Type };

            MapNode startNode = _nodes[startIndex];
            MapNode endNode = _nodes[endIndex];
            foreach (var link in Links)
            {
                if (!isStarted)
                {
                    if (startNode is VirtualNode virtualStartNode)
                    {
                        if (virtualStartNode.LinkName == link.Name)
                        {
                            isStarted = true;
                            newPath._nodes.Add(startNode);
                            newPath.AddLink(link);
                        }
                    }
                    else if(link.FromNode == startNode)
                    {
                        isStarted = true;
                        newPath._nodes.Add(_nodes[startIndex]);
                        newPath.AddLink(link);
                    }
                }
                else
                {
                    newPath.AddLink(link);
                }

                if (isStarted)
                {
                    if (endNode is VirtualNode virtualEndNode)
                    {
                        if (virtualEndNode.LinkName == link.Name)
                            break;
                    }
                    else if (link.ToNode == endNode)
                    {
                        break;
                    }
                }
            }
            return newPath;
        }

        public PinokioPath Range(MapNode startNode, MapNode endNode)
        {
            try
            {
                if (!_nodes.Contains(startNode)) throw new Exception("M(RangeNode) / Don't Contain StartNode");
                else if (!_nodes.Contains(endNode)) throw new Exception("M(RangeNode) / Don't Contain EndNode");
                else if (startNode == endNode) { var path = new PinokioPath(new Location(startNode)); path.Nodes.Add(startNode); return path; }
                List<int> endNodeIndexes = _nodes.FindAllIndexOf(endNode);
                int endIndex = endNodeIndexes.Min();
                if (endNodeIndexes.Count > 1 && endIndex == 0)
                    endIndex = endNodeIndexes[1];

                List<int> startNodeIndexes = _nodes.FindAllIndexOf(startNode);
                startNodeIndexes = startNodeIndexes.Where(i => i < endIndex).ToList();
                int startIndex = startNodeIndexes.Max();

                return RangeNode(startIndex, endIndex);
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Warn, "M(Range)_Node_Node / " + e.Message);
                return null;
            }
        }

        public PinokioPath Range(MapLink startLink, MapNode endNode)
        {
            try 
            {
                if (!_links.Contains(startLink)) throw new Exception("Wrong Start Link");
                else if (!_nodes.Contains(endNode)) throw new Exception("Wrong End Node");

                PinokioPath path;
                if (startLink.ToNode == endNode)
                {
                    path = new PinokioPath(new Location(endNode));
                    path.AddLink(startLink);
                } 
                else
                {
                    List<int> endNodeIndexes = _nodes.FindAllIndexOf(endNode);
                    int endIndex = endNodeIndexes.Min();
                    if (endNodeIndexes.Count > 1 && endIndex == 0)
                        endIndex = endNodeIndexes[1];

                    //if (_links[0] == startLink && _links.Count(l => l == startLink) == 1)
                    //{
                    //    path = RangeNode(0, endIndex);
                    //    if(path.Links[0] != startLink) path.PushLinkFront(startLink);
                    //    return path;
                    //}

                    List<int> startNodeIndexes = new List<int>();

                    if (_links[0] == startLink)
                        startNodeIndexes.Add(0);

                    foreach (var index in _nodes.FindAllIndexOf(startLink.FromNode))
                    {
                        if (_nodes.Count - 1 <= index) continue;
                        else if (_nodes[index + 1] == startLink.ToNode) startNodeIndexes.Add(index);
                        else if (_nodes[index + 1] is VirtualNode vNode && vNode.LinkName == startLink.Name) startNodeIndexes.Add(index);
                    }
                    startNodeIndexes = startNodeIndexes.Where(i => i < endIndex).Select(i => i).ToList();
                    int startIndex = startNodeIndexes.Max();
                    path = RangeNode(startIndex, endIndex);

                    if (_links[0] == startLink && startIndex == 0 && path.Links[0] != startLink)
                        path.PushLinkFront(startLink);
                }

                return path;
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Warn, "M(Range)_Link_Node / " + e.Message);
                return null;
            }
        }
        #endregion Merge And Split End

        #region Others

        public bool NodeStepUp(MapNode currentNode, Location location, bool debug = false)
        {
            bool changed = false;
            Action<MapNode> StepUp = (node) =>
            {
                if (debug)
                {
                    Console.WriteLine($"Step Up : {_currentNode} --> {node}");
                }

                _nodeIndex++;
                _currentNode = node;
                changed = true;
            };


            while (true && _nodeIndex < _nodes.Count - 1)
            {
                MapNode nextNode = _nodes[_nodeIndex + 1];
                if (nextNode is VirtualNode vNode)
                {
                    if (IsArriveAtNode(location, nextNode))
                    {
                        StepUp(nextNode);
                        continue;
                    }
                    else
                    {
                        if (_nodeIndex < _nodes.Count - 2)
                        {
                            MapNode n2Node = _nodes[_nodeIndex + 2];
                            if (IsArriveAtNode(location, n2Node))
                            {
                                StepUp(nextNode);
                                StepUp(n2Node);
                                break;
                            }
                        }

                        foreach (var inLink in location.Link.FromNode.InLinks)
                        {
                            if (inLink.FromNode == location.Link.ToNode) continue;
                            if (vNode.LinkName == inLink.Name)
                            {
                                StepUp(nextNode);
                            }
                        }
                    }
                }
                else if (nextNode == currentNode || nextNode == location)
                {
                    StepUp(nextNode);
                    continue;
                }

                break;
            }

            return changed;
        }

        public double CalculateRemainingDistance(Location startLocation)
        {
            if (this.Links.Count == 0) return 0;
            else if (this.Links.Count == 1)
            {
                return this.Destination.Offset - startLocation.Offset;
            }
            else
            {
                double remainingDistance = this.WeightSum;
                remainingDistance -= startLocation.Offset;
                remainingDistance -= (this.Destination.Link.Length - this.Destination.Offset);

                return remainingDistance;
            }
        }
        #endregion Other End

        #region Implementation of Interfaces
        public bool Equals(PinokioPath other)
        {
            if (this.Links.Count != other.Links.Count) return false;

            for (int i = 0; i < this.Links.Count; i++)
            {
                if (this.Links[i] != other.Links[i]) return false;
            }

            return true;
        }

        public int CompareTo(PinokioPath other)
        {
            if (other is null) throw new Exception("Null Path와는 비교할 수 없다.");

            return WeightSum.CompareTo(other.WeightSum);
        }

        public static bool operator ==(PinokioPath path1, PinokioPath path2)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(path1, null))
            {
                if (Object.ReferenceEquals(path2, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            else if (Object.ReferenceEquals(path2, null))
            {
                return false;
            }
            // Equals handles case of null on right side.
            return path1.Equals(path2);
        }

        public static bool operator !=(PinokioPath path1, PinokioPath path2)
        {
            return !(path1 == path2);
        }
        #endregion Implementation of Interfaces End
    }
}
