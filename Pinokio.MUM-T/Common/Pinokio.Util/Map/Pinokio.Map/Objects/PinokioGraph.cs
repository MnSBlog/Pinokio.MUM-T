using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Pinokio.Core;
using Pinokio.Geometry;

namespace Pinokio.Map
{
    public class PinokioGraph : AbstractObject
    {
        #region ObjectIds
        public uint LastNodeId;
        public uint LastLinkId;
        public uint LastPointId;
        protected uint LastVNodeId;
        #endregion

        #region Objects
        private Dictionary<string, MapNode> _nodes;
        private Dictionary<string, MapLink> _links;
        private Dictionary<string, MapPoint> _points;
        private Dictionary<string, VirtualNode> _virtualNodes;
        #endregion

        #region Public Properties
        public Dictionary<string, MapNode> Nodes { get => _nodes; }
        public Dictionary<string, VirtualNode> VirtualNodes { get => _virtualNodes; }
        public Dictionary<string, MapLink> Links { get => _links; }
        public Dictionary<string, MapPoint> Points { get => _points; }
        #endregion

        public PinokioGraph(uint id, string name = "") : base(id, name)
        { }

        public PinokioGraph(PinokioGraph graph) : base(0)
        {
            _nodes = new Dictionary<string, MapNode>(graph.Nodes);
            _links = new Dictionary<string, MapLink>(graph.Links);
            _points = new Dictionary<string, MapPoint>(graph.Points);
            _virtualNodes = new Dictionary<string, VirtualNode>(graph.VirtualNodes);
        }

        public override void Initialize()
        {
            base.Initialize();
            LastNodeId = 0;
            LastLinkId = 0;
            LastPointId = 0;
            LastVNodeId = 0;

            _nodes = new Dictionary<string, MapNode>();
            _links = new Dictionary<string, MapLink>();
            _points = new Dictionary<string, MapPoint>();
            _virtualNodes = new Dictionary<string, VirtualNode>();
        }

        #region [Setter / Getter]
        public MapNode GetNode(string name)
        {
            return _nodes.Values.First(n => n.Name == name);
        }

        public MapNode GetNode(uint id)
        {
            return _nodes.Values.First(n => n.Id == id);
        }

        public MapLink GetLink(MapNode fromNode, MapNode toNode)
        {
            try
            {
                foreach (var link in _links.Values)
                {
                    if (link.FromNode == fromNode && link.ToNode == toNode)
                        return link;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        public MapLink GetLink(string fromId, string toId)
        {
            return this.GetLink(_nodes[fromId], _nodes[toId]);
        }

        public MapPoint GetPoint(Location location)
        {
            if (location.Link is null) return null;
            int index = Convert.ToInt32(System.Math.Truncate(location.Offset / 100));
            if (index < location.Link.Points.Count())
            {
                return location.Link.Points[index];
            }
            else
            {
                LogHandler.AddLog(LogLevel.Error, $"PinokioGraph.cs : There is no map point for link{location.Link.Name} / {location.Offset}");
                LogHandler.AddLog(LogLevel.Error, $"PinokioGraph.cs : LinkLength = {location.Link.Length} / Point Count = {location.Link.Points.Count}");
                return null;
            }
        }

        public MapPoint GetPoint(string linkId, double offset)
        {
            if (!_links.ContainsKey(linkId)) return null;
            return this.GetPoint(new Location(_links[linkId], offset));
        }
        #endregion

        #region [Add]
        public void AddMapNode(MapNode node)
        {
            if (_nodes.ContainsKey(node.Name))
                throw new Exception($"This graph has already contained node {node.Name}");

            _nodes.Add(node.Name, node);
        }

        public void AddMapLink(MapLink link)
        {
            if (_links.ContainsKey(link.Name))
                throw new Exception($"This graph has already contained link {link.Name}");

            _links.Add(link.Name, link);
            link.FromNode.OutLinks.Add(link);
            link.ToNode.InLinks.Add(link);
        }

        public virtual void AddMapPoint(string pointName, MapPoint point)
        {
            if (this.Points.ContainsKey(pointName))
                throw new Exception($"This graph has already contained point {pointName}");
            else
            {
                _points.Add(pointName, point);
            }
        }

        public virtual void AddVirtualNode(MapLink link, Vector3 pos)
        {
            if (link.IsOntheLink(pos))
            {
                var fromVec = pos - link.FromNode.Position;
                var toVec = pos - link.ToNode.Position;
                if (fromVec.LengthSquared() > 1 && toVec.LengthSquared() > 1)
                {
                    var offset = link.GetOffset(pos);
                    if (offset < 100) { return; }
                    else if (offset > link.Length - 100) { return; }

                    int index = 0;
                    for (int i = 0; i < link.VirtalNodes.Count; i++)
                    {
                        var otherOffset = link.VirtalNodes[i].Offset;
                        if (offset < otherOffset - 1)
                        {
                            break;
                        }
                        else if (otherOffset - 1 <= offset && offset < otherOffset + 1)
                        {
                            return; // 이미 있는 다른 VirtualNode랑 위치가 동일 --> 생략가능
                        }
                        else
                        {
                            index = i + 1;
                        }
                    }

                    VirtualNode vNode = GenerateVirtualNode(link.Name, offset, pos);
                    vNode.InLinks.Add(link);
                    vNode.OutLinks.Add(link);
                    vNode.SweepingVolumes.Add(link.GetPolygon(offset));

                    link.VirtalNodes.Insert(index, vNode);
                    _nodes.Add(vNode.Name, vNode);
                    _virtualNodes.Add(vNode.Name, vNode);
                }
            }
        }
        #endregion

        #region [Generate]
        public virtual VirtualNode GenerateVirtualNode(string linkName, double offset, Vector3 pos)
        {
            var virtualNode = new VirtualNode(this.Id, ++LastVNodeId, pos, linkName, offset);
            return virtualNode;
        }
        #endregion

        public virtual void RemoveMapNode(string nodeName)
        {
            if (_nodes.ContainsKey(nodeName))
            {
                var connLinks = _nodes[nodeName].InLinks.ToList();
                connLinks.AddRange(_nodes[nodeName].OutLinks);
                connLinks.ForEach(l => this.RemoveMapLink(l.Name));
                _nodes.Remove(nodeName);
            }
        }

        public virtual void RemoveMapLink(string linkName)
        {
            if (_links.ContainsKey(linkName))
            {
                var link = _links[linkName];
                link.FromNode.OutLinks.Remove(link);
                link.ToNode.InLinks.Remove(link);
                _links.Remove(linkName);
            }
        }

        public virtual void SetNeighborhoodParallelly()
        {
            var sw = new Stopwatch();
            sw.Start();
            Parallel.ForEach(_links.Values, link =>
            {
                FindNeighbors(link.TurnPoint);
                Parallel.ForEach(link.Points, point =>
                {
                    FindNeighbors(point);
                });
            });
            sw.Stop();
            LogHandler.AddLog(LogLevel.Info, $"Point 이웃관계 계산시간 : {sw.ElapsedMilliseconds}");
        }

        public void SetPointsSweepVolume()
        {
            var sw = new Stopwatch();
            sw.Start();
            foreach (MapLink link in _links.Values)
            {
                // Find Sweeping Volumes for outlinks
                var pos = link.TurnPoint.Pos;
                var outPose = link.TurnPoint.Pose;
                var firstVolume = link.TurnPoint.SweepingVolumes[0];
                var volumes = new List<Polygon>();
                foreach (MapLink inLink in link.FromNode.InLinks)
                {
                    var inPose = inLink.Points.Last().Pose;
                    var angleDifference = Vector2.AngleDegree(outPose, inPose);

                    if (angleDifference <= 0 || angleDifference >= 180)
                        continue;

                    double angle = 5.0;
                    while (angle <= angleDifference)
                    {
                        var newVolume = Polygon.RotateByDegree(firstVolume, pos, angle);
                        volumes.Add(newVolume);
                        angle += 5.0;
                    }
                }

                link.TurnPoint.SweepingVolumes.AddRange(volumes);
            }
            sw.Stop();
            LogHandler.AddLog(LogLevel.Info, $"관리Point의 SweepVolume 구축시간: {sw.ElapsedMilliseconds}");
        }

        private void FindNeighbors(MapPoint point)
        {
            foreach (MapPoint otherPoint in _points.Values)
            {
                if (point == otherPoint) continue;
                else if (point.IsNeighbor(otherPoint))
                    point.AddNeighborPoint(otherPoint);
            }
        }

        #region Intersection
        public void FindAllIntersections()
        {
            this.FindIntersectionOfLinks();
            this.CheckAliasNodes();
            this.FindIntersectionOfNodesAndLinks();
        }

        /// <summary>
        /// Graph의 Link들간 교차여부 확인하여 교차하는 지점에 Virtual Node를 생성한다.
        /// </summary>
        private void FindIntersectionOfLinks()
        {
            _links.Values.ToList().ForEach(l => l.IntersecLinks.Clear());

            List<string> linkIds = _links.Keys.ToList();
            int count = linkIds.Count;

            // 모든 링크들에 대해서 교차여부 확인..
            for (int i = 0; i < count - 1; i++)
            {
                MapLink link1 = _links[linkIds[i]];
                for (int j = i + 1; j < count; j++)
                {
                    MapLink link2 = _links[linkIds[j]];
                    if (CheckIntersection(link1, link2, out List<Vector3> pointsOnLink))
                    {
                        link1.IntersecLinks.Add(link2);
                        link2.IntersecLinks.Add(link1);
                        foreach (Vector3 point in pointsOnLink)
                        {
                            AddVirtualNode(link1, point);
                            AddVirtualNode(link2, point);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Link들에 대한 Node의 교차여부 확인
        /// </summary>
        private void FindIntersectionOfNodesAndLinks()
        {
            // 모든 Link와 Node를 비교한다.
            foreach (MapLink link in _links.Values)
            {
                Vector3 fromPos = link.FromNode.Position;
                Vector3 toPos = link.ToNode.Position;
                var lineSegment = new LineSegment2D(new Vector2(fromPos.X, fromPos.Y), new Vector2(toPos.X, toPos.Y));
                foreach (MapNode node in _nodes.Values.ToList())
                {
                    if (CheckIntersection(link, lineSegment, node, out Vector2 pointOnLink))
                    {
                        AddVirtualNode(link, new Vector3(pointOnLink.X, pointOnLink.Y, 0));
                    }
                }
            }
        }

        private bool CheckIntersection(MapLink link1, MapLink link2, out List<Vector3> pointsOnLink)
        {
            pointsOnLink = null;

            // 쓸모없는 비교를 없애기 위한 조건문..(20%개선)
            if (link1.GetMaxX() + link1.Length < link2.GetMinX()) return false;
            else if (link2.GetMaxX() + link2.Length < link1.GetMinX()) return false;
            else if (link1.GetMaxY() + link1.Length < link2.GetMinY()) return false;
            else if (link2.GetMaxY() + link2.Length < link1.GetMinY()) return false;

            IntersectionType type = link1.CheckIntersect(link2, out pointsOnLink);

            return !(type is IntersectionType.None);
        }

        private bool CheckIntersection(MapLink link, LineSegment2D line, MapNode node, out Vector2 pointOnLink)
        {   
            pointOnLink = new Vector2(double.NaN);
            if (node is VirtualNode) return false;
            else if (node == link.FromNode) return false;
            else if (node == link.ToNode) return false;
            else if (link.GetMaxX() + link.Length < node.Position.X) return false;
            else if (link.GetMaxY() + link.Length < node.Position.Y) return false;
            else if (link.GetMinX() - link.Length > node.Position.X) return false;
            else if (link.GetMinY() - link.Length > node.Position.Y) return false;
            else if (link.FromNode.AliasNodes.Contains(node.Name)) return false;
            else if (link.ToNode.AliasNodes.Contains(node.Name)) return false;

            Vector2 pos = new Vector2(node.Position.X, node.Position.Y);
            if (line.IsOntheLine(pos)) return false;

            // 선분까지 수직선상 거리를 구했을 때 가까운지 확인
            pointOnLink = line.GetPerpencularPoint(pos);

            // 선분까지 수직선상 거리를 구할 수 없음
            if (double.IsNaN(pointOnLink.X) || double.IsNaN(pointOnLink.Y)) return false;

            double offset = link.GetOffset(new Vector3(pointOnLink, 0));
            Polygon svOnLink = link.GetPolygon(offset);

            bool intersect = false;
            foreach (Polygon sv in node.SweepingVolumes)
            {
                if (svOnLink.IsIntersect(sv))
                {
                    intersect = true;
                    break;
                }
            }

            if (!intersect) return false;

            // 가까이 있지만 연결관계상 Intersect로 처리할 필요가 없는 경우를 찾아냄
            foreach (MapLink outLink in node.OutLinks)
            {
                if (outLink.ToNode == link.ToNode || outLink.ToNode.AliasNodes.Contains(link.ToNode.Name) ||
                    outLink.ToNode == link.FromNode || outLink.ToNode.AliasNodes.Contains(link.FromNode.Name))
                {
                    //Console.WriteLine(node.Name + link.Name);
                }
                else if (link.IsIntersectWith(outLink)) // Link와 Node의 OutLink가 교차할 때?  왜지..
                {
                    if (outLink.ToNode.IsVeryClose(pointOnLink))
                    {
                        return false;
                    }

                    foreach (MapNode vNode in outLink.VirtalNodes) // OutLink의 VNode와 pointOnLink가 충분히 가깝다면 
                    {
                        var pos2D = new Vector2(vNode.Position.X, vNode.Position.Y);
                        double distSquared = Vector2.DistanceSquared(pointOnLink, pos2D);
                        // 충분히 가까울 경우에 VNode 생성
                        if (distSquared < 100) // dist is close than 100
                        {
                            return false;
                        }
                    }
                }
            }

            foreach (MapLink inLink in node.InLinks)
            {
                if (inLink.FromNode == link.ToNode || inLink.FromNode.AliasNodes.Contains(link.ToNode.Name) ||
                    inLink.FromNode == link.FromNode || inLink.FromNode.AliasNodes.Contains(link.FromNode.Name))
                {
                    //Console.WriteLine(node.Name + link.Name);
                }
                else if (link.IsIntersectWith(inLink))
                {
                    foreach (MapNode vNode in inLink.VirtalNodes)
                    {
                        var pos2D = new Vector2(vNode.Position.X, vNode.Position.Y);
                        var distSquared = Vector2.DistanceSquared(pointOnLink, pos2D);
                        if (distSquared < 100)
                        {
                            return false;
                        }
                    }

                    if (inLink.FromNode.IsVeryClose(pointOnLink))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

        #region [Other Methods]
        public void CheckAliasNodes()
        {
            // Initialize AliasNodes
            _nodes.Values.ToList().ForEach(n => n.AliasNodes.Clear());

            List<string> ids = _nodes.Keys.ToList();
            for (int i = 0; i < ids.Count - 1; i++)
            {
                MapNode node1 = _nodes[ids[i]];
                for (int j = i + 1; j < ids.Count; j++)
                {
                    MapNode node2 = _nodes[ids[j]];
                    if (node1.IsVeryClose(node2) && 
                        !node1.AliasNodes.Contains(node2.Name))
                    {
                        node1.AliasNodes.Add(node2.Name);
                        node2.AliasNodes.Add(node1.Name);
                    }
                }
            }
        }

        public void ConstructNodeSweepingVolume()
        {
            foreach (MapNode node in _nodes.Values)
            {
                var pos = new Vector2(node.Position.X, node.Position.Y);
                foreach (MapLink inLink in node.InLinks)
                {
                    if (inLink.Length <= double.Epsilon) continue;
                    Vector2 pose = inLink.GetPose(inLink.Length);
                    if (pose.Length > 0)
                    {
                        Polygon sv = SweepingVolume.FindVolumeByPose(pos, pose);
                        node.SweepingVolumes.Add(sv);

                        if (node.NoRotation) break;
                        foreach (MapLink outLink in node.OutLinks)
                        {
                            if (outLink.Length <= double.Epsilon) continue;
                            Vector2 outPose = outLink.GetPose(0);
                            double angleDifference = Math.Round(Vector2.AngleDegree(outPose, pose));
                            //double angleDifference = (Vector2.AngleDegree(outPose, pose));

                            if (angleDifference <= 0 || angleDifference >= 180)
                                continue;

                            double degree = 5.0;
                            while (degree <= angleDifference)
                            {
                                Polygon newVolume = Polygon.RotateByDegree(sv, pos, degree);
                                node.SweepingVolumes.Add(newVolume);
                                degree += 5.0;
                            }
                        }
                    }
                }
            }
        }

        public void CheckBidirectionalLink()
        {
            foreach (MapLink link in _links.Values)
            {
                bool isBidirection = false;
                foreach (MapLink outLink in link.ToNode.OutLinks)
                {
                    if (link.FromNode == outLink.ToNode)
                    {
                        isBidirection = true;
                        break;
                    }
                }

                if (!isBidirection)
                {
                    if (link.ToNode.AliasNodes.Count > 0)
                    {
                        foreach (var alias in link.ToNode.AliasNodes)
                        {
                            var aliasNode = _nodes[alias];
                            foreach (var outLink in aliasNode.OutLinks)
                            {
                                if (link.FromNode == outLink.ToNode ||
                                    link.FromNode.AliasNodes.Contains(outLink.ToNode.Name))
                                {
                                    isBidirection = true;
                                    break;
                                }
                            }

                            if (isBidirection)
                                break;
                        }
                    }
                }

                link.SetBidirection(isBidirection);
            }
        }

        public bool FindReplaceableLink(MapLink link, out List<MapLink> replaceableLinks)
        {
            replaceableLinks = new List<MapLink>();

            var fromNodeIds = new List<string>(link.FromNode.AliasNodes);
            fromNodeIds.Add(link.FromNode.Name);

            var toNodeIds = new List<string>(link.ToNode.AliasNodes);
            toNodeIds.Add(link.ToNode.Name);

            if (fromNodeIds.Count > 1 || toNodeIds.Count > 1)
            {
                foreach (string toId in toNodeIds)
                {
                    if (link.ToNode.Position == _nodes[toId].Position)
                    {
                        foreach (string fromId in fromNodeIds)
                        {
                            if (link.FromNode.Position == _nodes[fromId].Position)
                            {
                                var otherLink = this.GetLink(fromId, toId);
                                if (otherLink is null || otherLink == link) continue;
                                else replaceableLinks.Add(otherLink);
                            }
                        }
                    }
                }
            }

            return replaceableLinks.Count > 0;
        }
        #endregion
    }
}
