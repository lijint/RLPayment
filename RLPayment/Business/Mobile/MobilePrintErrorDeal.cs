using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Mobile
{
    class MobilePrintErrorDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Message").InnerText = "打印凭条失败";
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
        }

        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("手机充值打印成功");
        }
    }
}
