using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
//using ActUtlTypeLib;

namespace Pinokio.Socket
{
    //public class Connector : ActMLUtlType
    public class Connector
    {
        private object _lockObject = new object();
        private uint _id;
        private bool _isOpened;
        public double LastReportTime { get; set; }
        private List<PLCDevicePacket> _sendPackets; 
        private Dictionary<string, object> _devices;

        private object D0;
        private object D1;
        private object D2;
        private object D5;
        private object D6;
        private object D100;
        private object D101;
        private object D102;

        private object Speed;
        private object CurrentNode;
        private object D50;
        private object D51;
        private object D52;
        private object D53;
        private object D55;
        private object D56;
        private object D57;
        private object D58;
        private object D59;
        private object D60;

        private object MOVE;
        private object MOVE_NEXT;

        private object Busy;
        private object JobId;
        private object Destination;
        private object Time;

        private bool _isConnected;
        private Server _server;
        public bool IsConnected { get { return _isConnected; } set { _isConnected = value; } }
        public dynamic ActLogicalStationNumber { get; set; }
        public Connector(uint id, Server server)
        {
            _id = id;
            _server = server;
            Initialize();
        }

        private void Initialize()
        {
            _isOpened = false;
            _isConnected = false;
            _devices = new Dictionary<string, object>();

            // ACS Write
            D0 = "0";
            D1 = "0";
            D2 = "0";
            D5 = "0";
            D6 = "0";
            D100 = "";
            D101 = "0";
            D102 = "0";
            MOVE = "";
            MOVE_NEXT = "";
            Busy = "0";
            JobId = "";
            Destination = "";
            

            // ACS Read
            Speed = "0";
            D50 = "0";
            D51 = "0";
            D52 = "0";
            D53 = "0";
            D55 = "0";
            D56 = "0";
            D57 = "0";
            D58 = "0";
            D59 = "0";
            D60 = "0";
            CurrentNode = "0";
        }

        public dynamic Open()
        {
            if (_isConnected)
            {// success : 0
                _isOpened = true;
                return 0;
            }
            else
            {// fail : 1
                _isOpened = false;
                return 1;
            }
        }

        public dynamic Close()
        {
            _isOpened = false;
            return 0;
        }

        public dynamic SetDevice(object varDevice, object varData)
        {
            if (_isOpened)
            {
                switch (varDevice.ToString())
                {
                    case "D0":
                        D0 = varData;
                        break;
                    case "D1":
                        D1 = varData;
                        break;
                    case "D2":
                        D2 = varData;
                        break;
                    case "D5":
                        D5 = varData;
                        break;
                    case "D6":
                        D6 = varData;
                        break;
                    case "D100":
                        D100 = varData;
                        break;
                    case "D101":
                        D101 = varData;
                        break;
                    case "D102":
                        D102 = varData;
                        break;
                    case "MOVE":
                        MOVE = varData;
                        break;
                    case "MOVE_NEXT":
                        MOVE_NEXT = varData;
                        break;
                    case "Busy":
                        Busy = varData;
                        break;
                    case "JobId":
                        JobId = varData;
                        break;
                    case "Destination":
                        Destination = varData;
                        break;
                }

                // SendPacket
                PLCDevicePacket packet = GeneratePacket();
                _server.SendPacket(_id, packet);
                return 0;
            }
            return 1;
        }

        public dynamic GetDevice(object varDevice, out object lpvarData)
        {
            if (_isOpened)
            {
                lpvarData = new object();
                switch (varDevice.ToString())
                {
                    case "Speed":
                        lpvarData = Speed;
                        break;
                    case "D5":
                        lpvarData = D5;
                        break;
                    case "D50":
                        lpvarData = D50;
                        break;
                    case "D51":
                        lpvarData = D51;
                        break;
                    case "D52":
                        lpvarData = D52;
                        break;
                    case "D53":
                        lpvarData = D53;
                        break;
                    case "D55":
                        lpvarData = D55;
                        break;
                    case "D56":
                        lpvarData = D56;
                        break;
                    case "D57":
                        lpvarData = D57;
                        break;
                    case "D58":
                        lpvarData = D58;
                        break;
                    case "D59":
                        lpvarData = D59;
                        break;
                    case "D60":
                        lpvarData = D60;
                        break;
                    case "CUR_NODE":
                        lpvarData = CurrentNode;
                        break;
                    case "Time":
                        lpvarData = Time;
                        break;
                }
                return 0;
            }
            else
            {
                lpvarData = null;
                return 1;
            }
        }

        public void Disconnect()
        {
            _isConnected = false;
        }

        public void Reconnect()
        {
            _isConnected = true;
        }

        public void Update(PLCDevicePacket packet)
        {
            Speed = packet.Speed;
            CurrentNode = packet.CurrentNode;
            D50 = packet.D50;
            D51 = packet.D51;
            D52 = packet.D52;
            D53 = packet.D53;
            D55 = packet.D55;
            D56 = packet.D56;
            D57 = packet.D57;
            D58 = packet.D58;
            D59 = packet.D59;
            D60 = packet.D60;
            Time = packet.Time;
        }

        #region For Simulation
        private PLCDevicePacket GeneratePacket()
        {
            PLCDevicePacket packet = new PLCDevicePacket()
            {
                D0 = this.D0.ToString(),
                D1 = this.D1.ToString(),
                D2 = this.D2.ToString(),
                D5 = this.D5.ToString(),
                D6 = this.D6.ToString(),
                D100 = this.D100.ToString(),
                D101 = this.D101.ToString(),
                D102 = this.D102.ToString(),
                MOVE = this.MOVE.ToString(),
                MOVE_NEXT = this.MOVE_NEXT.ToString(),
                Busy = this.Busy.ToString(),
                JobId = this.JobId.ToString(),
                Destination = this.Destination.ToString()
            };

            return packet;
        }
        #endregion

        #region [Not Implemented]
        public dynamic ReadDeviceBlock(object varDevice, object varSize, out object lpvarData)
        {
            throw new NotImplementedException();
        }

        public dynamic WriteDeviceBlock(object varDevice, object varSize, object varData)
        {
            throw new NotImplementedException();
        }

        public dynamic ReadDeviceRandom(object varDeviceList, object varSize, out object lpvarData)
        {
            throw new NotImplementedException();
        }

        public dynamic WriteDeviceRandom(object varDeviceList, object varSize, object varData)
        {
            throw new NotImplementedException();
        }

        public dynamic ReadBuffer(object varStartIO, object varAddress, object varReadSize, out object lpvarData)
        {
            throw new NotImplementedException();
        }

        public dynamic WriteBuffer(object varStartIO, object varAddress, object varWriteSize, object varData)
        {
            throw new NotImplementedException();
        }

        public dynamic GetCpuType(out object lpvarCpuName, out object lpvarCpuCode)
        {
            throw new NotImplementedException();
        }

        public dynamic SetCpuStatus(object varOperation)
        {
            throw new NotImplementedException();
        }

        public dynamic GetClockData(out object lpvarYear, out object lpvarMonth, out object lpvarDay, out object lpvarDayOfWeek, out object lpvarHour, out object lpvarMinute, out object lpvarSecond)
        {
            throw new NotImplementedException();
        }

        public dynamic SetClockData(object varYear, object varMonth, object varDay, object varDayOfWeek, object varHour, object varMinute, object varSecond)
        {
            throw new NotImplementedException();
        }

        public dynamic EntryDeviceStatus(object varDeviceList, object varSize, object varMonitorCycle, object varData)
        {
            throw new NotImplementedException();
        }

        public dynamic FreeDeviceStatus()
        {
            throw new NotImplementedException();
        }

        public dynamic ReadDeviceBlock2(object varDevice, object varSize, out object lpvarData)
        {
            throw new NotImplementedException();
        }

        public dynamic WriteDeviceBlock2(object varDevice, object varSize, object varData)
        {
            throw new NotImplementedException();
        }

        public dynamic ReadDeviceRandom2(object varDeviceList, object varSize, out object lpvarData)
        {
            throw new NotImplementedException();
        }

        public dynamic WriteDeviceRandom2(object varDeviceList, object varSize, object varData)
        {
            throw new NotImplementedException();
        }

        public dynamic GetDevice2(object varDevice, out object lpvarData)
        {
            throw new NotImplementedException();
        }

        public dynamic SetDevice2(object varDevice, object varData)
        {
            throw new NotImplementedException();
        }

        public dynamic ActPassword { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //public event _IActMLUtlTypeEvents_OnDeviceStatusEventHandler OnDeviceStatus;
        #endregion
    }
}