using System;
using System.Collections.Generic;
using System.Drawing;

using Pinokio.Geometry;

using devDept.Geometry;
namespace Pinokio._3D.Eyeshot
{
    public static class EyeshotHelper
    {
        public static Vector3D ToVector3D(Vector3 vec3)
        {
            return new Vector3D(vec3.X, vec3.Y, vec3.Z);
        }

        public static Point3D ToPoint3D(Vector3 vec3)
        {
            return new Point3D(vec3.X, vec3.Y, vec3.Z);
        }
    }

    public static class EyeshotColor
    { 
        private static List<Color> _vehicleColors = new List<Color>()
        {
            Color.Aqua,
            Color.Green,
            Color.Blue,
            Color.Violet,
            Color.Red,
            Color.Orange,
            Color.Gray,
            Color.Black,
            Color.White,
            Color.Gold,
            Color.DarkSlateGray,
            Color.ForestGreen,
            Color.Chocolate,
            Color.Cornsilk,
            Color.Silver,
            Color.Navy,
            Color.DarkSeaGreen,
            Color.PaleVioletRed,
            Color.Cyan,
            Color.Firebrick,
            Color.DeepSkyBlue,
            Color.SaddleBrown,
        };
        public static List<Color> VehicleColors { get => _vehicleColors; }

        public static Color StatusColorGreen { get => Color.FromArgb(100, 167, 11); }
        public static Color StatusColorYellow { get => Color.FromArgb(255, 209, 0); }
        public static Color StatusColorRed{ get => Color.FromArgb(100, 192, 13, 30); }
    }
}
