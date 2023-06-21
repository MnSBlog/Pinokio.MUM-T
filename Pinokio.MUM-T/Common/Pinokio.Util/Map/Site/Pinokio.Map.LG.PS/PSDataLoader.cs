using System;
using System.Collections.Generic;

using Pinokio.Core;
using Pinokio.Database;
using ClosedXML.Excel;

namespace Pinokio.Map.LG.PS
{
    public class PSDataLoader : MapLoader
    {
        private List<string> _nodeNames;
        private List<string> _siteNodeNames;
        protected uint LastEQPId = 0;
        protected uint LastChargerId = 0;
        protected uint LastAGVId = 0;
        private XLWorkbook _workbook;
        public PSDataLoader(PinokioMap map, string path) : base(map)
        {
            _nodeNames = new List<string>();
            _siteNodeNames = new List<string>();
            _workbook = new XLWorkbook(path);
        }

        protected override bool LoadNodeData()
        {
            var worksheet = _workbook.Worksheet("3F_NODE");
            try
            {
                for (int i = 2; i <= worksheet.LastRowUsed().RowNumber(); i++)
                {
                    if (worksheet.Cell(i, 1).Value is null) break;
                    if (worksheet.Cell(i, 1).Value.ToString() == "") break;
                    
                    string name = worksheet.Cell(i, 1).Value.ToString();
                    if (_nodeNames.Contains(name)) continue; // Data 불완전 예외
                    
                    double x = Convert.ToDouble(worksheet.Cell(i, 3).Value);
                    double y = Convert.ToDouble(worksheet.Cell(i, 4).Value);
                    int type = Convert.ToInt32(worksheet.Cell(i, 5).Value);

                    string typeString = "";
                    switch (type)
                    {
                        case 1:
                            typeString = "Main";
                            break;
                        case 2:
                            typeString = "SiteInput";
                            break;
                        case 3:
                            typeString = "Site";
                            _siteNodeNames.Add(name);
                            break;
                    }

                    var node = Map.GenerateMapNode(name, new Geometry.Vector3(x, y, 0), typeString);
                    Map.Graph.AddMapNode(node);
                    _nodeNames.Add(name);

                    if (typeString == "Site")
                    {
                        node.SetNoRotation(true);
                    }

                    if (name.Contains("CH")) // charger
                    {
                        var chargerId = ++LastChargerId;
                        Map.AddCharger("Charger" + chargerId, name);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Error, e.ToString());
                return false;
            }
        }

        protected override bool LoadLinkData()
        {
            var worksheet = _workbook.Worksheet("3F_LINK");
            try
            {
                for (int i = 2; i <= worksheet.LastRowUsed().RowNumber(); i++)
                {
                    if (worksheet.Cell(i, 1).Value is null) break;
                    if (worksheet.Cell(i, 1).Value.ToString() == "") break;

                    var links = new List<MapLink>();
                    var reverseLinks = new Dictionary<MapLink, MapLink>(); // Key: ReverseLink & Value: OriginLink

                    string id = worksheet.Cell(i, 1).Value.ToString();
                    string node1 = worksheet.Cell(i, 3).Value.ToString();
                    string node2 = worksheet.Cell(i, 4).Value.ToString();
                    if (!_nodeNames.Contains(node1) || !_nodeNames.Contains(node2)) continue;

                    int dirType = Convert.ToInt32(worksheet.Cell(i, 5).Value);

                    //if (dirType == 1 || dirType == 0) // Node1 --> Node2로 가는 경로 존재
                    if (dirType == 1) // Node1 --> Node2로 가는 경로 존재
                    {
                        MapLink forwardLink = Map.GenerateMapLink(id, node1, node2, "STRAIGHT", "ONE", 0);
                        MapLink forwardReverseLink = Map.GenerateMapLink(id + "_rev", node2, node1, "STRAIGHT", "ONE", 0);

                        links.Add(forwardLink);
                        reverseLinks.Add(forwardReverseLink, forwardLink);
                    }

                    //if (dirType == 2 || dirType == 0) // Node2 --> Node1로가는 경로 존재
                    if (dirType == 2) // Node2 --> Node1로가는 경로 존재
                    {
                        MapLink backwardLink = Map.GenerateMapLink(id + "_b", node2, node1, "STRAIGHT", "ONE", 0);
                        MapLink backwardReverseLink = Map.GenerateMapLink(id + "_b_rev", node1, node2, "STRAIGHT", "ONE", 0);

                        links.Add(backwardLink);
                        reverseLinks.Add(backwardReverseLink, backwardLink);
                    }

                    if (dirType == 0)
                    {
                        MapLink forwardLink = Map.GenerateMapLink(id, node1, node2, "STRAIGHT", "ONE", 0);
                        links.Add(forwardLink);

                        MapLink backwardLink = Map.GenerateMapLink(id + "_b", node2, node1, "STRAIGHT", "ONE", 0);
                        links.Add(backwardLink);
                    }

                    foreach (MapLink link in links)
                    {
                        link.SetMaxSpeed(700);
                        if ((link.FromNode.Type is MapNodeType.Site && link.ToNode.Type is MapNodeType.SiteInput) ||
                            (link.ToNode.Type is MapNodeType.Site && link.FromNode.Type is MapNodeType.SiteInput))
                            link.SetSideWay(true);

                        Map.Graph.AddMapLink(link);
                    }

                    //foreach (MapLink reverseLink in reverseLinks.Keys)
                    //{
                    //    reverseLink.SetMaxSpeed(100);
                    //    if ((reverseLink.FromNode.Type is MapNodeType.Site && reverseLink.ToNode.Type is MapNodeType.SiteInput) ||
                    //        (reverseLink.ToNode.Type is MapNodeType.Site && reverseLink.FromNode.Type is MapNodeType.SiteInput))
                    //        reverseLink.SetSideWay(true);

                    //    if (reverseLinks[reverseLink].Type is MapLinkType.Straight)
                    //    {
                    //        reverseLinks[reverseLink].SetDesignatedPose(reverseLinks[reverseLink].GetPose());
                    //    }

                    //    Map.Graph.AddMapLink(reverseLink);
                    //}
                }

                return true;
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Error, e.ToString());
                return false;
            }
        }

        protected override bool LoadProcessData()
        {
            try
            {
                foreach (var nodeName in _siteNodeNames)
                {
                    if (nodeName.Contains("CH")) continue;
                    var eqpName = "EQP" + ++LastEQPId;
                    var pos = Map.Graph.Nodes[nodeName].Position;
                    var resource = Map.GenerateResource(eqpName, pos, "Process");
                    Map.SetResourcePorts(resource, nodeName, nodeName);
                    resource.SetLoadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 15 }));
                    resource.SetUnloadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 15 }));
                    Map.AddResource(resource);
                }

                return true;
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Error, e.ToString());
                return false;
            }
        }

        protected override bool LoadAGVData()
        {
            try
            {
                var worksheet = _workbook.Worksheet("AGV");
                for (int i = 2; i <= worksheet.LastRowUsed().RowNumber(); i++)
                {
                    if (worksheet.Cell(i, 1).Value is null) break;
                    if (worksheet.Cell(i, 1).Value.ToString() == "") break;

                    string name = worksheet.Cell(i, 1).Value.ToString();
                    uint id = Convert.ToUInt32(name.Split('R')[1]);
                    string currentNodeId = worksheet.Cell(i, 3).Value.ToString();

                    AoAGV agvSpec = Map.GenerateAGV(++LastAGVId); // currentNodeId
                    agvSpec.SetAGVSize(1100, 900, 1);
                    agvSpec.SetAGVSpeed(0.6, 1, 0.3, 1);
                    agvSpec.SetSensingRange(1500, 700, 300);
                    agvSpec.SetTurnTime(4, 4);

                    Map.AddAGV(agvSpec);

                    MySQLDB.InsertIfNotExist("simulation_agv_info",
                        new List<string>() { "map_id", "agv_id", "cur_node" },
                        new List<object>() { 1, agvSpec.Id, currentNodeId },
                        $"agv_id = '{agvSpec.Id}'");
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
