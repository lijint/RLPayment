using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Landi.FrameWorks
{
    public class MessageQueue
    {
        /// <summary>
        /// Callback interface for discovering when a thread is going to block waiting for more messages.
        /// </summary>
        public interface IdleHandler
        {
            /// <summary>
            /// Called when the message queue has run out of messages and will now 
            /// wait for more.  Return true to keep your idle handler active, false 
            /// to have it removed.  This may be called if there are still messages 
            /// pending in the queue, but they are all scheduled to be dispatched 
            /// after the current time. 
            /// </summary>
            /// <returns>true stands for keeping,false stands for removing</returns>
            bool QueueIdle();
        }

        internal Message mMessages;
        internal bool mQuitAllowed;
        private bool mBlocked;
        private bool mQuiting;
        private AutoResetEvent queueEvent;
        private List<IdleHandler> mIdleHandlers = new List<IdleHandler>();
        private IdleHandler[] mPendingIdleHandlers;

        public MessageQueue()
        {
            mMessages = null;
            mQuitAllowed = true;
            mQuiting = false;
            mBlocked = false;
            queueEvent = new AutoResetEvent(false);
        }

        public void AddIdleHandler(IdleHandler handler)
        {
            if (handler == null)
            {
                return;
            }
            lock (this)
            {
                mIdleHandlers.Add(handler);
            }
        }

        public void RemoveIdleHandler(IdleHandler handler)
        {
            lock (this)
            {
                mIdleHandlers.Remove(handler);
            }
        }

        public Message Next()
        {
            int pendingIdleHandlerCount = -1; // -1 only during first iteration
            int nextPollTimeoutMillis = 0;

            for (; ; )
            {
                queueEvent.WaitOne(nextPollTimeoutMillis);

                lock (this)
                {
                    int now = DllImport.GetTickCount();
                    Message msg = mMessages;
                    if (msg != null)
                    {
                        long when = msg.when;
                        if (now >= when)
                        {
                            mBlocked = false;
                            mMessages = msg.next;
                            msg.next = null;
                            //Log.Debug(this + " Returning : " + msg);
                            return msg;
                        }
                        else
                        {
                            nextPollTimeoutMillis = (int)Math.Min(when - now, int.MaxValue);
                        }
                    }
                    else
                    {
                        nextPollTimeoutMillis = -1;
                        //mBlocked = true;
                    }

                    // If first time, then get the number of idlers to run.
                    if (pendingIdleHandlerCount < 0)
                    {
                        pendingIdleHandlerCount = mIdleHandlers.Count;
                    }
                    if (pendingIdleHandlerCount == 0)
                    {
                        // No idle handlers to run.  Loop and wait some more.
                        mBlocked = true;
                        continue;
                    }

                    if (mPendingIdleHandlers == null)
                    {
                        mPendingIdleHandlers = new IdleHandler[pendingIdleHandlerCount];
                    }
                    mPendingIdleHandlers = mIdleHandlers.ToArray();
                }

                // Run the idle handlers.
                // We only ever reach this code block during the first iteration.
                for (int i = 0; i < pendingIdleHandlerCount; i++)
                {
                    IdleHandler idler = mPendingIdleHandlers[i];
                    mPendingIdleHandlers[i] = null; // release the reference to the handler

                    bool keep = false;
                    try
                    {
                        keep = idler.QueueIdle();
                    }
                    catch (Exception t)
                    {
                        Log.Error("MessageQueue IdleHandler threw exception", t);
                    }

                    if (!keep)
                    {
                        lock (this)
                        {
                            mIdleHandlers.Remove(idler);
                        }
                    }
                }

                // Reset the idle handler count to 0 so we do not run them again.
                pendingIdleHandlerCount = 0;

                // While calling an idle handler, a new message could have been delivered
                // so go back and look again for a pending message without waiting.
                nextPollTimeoutMillis = 0;
            }
        }

        public bool EnqueueMessage(Message msg, long when)
        {
            bool needWake;
            lock (this)
            {
                if (mQuiting)
                {
                    Log.Debug(msg.target + " sending message to a MessageQueue on a dead thread");
                    throw new Exception(msg.target + " sending message to a MessageQueue on a dead thread");
                }
                else if (msg.target == null)
                {
                    mQuiting = true;
                }

                msg.when = when;
                Message p = mMessages;
                if (p == null || when == 0 || when < p.when)
                {
                    msg.next = p;
                    mMessages = msg;
                    needWake = mBlocked; // new head, might need to wake up
                }
                else
                {
                    Message prev = null;
                    while (p != null && p.when <= when)
                    {
                        prev = p;
                        p = p.next;
                    }
                    msg.next = prev.next;
                    prev.next = msg;
                    needWake = false; // still waiting on head, no need to wake up
                }
            }
            if (needWake)
            {
                queueEvent.Set();
            }
            return true;
        }

        public bool RemoveMessages(Handler h, int what, object obj, bool doRemove)
        {
            lock (this)
            {
                Message p = mMessages;
                bool found = false;

                // Remove all messages at front.
                while (p != null && p.target == h && p.what == what
                       && (obj == null || p.obj == obj))
                {
                    if (!doRemove) return true;
                    found = true;
                    Message n = p.next;
                    mMessages = n;
                    p = n;
                }

                // Remove all messages after front.
                while (p != null)
                {
                    Message n = p.next;
                    if (n != null)
                    {
                        if (n.target == h && n.what == what
                            && (obj == null || n.obj == obj))
                        {
                            if (!doRemove) return true;
                            found = true;
                            Message nn = n.next;
                            p.next = nn;
                            continue;
                        }
                    }
                    p = n;
                }

                return found;
            }
        }

        public void RemoveMessages(Handler h, MethodInvoker r, object obj)
        {
            if (r == null)
            {
                return;
            }

            lock (this)
            {
                Message p = mMessages;
                // Remove all messages at front.
                while (p != null && p.target == h && p.callback == r
                       && (obj == null || p.obj == obj))
                {
                    Message n = p.next;
                    mMessages = n;
                    p = n;
                }

                // Remove all messages after front.
                while (p != null)
                {
                    Message n = p.next;
                    if (n != null)
                    {
                        if (n.target == h && n.callback == r
                            && (obj == null || n.obj == obj))
                        {
                            Message nn = n.next;
                            p.next = nn;
                            continue;
                        }
                    }
                    p = n;
                }
            }
        }

        public override string ToString()
        {
            return "MessageQueue{"
                   + this.GetHashCode()
                   + "}";
        }
    }
}
