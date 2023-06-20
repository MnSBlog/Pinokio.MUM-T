using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinokio.Database
{
    public class DBTable
    {
        public string TableName { get; set; }
        public Dictionary<string, DataType> Columns { get; set; }
        public List<string> PrimaryKeys { get; set; }

        public bool NeedToInitialize { get; set; }
        public string ColumnNames
        {
            get
            {
                string str = "";

                var columnNames = this.Columns.Keys.ToList();
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    str += columnNames[i];
                    if (i < this.Columns.Count - 1) str += ", ";
                }

                return str;
            }
        }

        public DBTable(string tableName)
        {
            this.TableName = tableName;
            this.Columns = new Dictionary<string, DataType>();
            this.PrimaryKeys = new List<string>();
            this.NeedToInitialize = false;
        }

        public bool AddColumn(string columnName, DataType type, bool isPrimaryKey = false)
        {
            if (isPrimaryKey)
            {
                type.IsNotNull = true;
                this.PrimaryKeys.Add(columnName);
            }

            this.Columns.Add(columnName, type);

            return true;
        }
    }
}
