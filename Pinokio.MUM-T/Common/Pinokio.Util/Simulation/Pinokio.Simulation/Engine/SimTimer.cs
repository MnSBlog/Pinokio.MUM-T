using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Simulation
{
    public class SimTimer
    {
        private SimTime _lastSimTime;
        private SimTime _tick;

        public SimTime Tick { get => _tick; }
        public delegate void TimerEventHandler();

        public event TimerEventHandler OnTimer = delegate () { };


        public SimTimer(SimTime timerTick)
        {
            _lastSimTime = new SimTime(0);
            _tick = timerTick;
        }

        public bool Check(SimTime timeNow)
        {
            if (timeNow > _lastSimTime + _tick)
            {
                OnTimer();
                _lastSimTime = timeNow;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Delegate[] GetAllDelegates() 
        {
            return OnTimer.GetInvocationList();
        }
    }
}
