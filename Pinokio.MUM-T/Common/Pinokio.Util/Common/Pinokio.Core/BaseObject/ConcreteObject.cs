using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Core
{
    public class ConcreteObject : PinokioObject
    {
        private AbstractObject _abstractObj;
        public AbstractObject AbstractObj { get =>  _abstractObj; }

        public ConcreteObject(uint id, Enum type = null) : base(id, type)
        { }

        public ConcreteObject(uint id, string name, Enum type = null) : base(id, name, type)
        { }

        public override void Initialize()
        {
            base.Initialize();
            _abstractObj = null;
        }

        public virtual void SetAbstractObj(AbstractObject aObj)
        {
            _abstractObj = aObj;
        }
    }
}
