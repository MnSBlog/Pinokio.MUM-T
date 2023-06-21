using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using Pinokio.Socket;

namespace Pinokio.Socket
{
    public class SocketServer : Server
    {
        #region Memeber Variables
        private TcpListener _tcpListener; // 서버
        private Dictionary<uint, TcpClient> _tcpClients;
        private Dictionary<uint, HandleClient> _handles;
        #endregion

        #region Properties
        public Dictionary<uint, HandleClient> Handles { get { return _handles; } }
        #endregion

        public SocketServer() : base()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            _tcpListener = null;
            _tcpClients = new Dictionary<uint, TcpClient>();
            _handles = new Dictionary<uint, HandleClient>();
        }
        
        #region DotNet Socket
        public void SocketClose()
        {
            _tcpListener.Stop(); // 서버 종료
        }

        public override void Start()
        {
            _tcpListener = new TcpListener(IPAddress.Any, 9999); // 서버 접속 IP, 포트
            TcpClient clientSocket = default(TcpClient); // client 소켓 접속 허용
            _tcpListener.Start(); // 서버 시작
            NetworkStream stream = null;
            BinaryReader reader = null;
            Console.WriteLine(">> Server Started");

            while (true)
            {
                try
                {
                    clientSocket = this._tcpListener.AcceptTcpClient(); // client 소켓 접속 허용
                    Console.WriteLine(">> Accept connection from a simulation model");

                    stream = clientSocket.GetStream();
                    reader = new BinaryReader(stream);
                    int length = reader.ReadInt32();
                    byte[] data = reader.ReadBytes(length);
                    var packet = Packet.Deserialize(data) as PLCDevicePacket;

                    if (packet != null)
                    {
                        if (packet.Message == "Connect")
                        {
                            var id = packet.Id;
                            if (_tcpClients.ContainsKey(id))
                                _tcpClients.Remove(id);
                            _tcpClients.Add(id, clientSocket);

                            //this.ExecuteOnConnected(packet);

                            //--> Communication Manager
                            if (!_connectors.ContainsKey(id))
                            {
                                var newConn = new Connector(id, this);
                                newConn.Update(packet);
                                newConn.IsConnected = true;
                                _connectors.Add(id, newConn);
                                Console.WriteLine($"Device {id} connected");
                            }
                            else
                                _connectors[id].Reconnect();

                            var agvClient = new HandleClient(packet.Id); // 클라이언트 추가
                            agvClient.OnReceived += new HandleClient.MessageDisplayHandler(OnReceived);
                            agvClient.OnDisconnected += new HandleClient.DisconnectedHandler(OnDisconnected);
                            agvClient.StartClient(clientSocket);
                            _handles.Add(packet.Id, agvClient);
                        }
                    }
                }
                catch (SocketException se)
                {
                    Console.WriteLine(se.ToString());
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            }

            clientSocket.Close(); // client 소켓 닫기
            _tcpListener.Stop(); // 서버 종료
        }

        private void OnDisconnected(uint vehicleId) // cleint 접속 해제 핸들러
        {
            if (_tcpClients.ContainsKey(vehicleId))
            {
                _tcpClients.Remove(vehicleId);
                _connectors[vehicleId].Disconnect();

                Console.WriteLine("device {0} is disconnected", vehicleId);
                //Log.WriteLog(LOG_LEVEL.INFO, string.Format("client count = {0}", _server.GetClientList().Length));
            }
        }

        private void OnReceived(uint vehicleId, Packet packet) // cleint로 부터 받은 데이터
        {
            try
            {
                var dPacket = packet as PLCDevicePacket;
                if (dPacket != null)
                {
                    if (dPacket.Message == "Disconnect")
                    {
                        // disconnect
                        if (_tcpClients.ContainsKey(vehicleId))
                            _tcpClients.Remove(vehicleId);

                        if (_connectors.ContainsKey(vehicleId))
                            _connectors[vehicleId].Disconnect();
                        Console.WriteLine($"Devie {vehicleId} disconnected");
                    }
                    else
                    {
                        if (_connectors.ContainsKey(vehicleId))
                        {
                            _connectors[vehicleId].Update(dPacket);

                            //Console.WriteLine($"{sPacket.Time} : AGV {vehicleId} is on {sPacket.CurrentLink}_{sPacket.Offset}");
                        }
                        else
                            Console.WriteLine($"There is no device for id : {vehicleId}");
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void SendPacket(uint id, Packet packet)
        {
            TcpClient client = _tcpClients[id] as TcpClient;
            NetworkStream stream = client.GetStream();

            BinaryWriter writer = new BinaryWriter(stream);

            byte[] buffer = Packet.Serialize(packet);
            writer.Write(buffer.Length);
            writer.Write(buffer);
        }
        #endregion
    }



}