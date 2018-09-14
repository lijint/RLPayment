using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceConfirmPayInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("PayAMount").InnerText = CommonData.Amount.ToString("########0.00");
  
            GetElementById("Back").Click += Back_Click;
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("雅安交警罚没违章显示");
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("雅安交警罚没插入银行卡");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
