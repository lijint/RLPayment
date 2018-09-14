using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PetroPay
{
    class PetroPayErrorDeal : MessageActivity
    {
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }

        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("中石油支付用户信息显示");
        }

        protected override void DoMessage(object message)
        {
            GetElementById("Message").InnerText = (string)message;
            if (IsBack)
            {
                GetElementById("Ok").Style = "display:block;";
                GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            }
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);

        }
    }
}
