using System;
using System.Collections.Generic;
using System.ComponentModel;

using Pinokio.Core;
using Pinokio.Geometry;

namespace Pinokio.Map
{
    public enum MapLinkType
    {
        None,
        Straight,
        Curved,
    }

    public class MapLink : MapObject
    {
        #region Private Variables
        private MapNode _fromNode;
        private MapNode _toNode;
        private Enum _subType;

        private List<MapPoint> _points;
        private MapPoint _turnPoint;
        private List<VirtualNode> _virtualNodes;
        private List<MapLink> _intersectLinks;

        private object _geometryObject;
        private double _weight;
        private double _length;

        private double _maxSpeed;
        private bool _isBidirectional;
        private bool _isSideWay;
        private Vector2 _designatedPose;
        #endregion

        #region Public Properties
        [PinokioCategory("Map Link", 1)]
        public MapNode FromNode { get => _fromNode; }

        [PinokioCategory("Map Link", 1)]
        public MapNode ToNode { get => _toNode; }

        [Browsable(false)]
        public Enum SubType { get => _subType; }

        [Browsable(false)]
        public List<MapPoint> Points { get => _points; }

        [Browsable(false)]
        public MapPoint TurnPoint { get => _turnPoint; }

        [Browsable(false)]
        public double Weight { get => _weight; }

        [PinokioCategory("Link", 1)]
        public object GeometryObj { get => _geometryObject; }

        [PinokioCategory("Link", 1)]
        public double MaxSpeed { get => _maxSpeed; }

        [PinokioCategory("Link", 1)]
        public bool IsBidirection { get => _isBidirectional; }

        [PinokioCategory("Link", 1)]
        public double Length { get => _length; }

        [PinokioCategory("Link", 1)]
        public bool IsSideWay { get => _isSideWay; }

        [Browsable(false)]
        public List<VirtualNode> VirtalNodes { get => _virtualNodes; }

        [Browsable(false)]
        public List<MapLink> IntersecLinks { get => _intersectLinks; }
        #endregion

        public MapLink(uint mapId, uint id, string name, MapLinkType type, MapNode fromNode, MapNode toNode) : base(mapId, id, name, type)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _intersectLinks = new List<MapLink>();
        }

        #region [Initialize]
        public override void Initialize()
        {
            base.Initialize();

            _isBidirectional = false;
            _points = new List<MapPoint>();
            _virtualNodes = new List<VirtualNode>();
            _designatedPose = new Vector2(0, 0);
        }
        #endregion

        #region [Setter/Gettter]
        public void SetGeometry(DirectionType direction, double radius)
        {
            switch ((MapLinkType)Type)
            {
                case MapLinkType.Curved:
                    try
                    {
                        if (direction == DirectionType.Colinear)
                            throw new Exception($"Can't generate curved link({this.Name}); with collinear direction");

                        var dist = Vector3.Distance(_fromNode.Position, _toNode.Position);
                        if (dist <= 2 * radius)
                        {
                            var startPos = new Vector2(_fromNode.Position.X, _fromNode.Position.Y);
                            var endPos = new Vector2(_toNode.Position.X, _toNode.Position.Y);
                            var arc = new Arc(startPos, endPos, radius, direction);
                            _geometryObject = arc;
                            _length = arc.Length;
                        }
                        else
                            throw new Exception($"Can't generate curved link({this.Name}); Radius is too small.");
                    }
                    catch (Exception e)
                    {
                        LogHandler.AddLog(LogLevel.Error, e.Message.ToString());
                        // 오류시 Geometry를 선으로 정의한다.
                        goto SetGeometryAsStraight;
                    }
                    break;
                case MapLinkType.Straight:
                SetGeometryAsStraight:
                    var line = new LineSegment3D(_fromNode.Position, _toNode.Position);
                    _geometryObject = line;
                    _length = line.Length;
                    break;
            }
        }

        public void SetBidirection(bool isBidirection)
        {
            _isBidirectional = isBidirection;
        }

        public void SetMaxSpeed(double maxSpeed)
        {
            _maxSpeed = maxSpeed;
        }

        public virtual void SetWeight(double weight)
        {
            _weight = weight;
        }

        public void SetLength(double length)
        {
            _length = length;
        }

        public void SetSideWay(bool isSideWay)
        {
            _isSideWay = isSideWay;
        }

        public void SetDesignatedPose(Vector2 pose)
        {
            _designatedPose = pose;
        }

        public void SetSubType(Enum subType)
        {
            _subType = subType;
        }

        public virtual Vector3 GetPosition(double offset = 0)
        {
            try
            {
                switch ((MapLinkType)Type)
                {
                    case MapLinkType.Straight:
                        var line = (LineSegment3D)_geometryObject;
                        return (line.Direction * offset) + _fromNode.Position;
                    case MapLinkType.Curved:
                        var arc = (Arc)_geometryObject;
                        return new Vector3(arc.GetPointByOffset(offset), 0);
                    default:
                        throw new Exception($"Invalid Link Type{this.Name}");
                } 
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Error, e.Message.ToString());
                return new Vector3(0, 0, 0);
            }

        }

        public virtual Vector3 GetDirection(double offset = 0) 
        {
            try
            {
                if (_length > 0)
                {
                    switch ((MapLinkType)Type)
                    {
                        case MapLinkType.Straight:
                            var line = (LineSegment3D)_geometryObject;
                            return (line.Direction);
                        case MapLinkType.Curved:
                            var arc = (Arc)_geometryObject;
                            return new Vector3(arc.DirectionOnArc(offset), 0);
                        default:
                            throw new Exception($"Invalid Link Type{this.Name}");
                    }
                }
                else
                {
                    foreach (var inLink in _fromNode.InLinks)
                    {
                        if (inLink.Length == 0) continue;
                        return inLink.GetDirection(inLink.Length);
                    }

                    return Vector3.UnitX; 
                }
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Error, e.Message.ToString());
                return new Vector3(0, 0, 0);
            }
        }

        public virtual double GetOffset(Vector3 pos)
        {
            try
            {
                switch ((MapLinkType)Type)
                {
                    case MapLinkType.Straight:
                        var line = (LineSegment3D)_geometryObject;
                        return line.GetOffsetByPoint(pos);
                    case MapLinkType.Curved:
                        var arc = (Arc)_geometryObject;
                        return arc.GetOffsetByPoint(new Vector2(pos.X, pos.Y));
                    default:
                        throw new Exception($"Invalid Link Type{this.Name}");
                }
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Error, e.Message.ToString());
                return -1;
            }
        }

        public virtual Vector2 GetPose(double offset = 0)
        {
            if (_designatedPose.Length > 0)
            {
                return _designatedPose;
            }
            else
            {
                var direction = GetDirection(offset);
                var pose = new Vector2(direction.X, direction.Y);
                if (_isSideWay)
                {
                    pose = Vector2.RotateByRadian(pose, System.Math.PI / 2);
                }

                return pose;
            }
        }

        public virtual Polygon GetPolygon(double offset)
        {
            Vector3 pos = this.GetPosition(offset);
            Vector2 pose = this.GetPose(offset);
            return SweepingVolume.FindVolumeByPose(new Vector2(pos.X, pos.Y), pose);
        }

        public VirtualNode GetVirtualNode(double offset)
        {
            if (_virtualNodes.Count > 0)
            {
                for (int i = 0; i < _virtualNodes.Count; i++)
                {
                    var vNode = _virtualNodes[i];
                    if (vNode.Offset > offset + 10) continue;
                    else if (vNode.Offset < offset - 10) continue;
                    else return vNode;
                }
            }

            return null;
        }

        public double GetMaxX()
        {
            return _fromNode.Position.X >= _toNode.Position.X ? _fromNode.Position.X : _toNode.Position.X;
        }

        public double GetMinX()
        {
            return _fromNode.Position.X >= _toNode.Position.X ? _toNode.Position.X : _fromNode.Position.X;
        }

        public double GetMaxY()
        {
            return _fromNode.Position.Y >= _toNode.Position.Y ? _fromNode.Position.Y : _toNode.Position.Y;
        }

        public double GetMinY()
        {
            return _fromNode.Position.Y >= _toNode.Position.Y ? _toNode.Position.Y : _fromNode.Position.Y;
        }

        #endregion

        #region [Point]
        public void SetTurnPoint(MapPoint point)
        {
            _turnPoint = point;
        }
        #endregion

        #region [Other Methods]

        public bool IsOntheLink(Vector3 pos)
        {
            switch (this.Type)
            {
                case MapLinkType.Straight:
                    var line = (LineSegment3D)this.GeometryObj;
                    return line.IsOntheLine(pos);
                case MapLinkType.Curved:
                    var arc = (Arc)this.GeometryObj;
                    return arc.IsOntheArc(new Vector2(pos.X, pos.Y));
            }
            return false;
        }

        public bool IsSamePosition(MapLink otherLink)
        {
            return ((FromNode == otherLink.FromNode || FromNode.AliasNodes.Contains(otherLink.FromNode.Name))
                && (ToNode == otherLink.ToNode || ToNode.AliasNodes.Contains(otherLink.ToNode.Name)))
                || ((FromNode == otherLink.ToNode || FromNode.AliasNodes.Contains(otherLink.ToNode.Name))
                && (ToNode == otherLink.FromNode || ToNode.AliasNodes.Contains(otherLink.FromNode.Name)));
        }

        public bool IsOverlap(MapLink otherLink)
        {
            if (_points.Count == 0) return false;

            MapPoint point10 = _points[0], point1n = _points[_points.Count - 1];
            bool point10OnTheOtherLink = false, point1nOnTheOtherLink = false;
            foreach (var point in otherLink._points)
            {
                if (point10.IsOverlap(point))
                    point10OnTheOtherLink = true;
                if (point1n.IsOverlap(point))
                    point1nOnTheOtherLink = true;
            }
            return (point10OnTheOtherLink && point1nOnTheOtherLink);
        }

        public bool IsIntersectWith(MapLink otherLink)
        {
            return _intersectLinks.Contains(otherLink);
        }

        public IntersectionType CheckIntersect(MapLink otherLink, out List<Vector3> crossingPoint)
        {
            var otherLinkType = (MapLinkType)otherLink.Type;
            switch ((MapLinkType)Type)
            {
                case MapLinkType.Straight:
                    var lineSegment1 = (LineSegment3D)_geometryObject;
                    if (otherLinkType == MapLinkType.Straight)
                    {
                        var lineSegement2 = (LineSegment3D)otherLink.GeometryObj;
                        return lineSegment1.IsIntersectWith(lineSegement2, out crossingPoint);
                    }
                    else if (otherLinkType == MapLinkType.Curved)
                    {
                        var arc1 = (Arc)otherLink.GeometryObj;
                        return arc1.IsIntersectWith(lineSegment1, out crossingPoint);
                    }
                    break;
                case MapLinkType.Curved:
                    var arc2 = (Arc)_geometryObject;
                    if (otherLinkType == MapLinkType.Straight)
                    {
                        var lineSegement2 = (LineSegment3D)otherLink.GeometryObj;
                        return arc2.IsIntersectWith(lineSegement2, out crossingPoint);
                    }
                    else if (otherLinkType == MapLinkType.Curved)
                    {
                        // Not Implemented
                    }
                    break;
            }

            crossingPoint = new List<Vector3>();
            return IntersectionType.None;
        }

        public override string ToString()
        {
            return this.Name;
        }
        #endregion
    }
}
