using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Creditcard  
{
    /// <summary>
    /// 交易成功是否打印
    /// </summary>
    class CreditcardPaySuccessDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Yes_Click;
            GetElementById("Return").Click += No_Click;
        }
        void No_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }
        void Yes_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("信用卡还款正在打印");
        }
       
    }
}
