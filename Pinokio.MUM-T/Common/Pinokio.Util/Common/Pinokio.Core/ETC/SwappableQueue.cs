using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pinokio.Core
{
    /// <summary>
    /// Value 중복이 허용되지 않는 Queue
    /// Value간 순서 Swap이 가능한 Queue
    /// Value Remove가 가능한 Queue
    /// </summary>
    public class SwappableQueue<T>
    {
        private DistinctList<T> _list;

        public int Count { get { return _list.Count; } }
        public SwappableQueue()
        {
            _list = new DistinctList<T>();
        }

        public SwappableQueue(Queue<T> queue)
        {
            _list = new DistinctList<T>(queue.ToList());
        }

        public void Enqueue(T value)
        {
            _list.Add(value);
        }

        public T Dequeue()
        {
            if (_list.Count > 0)
            {
                T value = _list[0];
                _list.RemoveAt(0);
                return value;
            }
            else
                return default(T);
        }

        public T Peek()
        {
            if (_list.Count > 0)
                return _list[0];
            else
                return default(T);
        }

        public bool Contains(T value)
        {
            return _list.Contains(value);
        }

        public int IndexOf(T value)
        {
            return _list.IndexOf(value);
        }

        public T GetValue(int index)
        {
            return _list[index];
        }

        #region 특성
        public void Remove(T targetValue)
        {
            if (_list.Contains(targetValue))
            {
                _list.Remove(targetValue);
            }
        }

        public void Swap(T value1, T value2)
        {
            if (_list.Contains(value1) && _list.Contains(value2))
            {
                int index1 = _list.IndexOf(value1);
                int index2 = _list.IndexOf(value2);
                _list[index1] = value2;
                _list[index2] = value1;
            }
        }

        public override string ToString()
        {
            string valueString = "";
            for (int i = 0; i < _list.Count; i++)
            {
                var value = _list[i];
                valueString += value.ToString();

                if (i < _list.Count - 1)
                    valueString += ", ";
            }
            return valueString;
        }
        #endregion

    }
}