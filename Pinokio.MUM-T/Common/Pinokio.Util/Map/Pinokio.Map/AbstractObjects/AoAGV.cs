using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;
using Pinokio.Geometry;
using Pinokio.Graph;

namespace Pinokio.Map
{
    public class AoAGV : AbstractObject
    {
        #region Memeber Variables
        private Vector3 _size = new Vector3(1000, 1000, 500);
        private double _diagonalLength;

        private double _maxSpeed;
        private double _acceleration;
        private double _sideSpeed;
        private double _sideAcceleration;

        private int _mainTurnTime;
        private int _subTurnTime;

        private double _mttr;
        private double _mtbf;

        private double _batteryLevel = 80;
        private double _batteryCapacity;
        private double _busyConsumptionRate;
        private double _idleConsumptionRate;

        private double _sensingRangeLong = 3000;
        private double _sensingRangeMid = 1000;
        private double _sensingRangeShort = 300;
        #endregion

        #region Public Properties
        /* AGV Size*/
        public Vector3 Size { get => _size; }
        public double Width { get { return _size.X; } }
        public double Depth { get { return _size.Y; } }
        public double Height { get { return _size.Z; } }
        public double DiagonalLength { get { return _diagonalLength; } }

        /* Speed */
        public double MaxSpeed { get { return _maxSpeed; } } // mm/s
        public double Acceleration { get { return _acceleration; } }// mm/s^2
        public double SideSpeed { get { return _sideSpeed; } } // mm/s
        public double SideAcceleration { get { return _sideAcceleration; } }// mm/s^2

        /* Turn Time */
        public int MainTurnTime { get { return _mainTurnTime; } }
        public int SubTurnTime { get { return _subTurnTime; } }

        /* Failure */
        public double MTTR { get { return _mttr; } }
        public double MTBF { get { return _mtbf; } }

        /* Battery */
        public double InitBatteryLevel { get { return _batteryLevel; } }
        public double BatteryCapacity { get { return _batteryCapacity; } }
        public double BusyConsumptionRate { get { return _busyConsumptionRate; } }
        public double IdleConsumptionRate { get { return _idleConsumptionRate; } }

        public double SensingRangeLong { get => _sensingRangeLong; }
        public double SensingRangeMid { get => _sensingRangeMid; }
        public double SensingRangeShort { get => _sensingRangeShort; }
        #endregion

        public AoAGV(uint id) : base(id)
        {
        }

        public void SetAGVSize(double width, double depth, double height)
        {
            _size = new Vector3(width, depth, height);
            _diagonalLength = System.Math.Sqrt((Width / 2) * (Width / 2) + (Depth / 2) * (Depth / 2));
        }

        public void SetAGVSpeed(double maxSpeed, double acceleration, double sideSpeed, double sideAcceleration)
        {
            _maxSpeed = System.Math.Round(maxSpeed * 1000);
            _acceleration = System.Math.Round(acceleration * 1000);
            _sideSpeed = System.Math.Round(sideSpeed * 1000);
            _sideAcceleration = System.Math.Round(sideAcceleration * 1000);
        }

        public void SetTurnTime(int mainTurnTime, int subTurnTime)
        {
            _mainTurnTime = mainTurnTime;
            _subTurnTime = subTurnTime;
        }

        public void SetFailureConstant(double mttr, double mtbf)
        {
            _mttr = mttr;
            _mtbf = mtbf;
        }

        public void SetBatteryOption(double initLevel, double capacity, double busyConsumRate, double idleConsumRate)
        {
            _batteryLevel = initLevel;
            _batteryCapacity = capacity;
            _busyConsumptionRate = busyConsumRate;
            _idleConsumptionRate = idleConsumRate;
        }

        public void SetSensingRange(double longRange, double middleRange, double shortRange)
        {
            _sensingRangeLong = longRange;
            _sensingRangeMid = middleRange;
            _sensingRangeShort = shortRange;
        }
    }
}
