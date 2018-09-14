using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Creditcard 
{
    /// <summary>
    /// ¥ÌŒÛ¥¶¿Ì
    /// </summary>
    class CreditcardErrorDeal : MessageActivity
    {
        protected override void DoMessage(object message)
        {
            GetElementById("Message").InnerText= (string)message;
            GetElementById("Return").Click += Return_Click;
        }
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
