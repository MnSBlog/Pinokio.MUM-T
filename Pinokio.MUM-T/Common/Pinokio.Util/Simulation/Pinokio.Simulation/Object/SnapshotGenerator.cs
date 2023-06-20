using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Pinokio.Simulation
{
    public enum SnapshotIntPort
    {
        None = 0,
        Shot = 1,
    }

    public class SnapshotGenerator : SimModel
    {
        private static double tick = 86400;
        public static double Tick { get => tick; }

        #region [Event Handlers]
        public SimModelEventHandler OnShot = null;
        #endregion [Event Handlers End]
        public SnapshotGenerator(uint id, string name) : base(id, name, ModelType.Snapshot)
        { }

        #region [Simulation Methods]
        public override void InitializeModel(EventCalendar eventCalendar)
        {
            base.InitializeModel(eventCalendar);
            EvtCalendar.AddEvent(SnapshotGenerator.Tick, this, new SimPort(SnapshotIntPort.Shot));
        }

        public override void InternalTransition(SimTime timeNow, SimPort port)
        {
            switch ((SnapshotIntPort)port.Type)
            {
                case SnapshotIntPort.Shot:
                    var dataTables = GenerateSnapshot(timeNow);

                    if (OnShot != null) OnShot(timeNow, this, port, dataTables);
                    EvtCalendar.AddEvent(timeNow + Tick, this, port);
                    break;
            }
        }
        #endregion [Simulation Methods End]

        protected virtual DataSet GenerateSnapshot(SimTime timeNow)
        {
            return null;
        }
    }


}
