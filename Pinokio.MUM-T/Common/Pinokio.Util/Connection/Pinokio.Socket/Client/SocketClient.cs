using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Pinokio.Socket
{
    // .Net Socket 사용 TCP/IP 통신 구현
    public class SocketClient : Client
    {
        private TcpClient _clientSocket;
        private NetworkStream _stream;
        private Guid _clientGuid;
        private string _ip;

        public SocketClient(string serverIP)
        {
            _isConnected = false;
            _clientGuid = Guid.Empty;
            _ip = serverIP;
            GeneratePacket += delegate () { return null; };
            OnReceived += delegate (Packet packet) { };
        }

        public override void TryConnect()
        {
            _clientSocket = new TcpClient();
            _stream = default(NetworkStream);
            try
            {
                _clientSocket.Connect(_ip, 9999); // 접속 IP 및 포트
                _stream = _clientSocket.GetStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            _isConnected = true;

            this.Send("Connect");

            var t_handler = new Thread(Receive);
            t_handler.IsBackground = true;
            t_handler.Start();
        }
        public override void Disconnect()
        {
            Send("Disconnect");
        }

        public override void Send(string msg, bool cycle = false)
        {
            try
            {
                //_stream = _clientSocket.GetStream();
                BinaryWriter writer = new BinaryWriter(_stream);
                PLCDevicePacket p = (PLCDevicePacket)this.DoGeneratePacket();
                p.Message = msg;

                byte[] buffer = Packet.Serialize(p);
                writer.Write(buffer.Length);
                writer.Write(buffer);
                _stream.Flush();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void Receive(object onReceived)
        {
            string msg = string.Empty;
            BinaryReader reader = null;
            while (true)
            {
                try
                {
                    _stream = _clientSocket.GetStream();
                    reader = new BinaryReader(_stream);

                    int length = reader.ReadInt32();
                    byte[] data = reader.ReadBytes(length);
                    Packet packet = Packet.Deserialize(data);

                    DoOnReceived(packet);
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("{0}r\n{1}", e.StackTrace, e.Message));
                    _isConnected = false;
                    break;
                }
            }
        }
    }
}
