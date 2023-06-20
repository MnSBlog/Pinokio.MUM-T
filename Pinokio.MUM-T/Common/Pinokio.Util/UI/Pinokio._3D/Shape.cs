using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;
using Pinokio.Geometry;
using Pinokio.Simulation;

namespace Pinokio._3D
{
    public class Shape : PinokioObject
    {
        private PinokioObject _core;
        private DrawSetting _drawSetting;
        protected double AngleX;
        protected double AngleY;
        protected double AngleZ;

        private Vector3 _pos;
        private Vector3 _direction;
        private object _data;

        public PinokioObject Core { get => _core; }
        public DrawSetting DrawSetting { get => _drawSetting; }
        public Vector3 Pos { get => _pos; }
        public Vector3 Direction { get => _direction; }
        public double Radian { get => _direction.AbsoluteAngleRadian(Vector3.Coordinate.Z); }
        public object Data { get => _data; set => _data = value; }
        public Shape(uint id, string name) : base(id, name)
        { } // Core가 없는 Shape, 움직일 수 없음(Like Floor, Fence, Map Node & Link)

        public Shape(uint id, ConcreteObject core) : base(id)
        {
            _core = core;
            if (core is SimObject simObj)
            {
                _pos = simObj.Position;
                _direction = simObj.Direction;
            }
        }

        public Shape(uint id, AbstractObject core) : base(id)
        {
            _core = core;
        }

        public override void Initialize()
        {
            base.Initialize();
            AngleX = 0;
            AngleY = 0;
            AngleZ = 0;

            _pos = Vector3.Zero;
            _direction = Vector3.UnitX;
        }

        public void SetDrawSetting(DrawSetting drawSetting)
        {
            _drawSetting = drawSetting;
        }

        protected void SetPosition(Vector3 pos)
        {
            _pos = pos;
        }

        protected void SetDirection(Vector3 direction)
        {
            _direction = direction;
        }


        public virtual void Draw()
        {

        }

        public virtual void UpdatePosition()
        { }
    }
}