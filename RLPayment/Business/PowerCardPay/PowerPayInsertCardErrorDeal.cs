using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayInsertCardErrorDeal : Activity
    {
        public void Retry_click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付插入银行卡");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Retry_click;
            GetElementById("Return").Click += Return_Click;
        }
    }
}
