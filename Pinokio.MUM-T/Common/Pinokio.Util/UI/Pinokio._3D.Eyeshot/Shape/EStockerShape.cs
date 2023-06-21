using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using devDept.Geometry;
using devDept.Eyeshot.Entities;

using Pinokio.Core;
using Pinokio.Geometry;
using Pinokio.Simulation;
using Pinokio.Map;

namespace Pinokio._3D.Eyeshot
{
    public class EStockerShape : EyeshotShape
    {
        private static Random r = new Random(0);
        private Dictionary<Vector2, Entity> _stacks;
        private int _numberOfRow;
        private int _numberOfCol;
        private int _numberOfFloor;

        public EStockerShape(uint id, ConcreteObject conObj, ViewPort viewPort) : base(id, conObj, viewPort)
        {
            _stacks = new Dictionary<Vector2, Entity>();
        }

        protected override List<Entity> DrawByShape()
        {
            var stockerModel = (ConcreteObject)this.Core;
            var stockerSpec = (AoStocker)stockerModel.AbstractObj;

            _numberOfRow = stockerSpec.NumberOfRow;
            _numberOfCol = stockerSpec.NumberOfColumn;
            _numberOfFloor = stockerSpec.NumberOfFloor;

            Dictionary<int, string> columnType = new Dictionary<int, string>(); // space와 rack부분 구분하기 위한.. 임시 방책..
            int index = 0;
            columnType.Add(index++, "rack");
            columnType.Add(index++, "space");

            int temp = _numberOfCol - 1;
            while (temp > 1)
            {
                columnType.Add(index++, "rack");
                columnType.Add(index++, "rack");
                columnType.Add(index++, "space");
                temp -= 2;
            }
            if (temp == 1)
                columnType.Add(index++, "rack");

            double xSize = 1.0 / (double)_numberOfRow;
            double ySize = 1.0 / (double)columnType.Count;
            Vector3D oneCellSize = new Vector3D(xSize, ySize, 1);
            var entities = new List<Entity>();
            int colIndex = 0;
            for (int i = 0; i < _numberOfRow; i++)
            {
                for (int j = 0; j < columnType.Count; j++)
                {
                    if (columnType[j] == "rack")
                    {
                        Mesh oneStack = EyeshotCADMart.CreateBox(oneCellSize, DrawSetting.MainColor, new Vector3D(i * xSize, j * ySize, 0));
                        oneStack.ColorMethod = colorMethodType.byEntity;
                        oneStack.Scale(DrawSetting.Width, DrawSetting.Depth, DrawSetting.Height);
                        oneStack.Translate(-DrawSetting.Width / 2, -DrawSetting.Depth / 2);

                        entities.Add(oneStack);
                        _stacks.Add(new Vector2(i, colIndex++), oneStack);
                    }
                }
            }
            
            return entities;
        }

        public override void UpdateColor(object obj)
        {
            var counts = new Dictionary<string, int>((Dictionary<string, int>)obj);
            // Initialize
            List<Vector2> positions = new List<Vector2>();
            var countPerStack = new Dictionary<Vector2, int>();
            foreach (Vector2 pos in _stacks.Keys)
            {
                positions.Add(pos);
                countPerStack.Add(pos, 0);
            }

            int posIndex = 0;
            foreach (string objName in counts.Keys.ToList())
            {
                while (counts[objName] > 0)
                {
                    Vector2 pos = positions[posIndex];
                    if (10 - countPerStack[pos] >= counts[objName])
                    {
                        countPerStack[pos] += counts[objName];
                        counts[objName] = 0;
                    }
                    else // 10 - countsPerStack[pos] < counts[objName]
                    {
                        counts[objName] -= (10 - countPerStack[pos]);
                        countPerStack[pos] = 10;
                    }

                    if (countPerStack[pos] == 10)
                        posIndex++;
                }
            }

            double warnLevel = (double)_numberOfFloor * 0.6;
            double medLevel = (double)_numberOfFloor * 0.2;
            foreach (Vector2 stackPos in _stacks.Keys)
            {
                if (countPerStack[stackPos] == _numberOfFloor)
                {
                    _stacks[stackPos].Color = EquipmentColors.High;
                }
                else if (countPerStack[stackPos] >= warnLevel)
                {
                    _stacks[stackPos].Color = EquipmentColors.Warning;
                }
                else if (countPerStack[stackPos] >= medLevel)
                {
                    _stacks[stackPos].Color = EquipmentColors.Medium;
                }
                else
                {
                    _stacks[stackPos].Color = EquipmentColors.Low;
                }
            }
        }
    }
}
