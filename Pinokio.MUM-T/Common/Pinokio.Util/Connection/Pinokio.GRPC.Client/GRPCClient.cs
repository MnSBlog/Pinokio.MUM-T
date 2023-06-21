using System;
using System.Collections.Generic;
using Grpc.Core;
using System.Threading;
using System.IO;
using Pinokio.Socket;

namespace Pinokio.GRPC.Client
{
    public enum GRPCRequestType
    {
        Read,
        Write,
    }
    public class GRPCClient
    {
        #region Private Members
        private static bool _isConnected = false;
        #endregion

        #region Public Properties
        public static bool IsConnected
        {
            get { return _isConnected; }
        }
        public static bool TryConnect()
        {
            var input = new Request { AGVid = "1", CurrentNode = "", D51 = "", D52 = "", D53 = "", D55 = "", Speed = "" };
            var channel = new Channel("localhost", 7049, ChannelCredentials.Insecure);
            var client = new Greeter.GreeterClient(channel);

            var reply = client.GetAGVStatuses(input);
            if (reply != null)
                return true;
            else
                return false;
        }
        #endregion
        public static Dictionary<string, string> SendRequest(uint agvId, int portNumber, List<string> packet, GRPCRequestType requestType)
        {
            var channel = new Channel("localhost", portNumber, ChannelCredentials.Insecure);
            var client = new Greeter.GreeterClient(channel);
            switch (requestType)
            {
                case GRPCRequestType.Write:
                    var writeInput = new Request
                    {
                        AGVid = agvId.ToString(),
                        CurrentNode = packet[0],
                        D51 = packet[1],
                        D52 = packet[2],
                        D53 = packet[3],
                        D55 = packet[4],
                        Speed = packet[5]
                    };
                    var writeReply = client.GetAGVStatuses(writeInput);
                    return null;
                case GRPCRequestType.Read:
                    var input = new Request { AGVid = agvId.ToString(), CurrentNode = "", D51 = "", D52 = "", D53 = "", D55 = "", Speed = "" };
                    var reply = client.GetAGVStatuses(input);
                    Dictionary<string, string> agvStatuses = new Dictionary<string, string>();
                    agvStatuses["D5"] = reply.D5;
                    agvStatuses["D100"] = reply.D100;
                    agvStatuses["D101"] = reply.D101;
                    agvStatuses["D102"] = reply.D102;
                    agvStatuses["JobId"] = reply.JobId;
                    agvStatuses["Destination"] = reply.Destination;

                    return agvStatuses;
                default:
                    return null;
            }
        }
    }
}
