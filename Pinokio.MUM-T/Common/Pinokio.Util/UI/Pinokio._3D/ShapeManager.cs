using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;

namespace Pinokio._3D
{
    public class ShapeManager : AbstractObject
    {
        private List<Shape> _trivialShapes;
        private Dictionary<uint, Shape> _shapes;
        
        public Dictionary<uint, Shape> Shapes { get => _shapes; }
        
        public ShapeManager() : base(0)
        { }

        public override void Initialize()
        {
            base.Initialize();
            _trivialShapes = new List<Shape>();
            _shapes = new Dictionary<uint, Shape>();
        }

        public void AddShape(Shape shape)
        {
            if (shape.Id == 0)
            {
                _trivialShapes.Add(shape);
            }
            else if (!_shapes.ContainsKey(shape.Id))
            {
                _shapes.Add(shape.Id, shape);
            }
        }

        public virtual Shape GenerateShape(ConcreteObject conObj, Enum type)
        {
            return null;
        }


        public virtual void DrawAll()
        {
            foreach (var tShape in _trivialShapes)
            {
                tShape.Draw();
            }

            foreach (var shape in _shapes.Values)
            {
                shape.Draw();
            }
        }

        public virtual void UpdateShapePositions()
        {
            foreach (var shape in _shapes.Values)
            {
                shape.UpdatePosition();
            }
        }
    }
}
