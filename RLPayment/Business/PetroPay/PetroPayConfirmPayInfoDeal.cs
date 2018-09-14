using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PetroPay
{
    class PetroPayConfirmPayInfoDeal : Activity
    {
        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            if (CommonData.BIsCardIn)
                StartActivity("中石油支付正在交易");
            else
                StartActivity("中石油支付插入银行卡");
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
           StartActivity("中石油支付账单费用显示");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnEnter()
        {
            GetElementById("PayMount").InnerText = CommonData.Amount.ToString("######0.00");
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Back").Click += new HtmlElementEventHandler(Back_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }
    }
}
