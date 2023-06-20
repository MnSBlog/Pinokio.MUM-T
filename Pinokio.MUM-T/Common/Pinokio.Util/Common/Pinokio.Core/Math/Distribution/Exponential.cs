using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Core
{
    public class Exponential : Distribution
    {
        public Exponential(double mean) : base(DistributionType.Exponential)
        {
            this.mean = mean;
        }

        public override double GetNumber()
        {
            if (mean <= 0) 
                throw new ArgumentException("Negative value is not allowed");
            double u = random.NextDouble();
            return (-mean * Math.Log(u));
        }
    }
}
