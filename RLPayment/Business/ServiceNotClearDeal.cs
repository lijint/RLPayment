using System.Windows.Forms;
using Landi.FrameWorks;

namespace RLPayment.Business
{
    class ServiceNotClearDeal : Activity
    {
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }

        protected override void OnEnter()
        {
            GetElementById("Return").Click += Return_Click;
        }
    }
}
