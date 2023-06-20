using System;
using System.Text;

namespace Pinokio.Socket
{
    public class PLCDevice
    {
        private string _name;
        private const int _length = 16;
        protected int[] Bits;

        public string Name { get => _name; }
        public PLCDevice(string name)
        {
            _name = name;
            Bits = new int[_length];
        }

        public PLCDevice(string name, uint value)
        {
            _name = name;
            Bits = new int[_length];
            this.SetInfo(value);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(uint))
            {
                uint value = Convert.ToUInt32(obj.ToString());
                string result = Convert.ToString(value, 2).PadLeft(16, '0');
                for (int i = 0; i < _length; i++)
                {
                    if (int.TryParse(result[i].ToString(), out int newBitCode))
                    {
                        if (Bits[_length - i - 1] != newBitCode)
                            return false;
                    }
                }
                return true;
            }
            else if (obj.GetType() == this.GetType())
            {
                var info = obj as PLCDevice;
                for (int i = 0; i < _length; i++)
                {
                    if (info.Bits[i] != this.Bits[i]) 
                        return false;
                }
                return true;
                    
            }
            else return false;
        }

        public static bool operator ==(PLCDevice device1, PLCDevice device2)
        {
            return device1.Equals(device2);
        }

        public static bool operator !=(PLCDevice device1, PLCDevice device2)
        {
            return !(device1 == device2);
        }

        public static bool operator ==(PLCDevice device, uint value)
        {
            return device.Equals(value);
        }

        public static bool operator !=(PLCDevice device, uint value)
        {
            return !(device == value);
        }

        public override int GetHashCode()
        {
            return Bits.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = _length; i > 0; i--)
            {
                sb.Append(Bits[i - 1]);
            }
            return sb.ToString();
        }

        public void SetInfo(uint value)
        {
            string result = Convert.ToString(value, 2).PadLeft(16, '0');
            for (int i = 0; i < _length; i++)
            {
                if(int.TryParse(result[i].ToString(), out int newBitCode))
                {
                    Bits[_length - i - 1] = newBitCode;
                }
            }
        }

        public int this[int key]
        {
            get => Bits[key];
            set => Bits[key] = value;

        }
    }
}

