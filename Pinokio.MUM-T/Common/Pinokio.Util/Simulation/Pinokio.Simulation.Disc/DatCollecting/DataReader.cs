using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Pinokio.Database;


namespace Pinokio.Simulation.Disc
{
    public static class DataReader
    {
        public static DataTable ReadTBTable()
        {
            return MySQLDB.SelectDataTable("stb");
        }
        public static DataTable ReadTBKPI(string bayName)
        {
            return MySQLDB.SelectDataTable("stb", $"BayName = '{bayName}'");
        }
        public static DataTable ReadTBKPI_Sum(string column)
        {
            List<string> columns = new List<string>();
            columns.Add("SimTime");
            columns.Add($"SUM({column}) AS {column}");
            
            return MySQLDB.SelectGroupDataTable("stb", columns, "", "SimTime");
        }
    }
}