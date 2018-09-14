using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPolicePasswordErrorDeal : Activity
    {
        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("雅安交警罚没输入密码");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }
    }
}
