using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Mobile
{
    class MobilePrinterErrorQueryDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("手机充值主界面");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
        
    }
}
