using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Landi.FrameWorks
{
    public class Looper
    {
        [ThreadStatic]
        private static Looper myLooper = null;

        private MessageQueue mQueue;
        public MessageQueue Queue
        {
            get { return mQueue; }
        }
        private volatile bool mRun;
        private Thread mThread;
        public Thread Thread
        {
            get { return mThread; }
        }

        public static void Prepare()
        {
            if (myLooper != null)
            {
                Log.Debug("Only one Looper may be created per thread");
                throw new Exception("Only one Looper may be created per thread");
            }
            myLooper = new Looper();
        }

        public static Looper MyLooper()
        {
            return myLooper;
        }

        private Looper()
        {
            mQueue = new MessageQueue();
            mRun = true;
            mThread = Thread.CurrentThread;
        }

        public void dump(string prefix)
        {
            StringBuilder sb = new StringBuilder();
            Log.Debug(prefix + this);
            Log.Debug(prefix + "mRun=" + mRun);
            Log.Debug(prefix + "mThread=" + mThread);
            Log.Debug(prefix + "mQueue=" + ((mQueue != null) ? mQueue.ToString() : "(null"));
            if (mQueue != null)
            {
                lock (mQueue)
                {
                    Message msg = mQueue.mMessages;
                    int n = 0;
                    while (msg != null)
                    {
                        Log.Debug(prefix + "  Message " + n + ": " + msg.ToString());
                        n++;
                        msg = msg.next;
                    }
                    Log.Debug(prefix + "(Total messages: " + n + ")");
                }
            }
        }

        public static MessageQueue MyQueue()
        {
            return MyLooper().mQueue;
        }

        public static void Loop()
        {
            Looper me = MyLooper();
            MessageQueue queue = me.mQueue;
            while (true)
            {
                Message msg = queue.Next(); // might block
                if (msg != null)
                {
                    if (msg.target == null)
                    {
                        return;
                    }
                    msg.target.DispatchMessage(msg);
                    msg.Recycle();
                }
            }

        }

        public void Quit()
        {
            // NOTE: By enqueueing directly into the message queue, the
            // message is left with a null target.  This is how we know it is
            // a quit message.
            mQueue.EnqueueMessage(Message.Obtain(), 0);
        }

        public override string ToString()
        {
            return "Looper{"
                + this.GetHashCode()
                + "}";
        }

    }
}
