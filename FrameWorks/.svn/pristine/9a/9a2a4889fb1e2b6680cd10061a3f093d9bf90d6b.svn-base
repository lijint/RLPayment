using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Landi.FrameWorks
{
    public class Handler
    {
        private MessageQueue mQueue;
        private Looper mLooper;

        public Handler()
        {
            mLooper = Looper.MyLooper();
            if (mLooper == null)
            {
                throw new Exception("Can't create handler inside thread that has not called Looper.prepare()");
            }
            mQueue = mLooper.Queue;
        }

        public Handler(Looper looper)
        {
            mLooper = looper;
            mQueue = looper.Queue;
        }

        public bool SendMessage(Message msg)
        {
            return SendMessageDelayed(msg, 0);
        }

        public bool SendMessageDelayed(Message msg, long delayMillis)
        {
            if (delayMillis < 0)
            {
                delayMillis = 0;
            }
            msg.dateTime = DateTime.Now + TimeSpan.FromMilliseconds(delayMillis);
            return SendMessageAtTime(msg, DllImport.GetTickCount() + delayMillis);
        }

        public bool SendMessageAtTime(Message msg, long uptimeMillis)
        {
            bool sent = false;
            MessageQueue queue = mQueue;
            if (queue != null)
            {
                msg.target = this;
                sent = queue.EnqueueMessage(msg, uptimeMillis);
            }
            else
            {
                Log.Debug(this + " sendMessageAtTime() called with no mQueue");
            }
            return sent;
        }

        public bool SendMessageAtFrontOfQueue(Message msg)
        {
            bool sent = false;
            MessageQueue queue = mQueue;
            if (queue != null)
            {
                msg.target = this;
                sent = queue.EnqueueMessage(msg, 0);
            }
            else
            {
                throw new Exception(
                    this + " sendMessageAtTime() called with no mQueue");
            }
            return sent;
        }

        public void DispatchMessage(Message msg)
        {
            try
            {
                if (msg.callback != null)
                {
                    handleCallback(msg);
                }
                else
                {
                    HandleMessage(msg);
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(this, ex);
            }
        }

        public bool Post(MethodInvoker r)
        {
            return SendMessageDelayed(getPostMessage(r), 0);
        }

        public bool PostDelayed(MethodInvoker r, long delayMillis)
        {
            return SendMessageDelayed(getPostMessage(r), delayMillis);
        }

        public void RemoveCallbacks(MethodInvoker r)
        {
            mQueue.RemoveMessages(this, r, null);
        }

        public void RemoveCallbacks(MethodInvoker r, object obj)
        {
            mQueue.RemoveMessages(this, r, obj);
        }

        public void RemoveMessages(int what)
        {
            mQueue.RemoveMessages(this, what, null, true);
        }

        public void RemoveMessages(int what, object obj)
        {
            mQueue.RemoveMessages(this, what, obj, true);
        }

        public bool HasMessages(int what)
        {
            return mQueue.RemoveMessages(this, what, null, false);
        }

        public bool HasMessages(int what, object obj)
        {
            return mQueue.RemoveMessages(this, what, obj, false);
        }

        private Message getPostMessage(MethodInvoker r)
        {
            return Message.Obtain(r);
        }

        protected virtual void HandleMessage(Message msg)
        {

        }

        private void handleCallback(Message message)
        {
            message.callback();
        }

        public override string ToString()
        {
            return "Handler{"
                   + this.GetHashCode()
                   + "}";
        }
    }
}
