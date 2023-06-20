using System.Collections.Generic;

namespace Pinokio.Database
{
    public class QuerySet
    {
        private int _count = 0;
        private string _query = "";
        private List<object> _values = new List<object>();

        public string Query { get { return _query; } }
        public List<object> Values { get { return _values; } }
        public int LastIndex { get { return _values.Count; } }

        public void AddQuery(string query)
        {
            _count++;
            _query += query;
        }

        public void AddValue(object value)
        {
            _values.Add(value);
        }
    }
}
