using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PetroPay
{
    class PetroPayPrinterErrorQueryDeal : Activity
    {
        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            //(BusinessLib as PetroPayLibrary).IsJumpPrintSlip = true;
            StartActivity("中石油支付用户信息查询");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnEnter()
        {
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }
    }
}
