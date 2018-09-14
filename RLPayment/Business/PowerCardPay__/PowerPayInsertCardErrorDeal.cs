using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayInsertCardErrorDeal : Activity
    {
        public void Retry_click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付插入银行卡");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnEnter()
        {
            GetElementById("Ok").Click += new HtmlElementEventHandler(Retry_click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }
    }
}
