using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;
using Pinokio.Geometry;

namespace Pinokio.Map
{
    public class MapPoint : MapObject
    {
        #region Member Variables
        private Vector2 _pos;
        private Vector2 _pose;
        private List<Polygon> _sweepingVolumes;
        private List<MapPoint> _neighbors;
        private string _linkName;
        #endregion

        #region Properties
        public Vector2 Pos { get { return _pos; } }
        public Vector2 Pose
        {
            get { return _pose; }
            set { _pose = value; }
        }
        public List<Polygon> SweepingVolumes { get { return _sweepingVolumes; } }
        public List<MapPoint> Neighbors { get { return _neighbors; } }

        public string LinkName { get => _linkName; }
        #endregion

        public MapPoint(uint mapId, uint id, string name, Vector3 posVec3, string linkName) : base(mapId, id, name)
        {
            _pos = new Vector2(posVec3.X, posVec3.Y);
            _linkName = linkName;
        }

        public override void Initialize()
        {
            base.Initialize();
            _sweepingVolumes = new List<Polygon>();
            _neighbors = new List<MapPoint>();
        }

        #region [:: Set & Get]
        public void SetPose(Vector2 pose)
        {
            _pose = Pose;
        }
        #endregion [Set & Get ::]
        #region [Other Methods]
        public void AddNeighborPoint(MapPoint neighbor)
        {
            if (_neighbors.Contains(neighbor)) return;
            _neighbors.Add(neighbor);
        }

        // 서로가 가진 Sweeping Volume이 교차하면 이웃으로 보는 로직
        public bool IsNeighbor(MapPoint otherPoint)
        {
            if (otherPoint.Pos.X < this.Pos.X - (2 * SweepingVolume.DiagonalLength)) return false;
            if (otherPoint.Pos.X > this.Pos.X + (2 * SweepingVolume.DiagonalLength)) return false;
            if (otherPoint.Pos.Y < this.Pos.Y - (2 * SweepingVolume.DiagonalLength)) return false;
            if (otherPoint.Pos.Y > this.Pos.Y + (2 * SweepingVolume.DiagonalLength)) return false;

            for (int i = 0; i < _sweepingVolumes.Count; i++)
            {
                var thisSV = _sweepingVolumes[i];
                for (int j = 0; j < otherPoint.SweepingVolumes.Count; j++)
                {
                    var otherSV = otherPoint.SweepingVolumes[j];
                    if (thisSV.IsIntersect(otherSV))
                        return true;
                }
            }

            return false;
        }

        public bool IsOverlap(MapPoint otherPoint)
        {
            return (this.Pos.X > otherPoint.Pos.X - 50 && this.Pos.X < otherPoint.Pos.X + 50
                && this.Pos.Y > otherPoint.Pos.Y - 50 && this.Pos.Y < otherPoint.Pos.Y + 50);
        }
        #endregion [Other Methods]
    }
}
