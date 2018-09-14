using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayPayChoiceDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Flush").Click += new HtmlElementEventHandler(Flush_Click);
            GetElementById("Hand").Click += new HtmlElementEventHandler(Hand_Click);
            GetElementById("Back").Click += new HtmlElementEventHandler(Back_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付金额确认");
        }

        private void Flush_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付输入感应银行卡密码");
        }

        private void Hand_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付退出电卡");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
