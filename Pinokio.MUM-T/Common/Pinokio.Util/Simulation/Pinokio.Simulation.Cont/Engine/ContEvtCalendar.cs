using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Simulation;

namespace Pinokio.Simulation.Cont
{
    public class ContEvtCalendar : EventCalendar
    {

        public void ExcuteEventBefore(SimTime time)
        {
            for (int i = 0; i < EvtList.Count; i++)
            {
                Event evt = EvtList[i];
                if (time >= evt.Time)
                {
                    evt.ProcessingEvent();
                    EvtList.Remove(evt);
                    i--;
                    if (EvtList.Count <= 0)
                        break;
                }
                else if (time < evt.Time)
                    break;
            }
        }
    }
}
