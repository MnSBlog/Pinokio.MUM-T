using System;
using System.Collections.Generic;

using Pinokio.Core;
using Pinokio.Database;

namespace Pinokio.Map.LG.Tennessee
{
    public class TNDataLoader : LGDataLoader
    {
        private TNPathFinder _pathFinder;

        protected uint LastDepotId = 0;
        
        public List<string> DepotPorts;
        public List<string> WaitingPorts;
        public TNDataLoader(PinokioMap map, TNPathFinder pathFinder) : base(map)
        {
            _pathFinder = pathFinder;

            // Depot Id 수동 입력
            this.DepotPorts = new List<string>();
            for (int i = 1003030; i <= 1003220; i += 10)
            {
                this.DepotPorts.Add(i.ToString());
                this.DepotPorts.Add((i + 1003).ToString());
                this.DepotPorts.Add((i + 1005).ToString());
            }

            // 대기장 Id 수동 입력
            //this.WaitingPorts = new List<string> { "1004353", "1004363", "1004373", "1004383", "1004393" };
            this.WaitingPorts = new List<string> { "1004353", "1004393", "1004313" };
        }

        protected override bool LoadNodeData()
        {
            var tb_base_generator_node = MySQLDB.SelectDataTable("map_node", $"map_id = {Map.Id}");
            try
            {
                for (int i = 0; i < tb_base_generator_node.Rows.Count; i++)
                {
                    if (tb_base_generator_node.Rows[i]["node_id"] == null) break;
                    if (tb_base_generator_node.Rows[i]["node_id"].ToString() == null) break;

                    var nodeId = tb_base_generator_node.Rows[i]["node_id"].ToString();
                    var x = Convert.ToDouble(tb_base_generator_node.Rows[i]["x_coordinate"]) / (34 / 10);
                    var y = Convert.ToDouble(tb_base_generator_node.Rows[i]["y_coordinate"]) / (34 / 10);
                    x = System.Math.Floor(x) * 50;
                    y = System.Math.Floor(y) * 50;
                    var nodeType = Convert.ToInt32(tb_base_generator_node.Rows[i]["node_type"]);
                    string typeString = nodeType == 0 ? "Main" : "Site";
                    var isPIO = Convert.ToInt32(tb_base_generator_node.Rows[i]["is_pio"]);
                    if (isPIO == 1)
                        typeString = "SiteInput";

                    var node = Map.GenerateMapNode(nodeId, new Geometry.Vector3(x, y, 0), typeString);
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
            var tb_generator_link = MySQLDB.SelectDataTable("map_node_link", $"map_id = {Map.Id}");
            try
            {
                var linkConfig = new TNLinkConfig(Map.Id);
                for (int i = 0; i < tb_generator_link.Rows.Count; i++)
                {
                    if (tb_generator_link.Rows[i]["link_id"] == null) break;
                    if (tb_generator_link.Rows[i]["link_id"].ToString() == null) break;

                    var id = tb_generator_link.Rows[i]["link_id"].ToString();
                    var from = tb_generator_link.Rows[i]["base_node"].ToString();
                    var to = tb_generator_link.Rows[i]["link_node"].ToString();
                    var speed = tb_generator_link.Rows[i]["nSpeed"].ToString();
                    var loadTypeString = tb_generator_link.Rows[i]["link_type"].ToString();
                    var loadType = (TNLinkType)Enum.Parse(typeof(TNLinkType), loadTypeString, true);
                    var link = Map.GenerateMapLink(id, from, to, "STRAIGHT", "ONE", 0);
                    if (link != null)
                    {
                        link.SetMaxSpeed(700);
                        linkConfig.AddLink(link, loadType);
                    }
                    Map.Graph.AddMapLink(link);
                }

                linkConfig.ConstructMap();
                _pathFinder.AddLinkConfiguration(linkConfig);
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
            var tb_equipment_info = MySQLDB.SelectDataTable("simulation_equipment_info", $"map_id = {Map.Id}");
            try
            {
                var processes = new List<AoResource>();
                if (Map.Id == 3284)
                {
                    var pos1 = Geometry.Vector3.Center(Map.Graph.Nodes["1107134"].Position, Map.Graph.Nodes["1107150"].Position);
                    var resource1 = Map.GenerateResource("P1", pos1, "Process");
                    Map.SetResourcePorts(resource1, "1107134", "1107150");
                    processes.Add(resource1);

                    
                    var pos2 = Geometry.Vector3.Center(Map.Graph.Nodes["1107020"].Position, Map.Graph.Nodes["1107045"].Position);
                    var resource2 = Map.GenerateResource("P2", pos2, "Process");
                    Map.SetResourcePorts(resource2, "1107020", "1107045");
                    processes.Add(resource2);

                    foreach (var p in processes)
                    {
                        p.SetCycleTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 50 }));
                        p.SetLoadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 10 }));
                        p.SetUnloadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 10 }));
                        Map.AddResource(p);
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
            var dataTable = MySQLDB.SelectDataTable("simulation_agv_info");
            try
            {
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
                    agvSpec.SetAGVSize(1130, 640, 280);

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

                }

                if (Map.Id == 3284)
                {
                    foreach (var waitingPortId in WaitingPorts)
                    {
                        var id = ++LastChargerId;
                        Map.AddCharger("Charger" + id, waitingPortId);
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

        protected override bool LoadBufferTable()
        {
            try
            {
                if (Map.Id == 3284)
                {
                    foreach (var deporPortId in DepotPorts)
                    {
                        var id = ++LastDepotId;
                        var pos = Map.Graph.Nodes[deporPortId].Position;
                        var depot = Map.GenerateResource("Depot" + id, pos, "Buffer");
                        Map.SetResourcePorts(depot, deporPortId, deporPortId);

                        depot.SetLoadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 10 }));
                        depot.SetUnloadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 10 }));
                        Map.AddResource(depot);
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

        protected override bool LoadStockerTable()
        {
            return true;
        }
    }
}
