using System;
using System.ComponentModel;

namespace Pinokio.Core
{
    public class AbstractObject : PinokioObject
    {
        private object _data;

        [Browsable(false)]
        public object Data { get => _data; set => _data = value; }

        public AbstractObject(uint id, Enum type = null) : base(id, type)
        {}

        public AbstractObject(string name, Enum type = null) : base(0, name, type)
        { }

        public AbstractObject(uint id, string name, Enum type = null) : base(id, name, type)
        { }
    }
}
