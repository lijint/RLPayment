using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Landi.FrameWorks
{
    public class Message
    {
        public int what;
        public int arg1;
        public int arg2;
        public object obj;
        internal bool handled;
        internal AutoResetEvent waitEvent;
        internal long when;
        internal DateTime dateTime;
        internal Handler target;
        internal MethodInvoker callback;
        internal Message next;

        private static readonly object mPoolSync = new object();
        private static readonly int MAX_POOL_SIZE = 10;
        private static Message mPool;
        private static int mPoolSize = 0;

        private Message() { }
             
        public static Message Obtain()
        {
            lock (mPoolSync)
            {
                if (mPool != null && mPoolSize > 0)
                {
                    Message m = mPool;
                    mPool = m.next;
                    m.next = null;
                    mPoolSize--;
                    return m;
                }
            }
            return new Message();
        }

        public static Message Obtain(Message orig)
        {
            Message m = Obtain();
            m.what = orig.what;
            m.arg1 = orig.arg1;
            m.arg2 = orig.arg2;
            m.obj = orig.obj;
            m.target = orig.target;
            m.callback = orig.callback;

            return m;
        }

        public static Message Obtain(MethodInvoker callback)
        {
            Message m = Obtain();
            m.callback = callback;

            return m;
        }

        public static Message Obtain(int what)
        {
            Message m = Obtain();
            m.what = what;

            return m;
        }

        public static Message Obtain(int what, object obj)
        {
            Message m = Obtain();
            m.what = what;
            m.obj = obj;

            return m;
        }

        public static Message Obtain(int what, int arg1, int arg2)
        {
            Message m = Obtain();
            m.what = what;
            m.arg1 = arg1;
            m.arg2 = arg2;

            return m;
        }

        public static Message Obtain(int what,
                int arg1, int arg2, object obj)
        {
            Message m = Obtain();
            m.what = what;
            m.arg1 = arg1;
            m.arg2 = arg2;
            m.obj = obj;

            return m;
        }

        public void Recycle()
        {
            lock (mPoolSync)
            {
                if (mPoolSize < MAX_POOL_SIZE)
                {
                    mayWait();
                    clearForRecycle();
                    next = mPool;
                    mPool = this;
                    mPoolSize++;
                }
            }
        }

        private void clearForRecycle()
        {
            what = 0;
            arg1 = 0;
            arg2 = 0;
            obj = null;
            handled = false;
            waitEvent = null;
            when = 0;
            target = null;
            callback = null;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            b.Append("Message");

            b.Append("{ what=");
            b.Append(what);

            b.Append(" when=");
            b.Append(dateTime.ToLocalTime());

            if (arg1 != 0)
            {
                b.Append(" arg1=");
                b.Append(arg1);
            }

            if (arg2 != 0)
            {
                b.Append(" arg2=");
                b.Append(arg2);
            }

            if (obj != null)
            {
                b.Append(" obj=");
                b.Append(obj);
            }

            b.Append(" }");

            return b.ToString();
        }

        private void mayWait()
        {
            if (waitEvent != null)
            {
                handled = true;
                waitEvent.WaitOne(1000);
            }
        }
    }
}
