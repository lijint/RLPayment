using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.CarTicket
{
    internal class CarCommonErrDeal : MessageActivity
    {
        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("车票预订主画面");
        }

        protected override void DoMessage(object message)
        {
            GetElementById("Message").InnerText = (string) message;
            if (IsBack)
            {
                GetElementById("Ok").Style = "display:block;";
                GetElementById("Ok").Click += Ok_Click;
            }
            GetElementById("Return").Click += Return_Click;
        }

    }
}
