using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Core
{
    public class Distribution
    {
        protected double mean;
        protected double std;
        protected double min;
        protected double max;
        protected double mode;
        protected double alpha;
        protected double beta;
        protected double lamdda;
        protected static Random random = new Random(0);
        public double Mean { get => mean; }
        public readonly DistributionType Type;

        public Distribution(DistributionType type)
        {
            this.Type = type;
        }

        public virtual double GetNumber()
        {
            return 0;
        }

        protected double InverseErrorFunc(double x)
        {
            double a = 0.140012;
            double signal = Signal(x);
            double err = Math.Log(1 - (x * x));
            double value1 = (2 / (Math.PI * a)) + (err / 2);
            double value2 = err / a;

            double result = Math.Sqrt(value1 * value1 - value2) - value1;
            return signal * Math.Sqrt(result);
        }

        protected double Signal(double x)
        {
            if (x > 0) return 1;
            else if (x == 0) return 0;
            else return -1;
        }
    }

    public class Const : Distribution
    {
        public Const(double mean) : base(DistributionType.Constant)
        {
            this.mean = mean;
        }

        public override double GetNumber()
        {
            return this.mean;
        }
    }
}
