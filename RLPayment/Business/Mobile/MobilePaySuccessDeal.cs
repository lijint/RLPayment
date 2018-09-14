using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Mobile
{
    class MobilePaySuccessDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Yes_Click;
            GetElementById("Return").Click += No_Click;
        }

        void No_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        void Yes_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("手机充值正在打印");
        }
    }
}