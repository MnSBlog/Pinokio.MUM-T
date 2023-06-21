using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;
using Pinokio.Geometry;
using System.Security.Cryptography.X509Certificates;

namespace Pinokio.Map
{
    public enum BayType
    {
        Interbay,
        Intrabay,
    }

    public class AoBay : AbstractObject
    {
        public string FabName { get; set; }

        public bool IsReticle { get; set; }

        public Location RoamingLocation1 { get; set; }

        public Location RoamingLocation2 { get; set; }

        public Location StartLocation1 { get; set; }

        public Location StartLocation2 { get; set; }

        public List<MapLink> Links { get; }

        public List<string> NeighborBayNames { get; }

        public double MaxX = double.MinValue;
        public double MinX = double.MaxValue;
        public double MaxY = double.MinValue;
        public double MinY = double.MaxValue;
        public Vector3 CenterPos
        {
            get => new Vector3((MinX + MaxX) / 2, (MinY + MaxY) / 2, 0);
        }

        public AoBay(string fabName, string name, BayType type, bool isReticle) : base(0, name, type)
        {
            FabName = fabName;
            IsReticle = isReticle;
            Links = new List<MapLink>();
            NeighborBayNames = new List<string>();
        }

        public AoBay(string fabName, uint id, string name) : base(id, name, BayType.Intrabay)
        {
            FabName = fabName;
            Links = new List<MapLink>();

        }

        public void InitializeRoamingLocation()
        {
            // 각 베이의 진출 링크 중, 제일 위쪽 링크와 아래쪽 링크를 설정한다.
            double maxY = double.MinValue;
            double minY = double.MaxValue;
            MapLink topLink = null, bottomLink = null; // 가장 위 아래에 위치하면서, OutLink > 1
            foreach (var link in Links)
            {
                if (link.ToNode.OutLinks.Count > 1)
                {
                    if (link.ToNode.Position.Y > maxY)
                    {
                        topLink = link;
                        maxY = link.ToNode.Position.Y;
                    }

                    if (link.ToNode.Position.Y < minY)
                    {
                        bottomLink = link;
                        minY = link.ToNode.Position.Y;
                    }
                }

            }

            if (topLink != null && bottomLink != null)
            {
                RoamingLocation1 = new Location(topLink, topLink.Length - 100);
                RoamingLocation2 = new Location(bottomLink, bottomLink.Length - 100);
            }
            else
            {
                // 분기가 없는 경우를 대비해서..
                RoamingLocation1 = new Location(Links[0], Links[0].Length - 100);
                RoamingLocation2 = new Location(Links.Last(), Links.Last().Length - 100);
            }
        }

        public void InitializeStartEndLocation()
        {
            double maxY = double.MinValue;
            double minY = double.MaxValue;
            MapLink startLink1 = null, startLink2 = null;
            foreach (var link in Links)
            {
                if (link.FromNode.InLinks.Count > 1)
                {
                    if (link.ToNode.Position.Y > maxY)
                    {
                        startLink1 = link;
                        maxY = link.ToNode.Position.Y;
                    }

                    if (link.ToNode.Position.Y < minY)
                    {
                        startLink2 = link;
                        minY = link.ToNode.Position.Y;
                    }
                }
            }

            if (startLink1 == null && startLink2 == null)
            {
                startLink1 = Links.First();
                startLink2 = Links.Last();
            }

            StartLocation1 = new Location(startLink1.FromNode);
            StartLocation2 = new Location(startLink2.FromNode);
        }
    }
}
