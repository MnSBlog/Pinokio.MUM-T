using Pinokio.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Simulation.Disc
{
    public static class DataCollectManager
    {
        public delegate void DataCollectHandler(SimTime simTime, object[] data);

        private static Dictionary<string, bool> _isCollecting = new Dictionary<string, bool>();

        public static Dictionary<string, bool> IsCollecting { get => _isCollecting; set => _isCollecting = value; }

        public static void CollectData(DataCollectHandler callBackMethod, SimTime simTime, object[] data)
        {
            callBackMethod.Invoke(simTime, data);
        }

        public static void ConnectDB(string DB, string user)
        {
            switch(user)
            {
                case "동욱" :
                    DBOption.SetOption("127.0.0.1", DB, "3306", "root", "1111"); 
                    break;
                case "시용":
                    DBOption.SetOption("127.0.0.1", DB, "3306", "root", "2618"); 
                    break;
                case "우성":
                    DBOption.SetOption("127.0.0.1", DB, "3306", "root", "1111");
                    break;
                case "관우":
                    DBOption.SetOption("127.0.0.1", DB, "3307", "root", "!rcn72001739");
                    break;
                default:
                    break;
            }

            MySQLDB.TryConnection();
        }
        public static void AddColletor(string name, bool canCollect)
        {
            _isCollecting.Add(name, canCollect);
        }
    }
}
