using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Geometry;

namespace Pinokio.Simulation.Cont
{
    public static class CollisionableExtensions
    {
        public static bool IsCollideWith(this ICollisionable thisCol, ICollisionable otherCol)
        {
            if (thisCol.GeometryObj is BoundingBox thisBB)
            {
                if (otherCol.GeometryObj is BoundingBox otherBB)
                {
                    return thisBB.Intersects(otherBB);
                }
            }
            else if (thisCol.GeometryObj is Polygon thisP)
            {
                if (otherCol.GeometryObj is Polygon otherP)
                {
                    return thisP.IsIntersect(otherP);
                }
            }

            return false;   
        }

        
    }
}
