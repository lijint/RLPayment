using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PowerCardPay.User
{
    class PowerPayConfirmPayUserInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("PayAMount").InnerText = CommonData.Amount.ToString("########0.00");
            //GetElementById("ShowMsg").InnerText = ConfigFile.ReadConfigAndCreate("Power", "UserPayMessage", "本机仅允许雅安市雨城区电力局用户缴费!");
            GetElementById("Back").Click += Back_Click;
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付用户账单信息");
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付插入银行卡");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
