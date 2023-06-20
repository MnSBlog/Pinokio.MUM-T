﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Core
{
    public enum DistributionType
    {
        None,
        Constant,
        Uniform,
        Lognormal,
        Normal,
        Triangle,
        Exponential,
        Gamma,
    }

    public static class Statistics
    {
        public static Distribution GetDistribution(DistributionType type, double mean, double offset = 0)
        {
            Distribution dist = null;
            switch (type)
            {
                case DistributionType.Constant:
                    dist = new Const(mean);
                    break;
                case DistributionType.Triangle:
                    if (offset >= 0)
                    {
                        dist = new Triangle(mean - offset, mean + offset, mean);
                    }
                    else
                        LogHandler.AddLog(LogLevel.Error, $"Cannot build '{type.ToString()}' distribution with negative offset ({offset})");
                    break;
                case DistributionType.Lognormal:
                    dist = new LogNormal(mean, offset);
                    break;
                case DistributionType.Uniform: // 여기부터!
                    if (offset >= 0)
                    {
                        dist = new Uniform(mean - offset, mean + offset);
                    }
                    else
                        LogHandler.AddLog(LogLevel.Error, $"Cannot build '{type.ToString()}' distribution with negative offset ({offset})");
                    break;
                case DistributionType.Exponential:
                    if (offset == 0)
                    {
                        dist = new Exponential(mean);
                    }
                    else
                        LogHandler.AddLog(LogLevel.Error, $"Cannot build '{type.ToString()}' distribution with Mean and Offset");
                    break;
                case DistributionType.Gamma:
                    if (offset == 0)
                    {
                        dist = new Gamma(mean);
                    }
                    else
                        LogHandler.AddLog(LogLevel.Error, $"Cannot build '{type.ToString()}' distribution with Mean and Offset");
                    break;
                case DistributionType.Normal:
                    throw new NotImplementedException($"Not Implemented Distribution '{type.ToString()}'");

            }
            return dist;
        }

        public static Distribution GetDistribution(DistributionType type, double[] parameters)
        {
            Distribution dist = new Distribution(type);
            switch (type)
            {
                case DistributionType.Constant:
                    if (parameters.Length >= 1)
                    {
                        dist = new Const(parameters[0]);
                    }
                    else
                        LogHandler.AddLog(LogLevel.Error, $"Not enough parameters to build '{type.ToString()}' distribution ({parameters.Length})");
                    break;
                case DistributionType.Triangle:
                    if (parameters.Length >= 3)
                    {
                        dist = new Triangle(parameters[0], parameters[1], parameters[2]);
                    }
                    else
                        LogHandler.AddLog(LogLevel.Error, $"Not enough parameters to build '{type.ToString()}' distribution ({parameters.Length})");
                    break;
                case DistributionType.Lognormal:
                    if (parameters.Length >= 2)
                    {
                        dist = new LogNormal(parameters[0], parameters[1]);
                    }
                    else
                        LogHandler.AddLog(LogLevel.Error, $"Not enough parameters to build '{type.ToString()}' distribution ({parameters.Length})");
                    break;
                case DistributionType.Uniform:
                    if (parameters.Length >= 2)
                    {
                        dist = new Uniform(parameters[0], parameters[1]);
                    }
                    else
                        LogHandler.AddLog(LogLevel.Error, $"Not enough parameters to build '{type.ToString()}' distribution ({parameters.Length})");
                    break;
                case DistributionType.Exponential:
                    if (parameters.Length >= 1)
                    {
                        dist = new Exponential(parameters[0]);
                    }
                    else
                        LogHandler.AddLog(LogLevel.Error, $"Not enough parameters to build '{type.ToString()}' distribution ({parameters.Length})");
                    break;
                case DistributionType.Gamma:
                    if (parameters.Length >= 1)
                    {
                        dist = new Gamma(parameters[0]);
                    }
                    else
                        LogHandler.AddLog(LogLevel.Error, $"Not enough parameters to build '{type.ToString()}' distribution ({parameters.Length})");
                    break;
                case DistributionType.Normal:
                    throw new NotImplementedException($"Not Implemented Distribution '{type.ToString()}'");
            }
            return dist;
        }


        public static double UpdateMean(double prevMean, int prevCount, double newValue)
        {
            return (prevMean * prevCount + newValue) / (prevCount + 1);
        }
        //public static DistributionType Parse(string distributionType)
        //{
            
        //}
    }
}
