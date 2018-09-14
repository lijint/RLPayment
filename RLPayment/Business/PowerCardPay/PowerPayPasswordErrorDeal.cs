using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayPasswordErrorDeal : Activity
    {
        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付输入密码");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }
    }
}
