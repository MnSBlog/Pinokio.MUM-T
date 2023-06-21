using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;
using Pinokio.Geometry;

namespace Pinokio.Simulation
{
    public class SimModel : SimObject
    {
        #region [Variables]
        protected EventCalendar EvtCalendar;
        private SimTime _lastUpdateTime;
        private List<SimEntity> _entities;
        #endregion [Variables End]

        #region [Properties]
        public List<SimEntity> Entities { get => _entities; }
        public SimTime LastUpdateTime { get => _lastUpdateTime; }
        #endregion [Properties End]

        #region [Event Handler]
        public delegate void SimModelEventHandler(SimTime timeNow, SimModel model, SimPort port, object obj = null);
        #endregion [Event Handler End]

        public SimModel(uint id, string name, Enum type = null) : base(id, name, type)
        {
            _entities = new List<SimEntity>();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public virtual void InitializeModel(EventCalendar eventCalendar)
        {
            EvtCalendar = eventCalendar;
        }

        public virtual void InternalTransition(SimTime timeNow, SimPort port)
        {
        }

        public virtual void ExternalTransition(SimTime timeNow, SimPort port)
        {
        }

        public virtual void SetState(SimTime timeNow, Enum state)
        {
            _lastUpdateTime = timeNow;
            St = state;
        }

        public virtual void SetPosition(Vector3 pos)
        {
            Pos = pos;
        }

        public virtual void SetDirection(Vector3 direction)
        {
            Dir = direction;
        }
    }
}
