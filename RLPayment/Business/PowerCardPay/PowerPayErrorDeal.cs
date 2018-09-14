using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayErrorDeal : MessageActivity
    {
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }

        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }

        protected override void DoMessage(object message)
        {
            GetElementById("Message").InnerText = (string)message;
            if (IsBack)
            {
                GetElementById("Ok").Style = "display:block;";
                GetElementById("Ok").Click += Ok_Click;
            }
            GetElementById("Return").Click += Return_Click;
        }
    }
}
