using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using devDept.Geometry;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;

namespace Pinokio._3D.Eyeshot
{ 
    public static class EyeshotCADMart
    {
        private static ViewPort _viewPort;
        private static Dictionary<string, List<Entity>> _models = new Dictionary<string, List<Entity>>();

        public static void SetEyeshotViewPort(ViewPort viewPort)
        {
            _viewPort = viewPort;
        }

        public static void AddObjModel(string name, string path)
        {
            if (_viewPort is null) return;
            if (_models.ContainsKey(name))
                throw new Exception("Already Contained CAD");

            var readObj = new ReadOBJ(path);
            readObj.DoWork();

            foreach (var mat in readObj.Materials)
            {
                if (!_viewPort.Materials.Contains(mat))
                    _viewPort.Materials.Add(mat);
            }
            _models.Add(name, readObj.Entities.ToList());
        }

        public static List<Entity> GetCADByName(string name)
        {
            List<Entity> entities = new List<Entity>();
            if (_models.ContainsKey(name))
            {
                // Reference 오류를 피하기 위해 Original Entities를 Shallow Copy한다..
                _models[name].ForEach(oe => entities.Add((Entity)oe.Clone()));
            }

            return entities;
        }

        public static Mesh CreateBox(Vector3D size, System.Drawing.Color color, Vector3D pos)
        {
            return CreateBox(size.X, size.Y, size.Z, color, pos);
        }

        public static Mesh CreateBox(double width, double depth, double height, System.Drawing.Color color, Vector3D pos)
        {
            Mesh box = Mesh.CreateBox(width, depth, height);
            box.ColorMethod = colorMethodType.byEntity;
            box.Color = color;
            box.Translate(pos);

            return box;
        }

        public static Mesh CreateBox(Vector3D size, string textureName, Vector3D pos)
        {
            return CreateBox(size.X, size.Y, size.Z, textureName, pos);
        }

        public static Mesh CreateBox(double width, double depth, double height, string textureName, Vector3D pos)
        {
            Mesh box = Mesh.CreateBox(width, depth, height);
            box.ApplyMaterial(textureName, textureMappingType.Cubic, 0.5, 1);
            box.Translate(pos);
            return box;
        }

        public static Mesh CreateCylinder(double radius, double height, System.Drawing.Color color, Vector3D pos)
        {
            Mesh box = Mesh.CreateCylinder(radius, height, 20);
            box.ColorMethod = colorMethodType.byEntity;
            box.Color = color;
            box.Translate(pos);
            return box;
        }

        private static float _low = 0.2f;
        private static float _high = 0.5f;
        public static Point2D[] OctagonPoints = new Point2D[]
        {
            new Point2D(_high, _low),
            new Point2D(_low, _high),
            new Point2D(-_low, _high),
            new Point2D(-_high, _low),
            new Point2D(-_high, -_low),
            new Point2D(-_low, -_high),
            new Point2D(_low, -_high),
            new Point2D(_high , -_low),
        };

        public static Region CreateTriangle(double size)
        {
            Point2D[] points = new Point2D[3];
            points[0] = new Point2D(-size / 2, size / 2);
            points[1] = new Point2D(-size / 2, -size / 2);
            points[2] = new Point2D(size / 2, 0);
            return Region.CreatePolygon(points);
        }

        public static Region CreateOctagon(double size)
        {
            var points = new List<Point2D>();
            foreach (var pt in OctagonPoints)
            {
                points.Add(new Point2D(pt.X * size, pt.Y * size));
            }

            var octagon = Region.CreatePolygon(points.ToArray());
            return octagon;
        }
    }
}
