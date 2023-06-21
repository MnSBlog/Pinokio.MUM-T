using System;
using System.Linq;
using System.Collections.Generic;

using Pinokio.Core;
using Pinokio.Database;

namespace Pinokio.Map.LG.Poland
{
    public class PolandDataLoader : MapLoader
    {
        private PNPathFinder _pathFinder;
        public PolandDataLoader(PinokioMap map, PNPathFinder pathFinder) : base(map)
        {
            _pathFinder = pathFinder;
        }

        protected override void Load()
        {
            base.Load();

            if (!LoadCommitInformation()) throw new Exception("Fail To Load Commit Information");

            if (!LoadCompleteInformation()) throw new Exception("Fail To Load Complete Information");

            if (!LoadChargerInformation()) throw new Exception("Fail To Load Charger Information");

            var config = new PNConfig(Map.Id, Map.ConnectedResources.Keys.ToList());
            _pathFinder.AddConfig(config);
        }

        protected override bool LoadNodeData()
        {
            var tb_base_map_node = MySQLDB.SelectDataTable("tb_base_map_node");
            try
            {
                for (int i = 0; i < tb_base_map_node.Rows.Count; i++)
                {
                    if (tb_base_map_node.Rows[i]["id"] == null) break;
                    if (tb_base_map_node.Rows[i]["id"].ToString() == null) break;

                    var id = tb_base_map_node.Rows[i]["nodeName"].ToString();
                    var x = (double)tb_base_map_node.Rows[i]["x"];
                    var y = (double)tb_base_map_node.Rows[i]["y"];
                    var z = (double)tb_base_map_node.Rows[i]["z"];
                    var typeString = tb_base_map_node.Rows[i]["nodeType"].ToString();
                    if (typeString == "SITE_INPUT")
                        typeString = "SiteInput";
                    else if (typeString == "SITE_OUTPUT")
                        typeString = "SiteOutput";
                    var node = Map.GenerateMapNode(id, new Geometry.Vector3(x, y, z), typeString);
                    Map.Graph.AddMapNode(node);
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
            var tb_base_map_link = MySQLDB.SelectDataTable("tb_base_map_link");
            string lastFrom = "";
            string lastTo = "";
            try
            {
                for (int i = 0; i < tb_base_map_link.Rows.Count; i++)
                {
                    if (tb_base_map_link.Rows[i]["id"] == null) break;
                    if (tb_base_map_link.Rows[i]["id"].ToString() == null) break;

                    var from = tb_base_map_link.Rows[i]["fromNode"].ToString();
                    var to = tb_base_map_link.Rows[i]["toNode"].ToString();
                    var linkType = tb_base_map_link.Rows[i]["linkType"].ToString();
                    var direction = tb_base_map_link.Rows[i]["direction"].ToString();
                    var radius = (double)tb_base_map_link.Rows[i]["radius"];
                    var maxSpeed = (double)tb_base_map_link.Rows[i]["maxSpeed"];
                    //var orientation = tb_base_map_link.Rows[i]["orientation"].ToString();
                    //bool isSideLink = orientation == "sidelink" ? true : false;

                    lastFrom = from;
                    lastTo = to;
                    var poseX = tb_base_map_link.Rows[i]["pose_x"].ToString() == "" ? 0 : (int)tb_base_map_link.Rows[i]["pose_x"];
                    var poseY = tb_base_map_link.Rows[i]["pose_y"].ToString() == "" ? 0 : (int)tb_base_map_link.Rows[i]["pose_y"];
                    var link1 = Map.GenerateMapLink("link" + from + "_" + to, from, to, linkType, direction, radius);
                    if (link1 != null)
                    {
                        link1.SetMaxSpeed(maxSpeed);
                        link1.SetDesignatedPose(new Geometry.Vector2(poseX, poseY));
                        Map.Graph.AddMapLink(link1);
                        if (direction == "BI")
                        {
                            var link2 = Map.GenerateMapLink("link" + to + "_" + from, to, from, linkType, direction, radius);
                            if (link2 != null)
                            {
                                link2.SetMaxSpeed(maxSpeed);
                                link2.SetDesignatedPose(new Geometry.Vector2(poseX, poseY));
                                Map.Graph.AddMapLink(link2);
                            }
                        }
                    }

                    

                }

                return true;
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Error, e.ToString());
            }
            return false;
        }

        protected override bool LoadProcessData()
        {
            var tb_base_process = MySQLDB.SelectDataTable("tb_base_process");
            try
            {
                for (int i = 0; i < tb_base_process.Rows.Count; i++)
                {
                    if (tb_base_process.Rows[i]["id"] == null) break;
                    if (tb_base_process.Rows[i]["id"].ToString() == null) break;

                    var name = tb_base_process.Rows[i]["modelName"].ToString();
                    var portType = tb_base_process.Rows[i]["portType"].ToString();
                    var inPort = tb_base_process.Rows[i]["inPorts"].ToString();
                    var outPort = tb_base_process.Rows[i]["outPorts"].ToString();
                    var distString = tb_base_process.Rows[i]["distributionType"].ToString();
                    var distTypeString = distString.Split('(')[0];
                    Distribution cycleTime = null;
                    if (Enum.TryParse(distTypeString, out DistributionType distType))
                    {
                        var distParamStrings = distString.Split('(')[1].Split(')')[0].Split('/');
                        var tempParams = new List<double>();
                        foreach (var paramString in distParamStrings)
                        {
                            tempParams.Add(Convert.ToDouble(paramString));
                        }
                        cycleTime = Statistics.GetDistribution(distType, tempParams.ToArray());
                    }
                    else
                    {
                        cycleTime = Statistics.GetDistribution(DistributionType.Const, new double[] { 2000 });
                    }

                    var destString = tb_base_process.Rows[i]["destination"].ToString();
                    List<string> destIds = destString.Split('/').ToList();

                    var pos = Geometry.Vector3.Center(Map.Graph.Nodes[inPort].Position, Map.Graph.Nodes[outPort].Position);
                    var resource = Map.GenerateResource(name, pos, "Process");
                    Map.SetResourcePorts(resource, inPort, outPort);
                    resource.SetCycleTime(cycleTime);
                    resource.SetLoadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 10 }));
                    resource.SetUnloadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 10 }));
                    Map.SetDestinations(resource, destIds);
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
                var dataTable = MySQLDB.SelectDataTable("simulation_agv_info");
                if (dataTable == null) return false;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i]["agv_id"] == null) break;
                    if (dataTable.Rows[i]["agv_id"].ToString() == null) break;

                    var agvId = Convert.ToUInt32(dataTable.Rows[i]["agv_id"].ToString());
                    var currentNodeId = dataTable.Rows[i]["cur_node"].ToString();

                    AoAGV agvSpec = Map.GenerateAGV(agvId); // currentNodeId

                    // AGV Size
                    var dbAGVSizeX = dataTable.Rows[i]["agvsizex"];
                    var dbAGVSizeY = dataTable.Rows[i]["agvsizey"];
                    double width = dbAGVSizeX is DBNull ? 0 : Convert.ToDouble(dbAGVSizeX) * 1000;
                    double depth = dbAGVSizeY is DBNull ? 0 : Convert.ToDouble(dbAGVSizeY) * 1000;
                    double height = 1;
                    agvSpec.SetAGVSize(width, depth, height);

                    // Speed
                    var dbSpeed = dataTable.Rows[i]["agvspeed"];
                    var dbAcc = dataTable.Rows[i]["agvacceleration"];
                    var dbSideSpeed = dataTable.Rows[i]["agvsidespeed"];
                    var dbSideAcc = dataTable.Rows[i]["agvsideacceleration"];
                    double speed = dbSpeed is DBNull ? 0 : Convert.ToDouble(dbSpeed);
                    double acceleration = dbSpeed is DBNull ? 0 : Convert.ToDouble(dbAcc);
                    double sideSpeed = dbSpeed is DBNull ? 0 : Convert.ToDouble(dbSideSpeed);
                    double sideAcceleration = dbSpeed is DBNull ? 0 : Convert.ToDouble(dbSideAcc);
                    agvSpec.SetAGVSpeed(speed, acceleration, sideSpeed, sideAcceleration);

                    // Turn
                    var dbMainTT = dataTable.Rows[i]["mainturntime"];
                    var dbSubTT = dataTable.Rows[i]["subturntime"];
                    int mainTurnTime = dbMainTT is DBNull ? 0 : Convert.ToInt32(dbMainTT);
                    int subTurnTime = dbSubTT is DBNull ? 0 : Convert.ToInt32(dbSubTT);
                    agvSpec.SetTurnTime(mainTurnTime, subTurnTime);

                    // MTTR & MTBF
                    var dbMTTR = dataTable.Rows[i]["mttr"];
                    var dbMTBF = dataTable.Rows[i]["mtbf"];
                    double mttr = dbMTTR is DBNull ? 0 : Convert.ToDouble(dbMTTR);
                    double mtbf = dbMTBF is DBNull ? 0 : Convert.ToDouble(dbMTBF);
                    agvSpec.SetFailureConstant(mttr, mtbf);

                    // Battery
                    var dbBatLevel = dataTable.Rows[i]["batlevel"];
                    var dbBatCapa = dataTable.Rows[i]["batcapacity"];
                    var dbBusyCon = dataTable.Rows[i]["batconsumption"];
                    var dbIdleCon = dataTable.Rows[i]["batidleconsumption"];
                    double batteryLevel = dbBatLevel is DBNull ? 0 : Convert.ToDouble(dbBatLevel);
                    double batteryCapacity = dbBatCapa is DBNull ? 0 : Convert.ToDouble(dbBatCapa);
                    double busyConsumptionRate = dbBusyCon is DBNull ? 0 : Convert.ToDouble(dbBusyCon);
                    double idleConsumptionRate = dbIdleCon is DBNull ? 0 : Convert.ToDouble(dbIdleCon);
                    agvSpec.SetBatteryOption(batteryLevel, batteryCapacity, busyConsumptionRate, idleConsumptionRate);

                    Map.AddAGV(agvSpec);
                    //var batCharge = Convert.ToDouble(dataTable.Rows[i]["batcharge"]); // ?
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool LoadCommitInformation()
        {
            var tb_base_plan = MySQLDB.SelectDataTable("tb_base_plan");
            try
            {
                for (int i = 0; i < tb_base_plan.Rows.Count; i++)
                {
                    if (tb_base_plan.Rows[i]["id"] == null) break;
                    if (tb_base_plan.Rows[i]["id"].ToString() == null) break;

                    var name = tb_base_plan.Rows[i]["modelName"].ToString();
                    var mapNodeId = tb_base_plan.Rows[i]["mapNodeId"].ToString();
                    var destString = tb_base_plan.Rows[i]["destination"].ToString();
                    List<string> destIds = destString.Split('/').ToList();

                    var pos = Map.Graph.Nodes[mapNodeId].Position;
                    var resource = Map.GenerateResource(name, pos, "Commit");
                    Map.SetResourcePorts(resource, "", mapNodeId);
                    resource.SetLoadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 10 }));
                    resource.SetTactTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 200 }));
                    Map.SetDestinations(resource, destIds);
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

        private bool LoadCompleteInformation()
        {
            var tb_base_complete = MySQLDB.SelectDataTable("tb_base_complete");
            try
            {
                for (int i = 0; i < tb_base_complete.Rows.Count; i++)
                {
                    if (tb_base_complete.Rows[i]["id"] == null) break;
                    if (tb_base_complete.Rows[i]["id"].ToString() == null) break;

                    var name = tb_base_complete.Rows[i]["modelName"].ToString();
                    var mapNodeId = tb_base_complete.Rows[i]["mapNodeId"].ToString();

                    var pos = Map.Graph.Nodes[mapNodeId].Position;
                    var resource = Map.GenerateResource(name, pos, "Complete");
                    Map.SetResourcePorts(resource, mapNodeId, "");
                    resource.SetUnloadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 10 }));
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

        private bool LoadChargerInformation()
        {
            var tb_base_mhe_sub = MySQLDB.SelectDataTable("tb_base_mhe_sub");
            try
            {
                // MHE Sub
                for (int i = 0; i < tb_base_mhe_sub.Rows.Count; i++)
                {
                    if (tb_base_mhe_sub.Rows[i]["id"] == null) break;
                    if (tb_base_mhe_sub.Rows[i]["id"].ToString() == null) break;

                    var id = uint.Parse(tb_base_mhe_sub.Rows[i]["id"].ToString());
                    var name = tb_base_mhe_sub.Rows[i]["modelName"].ToString();
                    var mapNodeId = tb_base_mhe_sub.Rows[i]["mapNodeId"].ToString();
                    var chargingSpeed = int.Parse(tb_base_mhe_sub.Rows[i]["chargingSpeed"].ToString());

                    Map.AddCharger(name, mapNodeId);
                }
                return true;
            }
            catch (Exception e)
            {
                LogHandler.AddLog(LogLevel.Error, e.ToString());
                return false;
            }
        }
    }
}
