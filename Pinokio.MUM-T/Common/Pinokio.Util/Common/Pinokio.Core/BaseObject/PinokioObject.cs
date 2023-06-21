using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinokio.Core
{
    public abstract class PinokioObject : Object
    {
        #region Variables
        private uint _id;
        private string _name;
        private Enum _type;
        private string _note;
        #endregion

        #region Properties
        [PinokioCategory("Base", 0)]
        public uint Id { get => _id; }

        [PinokioCategory("Base", 0)]
        public string Name { get => _name; }

        [PinokioCategory("Base", 0)]
        public Enum Type { get => _type; }

        [PinokioCategory("Base", 0)]
        public string Note { get => _note; set => _note = value; }
        #endregion

        public PinokioObject(uint id = 0)
        {
            this.Initialize();
            _id = id;
            _name = "";
        }

        public PinokioObject(uint id, Enum type)
        {
            this.Initialize();
            _id = id;
            _name = "";
            _type = type;
        }

        public PinokioObject(uint id, string name, Enum type = null)
        {
            this.Initialize();
            _id = id;
            _name = name;
            _type = type;
        }

        /// <summary>
        /// 각 Class의 Constructor가 호출되기 전에 호출되는 함수
        /// EX)
        ///     Origin Init 
        ///     Gen1 Init
        ///     Gen2 Init
        ///     Gen3 Init
        ///     Origin Called
        ///     Gen1 Called
        ///     Gen2 Called
        ///     Gen3 Called
        /// </summary>
        public virtual void Initialize()
        {
            return;
        }

        public override bool Equals(object obj)
        {
            if (obj is PinokioObject otherObj)
            {
                return (this.Id == otherObj.Id && this.Name == otherObj.Name);
            }
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
