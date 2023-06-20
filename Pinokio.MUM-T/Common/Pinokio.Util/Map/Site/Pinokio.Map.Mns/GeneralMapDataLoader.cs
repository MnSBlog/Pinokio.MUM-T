using System;

using Pinokio.Core;
using Pinokio.Database;

namespace Pinokio.Map.Mns
{
    public class GeneralMapDataLoader : MapLoader
    {
        public int AGVCount = 100;
        public GeneralMapDataLoader(PinokioMap map) : base(map)
        { }

        protected override bool LoadNodeData()
        {
            var dataTable = MySQLDB.SelectDataTable("map_node");
            try
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    var id = dataTable.Rows[i]["id"].ToString();
                    var x = Convert.ToDouble(dataTable.Rows[i]["x"]);
                    var y = Convert.ToDouble(dataTable.Rows[i]["y"]);
                    string nodeType = "Main";
                    var node = Map.GenerateMapNode(id, new Geometry.Vector3(x, y, 0), nodeType);
                    Map.Graph.AddMapNode(node);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override bool LoadLinkData()
        {
            var dataTable = MySQLDB.SelectDataTable("map_link");
            try
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    var name = dataTable.Rows[i]["name"].ToString();
                    var from = dataTable.Rows[i]["from_node"].ToString();
                    var to = dataTable.Rows[i]["to_node"].ToString();
                    var linkType = dataTable.Rows[i]["link_type"].ToString();
                    var direction = dataTable.Rows[i]["direction"].ToString();
                    var radius = Convert.ToDouble(dataTable.Rows[i]["radius"]);
                    var maxSpeed = Convert.ToDouble(dataTable.Rows[i]["max_speed"]);

                    var link1 = Map.GenerateMapLink(name, from, to, linkType, direction, radius);
                    if (link1 != null)
                    {
                        link1.SetMaxSpeed(maxSpeed);
                        Map.Graph.AddMapLink(link1);
                        if (direction == "BI")
                        {
                            var link2 = Map.GenerateMapLink("link" + to + "_" + from, to, from, linkType, direction, radius);
                            link2.SetMaxSpeed(maxSpeed);
                            Map.Graph.AddMapLink(link2);
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override bool LoadProcessData()
        {
            try
            {
                var dataTable = MySQLDB.SelectDataTable("process");
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    var type = dataTable.Rows[i]["process_type"].ToString();
                    var id = dataTable.Rows[i]["id"].ToString();
                    var name = type + id;
                    var inport = dataTable.Rows[i]["inport"].ToString();
                    var outport = dataTable.Rows[i]["outport"].ToString();
                    
                    if (type == "CHARGER")
                    {
                        Map.AddCharger(name, inport);
                    }
                    else
                    {
                        var pos = Geometry.Vector3.Center(Map.Graph.Nodes[inport].Position, Map.Graph.Nodes[outport].Position);
                        var resource = Map.GenerateResource(name, pos, type);
                        Map.SetResourcePorts(resource, inport, outport);
                        resource.SetLoadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 10 }));
                        resource.SetUnloadingTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 10 }));
                        switch (type)
                        {
                            case "PLAN":
                                resource.SetTactTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 200 }));
                                break;
                            case "PROCESS":
                                resource.SetCycleTime(Statistics.GetDistribution(DistributionType.Const, new double[] { 50 }));
                                break;
                        }
                        Map.AddResource(resource);
                    }
                }
                return true;
            }
            catch
            {
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

                    //var batCharge = Convert.ToDouble(dataTable.Rows[i]["batcharge"]); // ?
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
