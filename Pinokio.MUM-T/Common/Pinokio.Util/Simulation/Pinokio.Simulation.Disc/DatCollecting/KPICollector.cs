using Pinokio.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Pinokio.Simulation.Disc
{
    public static class KPICollector 
    {
        private static bool _isCollectingKPI = false;

        public static bool IsCollectingKPI { get => _isCollectingKPI; set => _isCollectingKPI = value; }

        public static void ConnectKPIDB()
        {
           //DBOption.SetOption("127.0.0.1", "kpi", "3306", "root", "1111"); // 우성
            //DBOption.SetOption("127.0.0.1", "kpi", "3306", "root", "2618"); //시용
            DBOption.SetOption("127.0.0.1", "kpi", "3306", "root", "1111"); //SuperComputer

            MySQLDB.TryConnection();
        }
        public static void RecordFABOutLot(string lotName, SimTime fabInTime, SimTime fabOutTime, SimTime dueDate, SimTime cycleTime)
        {
            if (_isCollectingKPI)
            {
                bool isOnTimeDelivery = true;
                if (dueDate < fabOutTime)
                    isOnTimeDelivery = false;

                var columns = new List<string>() { "LotName", "FAB_IN", "FAB_OUT", "DueDate", "CycleTime", "OnTimeDelivery" };

                var values = new List<object>() { lotName, fabInTime.TotalSeconds, fabOutTime.TotalSeconds, dueDate.TotalSeconds, cycleTime.TotalSeconds, isOnTimeDelivery.ToString() };
                MySQLDB.Insert("lotkpi", columns, values);
            }
        }
        public static void RecordToolgroupKPI(List<string> toolgroupName, List<object> utilization, List<object> availability)
        {
            if (_isCollectingKPI)
            {
                MySQLDB.Insert("toolgrouputilization", toolgroupName, utilization);
                MySQLDB.Insert("toolgroupavailability", toolgroupName, availability);
            }
        }

        public static void RecordWIP(List<string> toolgroupName, List<object> wip)
        {
            if (_isCollectingKPI)
            {
                MySQLDB.Insert("toolgroupwip", toolgroupName, wip);
            }
        }

        public static void RecordTBKPI(double simTime, string bayName, int total, int empty, int full)
        {
            if(_isCollectingKPI)
            {
                var columns = new List<string>() { "SimTime", "BayName", "TotalTBCount", "EmptyTBCount", "FullTBCount" };
                var values = new List<object>() { simTime / 86400, bayName, total, empty, full };
                MySQLDB.Insert("stb", columns, values);
            }
        }

        public static void RecordOHTKPI(List<string> columns,List<object> values) //Utiliztion 테이블 만들기, 컬럼 추가
        {
            if (_isCollectingKPI)
            {
                MySQLDB.Insert("ohtutilization", columns, values);
            }
        }
        public static void RecordOHTJobData(List<object> values)//Delivery Time, Dispatching Time, Tool to Tool
        {
            if (_isCollectingKPI)
            {
                var columns = new List<string>() { "JobID", "OHTID", "FromModel", "ToModel", "GenerateTime", "AssignTime", "LoadFinishTime", "UnloadFinishTime", "TooltoTool" };
                MySQLDB.Insert("ohtjob", columns, values);
            }
        }
        public static DataSet LoadProductionKPIDB()
        {
            var dataset = new DataSet();
            var lotKPI = MySQLDB.SelectDataTable("lotkpi");
            var utilization = MySQLDB.SelectDataTable("toolgrouputilization");
            var availability = MySQLDB.SelectDataTable("toolgroupavailability");
            var wip = MySQLDB.SelectDataTable("toolgroupwip");
            if (lotKPI != null)
            {
                dataset.Tables.Add(lotKPI);
            }
            dataset.Tables.Add(utilization);
            dataset.Tables.Add(availability);
            dataset.Tables.Add(wip);
            return dataset;
        }
    }
}
