using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Map.Algorithms
{
    public abstract class ShortestPathAlgorithm
    {
        public virtual PinokioPath FindShortestPath(PinokioGraph graph, string from, string to, List<MapNode> excludingNodes = null, List<MapLink> excludingLinks = null)
        {
            return null;
        }
    }

    public abstract class KShortestPathAlgorithm
    {
        protected ShortestPathAlgorithm _spAlgorithm;

        public KShortestPathAlgorithm(ShortestPathAlgorithm spAlgorithm)
        {
            _spAlgorithm = spAlgorithm;
        }

        public void SetShortestPathAlgorithm(ShortestPathAlgorithm spAlgorithm)
        {
            _spAlgorithm = spAlgorithm;
        }

        public virtual List<PinokioPath> FindKShortestPaths(PinokioGraph graph, string from, string to, int k, List<MapNode> excludingNodes, List<MapLink> excludingLinks)
        {
            return null;
        }
    }



}
