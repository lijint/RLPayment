using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayPayChoiceDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Flush").Click += Flush_Click;
            GetElementById("Hand").Click += Hand_Click;
            GetElementById("Back").Click += Back_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付金额确认");
        }

        private void Flush_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付输入感应银行卡密码");
        }

        private void Hand_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付退出电卡");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
