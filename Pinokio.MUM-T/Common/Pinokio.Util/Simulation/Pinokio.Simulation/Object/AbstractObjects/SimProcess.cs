using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;

namespace Pinokio.Simulation
{
    public class SimProcess : AbstractObject
    {
        private Distribution _processTime;
        public SimProcess(uint id) : base(id)
        {
            
        }
    }
}
