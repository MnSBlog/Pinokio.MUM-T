using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

using Pinokio.Core;

namespace Pinokio.Simulation
{
    public class SimEngine : AbstractObject
    {
        #region Variables
        private SimTime _tNow;
        private EventCalendar _evtCalendar;
        private SimModel[] _models;

        private Thread _engineThread;
        private EngineState _state;
        private EndCondition _endCondition;

        private List<SimTimer> _timers;

        #region [:: Engine Event Handle]
        public delegate void EngineEventHandler();
        public event EngineEventHandler OnEngineStart = delegate () { };
        public event EngineEventHandler OnEngineFinish = delegate () { };
        public event EngineEventHandler OnCycleStart = delegate () { };
        public event EngineEventHandler OnCycleFinish = delegate () { };
        public event EngineEventHandler OnStateChanged = delegate () { };
        #endregion [Engine Event Handle ::]
        #endregion

        #region Properties
        public SimTime TimeNow { get => _tNow; }
        public int ModelCount { get => _models.Length; }
        public EngineState State { get => _state; }
        public EventCalendar EvtCalendar { get => _evtCalendar; }
        #endregion

        public SimEngine() : base(0)
        {}

        public override void Initialize()
        {
            _tNow = new SimTime(0);
            _evtCalendar = new EventCalendar();
            _models = new SimModel[] { };

            _engineThread = null;
            _state = EngineState.Ready;
            //_endCondition = EndCondition.None;
            _endCondition = EndCondition.Time;

            _timers = new List<SimTimer>();
        }

        public virtual void InitializeModels(List<SimModel> models)
        {
            _models = models.ToArray();
            for (int i = 0; i < _models.Length; i++)
            {
                _models[i].InitializeModel(EvtCalendar);
            }
        }

        #region Get, Set Method
        public void SetTNow(SimTime time)
        {
            _tNow = time;
        }
        #endregion

        #region Engine Control Functions
        public Stopwatch sw = new Stopwatch();
        public DateTime startTime = DateTime.MinValue;
        public DateTime endTime = DateTime.MaxValue;
        public virtual bool Run()
        {
            try
            {
                if (_state == EngineState.Ready)
                {
                    sw.Start();
                    startTime = DateTime.Now;
                    _state = EngineState.Running;
                    OnStateChanged();
                    _engineThread = new Thread(Main);
                    _engineThread.IsBackground = true;
                    _engineThread.Start();
                    return true;
                }
                else
                {
                    LogHandler.AddLog(LogLevel.Warn, "Simulation Engine is not ready");
                    return false;
                }
            }
            catch(Exception e)
            {
                LogHandler.AddLog(LogLevel.Error, e.Message);
                return false;
            }
        }

        public virtual bool Pause()
        {
            if (_state == EngineState.Running)
            {
                _state = EngineState.Pause;
                OnStateChanged();
                return true;
            }
            else
            {
                LogHandler.AddLog(LogLevel.Warn, "Simulation Engine is not running");
                return false;
            }
        }

        public virtual bool Resume()
        {
            if (_state == EngineState.Pause)
            {
                _state = EngineState.Running;
                OnStateChanged();
                return true;
            }
            else
            {
                LogHandler.AddLog(LogLevel.Warn, "Simulation Engine is paused");
                return false;
            }
        }

        public virtual bool Stop()
        {
            if (_state == EngineState.Stop)
            {
                LogHandler.AddLog(LogLevel.Warn, "Simulation Engine is not running");
                return false;
            }
            else
            {
                _state = EngineState.Stop;
                OnStateChanged();
                return true;
            }
        }

        public virtual void Reset()
        {
            this.Initialize();
            _state = EngineState.Ready;
            OnStateChanged();
        }
        #endregion Engine Control Functions End

        #region Engine Status
        public virtual bool CheckEndCondition()
        {
            switch (_endCondition)
            {
                case EndCondition.Time:
                    if (SimParameter.EndOfSimulation > 0 &&
                        _tNow > SimParameter.EndOfSimulation + SimParameter.WarmUpPeriod)
                    {
                        _state = EngineState.End;
                        return true;
                    }
                    break;
                case EndCondition.None:
                default:
                    break;
            }
            return false;
        }

        public void AddTimer(SimTimer newTimer)
        {
            foreach (var timer in _timers)
            {
                if (timer.Tick == newTimer.Tick)
                {
                    var delegates = newTimer.GetAllDelegates();
                    foreach (var d in delegates)
                    {
                        timer.OnTimer += (SimTimer.TimerEventHandler)d;
                    }
                    return;
                }
            }
            _timers.Add(newTimer);
        }

        public void RemoveTimer(SimTimer timer)
        {
            for (int i = 0; i < _timers.Count; i++)
            {
                
            }
        }
        #endregion Engine Status End

        public void Main()
        {
            OnEngineStart();
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            while (true)
            {
                if (_state == EngineState.Running)
                {
                    OnCycleStart();
                    EngineCore();
                    OnCycleFinish();

                    foreach (var timer in _timers)
                    {
                        timer.Check(_tNow);
                    }

                    if (CheckEndCondition())
                    {
                        sw.Stop();
                        endTime = DateTime.Now;
                        break;
                    }
                }
                else if (_state == EngineState.Pause)
                    Thread.Sleep(50);
                else
                    break;
            }
            sw.Stop();

            var 배속 = _tNow.TotalSeconds * 1000 / sw.ElapsedMilliseconds;
            Console.WriteLine($"Total {_tNow.TotalSeconds * 1000}");
            Console.WriteLine($"Elapsed {sw.ElapsedMilliseconds}");
            Console.WriteLine($"베속 {배속}");
            OnEngineFinish();
        }

        protected virtual void EngineCore() // 수정
        {
            // Event 처리
            Event nextEvt = EvtCalendar.GetNextEvent();
            if (nextEvt != null)
            {
                _tNow = nextEvt.Time;
                nextEvt.ProcessingEvent();
            }
        }
    }
}
