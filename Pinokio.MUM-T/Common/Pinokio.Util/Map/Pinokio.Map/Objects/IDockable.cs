using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Map
{
    public interface IDockable
    {
        List<MapPort> InPorts { get; }
        List<MapPort> OutPorts { get; }
    }
}
