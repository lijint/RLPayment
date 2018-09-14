using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Creditcard 
{
    class CreditcardPrinterErrorQueryDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("信用卡还款温馨提示");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }
        
    }
}
