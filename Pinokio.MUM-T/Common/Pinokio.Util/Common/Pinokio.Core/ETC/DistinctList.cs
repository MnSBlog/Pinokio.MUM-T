using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pinokio.Core
{
    /// <summary>
    /// Value 중복이 허용되지 않는 List
    /// </summary>
    public class DistinctList<T> : IEquatable<DistinctList<T>>, IEnumerable<T>
    {
        private List<T> _list;

        public int Count { get { return _list.Count; } }

        public DistinctList()
        {
            _list = new List<T>();
        }

        public DistinctList(List<T> list)
        {
            _list = new List<T>(list);
        }

        public DistinctList(DistinctList<T> list)
        {
            _list = new List<T>(list._list);
        }

        #region IEnumerable Implementation
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (IEnumerator<T>)GetEnumerator();
        }

        public DistinctListEnumerator<T> GetEnumerator()
        {
            return new DistinctListEnumerator<T>(_list);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IEquatable Implementation
        public override int GetHashCode()
        {
            return _list.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DistinctList<T>);
        }

        public bool Equals(DistinctList<T> other)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(other, null)) return false;

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType()) return false;

            // If components counts are not exactly the same, return false.
            if (_list.Count != other.Count) return false;

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other)) return true;

            for (int i = 0; i < _list.Count; i++)
            {
                if (!other.Contains(_list[i]))
                    return false;
            }

            return true;
        }
        #endregion 

        public int FindIndex(Predicate<T> match)
        {
            return _list.FindIndex(match);
        }

        public T Find(Predicate<T> match)
        {
            return _list.Find(match);
        }

        public bool Contains(T id)
        {
            return _list.Contains(id);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(DistinctList<T> other)
        {
            // If other counts are bigger than this, return false.
            if (_list.Count < other.Count) return false;

            for (int i = 0; i < other.Count; i++)
            {
                if (!_list.Contains(other[i]))
                    return false;
            }

            return true;
        }

        public bool Intersects(DistinctList<T> other)
        {
            if (_list.Count > other.Count)
            {
                for (int i = 0; i < other.Count; i++)
                {
                    if (_list.Contains(other[i]))
                        return true;
                }
            }
            else
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    if (other.Contains(_list[i]))
                        return true;
                }
            }

            return false;
        }

        public bool Intersects(List<T> other)
        {
            if (_list.Count > other.Count)
            {
                for (int i = 0; i < other.Count; i++)
                {
                    if (_list.Contains(other[i]))
                        return true;
                }
            }
            else
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    if (other.Contains(_list[i]))
                        return true;
                }
            }

            return false;
        }
        public void Add(T id)
        {
            if (!_list.Contains(id))
                _list.Add(id);
        }

        public void AddRange(DistinctList<T> other)
        {
            for (int i = 0; i < other.Count; i++)
            {
                if (!_list.Contains(other[i]))
                    _list.Add(other[i]);
            }
        }

        public void AddRange(List<T> other)
        {
            for (int i = 0; i < other.Count; i++)
            {
                if (!_list.Contains(other[i]))
                    _list.Add(other[i]);
            }
        }

        public void AddRange(ICollection<T> other)
        {
            for (int i = 0; i < other.Count; i++)
            {
                if (!_list.Contains(other.ElementAt(i)))
                    _list.Add(other.ElementAt(i));
            }
        }

        public void Remove(T item)
        {
            if (_list.Contains(item))
                _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public void Merge(DistinctList<T> other)
        {
            for (int i = 0; i < other.Count; i++)
            {
                if (!_list.Contains(other[i]))
                    _list.Add(other[i]);
            }
        }

        public static DistinctList<T> Merge(DistinctList<T> list1, DistinctList<T> list2)
        {
            DistinctList<T> newList = null;
            if (list1.Count > list2.Count)
            {
                list1.Merge(list2);
                newList = new DistinctList<T>(list1);
            }
            else
            {
                list2.Merge(list1);
                newList = new DistinctList<T>(list2);
            }

            return newList;
        }

        public DistinctList<T> Except(DistinctList<T> other)
        {
            return new DistinctList<T>(_list.Except(other._list).ToList());
        }

        public DistinctList<T> Except(List<T> other)
        {
            return new DistinctList<T>(_list.Except(other).ToList());
        }
        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public List<T> ToList()
        {
            return new List<T>(_list);
        }

        public T[] ToArray()
        {
            return _list.ToArray();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < _list.Count; i++)
            {
                str += _list[i];
                if (i < _list.Count - 1)
                    str += ", ";
            }
            return str;
        }

        public T this[int key]
        {
            get => _list[key];
            set => _list[key] = value;
        }

        public T Last()
        {
            return _list.Last();
        }

        public static bool operator ==(DistinctList<T> list1, DistinctList<T> list2)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(list1, null))
            {
                if (Object.ReferenceEquals(list2, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return list1.Equals(list2);
        }

        public static bool operator !=(DistinctList<T> list1, DistinctList<T> list2)
        {
            return !(list1 == list2);
        }
    }

    public class DistinctListEnumerator<T> : System.Collections.IEnumerator
    {
        private List<T> _list;
        private int _position;

        public DistinctListEnumerator(List<T> list)
        {
            _list = list;
            _position = -1;
        }

        public bool MoveNext()
        {
            _position++;
            return (_position < _list.Count);
        }

        public void Reset()
        {
            _position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public T Current
        {
            get
            {
                try
                {
                    return _list[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
