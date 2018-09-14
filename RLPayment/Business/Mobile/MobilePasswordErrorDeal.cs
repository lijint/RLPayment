using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Mobile
{
    class MobilePasswordErrorDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Retry").Click += Retry_click;
            GetElementById("Return").Click += Return_click;
        }

        public void Retry_click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("手机充值输入密码");
        }

        public void Return_click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
