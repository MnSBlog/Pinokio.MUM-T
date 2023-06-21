using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Pinokio.Database
{
    public struct DataType
    {
        public enum TYPE
        {
            BIT,
            INT16,
            INT32,
            INT64,
            DOUBLE,
            VARCHAR,
            LONGTEXT,
            MEDIUMBLOB, // For Image
            TEXT,
        }

        public static DataType Short { get { return new DataType(TYPE.INT16); } }
        public static DataType ShortNotNull { get { return new DataType(TYPE.INT16, true); } }
        public static DataType Int32 { get { return new DataType(TYPE.INT32); } }
        public static DataType Int32NotNull { get { return new DataType(TYPE.INT32, true); } }
        public static DataType Long { get { return new DataType(TYPE.INT64); } }
        public static DataType LongNotNull { get { return new DataType(TYPE.INT64, true); } }
        public static DataType Double { get { return new DataType(TYPE.DOUBLE); } }
        public static DataType DoubleNotNull { get { return new DataType(TYPE.DOUBLE, true); } }
        public static DataType LongText { get { return new DataType(TYPE.LONGTEXT); } }
        public static DataType LongTextNotNull { get { return new DataType(TYPE.LONGTEXT, true); } }
        public static DataType Text { get { return new DataType(TYPE.TEXT); } }
        public static DataType TextNotNull { get { return new DataType(TYPE.TEXT, true); } }



        public TYPE Type { get; set; }
        public int Size { get; set; }
        public bool IsNotNull { get; set; }
        public string DBType
        {
            get
            {
                string type;
                switch (Type)
                {
                    case TYPE.VARCHAR:
                        type = String.Format("VARCHAR({0})", Size);
                        break;
                    case TYPE.INT16:
                        type = "SMALLINT";
                        break;
                    case TYPE.INT32:
                        type = "INT";
                        break;
                    case TYPE.INT64:
                        type = "BIGINT";
                        break;
                    case TYPE.DOUBLE:
                    case TYPE.MEDIUMBLOB:
                    case TYPE.LONGTEXT:
                    case TYPE.TEXT:
                        type = Type.ToString();
                        break;
                    default:
                        return "";
                }

                if (IsNotNull)
                    type += " NOT NULL";

                return type;
            }
        }

        public DataType(TYPE type, bool isNotNull = false)
        {
            this.Type = type;
            if (type == TYPE.VARCHAR)
                this.Size = 32;
            else
                this.Size = 0;
            this.IsNotNull = isNotNull;
        }

        public DataType(TYPE type, int size, bool isNotNull = false)
        {
            this.Type = type;
            this.Size = size;
            this.IsNotNull = isNotNull;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case TYPE.INT16:
                    return "smallint";
                case TYPE.INT32:
                    return "int";
                case TYPE.INT64:
                    return "bigint";
                default:
                    return Type.ToString().ToLower();
            }
        }

        public DataType(DataType type, bool isNotNull = false)
        {
            this.Type = type.Type;
            this.Size = type.Size;
            this.IsNotNull = isNotNull;
        }
    }
}

