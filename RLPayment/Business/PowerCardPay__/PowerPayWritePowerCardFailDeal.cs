using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using System.Windows.Forms;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayWritePowerCardFailDeal : Activity
    {
        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            if (ReceiptPrinter.ExistError())
                StartActivity("电力支付交易完成");
            else
                StartActivity("电力支付交易成功是否打印");
        }

        protected override void OnEnter()
        {
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
        }
    }
}
