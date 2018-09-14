using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Creditcard 
{
    class CreditcardPrintErrorDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Message").InnerText = "凭条打印失败";
            GetElementById("Ok").Click += Ok_Click;
        }

        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("信用卡还款打印成功");
        }
    }
}
