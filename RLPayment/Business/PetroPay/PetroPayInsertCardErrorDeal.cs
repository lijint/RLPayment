using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PetroPay
{
    class PetroPayInsertCardErrorDeal : Activity
    {
        public void Retry_click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("中石油支付插入银行卡");
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
