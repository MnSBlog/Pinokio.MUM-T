using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Pinokio.Core;

namespace Pinokio.Simulation
{
    public class EventCalendar : AbstractObject
    {
        protected object LockObj = new object();
        protected uint LastEvtId;
        protected List<Event> EvtList;

        public EventCalendar() : base(0)
        { }

        public override void Initialize()
        {
            base.Initialize();
            LastEvtId = 0;
            EvtList = new List<Event>();
        }

        public void AddEvent(SimTime time, SimModel model, SimPort port)
        {
            var nextEvent = new Event(++LastEvtId, time, model, port);
            lock (LockObj)
            {
                int index = -1;
                for (int i = 0; i < EvtList.Count; i++)
                {
                    if (EvtList[i] == null) // Null Exception
                    {
                        EvtList.RemoveAt(i);
                        i--; continue;
                    }

                    if (time >= EvtList[i].Time)
                        index = i;
                    else
                        break;
                }
                EvtList.Insert(index + 1, nextEvent);
            }
        }

        public Event GetNextEvent()
        {
            Event nextEvent = null;
            if (EvtList.Count > 0)
            {
                nextEvent = EvtList[0];
                EvtList.RemoveAt(0);
            }
            return nextEvent;
        }
        public List<Event> GetEventList()
        {
            return EvtList;
        }

        public void Clear()
        {
            EvtList.Clear();
        }

    }
}
