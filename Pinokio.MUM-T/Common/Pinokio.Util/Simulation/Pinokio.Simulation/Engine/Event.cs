using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;

namespace Pinokio.Simulation
{
    public class Event : AbstractObject
    {
        private SimTime _time;
        private SimModel _coreModel;
        private SimPort _port;

        public SimTime Time
        {
            get { return _time; }
        }
        public SimModel CoreModel
        {
            get { return _coreModel; }
        }
        public SimPort Port
        {
            get { return _port; }
        }

        public Event(uint id, SimTime time, SimModel model, SimPort port) : base(id)
        {
            _time = time;
            _coreModel = model;
            _port = port;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Event)
            {
                var otherEvent = obj as Event;
                if (otherEvent.Id == Id && otherEvent._time == _time && otherEvent._coreModel == _coreModel)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool ProcessingEvent()
        {
            _coreModel.InternalTransition(_time, _port);
            return true;
        }

        public override string ToString()
        {
            return $"{this.Time.TotalSeconds,-7} / Id:{Id.ToString()} / ({_port.Type.ToString()}) / {_coreModel.Name}";
        }
    }
}
