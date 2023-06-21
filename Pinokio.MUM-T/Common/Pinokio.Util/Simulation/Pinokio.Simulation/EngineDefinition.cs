using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Simulation
{
    public enum EngineState
    {
        Ready,
        Pause, 
        Running,
        Stop, 
        End,
    }

    public enum EndCondition
    {
        None, 
        Time, 
    }

    public enum EventType
    {
        Internal,
        External,
    }
}
