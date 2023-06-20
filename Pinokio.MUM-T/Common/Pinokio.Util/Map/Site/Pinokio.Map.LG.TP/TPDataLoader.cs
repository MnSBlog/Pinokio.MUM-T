using System;

using Pinokio.Core;
using Pinokio.Database;

namespace Pinokio.Map.LG.TP
{
    public class TPDataLoader : LGDataLoader
    {
        protected uint LastEqpId = 0;
        public TPDataLoader(PinokioMap map) : base(map)
        { }

        protected override bool LoadNodeData()
        {
            var tb_base_map_node = MySQLDB.SelectDataTable("map_node", $"map_id = {Map.Id}");
            try
            {
                for (int i = 0; i < tb_base_map_node.Rows.Count; i++)
                {
                    if (tb_base_map_node.Rows[i]["node_id"] == null) break;
                    if (tb_base_map_node.Rows[i]["node_id"].ToString() == null) break;

                    var nodeId = tb_base_map_node.Rows[i]["node_id"].ToString();
                    var x = Convert.ToDouble(tb_base_map_node.Rows[i]["x_coordinate"]);
                    var y = Convert.ToDouble(tb_base_map_node.Rows[i]["y_coordinate"]);
                    var nodeType = Convert.ToInt32(tb_base_map_node.Rows[i]["node_type"]);
                    var typeString = nodeType == 0 ? "Main" : "Site";
                    //var isAngleFixedPosition = Convert.ToInt32(tb_base_map_node.Rows[i]["is_angle_fixed_position"]);
                    //if (isAngleFixedPosition == 1)
                    //type = NODE_TYPE.SITE_INPUT;
                    var isPIO = Convert.ToInt32(tb_base_map_node.Rows[i]["is_pio"]);
                    if (isPIO == 1)
                        typeString = "SiteInput";

                    var node = Map.GenerateMapNode(nodeId, new Geometry.Vector3(x, y, 0), typeString);
                    Map.Graph.AddMapNode(node);
                    if (nodeType == 1) // charger
                    {
                        var chargerId = ++LastChargerId;
                        Map.AddCharger("Charger" + chargerId, nodeId);
                    }
                    else if (nodeType == 2) // equipment
                    {
                        var eqpId = ++LastEqpId;
                        var pos = Map.Graph.Nodes[nodeId].Position;
                        var resource = Map.GenerateResource("Eqp" + eqpId, pos, "Process");
                        Map.SetResourcePorts(resource, nodeId, nodeId);
                        resource.SetCycleTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 1000000 }));
                        resource.SetLoadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 80 }));
                        resource.SetUnloadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 80 }));
                        Map.AddResource(resource);
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
            var tb_map_link = MySQLDB.SelectDataTable("map_node_link", $"map_id = {Map.Id}");
            try
            {
                for (int i = 0; i < tb_map_link.Rows.Count; i++)
                {
                    if (tb_map_link.Rows[i]["link_id"] == null) break;
                    if (tb_map_link.Rows[i]["link_id"].ToString() == null) break;

                    var name = tb_map_link.Rows[i]["link_id"].ToString();
                    var from = tb_map_link.Rows[i]["base_node"].ToString();
                    var to = tb_map_link.Rows[i]["link_node"].ToString();
                    var note = tb_map_link.Rows[i]["orientation"].ToString();

                    var link = Map.GenerateMapLink(name, from, to, "STRAIGHT", "ONE", 0);
                    if (link != null)
                    {
                        link.SetMaxSpeed(700);
                        if (note == "side")
                            link.SetSideWay(true);

                        foreach (var resource in Map.Resources.Values)
                        {
                            if (link.ToNode == resource.InPort || link.FromNode == resource.InPort)
                                link.SetSideWay(true);
                            else if (link.ToNode == resource.OutPort || link.FromNode == resource.OutPort)
                                link.SetSideWay(true);
                        }
                        Map.Graph.AddMapLink(link);
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

                    AoAGV agvSpec = Map.GenerateAGV(agvId) // currentNodeId;

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
                    //var batCharge = Convert.ToDouble(dataTable.Rows[i]["batcharge"]); // ?

                    Map.AddAGV(agvSpec);
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
