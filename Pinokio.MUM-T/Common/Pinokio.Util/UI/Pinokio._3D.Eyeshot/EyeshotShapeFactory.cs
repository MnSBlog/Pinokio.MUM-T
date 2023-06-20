using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;

using devDept.Eyeshot;
using devDept.Eyeshot.Entities;

namespace Pinokio._3D.Eyeshot
{
    public static class EyeshotShapeFactory
    {
        private static ViewPort _viewPort;
        public static void SetEyeshotViewPort(ViewPort viewPort)
        {
            _viewPort = viewPort;
        }

        public static Shape AddEyeshotShape(this ShapeManager shapeManager, ConcreteObject conObj, Enum shapeType)
        {
            EyeshotShape shape = null;
            switch ((EyeshotShapeType)shapeType)
            {
                case EyeshotShapeType.AGV:
                    shape = new EAGVShape(conObj.Id, conObj, _viewPort);
                    break;
                case EyeshotShapeType.OHT:
                    shape = new EOHTShape(conObj.Id, conObj, _viewPort);
                    break;
                case EyeshotShapeType.Process:
                    shape = new EProcessShape(conObj.Id, conObj, _viewPort);
                    break;
                case EyeshotShapeType.Conveyor:
                    shape = new EConveyorShape(conObj.Id, conObj, _viewPort);
                    break;
                case EyeshotShapeType.InOutConveyor:
                    shape = new EInOutConvShape(conObj.Id, conObj, _viewPort);
                    break;
                case EyeshotShapeType.Boundary:
                    shape = new EBoundaryShape(conObj.Id, conObj, _viewPort);
                    break;
                case EyeshotShapeType.Stocker:
                    shape = new EStockerShape(conObj.Id, conObj, _viewPort);
                    break;
            }

            if (shape != null)
            {
                shapeManager.AddShape(shape);
            }
                
            return shape;
        }

        public static Shape AddEyeshotMapShape(this ShapeManager shapeManager, AbstractObject mapObject, Enum shapeType)
        {
            EyeshotShape shape = null;
            switch ((EyeshotShapeType)shapeType)
            {
                case EyeshotShapeType.Node:
                    shape = new ENodeShape(0, mapObject, _viewPort);
                    break;
                case EyeshotShapeType.Link:
                    shape = new ELinkShape(0, mapObject, _viewPort);
                    break;
                case EyeshotShapeType.Floor:
                    shape = new EFloorShape(0, _viewPort);
                    break;
            }

            if (shape != null)
            {
                shapeManager.AddShape(shape);
            }

            return shape;
        }
    }

}
