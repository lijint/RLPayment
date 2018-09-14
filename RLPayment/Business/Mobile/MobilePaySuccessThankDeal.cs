using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Mobile
{
    class MobilePaySuccessThankDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Return").Click += Return_Click;
        }
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }


    }
}
