using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.TrafficPolice
{
    /// <summary>
    /// 交易成功是否打印
    /// </summary>
    class YATrafficPoliceSuccessDeal : Activity
    {

        void Print_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("雅安交警罚没正在打印");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Print_Click;
            GetElementById("Return").Click += Return_Click;
        }
    }
}
