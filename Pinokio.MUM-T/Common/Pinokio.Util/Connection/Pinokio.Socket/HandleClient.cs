using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;

namespace Pinokio.Socket
{
    public class HandleClient
    {
        #region Member Variables
        private uint _id;
        private NetworkStream _stream;
        private TcpClient _clientSocket;
        #endregion

        #region Event & Delegate
        public delegate void MessageDisplayHandler(uint id, Packet packet);
        public event MessageDisplayHandler OnReceived;
        public delegate void DisconnectedHandler(uint id);
        public event DisconnectedHandler OnDisconnected;
        #endregion

        public HandleClient(uint id)
        {
            _id = id;
            _clientSocket = null;
        }

        public void StartClient(TcpClient clientSocket)
        {
            _clientSocket = clientSocket;
            //_stream = default(NetworkStream);

            var thread = new Thread(Communicate);
            //thread.IsBackground = true;
            thread.Start();
        }

        private void Communicate()
        {
            //NetworkStream stream = null;
            BinaryReader reader = null;
            try
            {
                byte[] buffer = new byte[1024];
                int length = 0;
                byte[] data = null;
                while (true)
                {
                    _stream = _clientSocket.GetStream();
                    reader = new BinaryReader(_stream);

                    length = reader.ReadInt32();
                    data = reader.ReadBytes(length);
                    Packet packet = Packet.Deserialize(data);

                    var pPacket = packet as PLCDevicePacket;
                    
                    if (OnReceived != null)
                        OnReceived(_id, packet);
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(string.Format("Communicate - SockBtException : {0}", se.Message));

                if (_clientSocket != null)
                {
                    if (OnDisconnected != null)
                        OnDisconnected(_id);

                    _clientSocket.Close();
                    _stream.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Communicate - Exception : {0}", ex.Message));

                if (_clientSocket != null)
                {
                    if (OnDisconnected != null)
                        OnDisconnected(_id);

                    _clientSocket.Close();
                    _stream.Close();
                }
            }
            finally
            {
                
            }
        }


    }
}
