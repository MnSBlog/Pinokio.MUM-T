using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Simulation.Cont
{
    public enum CollisionBoundType
    {
        BoundingBox,
        Polygon2D,
    }

    public interface ICollisionable
    {
        object GeometryObj { get; }

        void UpdateCollisionBoundary();
    }
}
