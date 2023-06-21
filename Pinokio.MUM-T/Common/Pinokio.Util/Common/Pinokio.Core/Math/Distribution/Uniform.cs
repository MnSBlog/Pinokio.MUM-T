using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Core
{
    public class Uniform : Distribution
    {
        public Uniform(double min, double max) : base(DistributionType.Uniform)
        {
            this.min = min;
            this.max = max;
        }

        public override double GetNumber()
        {
            if (max < min) throw new Exception("The range is not valid.");
            double u = random.NextDouble();
            return (min + (max - min) * u);
        }
    }
}
