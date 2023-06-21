using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Core;

namespace Pinokio.Map
{
    public enum ChargerState
    {
        Error,
        Idle,
        Reserved,
        Charging,
    }

    public class AoCharger : AoResource
    {
        private ChargerState _state;
        public ChargerState State { get => _state; }

        public AoCharger(uint mapId, uint id, string name) : base(mapId, id, name, ResourceType.WaitingSpot)
        { }

        public override void Initialize()
        {
            base.Initialize();
            _state = ChargerState.Idle;
        }

        public void Reserve()
        {
            if (this.State is ChargerState.Idle)
            {
                LogHandler.AddLog(LogLevel.Info, $"Charger Id: {this.Id}, Reserved");
                _state = ChargerState.Reserved;
            }
            else
            {
                LogHandler.AddLog(LogLevel.Error, $"Charger Id: {this.Id}, Wrong State({this.State}), M(Reserve)");
            }
        }

        public void StartCharge()
        {
            if (this.State is ChargerState.Reserved)
            {
                LogHandler.AddLog(LogLevel.Info, $"Charger Id: {this.Id}, Start Charging");
                _state = ChargerState.Charging;
            }
            else
            {
                LogHandler.AddLog(LogLevel.Error, $"Charger Id: {this.Id}, Wrong State({this.State}), M(StartCharge)");
            }
        }

        public void StopCharge()
        {
            if (this.State is ChargerState.Charging)
            {
                LogHandler.AddLog(LogLevel.Info, $"Charger Id: {this.Id}, Stop Charging");
                _state = ChargerState.Reserved;
            }
            else
            {
                LogHandler.AddLog(LogLevel.Error, $"Charger Id: {this.Id}, Wrong State({this.State}), M(StartCharge)");
            }
        }

        public void Release()
        {
            if (this.State is ChargerState.Reserved)
            {
                LogHandler.AddLog(LogLevel.Info, $"Charger Id: {this.Id}, Released");
                _state = ChargerState.Idle;
            }
            else
             {
                LogHandler.AddLog(LogLevel.Error, $"Charger Id: {this.Id}, Wrong State({this.State}), M(Release)");
            }
        }
    }
}
