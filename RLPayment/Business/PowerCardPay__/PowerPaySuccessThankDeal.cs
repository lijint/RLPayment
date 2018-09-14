using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPaySuccessThankDeal : Activity
    {
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnEnter()
        {
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }
    }
}
