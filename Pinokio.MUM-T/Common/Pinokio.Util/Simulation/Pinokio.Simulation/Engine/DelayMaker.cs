using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Simulation
{
    public class DelayMaker
    {
        private DateTime _lastTime;
        private long _tick;
        public long Tick { get => _tick; }

        public DelayMaker(DateTime timeNow, double delay)
        {
            _lastTime = timeNow;
            _tick = (long)delay;
        }

        public void SetDelay(double ms)
        {
            _tick = (long)ms;
        }

        public void Delay()
        {
            if (_tick > 0)
            {
                var delay = new TimeSpan(_tick * TimeSpan.TicksPerMillisecond);
                var elapsedTime = DateTime.Now - _lastTime;
                //Console.WriteLine(elapsedTime.Milliseconds);
                if (elapsedTime < delay)
                {
                    delay -= elapsedTime;
                    DelayAccurately(delay);
                }
            }

            _lastTime = DateTime.Now;
        }

        private void DelayAccurately(TimeSpan time)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            if (time.Ticks >= 50 * TimeSpan.TicksPerMillisecond)
                System.Threading.Thread.Sleep(time - new TimeSpan(250000));

            while (sw.Elapsed < time)
                ;
        }
    }
}
