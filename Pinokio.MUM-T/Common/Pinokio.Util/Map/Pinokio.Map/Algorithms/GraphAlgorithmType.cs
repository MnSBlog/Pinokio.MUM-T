using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Map.Algorithms
{
    public enum ShortestPathAlgorithmType
    {
        None,
        Dijkstra,
        FloydWarshall,
        BellmanFord,
        AStar, // Not Implemented
        KShortest, // Not Implemented
        TurnDijkstra,
    }

    public enum KShortestPathAlgorithmType
    {
        None, Yen, Eppstein,
    }
}
