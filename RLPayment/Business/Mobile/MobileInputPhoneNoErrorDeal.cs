using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Mobile
{
    class MobileInputPhoneNoErrorDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Retry").Click += Retry_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Retry_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("手机充值输入手机号");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
