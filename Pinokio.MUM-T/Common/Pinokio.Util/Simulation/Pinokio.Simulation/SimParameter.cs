using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Simulation
{
    public static class SimParameter
    {
        private static bool _useWarmUp = false;
        private static DateTime _startDateTime;
        private static SimTime _warmUpPeriod;
        private static SimTime _endOfSimulation = new SimTime(int.MaxValue);

        public static bool UseWarmUp { get => _useWarmUp; }
        public static DateTime StartDateTime { get => _startDateTime; }
        public static SimTime WarmUpPeriod { get =>_warmUpPeriod; }
        public static SimTime EndOfSimulation { get => _endOfSimulation; }

        public static void SetStartDateTime(DateTime startDateTime)
        {
            _startDateTime = startDateTime;
        }

        public static void SetWarmUp(bool needToWarmUp)
        {
            _useWarmUp = needToWarmUp;
        }
       
        public static void SetWarmUpPeriod(SimTime warmUpPeriod)
        {
            _warmUpPeriod= warmUpPeriod;
        }

        public static void SetEndOfSimulation(SimTime eos)
        {
            _endOfSimulation = eos;
        }
    }
}
