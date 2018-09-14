using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace YAPayment.Business.PowerCardPay.Brush
{
    class PowerPayInputBrushPasswordErrorDeal : Activity
    {
        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付输入感应银行卡密码");
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
