using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Landi.FrameWorks
{
    public abstract class MessageActivity : FrameActivity
    {
        internal const string MESSAGE = "System.Message";
        internal const string Main_OR_BACK = "System.MainOrBack";
        internal const string METHOD = "System.Method";

        private object mMessage;
        protected bool IsBack = false;

        protected sealed override void OnEnter()
        {
            if (MyIntent.GetExtra(MESSAGE) != null)
                mMessage = MyIntent.GetExtra(MESSAGE);
            if (mMessage == null)
                mMessage = "Not Defined";
            if (MyIntent.GetExtra(Main_OR_BACK) != null)
                IsBack = !(bool)MyIntent.GetExtra(Main_OR_BACK);
            DoMessage(mMessage);
            MethodInvoker method = (MethodInvoker)MyIntent.GetExtra(METHOD);
            if (method != null)
                PostAsync(method);
        }

        protected abstract void DoMessage(object message);

        //protected override void OnTimeOut()
        //{
        //    if (!IsBack)
        //        GotoMain();
        //    else
        //        GoBack();
        //}
    }
}
