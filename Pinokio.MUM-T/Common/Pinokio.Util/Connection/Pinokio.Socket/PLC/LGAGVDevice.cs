using System;
using System.Text;

namespace Pinokio.Socket
{
    public class D5 : PLCDevice
    {
        public int Pause
        {
            get { return Bits[15]; }
            set { Bits[15] = value; }
        }
        public int Reset
        {
            get { return Bits[14]; }
            set { Bits[14] = value; }
        }
        public int PathClear
        {
            get { return Bits[13]; }
            set { Bits[13] = value; }
        }
        public int RebootCheck
        {
            get { return Bits[11]; }
            set { Bits[11] = value; }
        }
        public int ChargingStop
        {
            get { return Bits[10]; }
            set { Bits[10] = value; }
        }
        public int HybernationStop
        {
            get { return Bits[9]; }
            set { Bits[9] = value; }
        }
        public int CompleteAck
        {
            get { return Bits[6]; }
            set { Bits[6] = value; }
        }
        public int RequestAck
        {
            get { return Bits[5]; }
            set { Bits[5] = value; }
        }
        public int JobReqeustAck
        {
            get { return Bits[4]; }
            set { Bits[4] = value; }
        }

        public D5() : base("D5") { }
        public D5(uint value) : base("D5", value) { }
    }

    public class D51 : PLCDevice
    {
        public int IsAuto
        {
            get { return Bits[15]; }
            set { Bits[15] = value; }
        }
        public int IsRemote
        {
            get { return Bits[14]; }
            set { Bits[14] = value; }
        }
        public int IsRouteEnd
        {
            get { return Bits[13]; }
            set { Bits[13] = value; }
        }
        public int OnThePackage
        {
            get { return Bits[12]; }
            set { Bits[12] = value; }
        }
        public int HaveOP
        {
            get { return Bits[11]; }
            set { Bits[11] = value; }
        }
        public int Complete
        {
            get { return Bits[10]; }
            set { Bits[10] = value; }
        }
        public int ForgotLocation
        {
            get { return Bits[9]; }
            set { Bits[9] = value; }
        }
        public int Reboot
        {
            get { return Bits[8]; }
            set { Bits[8] = value; }
        }
        public int PathError
        {
            get { return Bits[7]; }
            set { Bits[7] = value; }
        }
        public int IsInMove
        {
            get { return Bits[6]; }
            set { Bits[6] = value; }
        }
        public int IsInLoading
        {
            get { return Bits[5]; }
            set { Bits[5] = value; }
        }
        public int IsInUnloading
        {
            get { return Bits[4]; }
            set { Bits[4] = value; }
        }
        public int IsInCharging
        {
            get { return Bits[3]; }
            set { Bits[3] = value; }
        }
        public int IsInHybernation
        {
            get { return Bits[2]; }
            set { Bits[2] = value; }
        }
        public int RebootComplete
        {
            get { return Bits[1]; }
            set { Bits[1] = value; }
        }
        public int RiseAlarm
        {
            get { return Bits[0]; }
            set { Bits[0] = value; }
        }

        public D51() : base("D51") { }
        public D51(uint value) : base("D51", value) { }
    }

    public class D52 : PLCDevice
    {
        public int CommError
        {
            get { return Bits[15]; }
            set { Bits[15] = value; }
        }
        public int Emergency
        {
            get { return Bits[14]; }
            set { Bits[14] = value; }
        }
        public int Frontobstacle
        {
            get { return Bits[13]; }
            set { Bits[13] = value; }
        }
        public int Backobstacle
        {
            get { return Bits[12]; }
            set { Bits[12] = value; }
        }
        public int BumperStop
        {
            get { return Bits[11]; }
            set { Bits[11] = value; }
        }
        public int PreOPStop
        {
            get { return Bits[10]; }
            set { Bits[10] = value; }
        }
        public int InOP
        {
            get { return Bits[9]; }
            set { Bits[9] = value; }
        }
        public int ETCStop
        {
            get { return Bits[8]; }
            set { Bits[8] = value; }
        }
        public int LowVoltage
        {
            get { return Bits[7]; }
            set { Bits[7] = value; }
        }
        public int BlockFreeRequest
        {
            get { return Bits[6]; }
            set { Bits[6] = value; }
        }
        public int ChargingFail
        {
            get { return Bits[5]; }
            set { Bits[5] = value; }
        }
        public int RequestDelete
        {
            get { return Bits[4]; }
            set { Bits[4] = value; }
        }
        public int RequestChargingStop
        {
            get { return Bits[3]; }
            set { Bits[3] = value; }
        }
        public int RequestComplet
        {
            get { return Bits[2]; }
            set { Bits[2] = value; }
        }
        public int RequestJob
        {
            get { return Bits[1]; }
            set { Bits[1] = value; }
        }
        public int NeedCharging
        {
            get { return Bits[0]; }
            set { Bits[0] = value; }
        }

        public D52() : base("D52") { }
        public D52(uint value) : base("D52", value) { }
    }

    public class D53 : PLCDevice
    {
        public int Goods1
        {
            get { return Bits[15]; }
            set { Bits[15] = value; }
        }
        public int Goods2
        {
            get { return Bits[14]; }
            set { Bits[14] = value; }
        }
        public int InProgressPIO
        {
            get { return Bits[13]; }
            set { Bits[13] = value; }
        }
        public int CompletPIO
        {
            get { return Bits[12]; }
            set { Bits[12] = value; }
        }
        public int ErrorPIO
        {
            get { return Bits[11]; }
            set { Bits[11] = value; }
        }
        public int UpSystem_ResetPossible
        {
            get { return Bits[10]; }
            set { Bits[10] = value; }
        }
        public int PortCenterPosition
        {
            get { return Bits[9]; }
            set { Bits[9] = value; }
        }
        public int LiftCenterPosition
        {
            get { return Bits[8]; }
            set { Bits[8] = value; }
        }
        public int ClearAgvInfomation
        {
            get { return Bits[7]; }
            set { Bits[7] = value; }
        }

        public D53() : base("D53") { }
        public D53(uint value) : base("D53", value) { }
    }
}

