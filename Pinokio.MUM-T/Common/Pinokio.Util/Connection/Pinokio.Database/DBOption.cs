using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Pinokio.Database
{
    public static class DBOption
    {
        private static string _address;
        private static string _port = "3306";
        private static string _userId;
        private static string _password;
        private static string _dbName = "";
        private static Dictionary<string, DBTable> _tables = new Dictionary<string, DBTable>();

        public static string Address { get { return _address; } }
        public static string Port { get { return _port; } }
        public static string UserId { get { return _userId; } }
        public static string Password { get { return _password; } }
        public static string DBName { get { return _dbName; } }
        public static Dictionary<string, DBTable> Tables { get { return _tables; } }

        public static void SetOption(string address, string dbName, string port, string userId, string password)
        {
            _address = address;
            _dbName = dbName;
            _port = port;
            _userId = userId;
            _password = password;
        }

        public static string ConnectionString()
        {
            return "Server=" + _address +
                   ";Port=" + _port +
                   ";Database=" + _dbName +
                   ";Uid=" + _userId +
                   ";Pwd=" + _password +
                   ";SslMode=None;";
        }

        public static void SetTableInfos(List<DBTable> tables)
        {
            _tables = tables.ToDictionary(x => x.TableName, x => x);
        }

        public static void CheckTableConfiguration()
        {
            var tables = MySQLDB.SelectMultiValues("INFORMATION_SCHEMA.TABLES", "TABLE_NAME", $"TABLE_SCHEMA = '{DBOption.DBName}'");
            foreach (var tb in DBOption.Tables.Values)
            {
                if (tables.Contains(tb.TableName.ToLower()))
                {
                    // Column Check
                    bool isTheSame = CheckColumn(tb);
                    if (!isTheSame)
                    {
                        if (tb.Columns.Count == 0) break;
                        // DROP Table & Re-Create Table
                        Console.WriteLine($"INFO - '{tb.TableName}' in DB does not match with table configuration.");
                        MySQLDB.Drop(tb.TableName);
                        MySQLDB.Create(tb);
                    }
                    else
                    {
                        if (tb.NeedToInitialize)
                        {
                            MySQLDB.Truncate(tb.TableName);
                        }
                    }
                }
                else
                {
                    MySQLDB.Create(tb);
                }
            }
        }

        private static bool CheckColumn(DBTable table)
        {
            var columns = MySQLDB.SelectDataTable("INFORMATION_SCHEMA.COLUMNS",
                        new List<string>() { "COLUMN_NAME", "IS_NULLABLE", "DATA_TYPE", "CHARACTER_MAXIMUM_LENGTH", "COLUMN_KEY" },
                        $"TABLE_SCHEMA = '{DBName}' AND TABLE_NAME = '{table.TableName}'");

            Dictionary<string, bool> columnCheck = table.Columns.ToDictionary(c => c.Key, c => false);
            foreach (DataRow row in columns.Rows)
            {
                string name = row["COLUMN_NAME"].ToString();
                if (table.Columns.ContainsKey(name))
                {
                    var dataType = table.Columns[name];
                    bool isNotNull = row["IS_NULLABLE"].ToString() == "NO";
                    string type = row["DATA_TYPE"].ToString();

                    // DataType Check
                    if ((dataType.IsNotNull != isNotNull) ||
                        (dataType.ToString() != type))
                        return false;

                    // Size Check
                    if (type == "varchar")
                    {
                        int length = Convert.ToInt16(row["CHARACTER_MAXIMUM_LENGTH"]);
                        if (dataType.Size != length) return false;
                    }

                    // PK Check
                    bool isPK = row["COLUMN_KEY"].ToString() == "PRI";
                    if (isPK)
                    {
                        if (!table.PrimaryKeys.Contains(name)) return false;
                    }

                    columnCheck[name] = true;
                }
                else
                    return false;
            }

            if (columnCheck.Values.Contains(false))
                return false;

            return true;
        }
    }
}
