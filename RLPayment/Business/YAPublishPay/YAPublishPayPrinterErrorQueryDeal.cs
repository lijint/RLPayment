using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.YAPublishPay
{
    class YAPublishPayPrinterErrorQueryDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("雅安支付输入用户号");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
        
    }
}
