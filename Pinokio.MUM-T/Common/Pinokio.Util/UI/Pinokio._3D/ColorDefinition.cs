using System.Drawing;

namespace Pinokio._3D
{
    public static class ColorDefinition
    {
        public static Color Basic = Color.FromArgb(91, 155, 213);
        public static Color Selected = Color.FromArgb(22, 226, 48);
        public static Color SecondarySelected = Color.FromArgb(0, 0, 255);
        public static Color SiteNode = Color.FromArgb(177, 71, 151);
        public static Color TurningNode = Color.FromArgb(215, 138, 34);
        public static Color InLinks = Color.FromArgb(247, 235, 17);
        public static Color OutLinks = Color.FromArgb(255, 0, 0);
        public static Color MZone = Color.FromArgb(255, 127, 39);
        public static Color TZone = Color.FromArgb(255, 0, 0);
        public static Color Neighbor = Color.FromArgb(232, 221, 11);
        public static Color IntersectNeighbor = Color.FromArgb(227, 107, 237);
        public static Color HasManySweepingVolume = Color.FromArgb(17, 205, 244);
        public static Color BufferColor = Color.FromArgb(228, 73, 73);
        public static Color ChargerColor = Color.FromArgb(228, 170, 73);
        public static Color CommitColor = Color.FromArgb(231, 226, 71);
        public static Color CompleteColor = Color.FromArgb(183, 230, 72);
        public static Color ProcessColor = Color.DarkGray;
        public static Color Editing = Color.FromArgb(181, 181, 181);

        public static Color OHT_MoveToLoad = Color.FromArgb(0, 255, 255);
        public static Color OHT_MoveToUnload = Color.FromArgb(0, 255, 255);
        public static Color OHT_Loading = Color.Gold;
        public static Color OHT_Unloading = Color.Gold;
        public static Color OHT_Idle = Color.FromArgb(0, 255, 0);
        public static Color OHT_AreaBalance = Color.FromArgb(204, 255, 204);
        public static Color OHT_Stage = Color.Red;

        public static Color Port_Full = Color.Fuchsia;
        public static Color Port_Reserved = Color.DarkOrange;
        public static Color Port_Idle = Color.Aqua;
        public static Color Port_MHS = Color.Gray;

        public static Color Commit = Color.GreenYellow;
        public static Color Complete = Color.Red;

    }

}
