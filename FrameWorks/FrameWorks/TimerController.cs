using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Landi.FrameWorks
{
    public class TimerController
    {
        private Dictionary<string, TimerWrapper> timers;

        public TimerController()
        {
            timers = new Dictionary<string, TimerWrapper>();
        }

        public void AddTimerWrapper(string name, int outTime, int interval, TimerWrapperEventHandler tickHandler, TimerWrapperEventHandler timeOutHandler)
        {
            if (!timers.ContainsKey(name))
            {
                timers.Add(name, new TimerWrapper(outTime, interval, tickHandler, timeOutHandler));
            }
        }

        public void AddTimerWrapper(string name, int interval, TimerWrapperEventHandler tickHandler)
        {
            if (!timers.ContainsKey(name))
            {
                timers.Add(name, new TimerWrapper(interval, tickHandler));
            }
        }

        public void DelTimerWrapper(string name)
        {
            if (timers.ContainsKey(name))
            {
                timers[name].Stop();
                timers.Remove(name);
            }
        }

        public void Start(string name)
        {
            if (timers.ContainsKey(name))
                timers[name].Start();
        }

        public void Stop(string name)
        {
            if (timers.ContainsKey(name))
                timers[name].Stop();
        }

        public void SetOutTime(string name,int outTime)
        {
            if (timers.ContainsKey(name))
                timers[name].OutTime = outTime;
        }
    }

    public class TimerWrapperEventArgs : EventArgs
    {
        public TimerWrapperEventArgs(int curCount)
            : base()
        {
            CurrentCount = curCount;
            OutCount = 0;
        }

        public TimerWrapperEventArgs(int curCount,int outCount)
            : this(curCount)
        {
            OutCount = outCount;
        }

        public int CurrentCount
        {
            get { return currentCount; }
            private set { currentCount = value; }
        }

        private int currentCount;

        public int OutCount
        {
            get { return outCount; }
            private set { outCount = value; }
        }

        private int outCount;
    }

    public delegate void TimerWrapperEventHandler(object sender, TimerWrapperEventArgs args);

    public class TimerWrapper
    {
        private Timer timer;
        private int currentCount;
        public int CurrentCount
        {
            set { currentCount = value; }
            get { return currentCount; }
        }

        public event TimerWrapperEventHandler TimeTick;
        public event TimerWrapperEventHandler TimeOut;

        private int outTime;
        public int OutTime
        {
            set { outTime = value; }
            get { return outTime; }
        }

        private int interval;
        public int Interval
        {
            set { if (!timer.Enabled) interval = value; }
            private get { return interval; }
        }

        public TimerWrapper()
        {
            timer = new Timer();
            Interval = 1;
            timer.Interval = Interval * 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            CurrentCount = 0;
            OutTime = 0;
        }

        public TimerWrapper(int outTime, int interval, TimerWrapperEventHandler tickHandler, TimerWrapperEventHandler timeOutHandler)
            : this()
        {
            OutTime = outTime;
            Interval = interval;
            timer.Interval = Interval * 1000;
            TimeTick += tickHandler;
            TimeOut += timeOutHandler;
        }

        public TimerWrapper(int interval, TimerWrapperEventHandler tickHandler)
            : this()
        {
            Interval = interval;
            timer.Interval = Interval * 1000;
            TimeTick += tickHandler;
        }

        public void Start()
        {
            if (!timer.Enabled)
                timer.Start();
        }

        public void Stop()
        {
            CurrentCount = 0;
            if (timer.Enabled)
                timer.Stop();
        }

        /// <summary>
        /// ΩÁ√Ê√Î±Ì
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CurrentCount++;
            if (OutTime <= 0)
                OnTimeTick(this, new TimerWrapperEventArgs(CurrentCount, OutTime));
            else
            {
                if (CurrentCount >= OutTime)
                {
                    Stop();
                    OnTimeTick(this, new TimerWrapperEventArgs(CurrentCount, OutTime)); 
                    OnTimeOut(this, new TimerWrapperEventArgs(CurrentCount, OutTime));
                }
                else
                {
                    OnTimeTick(this, new TimerWrapperEventArgs(CurrentCount, OutTime));
                }
            }
        }

        private void OnTimeOut(object sender, TimerWrapperEventArgs e)
        {
            if (TimeOut != null)
                TimeOut(sender, e);
        }

        private void OnTimeTick(object sender, TimerWrapperEventArgs e)
        {
            if (TimeTick != null)
                TimeTick(sender, e);
        }

    }

}
