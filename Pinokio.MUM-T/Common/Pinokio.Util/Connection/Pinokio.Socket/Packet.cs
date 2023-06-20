using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pinokio.Socket
{
    [Serializable]
    public class Packet
    {
        public uint Id { get; set; }
        public int PacketType { get; set; }
        public int PacketLength { get; set; }
        public string Message { get; set; }

        public Packet()
        {
            this.PacketType = 0;
            this.PacketLength = 0;
        }

        public static Packet Deserialize(byte[] data)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(data);

                // To prevent errors serializing between version number differences(e.g. version 1 serializes, and version 2 deserializes)
                formatter.Binder = new DesrializationBinder();

                // Allow the exceptions to bubble up
                // System.ArgumentNullException
                // System.Runtime.Serialization/SerializationException
                // System.Security.SecurityException
                Packet packet = formatter.Deserialize(ms) as Packet;
                ms.Close();

                return packet;
            }
            catch
            {
                return null;
            }

        }

        public static byte[] Serialize(object obj)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }
    }

    [Serializable]
    public class PLCDevicePacket : Packet
    {

        public PLCDevicePacket()
        { }

        public PLCDevicePacket(PLCDevicePacket otherPacket)
        {
            Speed = otherPacket.Speed;
            D0 = otherPacket.D0;
            D1 = otherPacket.D1;
            D2 = otherPacket.D2;
            D5 = otherPacket.D5;
            D6 = otherPacket.D6;
            D100 = otherPacket.D100;
            D101 = otherPacket.D101;
            D102 = otherPacket.D102;
            MOVE = otherPacket.MOVE;
            MOVE_NEXT = otherPacket.MOVE_NEXT;
            Busy = otherPacket.Busy;
            D50 = otherPacket.D50;
            D51 = otherPacket.D51;
            D52 = otherPacket.D52;
            D53 = otherPacket.D53;
            D54 = otherPacket.D54;
            D55 = otherPacket.D55;
            D56 = otherPacket.D56;
            D57 = otherPacket.D57;
            D58 = otherPacket.D58;
            D59 = otherPacket.D59;
            D60 = otherPacket.D60;
            CurrentNode = otherPacket.CurrentNode;
            JobId = otherPacket.JobId;
            Time = otherPacket.Time;
        }
        public double Speed { get; set; }
        
        // ACS Write
        public string D0 { get; set; }
        public string D1 { get; set; }
        public string D2 { get; set; }
        public string D5 { get; set; }
        public string D6 { get; set; }
        public string D100 { get; set; }
        public string D101 { get; set; }
        public string D102 { get; set; }
        public string MOVE { get; set; }
        public string MOVE_NEXT { get; set; }
        
        public string Busy { get; set; }
        // ACS Read
        public string D50 { get; set; } // Not Implemented
        public string D51 { get; set; }
        public string D52 { get; set; }
        public string D53 { get; set; }
        public string D54 { get; set; } // Not Implemented
        public string D55 { get; set; } // Current Location(CurrentLink)
        public string D56 { get; set; } // Not Implemented
        public string D57 { get; set; } // Not Implemented
        public string D58 { get; set; } // Not Implemented
        public string D59 { get; set; } // Not Implemented
        public string D60 { get; set; } // Not Implemented
        public string CurrentNode { get; set; }
        public string JobId { get; set; }
        public string Destination { get; set; }
        public string Time { get; set; }
    }

    public sealed class DesrializationBinder : System.Runtime.Serialization.SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;
            string currentlyAseembly = Assembly.GetExecutingAssembly().FullName;
            assemblyName = currentlyAseembly;

            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

            return typeToDeserialize;
        }
    }
}
