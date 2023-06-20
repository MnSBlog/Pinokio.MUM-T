using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Core
{
    public class LogNormal : Distribution
    {
        public LogNormal(double mean, double std) : base(DistributionType.Lognormal)
        {
            this.mean = mean;
            this.std = std;
        }

        public override double GetNumber()
        {
            // Parameter 
            var v = Math.Log((std * std) / (mean * mean) + 1);
            var m = Math.Log(mean) - v / 2;

            double u = random.NextDouble();
            double sqrt2 = 1.414213562373095; // Math.Sqrt(2)
            double tmp = (InverseErrorFunc(2 * u - 1) * sqrt2 * Math.Sqrt(v)) + m;
            double result = Math.Exp(tmp);
            if (double.IsNaN(result) || double.IsInfinity(result))
                new ArgumentException("Result is not valid");

            return result;
        }
    }
}
