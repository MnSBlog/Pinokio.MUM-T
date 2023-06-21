using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;
using Pinokio.Geometry;

namespace Pinokio.Map
{
    public class AoStocker : AoResource
    {
        #region Variable
        private int _capacity;
        private Distribution _inputTime;
        private Distribution _outputTime;
        private int _numberOfRow;
        private int _numberOfColumn;
        private int _numberOfFloor;
        #endregion

        #region Propoerty
        public int Capacity { get => _capacity; }
        public Distribution InputTime { get => _inputTime; }
        public Distribution OutputTime { get => _outputTime; }
        public int NumberOfRow { get => _numberOfRow; }
        public int NumberOfColumn { get => _numberOfColumn; }
        public int NumberOfFloor { get => _numberOfFloor; }
        #endregion

        public AoStocker(uint mapId, string name, uint id) : base(mapId, id, name, ResourceType.Stocker)
        {

        }

        public void SetCapacity(int capacity)
        {
            _capacity = capacity;
        }

        public void SetInputTime(Distribution inputTime)
        {
            _inputTime = inputTime;
        }

        public void SetOutputTime(Distribution outputTime)
        {
            _outputTime = outputTime;
        }

        public void SetStockerSize(int rowNum, int colNum, int floorNum)
        {
            _numberOfRow = rowNum;
            _numberOfColumn = colNum;
            _numberOfFloor = floorNum;
        }

    }
}
