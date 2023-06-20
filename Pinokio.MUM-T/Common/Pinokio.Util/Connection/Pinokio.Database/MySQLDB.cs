//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using MySql.Data.MySqlClient;

//namespace Pinokio.Database
//{
//    public static class MySQLDB
//    {
//        #region Private Members
//        private static bool _isConnected = false;
//        #endregion

//        #region Public Properties
//        public static bool IsConnected
//        {
//            get { return _isConnected; }
//        }
//        #endregion

//        #region Handler
//        public delegate void IsConnectedHandler();
//        public static event IsConnectedHandler Connect = delegate () { };
//        public delegate void InfoMessageHandler(string log);
//        public static event InfoMessageHandler DisplayMsg = delegate (string log) { Console.WriteLine("[INFO] >> " + log); };

//        public delegate void ErrorMessageHandler(string log);
//        public static event ErrorMessageHandler DisplayErrorMsg = delegate (string log) { Console.WriteLine("[ERROR] >> " + log); };
//        #endregion

//        #region About Connection
//        public static bool TryConnection()
//        {
//            var connString = DBOption.ConnectionString();
//            var conn = new MySqlConnection(connString);
//            try
//            {
//                conn.Open();
//                _isConnected = true;
//                conn.Close();
//                Connect();
//                DisplayMsg("Database is connected");
//            }
//            catch (System.Exception ex)
//            {
//                _isConnected = false;
//                DisplayErrorMsg(ex.ToString());
//            }
//            return _isConnected;
//        }
//        #endregion

//        #region Select
//        public static object SelectSingleValue(string tableName, string column, string condition = "")
//        {
//            if (condition == null) return null;
//            string query = $"SELECT {column} " +
//                           $"FROM {tableName} ";
//            query += condition != "" ? "WHERE " + condition : "";

//            var list = SendQueryForList(query);
//            if (list == null) return null;
//            else
//            {
//                if (list.Count == 0) return null;
//                else return list[0];
//            }
//        }

//        public static object Select(string tableName, List<string> columns, string condition = "")
//        {
//            string query = $"SELECT ";
//            if (columns == null || columns.Count <= 0) query += $"*";
//            else
//            {
//                for (var i = 0; i < columns.Count; i++)
//                {
//                    query += columns[i].ToString();
//                    if (i < columns.Count - 1) query += ", ";
//                }

//            }

//            query += $" FROM {tableName} ";
//            query += condition != "" ? "WHERE " + condition : "";

//            return SendQueryForList(query)[0];
//        }

//        public static List<string> SelectMultiValues(string tableName, string column, string condition = "")
//        {
//            if (condition == null) return null;
//            string query = $"SELECT {column} " +
//                           $"FROM {tableName} ";
//            query += condition != "" ? "WHERE " + condition : "";

//            return SendQueryForList(query);
//        }

//        public static List<string> SelectDistinct(string tableName, string column, string condition = "")
//        {
//            if (condition == null) return null;
//            string query = $"SELECT DISTINCT {column} " +
//                           $"FROM {tableName} ";
//            query += condition != "" ? "WHERE " + condition : "";

//            return SendQueryForList(query);
//        }

//        public static DataTable SelectDataTable(string tableName, string condition = "")
//        {
//            string query = $"SELECT * FROM {tableName} ";
//            query += condition != "" ? "WHERE " + condition : "";

//            return MySQLDB.SendQueryForDT(query);
//        }

//        public static DataTable SelectDataTable(string tableName, List<string> columns, string condition = "")
//        {
//            string query = $"SELECT ";
//            if (columns == null) query += $"*";
//            else
//            {
//                for (var i = 0; i < columns.Count; i++)
//                {
//                    query += columns[i].ToString();
//                    if (i < columns.Count - 1) query += ", ";
//                }
//            }

//            query += $" FROM {tableName} ";
//            query += condition != "" ? "WHERE " + condition : "";

//            return MySQLDB.SendQueryForDT(query);
//        }

//        public static byte[] SelectImage(string tableName, string columnName, string condition = "")
//        {
//            var query = "SELECT " + columnName + " FROM " + tableName;
//            query += condition != "" ? " WHERE " + condition : "";

//            byte[] bimage = null;
//            using (var conn = new MySqlConnection(DBOption.ConnectionString()))
//            {
//                conn.Open();
//                var cmd = new MySqlCommand(query, conn);
//                using (var myReader = cmd.ExecuteReader())
//                {
//                    while (myReader.Read())
//                    {
//                        bimage = (byte[])myReader[0];
//                    }
//                    myReader.Close();
//                }
//                conn.Close();
//            }

//            return bimage;
//        }
//        #endregion

//        #region Insert
//        public static void Insert(string tableName, List<object> values)
//        {
//            if (values == null) return;
//            if (values.Count <= 0) return;

//            var query = $"INSERT INTO {tableName} (";
//            var columns = DBOption.Tables[tableName].Columns;
//            if (columns.Count == values.Count)
//            {
//                query += DBOption.Tables[tableName].ColumnNames + ") VALUES (";
//            }
//            else if (columns.Count > values.Count)
//            {
//                var columnNames = columns.Keys.ToList();
//                for (int i = 0; i < values.Count; i++)
//                {
//                    query += columnNames[i];
//                    if (i < values.Count - 1) query += ", ";
//                }
//                query += ") VALUES (";
//            }
//            else
//            {
//                return;
//            }

//            for (int j = 0; j < values.Count; j++)
//            {
//                query += "@val" + j;
//                if (j == values.Count - 1) query += ")";
//                else query += ", ";
//            }

//            using (var conn = new MySqlConnection(DBOption.ConnectionString()))
//            {
//                conn.Open();
//                var cmd = new MySqlCommand(query, conn);

//                for (int k = 0; k < values.Count; k++)
//                    cmd.Parameters.AddWithValue("@val" + k, values[k]);

//                cmd.ExecuteNonQuery();
//                conn.Close();
//            }

//        }

//        public static void Insert(string tableName, List<string> columns, List<object> values)
//        {
//            if (columns == null || values == null) return;
//            if (columns.Count != values.Count) return;
//            //foreach (var column in columns)
//            //{
//            //    if (!DBOption.Tables[tableName].Columns.ContainsKey(column))
//            //    {
//            //        DisplayErrorMsg($"There is no column({column}) in this table({tableName})");
//            //        return;
//            //    }
//            //}

//            var query = "INSERT INTO " + tableName + " (";

//            for (int i = 0; i < columns.Count; i++)
//            {
//                query += columns[i];
//                if (i == columns.Count - 1) query += ") VALUES (";
//                else query += ", ";
//            }

//            for (int j = 0; j < values.Count; j++)
//            {
//                query += "@val" + j;
//                if (j == values.Count - 1) query += ")";
//                else query += ", ";
//            }

//            using (var conn = new MySqlConnection(DBOption.ConnectionString()))
//            {
//                conn.Open();
//                var cmd = new MySqlCommand(query, conn);

//                for (int k = 0; k < values.Count; k++)
//                    cmd.Parameters.AddWithValue("@val" + k, values[k]);

//                cmd.ExecuteNonQuery();
//                conn.Close();
//            }

//        }

//        public static void InsertIfNotExits(string tableName, List<object> values, string condition)
//        {
//            if (values == null) return;
//            if (values.Count <= 0) return;

//            if (DBOption.Tables.ContainsKey(tableName))
//            {
//                var query = $"INSERT INTO {tableName} (";
//                var columns = DBOption.Tables[tableName].Columns;

//                if (columns.Count == values.Count)
//                {
//                    query += DBOption.Tables[tableName].ColumnNames;
//                }
//                else if (columns.Count > values.Count)
//                {
//                    var columnNames = columns.Keys.ToList();
//                    for (int i = 0; i < values.Count; i++)
//                    {
//                        query += columnNames[i];
//                        if (i < values.Count - 1) query += ", ";
//                    }
//                }
//                else
//                {
//                    return;
//                }

//                query += ") SELECT ";
//                for (int j = 0; j < values.Count; j++)
//                {
//                    query += $"'{values[j]}'";
//                    if (j == values.Count - 1) query += " FROM DUAL";
//                    else query += ", ";
//                }

//                query += $" WHERE NOT EXISTS ( SELECT * FROM {tableName} WHERE {condition})";

//                using (var conn = new MySqlConnection(DBOption.ConnectionString()))
//                {
//                    conn.Open();
//                    var cmd = new MySqlCommand(query, conn);

//                    cmd.ExecuteNonQuery();
//                    conn.Close();
//                }
//            }

//        }
//        #endregion

//        #region Delete
//        public static void Delete(string tableName, string condition = "")
//        {
//            if (condition == null) return;

//            var query = $"DELETE FROM {tableName}";
//            query += condition != "" ? " WHERE " + condition : "";

//            SendQueryForEdit(query);
//        }
//        #endregion

//        #region Update
//        public static void Update(string tableName, List<string> columns, List<object> values, string condition = "")
//        {
//            if (columns == null || values == null) return;
//            if (columns.Count != values.Count) return;

//            var query = string.Empty;
//            query = $"UPDATE {tableName} SET ";

//            for (int i = 0; i < columns.Count; i++)
//            {
//                query += columns[i] + " = @val" + i;
//                if (i < columns.Count - 1) query += ", ";
//            }

//            query += condition != "" ? " WHERE " + condition : "";

//            using (var conn = new MySqlConnection(DBOption.ConnectionString()))
//            {
//                conn.Open();
//                var cmd = new MySqlCommand(query, conn);

//                for (int k = 0; k < values.Count; k++)
//                    cmd.Parameters.AddWithValue("@val" + k, values[k]);

//                cmd.ExecuteNonQuery();
//                conn.Close();
//            }

//        }

//        // TO DO: groupid --> condition column and value
//        //public static void UpdateImage(string tableName, byte[] image, string groupid)
//        //{
//        //    var query = "UPDATE " + tableName + " SET lastImage = @image WHERE groupId = @id";

//        //    var cmd = new MySqlCommand(query, _mysqlConnection);
//        //    //var paramImage = new SqlParameter("@image", SqlDbType.Image, image.Length);
//        //    //paramImage.Value = image;
//        //    cmd.Parameters.Add("@image", MySqlDbType.MediumBlob).Value = image;
//        //    cmd.Parameters.AddWithValue("@id", groupid);

//        //    cmd.ExecuteNonQuery();
//        //}
//        #endregion

//        #region Create Table
//        public static void Create(string tableName, Dictionary<string, DataType> columns, List<string> primaryKeys)
//        {
//            if (columns == null) return;
//            if (columns.Count <= 0) return;
//            if (primaryKeys == null) return;
//            if (primaryKeys.Count <= 0) return;

//            var query = $"CREATE TABLE IF NOT EXISTS {tableName} ( ";

//            var names = columns.Keys.ToList();
//            for (int i = 0; i < columns.Count; i++)
//            {
//                query += names[i] + " " + columns[names[i]].DBType;
//                if (i < names.Count - 1) query += ", ";
//            }

//            query += " , PRIMARY KEY( ";
//            for (int j = 0; j < primaryKeys.Count; j++)
//            {
//                query += primaryKeys[j];
//                if (j == primaryKeys.Count - 1)
//                    query += " )";
//                else query += ", ";
//            }

//            query += " )";
//            MySQLDB.SendQueryForEdit(query);
//        }

//        public static void Create(DBTable info)
//        {
//            MySQLDB.Create(info.TableName, info.Columns, info.PrimaryKeys);
//        }

//        public static void CreateSchema(string connString, string dbName)
//        {
//            var query = "CREATE DATABASE IF NOT EXISTS " + dbName;
//            using (var conn = new MySqlConnection(connString))
//            {
//                try
//                {
//                    conn.Open();
//                    var cmd = new MySqlCommand(query, conn);
//                    cmd.ExecuteNonQuery();
//                    conn.Close();
//                }
//                catch (Exception e)
//                {
//                    DisplayErrorMsg(" Wrong Query;" + query);
//                }
//            }
//        }
//        #endregion

//        #region Drop Table
//        public static void Drop(string tableName)
//        {
//            var query = $"DROP TABLE {tableName}";
//            MySQLDB.SendQueryForEdit(query);
//        }
//        #endregion

//        #region Truncate Table
//        public static void Truncate(string tableName)
//        {
//            var query = $"TRUNCATE TABLE {tableName}";
//            MySQLDB.SendQueryForEdit(query);
//        }
//        #endregion

//        #region Send Query
//        public static void SendQueryForEdit(string query)
//        {
//            try
//            {
//                if (query == null) return;
//                if (!IsConnected)
//                {
//                    DisplayErrorMsg("Databae is not Connected");
//                    return;
//                }

//                using (var conn = new MySqlConnection(DBOption.ConnectionString()))
//                {
//                    conn.Open();
//                    var cmd = new MySqlCommand(query, conn);
//                    cmd.ExecuteNonQuery();
//                    conn.Close();
//                }
//            }
//            catch(Exception e)
//            {
//                DisplayErrorMsg(" Wrong Query;"+ query);
//            }
//        }

//        public static List<string> SendQueryForList(string query)
//        {
//            if (query == null) return null;
//            if (!IsConnected)
//            {
//                DisplayErrorMsg("Databae is not Connected");
//                return null;
//            }

//            var list = new List<string>();
//            using (var conn = new MySqlConnection(DBOption.ConnectionString()))
//            {
//                try
//                {
//                    conn.Open();
//                    var cmd = new MySqlCommand(query, conn);
//                    using (var myReader = cmd.ExecuteReader())
//                    {
//                        while (myReader.Read())
//                        {
//                            list.Add(myReader.GetString(0));
//                        }
//                    }
//                    conn.Close();
//                }
//                catch (SystemException ex)
//                {
//                    DisplayErrorMsg(ex.ToString());
//                }
//            }

//            return list;
//        }

//        public static DataTable SendQueryForDT(string query)
//        {
//            if (query == null) return null;
//            if (!IsConnected)
//            {
//                DisplayErrorMsg("Databae is not Connected");
//                return null;
//            }

//            var dataSet = new DataSet();
//            using (var conn = new MySqlConnection(DBOption.ConnectionString()))
//            {
//                try
//                {
//                    conn.Open();
//                    var cmd = new MySqlCommand(query, conn);
//                    cmd.ExecuteNonQuery();
//                    using (var adapter = new MySqlDataAdapter())
//                    {
//                        adapter.SelectCommand = cmd;
//                        adapter.Fill(dataSet);
//                    }
//                    conn.Close();
//                }
//                catch (SystemException ex)
//                {
//                    DisplayErrorMsg(ex.ToString());
//                    return null;
//                }
//            }

//            return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
//        }

//        #endregion

//        #region About Message Handler
//        public static void RemoveInfoMsgHandler()
//        {
//            foreach (InfoMessageHandler d in DisplayMsg.GetInvocationList())
//                DisplayMsg -= d;
//        }

//        public static void RemoveErrorMsgHandler()
//        {
//            foreach (ErrorMessageHandler d in DisplayErrorMsg.GetInvocationList())
//                DisplayErrorMsg -= d;
//        }
//        #endregion
//    }
//}
