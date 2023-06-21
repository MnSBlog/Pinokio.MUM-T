using System;
using System.Linq;
using Pinokio.Simulation;

namespace Pinokio.Simulation.Cont
{
    public class ContSimEngine : SimEngine
    {
        public ContSimEngine() : base()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            //EvtCalendar = new ContEvtCalendar();
        }

        protected override void EngineCore()
        {
            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            //var cEvtCalendar = EvtCalendar as ContEvtCalendar;
            //cEvtCalendar.ExcuteEventBefore(TNow);

            //var contModels = this.Models.FindAll(m => m is ContSimModel).ToList();
            //contModels.ForEach(cm => ((ContSimModel)cm).Update(TNow));
            //sw.Stop();
            //TNow = TNow + SimTime.FromMilliseconds(ContSimParameter.Tick);
            //var a = ((double)ContSimParameter.Tick / (double)ContSimParameter.RunSpeed) * TimeSpan.TicksPerMillisecond;
            ////var sleepTime = new TimeSpan((ContSimParameter.Tick / ContSimParameter.RunSpeed) * TimeSpan.TicksPerMillisecond);
            //var sleepTime = new TimeSpan((long)a);
            //var elapsedTime = sw.Elapsed;
            //if (elapsedTime < sleepTime)
            //{
            //    sleepTime -= elapsedTime;
            //    SleepAccurately(sleepTime);
            //}
        }

        public void SleepAccurately(TimeSpan time)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            if (time.Ticks >= 50 * TimeSpan.TicksPerMillisecond)
                System.Threading.Thread.Sleep(time - new TimeSpan(250000));

            while (sw.Elapsed < time)
                ;
        }
    }
}
