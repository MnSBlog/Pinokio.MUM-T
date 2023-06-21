using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Simulation;

namespace Pinokio.Simulation.Cont
{
    public class ContSimModel : SimModel
    {
        public ContSimModel(uint id, string name, Enum type) : base(id, name, type)
        {
            
        }

        public override void InitializeModel(EventCalendar eventCalendar)
        {
            base.InitializeModel(eventCalendar);
        }

        public virtual void Update(SimTime timeNow)
        {
            
        }
    }
}
