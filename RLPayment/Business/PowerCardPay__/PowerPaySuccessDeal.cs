using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPaySuccessDeal : Activity
    {
        void Print_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付正在打印");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnEnter()
        {
            GetElementById("Print").Click += new HtmlElementEventHandler(Print_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }
    }
}
