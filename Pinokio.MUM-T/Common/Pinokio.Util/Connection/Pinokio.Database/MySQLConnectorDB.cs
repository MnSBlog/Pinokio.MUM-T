using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pinokio.Core;
using MySqlConnector;

namespace Pinokio.Database
{
    public static class MySQLDB
    {
        #region Private Members
        private static bool _isConnected = false;
        #endregion

        #region Public Properties
        public static bool IsConnected
        {
            get { return _isConnected; }
        }
        #endregion

        #region Handler
        public delegate void IsConnectedHandler();
        public static event IsConnectedHandler Connect = delegate () { };
        #endregion

        #region About Connection
        public static bool TryConnection()
        {
            var connString = DBOption.ConnectionString();
            var conn = new MySqlConnection(connString);
            try
            {
                conn.Open();
                _isConnected = true;
                conn.Close();
                Connect();
                LogHandler.AddLog(LogLevel.Info, "Database is connected");
            }
            catch (System.Exception ex)
            {
                _isConnected = false;
                LogHandler.AddLog(LogLevel.Error, ex.ToString());
            }
            return _isConnected;
        }
        #endregion

        #region Select
        public static object Select(string tableName, List<string> columnNames, string condition = "")
        {
            string query = BuildSelectQuery(tableName, columnNames, condition);
            List<object> value = SendQueryForList(query);
            if (value.Count == 0) return null;
            else return value[0];
        }

        public static List<object> SelectMultiValues(string tableName, string column, string condition = "")
        {
            string query = BuildSelectQuery(tableName, new List<string>() { column }, condition);
            return SendQueryForList(query);
        }

        public static List<object> SelectDistinct(string tableName, string column, string condition = "")
        {
            string query = BuildSelectQuery(tableName, new List<string>() { column }, condition, true);
            return SendQueryForList(query);
        }

        public static DataTable SelectDataTable(string tableName, string condition = "")
        {
            string query = BuildSelectQuery(tableName, null, condition);
            return SendQueryForDT(query);
        }

        public static DataTable SelectDataTable(string tableName, List<string> columns, string condition = "")
        {
            string query = BuildSelectQuery(tableName, columns, condition);
            return SendQueryForDT(query);
        }
        public static DataTable SelectGroupDataTable(string tableName, List<string> columns, string condition = "", string group = "")
        {
            string query = BuildGroupSelectQuery(tableName, columns, condition, group);
            return SendQueryForDT(query);
        }

        public static byte[] SelectImage(string tableName, string columnName, string condition = "")
        {
            var query = "SELECT " + columnName + " FROM " + tableName;
            query += condition != "" ? " WHERE " + condition : "";

            byte[] bimage = null;
            using (var conn = new MySqlConnection(DBOption.ConnectionString()))
            {
                conn.Open();
                var cmd = new MySqlCommand(query, conn);
                using (var myReader = cmd.ExecuteReader())
                {
                    while (myReader.Read())
                    {
                        bimage = (byte[])myReader[0];
                    }
                    myReader.Close();
                }
                conn.Close();
            }

            return bimage;
        }

        private static string BuildSelectQuery(string tableName, List<string> columnNames, string condition = "", bool isDistinct = false)
        {
            string query = $"SELECT ";
            if (isDistinct)
                query += " DISTINCT ";

            if (columnNames == null || columnNames.Count <= 0) query += $"*";
            else
            {
                for (var i = 0; i < columnNames.Count; i++)
                {
                    query += columnNames[i].ToString();
                    if (i < columnNames.Count - 1) query += ", ";
                }
            }

            query += $" FROM {tableName} ";
            query += condition != "" ? "WHERE " + condition : "";

            return query;
        }

        private static string BuildGroupSelectQuery(string tableName, List<string> columnNames, string condition = "", string group = "", bool isDistinct = false)
        {
            string query = $"SELECT ";
            if (isDistinct)
                query += " DISTINCT ";

            if (columnNames == null || columnNames.Count <= 0) query += $"*";
            else
            {
                for (var i = 0; i < columnNames.Count; i++)
                {
                    query += columnNames[i].ToString();
                    if (i < columnNames.Count - 1) query += ", ";
                }
            }

            query += $" FROM {tableName} ";
            query += condition != "" ? "WHERE " + condition : "";
            query += group != "" ? "GROUP BY " + group : "";

            return query;
        }

        #endregion

        #region Insert
        public static void Insert(string tableName, List<object> values)
        {
            if (DBOption.Tables.ContainsKey(tableName))
            {
                string query = BuildInsertQuery(tableName, DBOption.Tables[tableName].Columns.Keys.ToList(), values);
                SendQuery(query, values);
            }
            else
            {
                throw new Exception($"Can't Insert Value, There is no information about table({tableName})");
            }
        }

        public static void Insert(string tableName, List<string> columns, List<object> values)
        {
            string query = BuildInsertQuery(tableName, columns, values);
            SendQuery(query, values);
        }

        public static void InsertIfNotExist(string tableName, List<object> values, string condition)
        {
            if (DBOption.Tables.ContainsKey(tableName))
            {
                string query = BuildInsertQuery(tableName, DBOption.Tables[tableName].Columns.Keys.ToList(), values, true, condition);
                SendQuery(query, values);
            }
            else
            {
                throw new Exception($"Can't Insert Value, There is no information about table({tableName})");
            }
        }

        public static void InsertIfNotExist(string tableName, List<string> columns, List<object> values, string condition)
        {
            string query = BuildInsertQuery(tableName, columns, values, true, condition);
            SendQuery(query, values);
        }

        public static string BuildInsertQueryWithValue(string tableName, List<string> columnNames, List<object> values)
        {
            if (columnNames is null) throw new Exception($"Can't Build Insert Query; Wrong Input, There are no columnNames");
            if (values is null) throw new Exception($"Can't Build Insert Query; Wrong Input, There are no values");
            if (columnNames.Count != values.Count) throw new Exception($"Can't Build Insert Query; Wrong Input, ");

            string query = $"INSERT INTO {tableName} (";
            for (int i = 0; i < columnNames.Count; i++)
            {
                query += columnNames[i];
                if (i < values.Count - 1) query += ", ";
            }

            query += ") VALUES (";
            for (int j = 0; j < values.Count; j++)
            {
                if (values[j] is string)
                    query += "'" + values[j] + "'";
                else
                    query += values[j];

                if (j == values.Count - 1) query += ");";
                else query += ", ";
            }
            return query;
        }

        private static string BuildInsertQuery(string tableName, List<string> columnNames, List<object> values, bool ifNotExist = false, string condition = "")
        {
            if (columnNames is null) throw new Exception($"Can't Build Insert Query; Wrong Input, There are no columnNames");
            if (values is null) throw new Exception($"Can't Build Insert Query; Wrong Input, There are no values");
            if (columnNames.Count != values.Count) throw new Exception($"Can't Build Insert Query; Wrong Input, ");

            string query = $"INSERT INTO {tableName} (";
            for (int i = 0; i < columnNames.Count; i++)
            {
                query += columnNames[i];
                if (i < values.Count - 1) query += ", ";
            }

            if (!ifNotExist)
            {
                query += ") VALUES (";
                for (int j = 0; j < columnNames.Count; j++)
                {
                    query += "@val" + j;
                    if (j == values.Count - 1) query += ")";
                    else query += ", ";
                }
            }
            else
            {
                if (condition == "")
                    throw new Exception($"Can't Build Insert Query; Wrong Condition ({condition})");

                query += ") SELECT ";
                for (int j = 0; j < values.Count; j++)
                {
                    query += $"'{values[j]}'";
                    if (j == values.Count - 1) query += " FROM DUAL";
                    else query += ", ";
                }

                query += $" WHERE NOT EXISTS (SELECT * FROM {tableName} WHERE {condition})";
            }

            return query;
        }
        #endregion

        #region Delete
        public static void Delete(string tableName, string condition = "")
        {
            if (condition == null) return;

            var query = $"DELETE FROM {tableName}";
            query += condition != "" ? " WHERE " + condition : "";

            SendQuery(query);
        }
        #endregion

        #region Update
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        /// <param name="condition"></param>
        public static void Update(string tableName, List<string> columns, List<object> values, string condition = "")
        {
            if (columns == null || values == null) return;
            if (columns.Count != values.Count) return;

            var query = string.Empty;
            query = $"UPDATE {tableName} SET ";

            for (int i = 0; i < columns.Count; i++)
            {
                query += columns[i] + " = @val" + i;
                if (i < columns.Count - 1) query += ", ";
            }

            query += condition != "" ? " WHERE " + condition : "";

            using (var conn = new MySqlConnection(DBOption.ConnectionString()))
            {
                conn.Open();
                var cmd = new MySqlCommand(query, conn);

                for (int k = 0; k < values.Count; k++)
                    cmd.Parameters.AddWithValue("@val" + k, values[k]);

                cmd.ExecuteNonQuery();
                conn.Close();

            }

        }

        // TO DO: groupid --> condition column and value
        //public static void UpdateImage(string tableName, byte[] image, string groupid)
        //{
        //    var query = "UPDATE " + tableName + " SET lastImage = @image WHERE groupId = @id";

        //    var cmd = new MySqlCommand(query, _mysqlConnection);
        //    //var paramImage = new SqlParameter("@image", SqlDbType.Image, image.Length);
        //    //paramImage.Value = image;
        //    cmd.Parameters.Add("@image", MySqlDbType.MediumBlob).Value = image;
        //    cmd.Parameters.AddWithValue("@id", groupid);

        //    cmd.ExecuteNonQuery();
        //}
        #endregion

        #region Create Table
        public static void Create(string tableName, Dictionary<string, DataType> columns, List<string> primaryKeys)
        {
            if (columns == null) return;
            if (columns.Count <= 0) return;
            if (primaryKeys == null) return;
            if (primaryKeys.Count <= 0) return;

            var query = $"CREATE TABLE IF NOT EXISTS {tableName} ( ";

            var names = columns.Keys.ToList();
            for (int i = 0; i < columns.Count; i++)
            {
                query += names[i] + " " + columns[names[i]].DBType;
                if (i < names.Count - 1) query += ", ";
            }

            query += " , PRIMARY KEY( ";
            for (int j = 0; j < primaryKeys.Count; j++)
            {
                query += primaryKeys[j];
                if (j == primaryKeys.Count - 1)
                    query += " )";
                else query += ", ";
            }

            query += " )";
            MySQLDB.SendQuery(query);
        }

        public static void Create(DBTable info)
        {
            Create(info.TableName, info.Columns, info.PrimaryKeys);
        }

        public static void CreateSchema(string connString, string dbName)
        {
            string query = "CREATE DATABASE IF NOT EXISTS " + dbName;
            using (var conn = new MySqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception e)
                {
                    LogHandler.AddLog(LogLevel.Error, "Wrong Query;" + query + ", " + e.Message);
                }
            }
        }
        #endregion

        #region Drop Table
        public static void Drop(string tableName)
        {
            var query = $"DROP TABLE {tableName}";
            MySQLDB.SendQuery(query);
        }
        #endregion

        #region Truncate Table
        public static void Truncate(string tableName)
        {
            var query = $"TRUNCATE TABLE {tableName}";
            MySQLDB.SendQuery(query);
        }
        #endregion

        #region Alter Table
        public static void AlterIfAddColumn(string tableName, string columnName, string dataType)
        {
            var query = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {dataType}";
            MySQLDB.SendQuery(query);
        }

        public static void AlterIfDropColumn(string tableName, string columnName)
        {
            var query = $"ALTER TABLE {tableName} DROP COLUMN {columnName}";
            MySQLDB.SendQuery(query);
        }
        #endregion

        #region Send Query
        public static void SendQuery(QuerySet set)
        {
            if (set.Query == "") return;
            using (var conn = new MySqlConnection(DBOption.ConnectionString()))
            {
                conn.Open();
                var cmd = new MySqlCommand(set.Query, conn);
                for (int i = 0; i < set.Values.Count; i++)
                    cmd.Parameters.AddWithValue("@val" + i, set.Values[i]);

                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static void SendQuery(string query)
        {
            try
            {
                if (query == null) return;
                if (!IsConnected)
                {
                    LogHandler.AddLog(LogLevel.Error, "Databae is not Connected");
                    return;
                }

                using (var conn = new MySqlConnection(DBOption.ConnectionString()))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch(Exception e)
            {
                LogHandler.AddLog(LogLevel.Error, " Wrong Query;" + query + ", " + e.Message);
            }
        }

        public static void SendQuery(string query, List<object> values)
        {
            try
            {
                using (var conn = new MySqlConnection(DBOption.ConnectionString()))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(query, conn);

                    for (int k = 0; k < values.Count; k++)
                        cmd.Parameters.AddWithValue("@val" + k, values[k]);

                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }
            catch (MySqlConnector.MySqlException ex)
            {

            }
        }

        public static List<object> SendQueryForList(string query)
        {
            if (!IsConnected)
            {
                LogHandler.AddLog(LogLevel.Error, "Databae is not Connected");
                return null;
            }

            var list = new List<object>();
            using (var conn = new MySqlConnection(DBOption.ConnectionString()))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand(query, conn);
                    using (var myReader = cmd.ExecuteReader())
                    {
                        while (myReader.Read())
                        {
                            try
                            {
                                list.Add(myReader.GetValue(0));
                            }
                            catch
                            {
                                list.Add(myReader.GetInt32(0));
                            }
                        }
                    }
                    conn.Close();
                }
                catch (SystemException ex)
                {
                    LogHandler.AddLog(LogLevel.Error, ex.ToString());
                }
            }

            return list;
        }

        public static DataTable SendQueryForDT(string query)
        {
            if (query == null) return null;
            if (!IsConnected)
            {
                LogHandler.AddLog(LogLevel.Error, "Databae is not Connected");
                return null;
            }

            var dataSet = new DataSet();
            using (var conn = new MySqlConnection(DBOption.ConnectionString()))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    using (var adapter = new MySqlDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataSet);
                    }
                    conn.Close();
                }
                catch (SystemException ex)
                {
                    LogHandler.AddLog(LogLevel.Error, ex.ToString());
                    return null;
                }
            }

            return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
        }
        #endregion
    }
}
