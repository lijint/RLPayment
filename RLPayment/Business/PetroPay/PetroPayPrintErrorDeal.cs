using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PetroPay
{
    class PetroPayPrintErrorDeal : Activity
    {
        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("中石油支付交易完成");
        }

        protected override void OnEnter()
        {
            GetElementById("Message").InnerText = "凭条打印失败";
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
        }
    }
}
