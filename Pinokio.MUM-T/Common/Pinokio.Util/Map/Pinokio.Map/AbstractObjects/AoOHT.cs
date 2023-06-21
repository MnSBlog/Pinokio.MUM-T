using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;
namespace Pinokio.Map
{
    public class AoOHT : AbstractObject
    {
        public string FabName { get; set; }
        public double Acceleration { get; set; }
        public double Decceleration { get; set; }
        public double Size { get; set; }
        public double MaxSpeed { get; set; }
        public double MinimumDistance { get; set; }
        public double HoitingSpeed { get; set; }
        public bool UseAcceleration { get; set; }

        public AoOHT(string name) : base(0, name)
        { }
    }
}
