using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Geometry;

namespace Pinokio.Core
{
    public enum CollisionBoundType
    {
        
    }

    public interface ICollisionable
    {
        object GeometryObj { get; }

        void UpdateCollisionBoundary();
    }

    public static class CollisionableExtensions
    {
        public static bool IsCollidedWith(this ICollisionable thisCol, ICollisionable otherCol)
        {
            if (thisCol.GeometryObj is null) return false;
            else if (otherCol.GeometryObj is null) return false;
            else if (thisCol.GeometryObj is Polygon thisPolygon)
            {
                if (otherCol.GeometryObj is Polygon ohterPolygon)
                {
                    return thisPolygon.IsIntersect(ohterPolygon);
                }
            }
            else if (thisCol.GeometryObj is BoundingBox thisBB)
            {
                if (otherCol.GeometryObj is BoundingBox otherBB)
                {
                    return thisBB.Intersects(otherBB);
                }
            }

            return false;
        }
    }
}
