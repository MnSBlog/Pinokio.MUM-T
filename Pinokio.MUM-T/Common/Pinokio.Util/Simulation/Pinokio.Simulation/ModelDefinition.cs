using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Simulation
{
    public enum ModelType
    {
        None = 0,
        Commit,
        Complete,
        Stocker,
        Equipment,
        AGV,
        OHT,
        Port,
        Snapshot,
    }
}
