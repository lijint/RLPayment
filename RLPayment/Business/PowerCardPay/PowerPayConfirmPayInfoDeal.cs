using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using YAPayment.Entity;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayConfirmPayInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("PayAMount").InnerText = CommonData.Amount.ToString("########0.00");

            GetElementById("Back").Click += new HtmlElementEventHandler(Back_Click);
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付输入缴费金额");
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            //StartActivity("电力支付选择支付方式");
            StartActivity("电力支付退出电卡");
            
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
