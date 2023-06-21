using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;
using Pinokio.Geometry;

namespace Pinokio.Map
{
    public class PinokioMap : AbstractObject
    {
        #region Ids
        protected uint LastNodeSetId;
        protected uint LastResourceId;
        protected uint LastZoneId;
        protected uint LastChargerId;
        protected uint LastOHTId;
        #endregion

        private PinokioGraph _graph;

        #region MapObjects
        private Dictionary<uint, AoAGV> _agvTypes;
        private Dictionary<string, AoOHT> _ohtTypes;
        private Dictionary<uint, AoCharger> _chargers;
        private Dictionary<uint, AoZone> _zones;
        protected Dictionary<string, AoResource> _resources;
        private Dictionary<uint, MapPort> _ports;
        #endregion

        #region NodeSet
        private List<MapNodeSet> _nodeSets;
        private Dictionary<string, MapNodeSet> _belongedSets;
        #endregion NodeSet

        #region ETC
        private Dictionary<MapNode, AoResource> _connResources;
        #endregion

        #region Properties
        public PinokioGraph Graph { get => _graph; }
        public List<MapNodeSet> NodeSets { get => _nodeSets; }
        public Dictionary<uint, AoAGV> AGVTypes { get => _agvTypes; }
        public Dictionary<string, AoOHT> OHTTypes { get => _ohtTypes; }
        public Dictionary<uint, AoCharger> Chargers { get => _chargers; }
        public Dictionary<string, MapNodeSet> BelongedSets { get => _belongedSets; }
        public Dictionary<string, AoResource> Resources { get => _resources; }
        public Dictionary<MapNode, AoResource> ConnectedResources { get => _connResources; }
        public Dictionary<uint, MapPort> Ports { get => _ports; }

        #region [:: Map Setting]
        public bool NeedToFindIntersection { get; set; }
        public bool GenerateNodeSets { get; set; }
        #endregion [Map Setting ::]
        #endregion

        public PinokioMap(uint id, string name = "") : base(id, name)
        {
            _graph = new PinokioGraph(this.Id);
             
            // Point Map Setting
            NeedToFindIntersection = false;
            GenerateNodeSets = false;
        }

        #region [:: Init]
        public override void Initialize()
        {
            base.Initialize();
            LastNodeSetId = 0;
            LastResourceId = 0;
            LastZoneId = 0;
            LastChargerId = 0;
            LastOHTId = 0;

            _zones = new Dictionary<uint, AoZone>();
            _agvTypes = new Dictionary<uint, AoAGV>();
            _ohtTypes = new Dictionary<string, AoOHT>();
            _chargers = new Dictionary<uint, AoCharger>();
            _resources = new Dictionary<string, AoResource>();
            _ports = new Dictionary<uint, MapPort>();

            _nodeSets = new List<MapNodeSet>();
            _belongedSets = new Dictionary<string, MapNodeSet>();

            _connResources = new Dictionary<MapNode, AoResource>();
        }
        #endregion [Init ::]

        #region [:: Get / Set]
        public AoCharger GetCharger(Location location)
        {
            var nodeNames = new List<string>();
            if (location.Node is null)
            {
                if (location == location.Link.FromNode)
                    nodeNames.Add(location.Link.FromNode.Name);

                if (location == location.Link.ToNode)
                    nodeNames.Add(location.Link.ToNode.Name);
            }
            else
            {
                nodeNames.Add(location.Node.Name);
            }

            foreach (string nodeName in nodeNames)
            {
                MapNode node = _graph.Nodes[nodeName];
                if (_connResources.ContainsKey(node) &&
                    _connResources[node] is AoCharger charger)
                {
                    return charger;
                }
            }

            return null;
        }

        public void SetResourcePorts(AoResource resource, string inPortId, string outPortId)
        {
            MapNode inPortNode = null, outPortNode = null;
            if (Graph.Nodes.ContainsKey(inPortId))
            {
                inPortNode = Graph.Nodes[inPortId];
                ConnectResourceToNode(inPortNode, resource);
            }

            if (Graph.Nodes.ContainsKey(outPortId))
            {
                outPortNode = Graph.Nodes[outPortId];
                if (inPortId != outPortId)
                {
                    ConnectResourceToNode(outPortNode, resource);
                }
            }

            resource.SetInOutPort(inPortNode, outPortNode);
        }
        #endregion [Get / Set ::]

        #region [:: Configuration]
        public virtual void Configure()
        {
            this.DeleteUnusingNodes();
            Graph.ConstructNodeSweepingVolume();
            Graph.CheckBidirectionalLink();

            if (NeedToFindIntersection)
            {
                Graph.FindAllIntersections();
            }

            Graph.CheckAliasNodes();
            if (GenerateNodeSets)
            {
                ConstructNodeSets();
            }
        }

        public void ConstructNodeSets()
        {
            this.GenerateAllNodeSets();
            this.ConnectNodeSets();
            this.CheckConnectedNodeSets();
        }
        #endregion [Configuration ::]

        #region [Add Methods]
        #region [Add AMHS Methods]
        public virtual void AddAGV(AoAGV agv)
        {
            if (_agvTypes.ContainsKey(agv.Id))
            {
                throw new Exception($"Already Contained AGV{agv.Id}");
            }

            bool isUpdated = false;
            double width = SweepingVolume.Width;
            double depth = SweepingVolume.Depth;

            // +350의 이유: SensingRangeShort 고려..
            if (agv.Width > SweepingVolume.Width)
            {
                width = agv.Width;
                //width = agv.Width;
                isUpdated = true;
            }

            if (agv.Depth > SweepingVolume.Depth)
            {
                depth = agv.Depth;
                //depth = agv.Depth;
                isUpdated = true;
            }

            if (isUpdated)
                SweepingVolume.SetSize(width, depth);

            _agvTypes.Add(agv.Id, agv);
        }

        public virtual void AddAGV(string name, AoAGV agvType, string initLinkName, double initOffset)
        {
            return;
        }

        public virtual void AddOHT(string name, AoOHT ohtType, string initLinkName, double initOffset)
        {
            return;
        }
        #endregion [Add AMHS Methods Finish]
        public virtual void AddResource(AoResource resource)
        {
            if (Resources.ContainsKey(resource.Name)) return;

            _resources.Add(resource.Name, resource);
        }

        public virtual AoCharger AddCharger(string name, string portId)
        {
            if (Resources.ContainsKey(name)) return null;

            var charger = GenerateCharger(name);
            _chargers.Add(charger.Id, charger);
            Resources.Add(name, charger);

            var node = Graph.Nodes[portId];
            ConnectResourceToNode(node, charger);

            charger.SetInOutPort(node, node);
            charger.SetPosition(node.Position);

            return charger;
        }

        public virtual void AddMapNode(MapNode node)
        {
            this.Graph.AddMapNode(node);
        }

        public virtual void AddMapLink(MapLink link)
        {
            link.SetWeight(link.Length);
            this.Graph.AddMapLink(link);
        }

        public virtual void AddMapPort(MapPort port)
        {
            _ports.Add(port.Id, port);
        }
        #endregion [Add Methods]

        #region [Generate Methods]
        #region [Generate Map Components]
        public virtual MapNode GenerateMapNode(string name, Vector3 pos, string type)
        {
            if (Enum.TryParse(type, true, out MapNodeType nodeType))
            {
                var mapNode = new MapNode(this.Id, this.Graph.LastNodeId++, name, pos, nodeType);
                return mapNode;
            }
            else
            {
                return null;
            }
        }

        public virtual MapNode GenerateMapNode(uint id, string name, Vector3 pos, string type)
        {
            if (Enum.TryParse(type, true, out MapNodeType nodeType))
            {
                var mapNode = new MapNode(this.Id, id, name, pos, nodeType);
                return mapNode;
            }
            else
            {
                return null;
            }
        }

        public virtual MapLink GenerateMapLink(string name, string fromNodeId, string toNodeId, string type, string direction, double radius = 0)  
        {
            if (Enum.TryParse(type, true, out MapLinkType linkType))
            {
                var fromNode = this.Graph.Nodes[fromNodeId];
                var toNode = this.Graph.Nodes[toNodeId];
                var link = new MapLink(this.Id, this.Graph.LastLinkId++, name, linkType, fromNode, toNode);

                DirectionType directionType = DirectionType.Colinear; // For StraightLink
                Enum.TryParse(direction, true, out directionType);
                link.SetGeometry(directionType, radius);

                return link;
            }
            else
            {
                return null;
            }
        }

        public virtual MapPort GenerateMapPort(uint id, string name, string typeString, string linkName, double offset)
        {
            if (typeString == "BOTH")
                typeString = "InOut";
            if (Enum.TryParse(typeString, true, out MapPortType type))
            {
                MapLink link = this.Graph.Links[linkName];
                var port = new MapPort(this.Id, id, name, new Location(link, offset), type);

                return port;
            }
            else
            {
                return null;
            }
        }

        public virtual MapPoint GenerateMapPoint(string pointName, MapLink link, double offset)
        {
            Vector3 pos = link.FromNode.Position;
            if (link.Length > 0)
                pos = link.GetPosition(offset);

            var newPoint = new MapPoint(this.Id, ++this.Graph.LastPointId, pointName, pos, link.Name);
            newPoint.SetPose(link.GetPose(offset));

            var initVolume = SweepingVolume.FindVolumeByPose(newPoint.Pos, newPoint.Pose);
            newPoint.SweepingVolumes.Add(initVolume);

            return newPoint;
        }
        #endregion [Generate Map Components Finish]

        #region [Generate AMHS Components]
        public virtual AoAGV GenerateAGV(uint id)
        {
            return new AoAGV(id);
        }

        public virtual AoOHT GenerateOHT(string name, double size, double maxSpeed, double acceleration, double deceleration, double minDistance, bool useAcceleration = true, double hoistingSpeed = 0)
        {
            if (_ohtTypes.ContainsKey(name))
            {
                return _ohtTypes[name];
            }
            else
            {
                var aOHT = new AoOHT(name)
                {
                    Size = size,
                    MaxSpeed = maxSpeed,
                    Acceleration = acceleration,
                    Decceleration = deceleration,
                    MinimumDistance = minDistance,
                    HoitingSpeed = hoistingSpeed,
                    UseAcceleration = useAcceleration,
                };

                _ohtTypes.Add(name, aOHT);
                return aOHT;
            }
        }
        #endregion [Generate AMHS Components Finish]

        #region [Generate Others Start]
        public virtual AoResource GenerateResource(string name, Vector3 pos, string type)
        {
            if (Enum.TryParse(type, out ResourceType resourceType))
            {
                var resource = new AoResource(this.Id, ++LastResourceId, name, resourceType);
                resource.SetPosition(pos);
                return resource;
            }
            else
            {
                return null;
            }
        }

        public virtual AoCharger GenerateCharger(string name)
        {
            AoCharger charger = new AoCharger(this.Id, ++LastChargerId, name);
            return charger;
        }
        #endregion [Generate Others Finish]

        #endregion [Generate Methods]

        private void DeleteUnusingNodes()
        {
            List<string> ids = _graph.Nodes.Keys.ToList();

            string log = "Delete Unusing Nodes: ";
            for (int i = 0; i < ids.Count; i++)
            {
                MapNode node = _graph.Nodes[ids[i]];
                if (node.InLinks.Count == 0 || node.OutLinks.Count == 0)
                {
                    _graph.RemoveMapNode(ids[i]);
                    log += ids[i] + ",";
                }
                else if (node.InLinks.Count == 1 && node.OutLinks.Count == 1)
                {
                    if (_connResources.ContainsKey(node)) continue;
                    else if (node.InLinks[0].FromNode == node.OutLinks[0].ToNode)
                    {
                        _graph.RemoveMapNode(ids[i]);
                        log += ids[i] + ",";
                    }
                }
            }
            LogHandler.AddLog(LogLevel.Info, log);
        }

        public virtual void ConnectResourceToNode(MapNode node, AoResource resource)
        {
            if (_connResources.ContainsKey(node))
                throw new Exception($"This node({node.Name}) has already connected resource {_connResources[node].Name}");

            _connResources.Add(node, resource);
        }

        public virtual void SetDestinations(AoResource resource, List<string> destIds)
        {
            resource.Destinations.Clear();
            foreach (var destId in destIds)
            {
                if (this.Graph.Nodes.ContainsKey(destId))
                {
                    resource.Destinations.Add(this.Graph.Nodes[destId]);
                }
            }
        }

        #region [NodeSet]
        private void GenerateAllNodeSets()
        {
            string[] nodeIds = _graph.Nodes.Keys.ToArray();
            int count = _graph.Nodes.Count;
            // Node Set 생성..
            //  --> Sweeping Volume안에 포함될 경우, Merge
            //  --> 포함되진 않지만 Sweeping Volume이 겹칠 경우, Neighbor
            List<MapNode> terminalNodes = _graph.Nodes.Values.ToList().Where(n => n.Type is MapNodeType.Site).ToList();
            foreach (MapNode node in terminalNodes)
            {
                MapNodeSet terminalSet = GetNodeSet(node);
                terminalSet.IsTerminal = true;
            }

            for (int i = 0; i < count - 1; i++)
            {
                MapNode node1 = _graph.Nodes[nodeIds[i]];
                MapNodeSet set1 = GetNodeSet(node1);
                for (int j = i + 1; j < count; j++) 
                {
                    MapNode node2 = _graph.Nodes[nodeIds[j]];
                    MapNodeSet set2 = GetNodeSet(node2);
                    if (set1 == set2) continue; // Same Set
                    else if (!set1.IsTerminal && node1.IsVeryClose(node2))
                    {
                        this.MergeNodeSets(set1, set2);
                    }
                    else if (node1.IsNeighbor(node2))
                    {
                        set1.AddNeighbor(set2);
                    }
                }
            }
        }

        private void ConnectNodeSets()
        {
            // Search Prev & Next Sets
            foreach (var set in _nodeSets)
            {
                foreach (var node in set.Nodes)
                {
                    if ((MapNodeType)node.Type == MapNodeType.Virtual)
                    {
                        continue;
                    }

                    foreach (var inLink in node.InLinks)
                    {
                        var otherSet = _belongedSets[inLink.FromNode.Name];
                        if (set != otherSet && !set.PrevSets.Contains(otherSet))
                        {
                            set.PrevSets.Add(otherSet);
                        }
                    }

                    foreach (var outLink in node.OutLinks)
                    {
                        var otherSet = _belongedSets[outLink.ToNode.Name];
                        if (set != otherSet && !set.NextSets.Contains(otherSet))
                        {
                            set.NextSets.Add(otherSet);
                        }
                    }
                }
            }

            // It is need to reconnect because virtaulNodes..
            var dividedEdges = _graph.Links.Values.ToList().FindAll(e => e.VirtalNodes.Count > 0);
            foreach (var edge in dividedEdges)
            {
                var fromSet = _belongedSets[edge.FromNode.Name];
                var toSet = _belongedSets[edge.ToNode.Name];
                if (fromSet != toSet)
                {
                    // Edge의 FromSet과 ToSet이 다르다면, 연결관계를 형성한다.
                    var firstVirtualVertex = edge.VirtalNodes.First();
                    var firstSet = _belongedSets[firstVirtualVertex.Name];// Link 위의 첫 번째 VirtualNode가 속한 NodeSet

                    if (firstSet != fromSet && // firstSet이 FromSet이 아니면서
                        firstSet != toSet ) // firstSet이 ToSet과 다를 때, 연결관계를 변경해야한다.
                    {
                        // AsIs: fromSet --> toSet
                        // ToBe: fromSet --> firstSet
                        fromSet.NextSets.Remove(toSet);
                        fromSet.NextSets.Add(firstSet);
                        firstSet.PrevSets.Add(fromSet);
                    }

                    var lastVirtualVertex = edge.VirtalNodes.Last();
                    var lastSet = _belongedSets[lastVirtualVertex.Name];

                    if (lastSet != toSet && // lastSet이 ToSet이 아니면서
                        lastSet != fromSet) // lastSet이 FromSet과 다를 때, 연결관계를 변경해야한다.
                    {
                        // AsIs: fromSet --> toSet
                        // ToBe: lastSet --> toSet;
                        toSet.PrevSets.Remove(fromSet);
                        toSet.PrevSets.Add(lastSet);
                        lastSet.NextSets.Add(toSet);
                    }

                    for (int i = 1; i < edge.VirtalNodes.Count - 1; i++)
                    {
                        var prevVirtualSet = _belongedSets[edge.VirtalNodes[i - 1].Name];
                        var nextVirtualSet = _belongedSets[edge.VirtalNodes[i].Name];

                        prevVirtualSet.NextSets.Remove(toSet);
                        prevVirtualSet.NextSets.Add(nextVirtualSet);

                        nextVirtualSet.PrevSets.Remove(fromSet);
                        nextVirtualSet.PrevSets.Add(nextVirtualSet);
                    }
                }
                else
                {
                    bool containsAll = true;
                    foreach (var vNode in edge.VirtalNodes)
                    {
                        if (!fromSet.Nodes.Contains(vNode))
                        {
                            containsAll = false;
                        }
                    }

                    if(!containsAll)
                        throw new Exception("!!");
                }
            }
        }

        /// <summary>
        /// Connected Nodeset: Nodeset안의 모든 Node들이 연결되어있음
        /// NotConnected Nodeset: Nodeset안의 모든 Node들이 연결되지 않은 경우가 있음
        /// </summary>
        private void CheckConnectedNodeSets()
        {
            foreach (MapNodeSet set in _nodeSets)
            {
                if (set.Nodes.ToList().Any(n => n is VirtualNode))
                {
                    set.IsConnected = false;
                    continue;
                }

                bool notConnected = false;
                foreach (MapNode node in set.Nodes)
                {
                    var connectedNodes = new List<MapNode>() {  node};
                    foreach (var inLink in node.InLinks)
                    {
                        SearchConnectivity(inLink.FromNode, set, true, ref connectedNodes);
                    }

                    foreach (var outLink in node.OutLinks)
                    {
                        SearchConnectivity(outLink.FromNode, set, true, ref connectedNodes);
                    }

                    if (connectedNodes.Count < set.Nodes.Count - 1)
                        notConnected = true; break;
                }

                if (notConnected)
                {
                    set.IsConnected = false;
                    continue;
                }
            }
        }

        /// <summary>
        /// NodeSet의 Connectivity를 확인한다.
        /// </summary>
        /// <param name="nextNode"></param>
        /// <param name="thisSet"></param>
        /// <param name="inLinkSide"></param>
        /// <param name="connectedNodes"></param>
        private void SearchConnectivity(MapNode nextNode, MapNodeSet thisSet, bool inLinkSide, ref List<MapNode> connectedNodes)
        {
            if (connectedNodes.Contains(nextNode)) return;
            else if (!thisSet.Nodes.Contains(nextNode)) return;

            connectedNodes.Add(nextNode);
            if (inLinkSide)
            {
                foreach (MapLink inLink in nextNode.InLinks)
                {
                    SearchConnectivity(inLink.FromNode, thisSet, true, ref connectedNodes);
                }
            }
            else // OutlinkSide
            {
                foreach (MapLink outLink in nextNode.OutLinks)
                {
                    SearchConnectivity(outLink.ToNode, thisSet, false, ref connectedNodes);
                }
            }
        }

        private MapNodeSet GetNodeSet(MapNode node)
        {
            if (!_belongedSets.ContainsKey(node.Name))
            {
                var set = new MapNodeSet(++LastNodeSetId, node);
                _belongedSets.Add(node.Name, set);
                _nodeSets.Add(set);

                if (_connResources.ContainsKey(node)) set.IsTerminal = true;
            }
            return _belongedSets[node.Name];
        }

        private void MergeNodeSets(MapNodeSet set1, MapNodeSet set2)
        {
            foreach (var vertex in set2.Nodes)
            {
                set1.Nodes.Add(vertex);
                _belongedSets[vertex.Name] = set1;
            }

            foreach (var neighborSet in set2.Neighbors.ToList())
            {
                neighborSet.Neighbors.Remove(set2);
                if (neighborSet != set1)
                {
                    set1.AddNeighbor(neighborSet);
                }
            }

            if (set2.IsTerminal && !set1.IsTerminal)
                set1.IsTerminal = true;

            _nodeSets.Remove(set2);
        }
        #endregion [NodeSet ::]

        public virtual void FinishToLoadNode()
        {
            
        }
    }
}
