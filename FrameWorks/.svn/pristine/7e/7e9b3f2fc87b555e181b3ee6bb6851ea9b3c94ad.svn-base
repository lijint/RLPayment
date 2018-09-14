using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Landi.FrameWorks
{
    public class EventHandlerWrapper
    {
        private EventHandler m_handler;
        private MethodInfo m_method;
        private object m_target;
        private MethodInvoker m_doEvent;

        public MethodInvoker DoEvent
        {
            get { return m_doEvent; }
            set { m_doEvent = value; }
        }

        public object Target
        {
            get { return m_target; }
            set { m_target = value; }
        }

        public MethodInfo Method
        {
            get { return m_method; }
            set { m_method = value; }
        }

        public EventHandler Handler
        {
            get { return m_handler; }
            private set { m_handler = Handler; }
        }

        // Methods
        public EventHandlerWrapper()
        {
        }

        public EventHandlerWrapper(EventHandler eventHandler, MethodInvoker doEvent)
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException("eventHandler");
            }
            this.Target = eventHandler.Target;
            this.Method = eventHandler.Method;
            this.Handler = (EventHandler)Delegate.Combine(this.Handler, new EventHandler(this.Invoke));
            this.DoEvent = (MethodInvoker)Delegate.Combine(this.DoEvent, doEvent);
        }

        private void Invoke(object sender, EventArgs args)
        {
            try
            {
                this.Method.Invoke(this.Target, new object[] { sender, args });
            }
            catch (TargetInvocationException exception)
            {
                //AppLog.Write(exception.InnerException.Message, AppLog.LogMessageType.Error, exception, base.GetType());
                this.OnDoEvent();
            }
        }

        protected void OnDoEvent()
        {
            if (this.DoEvent != null)
            {
                this.DoEvent();
            }
        }

        /// <summary>
        /// implicit operator 隐式转化A=B
        /// explicit operator 显示转化A=(A)B
        /// </summary>
        /// <param name="eventHandlerWrapper"></param>
        /// <returns></returns>
        public static implicit operator EventHandler(EventHandlerWrapper eventHandlerWrapper)
        {
            return eventHandlerWrapper.Handler;
        }
    }
}
