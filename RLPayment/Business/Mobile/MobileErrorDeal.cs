using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Landi.FrameWorks;

namespace YAPayment.Business.Mobile
{
    class MobileErrorDeal : MessageActivity
    {
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void DoMessage(object message)
        {
            GetElementById("Message").InnerText = (string)message;
            GetElementById("Return").Click += Return_Click;

            
        }
    }
}
