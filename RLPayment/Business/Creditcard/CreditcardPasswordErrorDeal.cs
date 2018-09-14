using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Creditcard 
{
    class CreditcardPasswordErrorDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Retry").Click += Retry_click;
            GetElementById("Return").Click += Return_click;
        }

        public void Retry_click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("信用卡还款输入密码");
        }

        public void Return_click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }
    }
}
