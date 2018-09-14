using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Landi.FrameWorks
{
    public class HandlerThread
    {
        private Looper mLooper;
        private Thread mThread;
        private static AutoResetEvent mEvent = new AutoResetEvent(false);
        private bool mRunning;

        public HandlerThread(string threadName)
        {
            mRunning = false;
            mEvent = new AutoResetEvent(false);
            mThread = new Thread(StartLoop);
            mThread.Name = threadName;
            mThread.IsBackground = true;
        }

        public void Run()
        {
            lock (this)
            {
                if (!mRunning)
                    mRunning = true;
                else
                    return;
            }
            mThread.Start();
            mEvent.WaitOne(-1);
        }

        private void StartLoop()
        {
            Looper.Prepare();
            lock (this)
            {
                mLooper = Looper.MyLooper();
                mEvent.Set();
            }
            Looper.Loop();
        }

        public Looper GetLooper()
        {
            if (!Thread.CurrentThread.IsAlive)
                return null;
            return mLooper;
        }

        public Thread GetThread()
        {
            return mThread;
        }

        public bool Quit()
        {
            lock (this)
            {
                if (mRunning)
                    mRunning = false;
                else
                    return true; ;
            }

            Looper looper = GetLooper();
            if (looper != null)
            {
                looper.Quit();
                return true;
            }
            return false;
        }
    }
}
