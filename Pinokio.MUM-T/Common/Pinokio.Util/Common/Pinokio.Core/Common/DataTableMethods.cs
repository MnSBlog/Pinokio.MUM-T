using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Pinokio.Core
{

    public static class DataTableMethods
    {
        public static string Format(this DataTable dt, int header = 1)
        {
            // header = 1 -> Print header, header = 0 -> No print header
            int m = dt.Columns.Count; // number of column
            int n = dt.Rows.Count; // number of row

            string result = "";
            if (header == 1)
            {
                foreach (var col in dt.Columns)
                    result += "\t" + col.ToString();
            }
            result += "\n";

            for (int i = 0; i < n; i++)
            {
                string line = i + "\t";
                for (int j = 0; j < m; j++)
                    line += dt.Rows[i][j].ToString() + "\t";

                result += line + "\n";
            }

            return result;
        }

        const string No = " №";
        public static void Print(this DataTable table, bool rowNum = false) =>
            Print(table, null, rowNum);
        public static void Print(this DataTable table, int top, bool rowNum = false) =>
            Print(table, top, null, rowNum);
        public static void Print(this DataTable table, string[] col, bool rowNum = false) =>
            Print(table, int.MaxValue, col, rowNum);
        public static void Print(this DataTable table, int top, string[] col, bool rowNum = false)
        {
            if (rowNum == true)
            {
                table = table.Copy();
                table.Columns.Add(No, typeof(int)).SetOrdinal(0);
                foreach(DataRow row in table.Rows) 
                {
                    row[No] = table.Rows.IndexOf(row);
                }
                //Array.ForEach(table.AsEnumerable().Take(top).ToArray(), row =>
                //row[No] = table.Rows.IndexOf(row));
            }


            DataColumn[] columns = table.Columns.Cast<DataColumn>().ToArray();


            if (col != null)
            {
                if (rowNum == true) col = col.Append(No).ToArray();
                columns = columns.Where(c => col.Contains(c.ColumnName)).ToArray();
            }


            if (columns.Length == 0)
            {
                Console.WriteLine("NO columns");
                return;
            }


            int RealLen(string str) => str.Select(x => (0xFF00 & x) == 0 ? 1 : 2).Sum();
            int UnLen(string str) => str.Select(x => (0xFF00 & x) == 0 ? 0 : 1).Sum();

            int GetAlign(Type t)
            {
                string typeStr = t.ToString().ToLower();
                switch (typeStr)
                {
                    case "system.byte":
                    case "system.sbyte":
                    case "system.short":
                    case "system.ushort":
                    case "system.int":
                    case "system.uint":
                    case "system.long":
                    case "system.ulong":
                    case "system.float":
                    case "system.double":
                    case "system.decimal":
                        return 1;
                    default:
                        return -1;
                }
            }

            string Escape(string str)
            {
                return String.Join("", str.Select(x =>
                {
                    switch (x)
                    {
                        case '\b': return "\\b";
                        case '\f': return "\\f";
                        case '\n': return "\\n";
                        case '\r': return "\\r";
                        case '\t': return "\\t";
                        default: return x.ToString();
                    }
                }));
            }

            List<string> colNames = columns.Select(x => Escape(x.ColumnName)).ToList();

            var columnLengths = new List<int>();
            colNames.ForEach(cn => columnLengths.Add(cn.Length));

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < colNames.Count; i++)
                {
                    string columnName = colNames[i];
                    int tempLength = RealLen(row[columnName].ToString());
                    if (tempLength > columnLengths[i])
                        columnLengths[i] = tempLength;
                }
            }


            var align = columns.Select(x => GetAlign(x.DataType)).Select((al, i) =>
                             new Func<string, string>(str =>
                             String.Format($"{{0,{al * (columnLengths[i] - UnLen("" + str))}}}", str))).ToArray();


            string PadBoth(string source, int length)
            {
                int spaces = length - RealLen(source);
                return new String(' ', spaces / 2) + source + new string(' ', spaces - spaces / 2);
            }


            Action<string> WL = Console.WriteLine;
            var preBg = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            WL(PadBoth(table.TableName, (columnLengths.Sum() + colNames.Count() * 3 + 2)));
            Console.BackgroundColor = preBg;


            WL($"┌{String.Join("┬", columnLengths.Select(x => new String('-', x + 2)))}┐");
            WL($"│ {String.Join(" │ ", colNames.Select((x, i) => x.PadRight(columnLengths[i] - UnLen(x))))} │");
            WL($"├{String.Join("┼", columnLengths.Select(x => new String('-', x + 2)))}┤");
            foreach(DataRow row in table.Rows) 
            {
                WL($"│ {String.Join(" │ ", columns.Select((x, i) => align[i](Escape("" + row[x]))))} │");
            }
            WL($"└{String.Join("┴", columnLengths.Select(x => new String('-', x + 2)))}┘");

            Console.WriteLine();
        }
    }
}

