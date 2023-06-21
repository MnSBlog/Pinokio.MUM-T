using System;
using System.IO;
using System.Threading;

namespace Pinokio.Socket
{
    public interface IHandler
    {
        void OnReceived(Packet packet);
    }
    public abstract class Client
    {
        protected bool _isConnected;
        public bool IsConnected { get { return _isConnected; } }
        public abstract void TryConnect();
        public abstract void Disconnect();
        public abstract void Send(string msg, bool cycle = false);
        public abstract void Receive(object onReceived);

        protected void DoOnReceived(Packet packet)
        {
            OnReceived?.Invoke(packet);
        }
        protected Packet DoGeneratePacket()
        {
            return GeneratePacket?.Invoke();
        }

        #region Event & Delegate
        public delegate Packet GeneratePacketHandler();
        public event GeneratePacketHandler GeneratePacket;

        public delegate void ReceiveHandler(Packet packet);
        public event ReceiveHandler OnReceived;
        #endregion
    }


}
