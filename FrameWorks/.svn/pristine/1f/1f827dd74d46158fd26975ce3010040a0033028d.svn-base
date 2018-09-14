using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace Landi.FrameWorks
{
    public class HtmlElementEventHandlerWrapper : EventHandlerWrapper
    {
        // Fields
        private HtmlElementEventHandler m_htmlEventHandler;
        private BackgroundWorker bgWorker;
        

        public HtmlElementEventHandler HtmlEventHandler
        {
            get { return m_htmlEventHandler; }
            private set { m_htmlEventHandler = value; }
        }

        // Methods
        public HtmlElementEventHandlerWrapper(HtmlElementEventHandler eventHandler, MethodInvoker doEvent)
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException("eventHandler");
            }
            base.Target = eventHandler.Target;
            base.Method = eventHandler.Method;
            this.HtmlEventHandler = (HtmlElementEventHandler)Delegate.Combine(this.HtmlEventHandler, new HtmlElementEventHandler(this.Invoke));
            base.DoEvent = (MethodInvoker)Delegate.Combine(base.DoEvent, doEvent);

            bgWorker = new BackgroundWorker();
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);

        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //执行按钮委托
                base.Method.Invoke(base.Target, (object[])e.Result);
                Activity.Context.GetElementById("efeEnd").InnerText = "0";
            }
            catch (TargetInvocationException exception)
            {
                Log.Error("bgWorker_RunWorkerCompleted error", exception);
                base.OnDoEvent();
            }
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //开始特效
            Activity.Context.InvokeScript("btn_click", new object[] { sender });



            //结束特效
            while (true)
            {
                HtmlElement efeEnd = Activity.Context.GetElementById("efeEnd");
                if (efeEnd != null && efeEnd.InnerText == "1")
                    break;
            }
            
            e.Result = e.Argument;
        }

        private void Invoke(object sender, HtmlElementEventArgs args)
        {
            try
            {
                //执行按钮委托
                bgWorker.RunWorkerAsync(new object[] { sender, args });
            }
            catch (TargetInvocationException exception)
            {
                Log.Error("Invoke error", exception);
                base.OnDoEvent();
            }
        }

        public static implicit operator HtmlElementEventHandler(HtmlElementEventHandlerWrapper eventHandlerWrapper)
        {
            return eventHandlerWrapper.HtmlEventHandler;
        }

    }
}
