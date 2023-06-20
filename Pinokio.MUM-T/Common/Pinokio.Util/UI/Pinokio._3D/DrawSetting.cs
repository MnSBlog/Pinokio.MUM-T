using System.Drawing;
using Pinokio.Geometry;

namespace Pinokio._3D
{
    public enum FileType
    {
        None, Obj,
    }
    public class DrawSetting
    {
        public Vector3 Size { get; set;  }
        public double Width { get => Size.X; }
        public double Depth { get => Size.Y; }
        public double Height { get => Size.Z; }
        public Color MainColor { get; set; }
        public Color SubColor { get; set; }
        public Vector3 FromPos { get; set; }
        public Vector3 ToPos { get; set; }
        public bool DrawByFile { get; set; }
        public FileType FileType { get; set; }
        public string FileName { get; set; }
    }
}
