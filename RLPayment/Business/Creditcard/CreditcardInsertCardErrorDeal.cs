using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Creditcard 
{
    class CreditcardInsertCardErrorDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Retry").Click += Retry_click;
            GetElementById("Return").Click += Return_Click;
        }

        public void Retry_click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("信用卡还款插入银行卡");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }
        
       
    }
}
