using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinokio.Socket
{
    public abstract class Server
    {
        private Guid _serverGuid;
        protected Dictionary<uint, Connector>  _connectors;
        public Dictionary<uint, Connector> Connectors { get { return _connectors; } }
        public Server()
        {
            _serverGuid = Guid.Empty;
            _connectors = new Dictionary<uint, Connector>();
            this.Initialize();
        }

        public virtual void Initialize()
        {
            _connectors.Clear();
        }

        public abstract void Start();

        public abstract void SendPacket(uint id, Packet packet);

        public void ExecuteOnConnected(Packet paket)
        {
            this.OnConnected(paket);
        }

        #region Event & Delegate
        public delegate void ConnectHandler(Packet paket);
        public event ConnectHandler OnConnected;
        #endregion
    }
}
