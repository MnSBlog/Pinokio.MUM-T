using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Core
{
    public class NormalDist : Distribution
    {
        public NormalDist(double mean, double std) : base(DistributionType.Normal)
        {
            this.mean = mean;
            this.std = std;
        }

        public override double GetNumber()
        {
            // Parameter 
            double u = random.NextDouble(); // 0.0 ~ 1.0
            double sqrt2 = 1.414213562373095; // Math.Sqrt(2)

            return mean + InverseErrorFunc(2 * u - 1) * sqrt2 * std;
        }
    }
}
