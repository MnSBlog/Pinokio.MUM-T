using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Geometry;

namespace Pinokio.Map
{
    public struct Location
    {
        #region [Static]
        private static double _window = 100;
        public static void SetWindow(double window)
        {
            _window = window;
        }

        public static double Window { get { return _window; } }
        #endregion

        private MapNode _node;
        private MapLink _link;
        private double _offset;

        public MapNode Node{ get => _node; }
        public MapLink Link { get => _link; }
        public double Offset { get => _offset; set => _offset = value; }

        public Location(MapNode node)
        {
            _node = node;
            _link = null;
            _offset = -1.0;
        }

        public Location(MapLink link, double offset)
        {
            _node = null;
            _link = link;
            _offset = offset;
        }

        public Vector3 GetPosition()
        {
            if (_node is null)
            {
                return _link.GetPosition(_offset);
            }
            else
            {
                return _node.Position;
            }
        }

        public Vector3 GetDirection()
        {
            if (_node is null)
            {
                return _link.GetDirection(_offset);
            }
            else
            {
                return new Vector3(1,0,0);
            }
        }

        public override string ToString()
        {
            if (_node is null && _link is null) 
                return "";
            else if (_node is null)
                return _link.Name + "/" + _offset.ToString("F");
            else
                return _node.Name;
        }

        public static bool operator ==(Location location, MapNode node)
        {
            if (location.Node is null)
            {
                if (node is VirtualNode vNode)
                    return (location.Link.Name == vNode.LinkName && Math.Abs(location.Offset - vNode.Offset) <= _window);
                else if (node is null)
                    return location.Link == null;
                else
                    return (location.Link.FromNode == node && location.Offset <= _window) ||
                    (location.Link.ToNode == node && location.Offset >= location.Link.Length - _window);
            }
            else
            {
                return location.Node == node;
            }
        }

        public static bool operator !=(Location location, MapNode node)
        {
            return !(location == node);
        }

        public static bool operator ==(MapNode node, Location location)
        {
            return (location == node);
        }

        public static bool operator !=(MapNode node, Location location)
        {
            return !(location == node);
        }

        public static bool operator ==(Location location1, Location location2)
        {
            if (location1.Node is null)
            {
                if (location2.Node is null)
                 {
                    // Link Vs. Link
                    // Case1) Same Link Case
                    // Abs(offset1 - offset2) <= 100
                    // Case2-1) Other Link Case1
                    // link1.ToNode.OutLinks.Contains(link2) &&
                    // (link1.Length - offset1) + offset2 <= 100
                    // Case2-2) Ohter Link Case2
                    // link1.FromNode.InLinks.Contains(link2) &&
                    // offset1 + (link2.Length - offset2) <= 100
                    // Case3) Else --> False

                    if (location1.Link == location2.Link)
                    {
                        return System.Math.Abs(location1.Offset - location2.Offset) <= _window;
                    }
                    else
                    {
                        if (location1.Link.ToNode.OutLinks.Contains(location2.Link))
                        {
                            return (location1.Link.Length - location1.Offset) + location2.Offset <= _window;
                        }
                        else if (location1.Link.FromNode.InLinks.Contains(location2.Link))
                        {
                            return location1.Offset + (location2.Link.Length - location2.Offset) <= _window;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    // Link Vs. Node
                    return location1 == location2.Node;
                }
            }
            else
            {
                if (location2.Node is null)
                {
                    // Node vs. Link
                    return location1.Node == location2;
                }
                else
                {
                    // Node vs. Node
                    return location1.Node == location2.Node;

                }
            }
        }

        public static bool operator !=(Location location1, Location location2)
        {
            return !(location1 == location2);
        }

    }
}
