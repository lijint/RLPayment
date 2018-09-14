using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Creditcard 
{
    class CreditcardInputCardNoErrorDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Retry").Click += Retry_Click;
            GetElementById("Return").Click += Return_Click;
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }

        void Retry_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("信用卡还款输入卡号");
        }
    }
}
