using System.Drawing;

namespace Pinokio._3D.Eyeshot
{
    public enum EyeshotShapeType
    {
        None,
        Node,
        Link,
        AGV,
        Process,
        InOutConveyor,
        Conveyor,
        Entity,

        OHT,
        Stocker,

        Floor,
        Boundary,
    }

    public static class OHTColors
    {
        public static Color PreDriving = Color.FromArgb(0, 255, 255);
        public static Color MainDriving = Color.FromArgb(0, 255, 255);
        public static Color Working = Color.Gold;
        public static Color Idle = Color.FromArgb(0, 255, 0);
    }

    public static class EquipmentColors
    {
        public static Color High = Color.FromArgb(192, 13, 30);
        public static Color Warning = Color.Orange;
        public static Color Medium = Color.FromArgb(255, 209, 0);
        public static Color Low = Color.FromArgb(100, 167, 11);
    }
}
