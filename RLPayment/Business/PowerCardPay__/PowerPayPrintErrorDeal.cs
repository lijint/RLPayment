using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayPrintErrorDeal : Activity
    {
        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付交易完成");
        }

        protected override void OnEnter()
        {
            GetElementById("Message").InnerText = "凭条打印失败";
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
        }
    }
}
