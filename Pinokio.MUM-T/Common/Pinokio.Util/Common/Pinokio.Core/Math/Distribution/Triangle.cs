using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Core
{
    public class Triangle : Distribution
    {

        public Triangle(double min, double max, double mode) : base(DistributionType.Triangle)
        {
            this.min = min;
            this.max = max;
            this.mode = mode;
        }

        public override double GetNumber()
        {
            if (max < min) throw new ArgumentException("The range is not valid.");
            if (min > mode || mode > max) throw new ArgumentException("Mode value is not valid");

            double u = random.NextDouble();
            double s = (mode - min) / (max - min);
            if (0 < u && u < s)
            {
                double temp = (max - min) * (max - mode);
                return min + Math.Sqrt(s * u);
            }
            else if (s <= u && s < 1)
            {
                double temp = (max - min) * (max - mode);
                return max - Math.Sqrt(temp * (1 - u));
            }
            else
                throw new ArgumentException("Mode value is not valid");

        }
    }
}
