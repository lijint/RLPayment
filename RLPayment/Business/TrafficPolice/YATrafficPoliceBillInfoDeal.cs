using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceBillInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            YAEntity entity = GetBusinessEntity() as YAEntity;
            GetElementById("UserName").InnerText = entity.TPUserName;
            GetElementById("UserID").InnerText = entity.TPUserID;
            GetElementById("Fee").InnerText = entity.TPFeeAmount.ToString("########0.00") + " 元";
            GetElementById("Amount").InnerText = entity.TPPrinAmount.ToString("########0.00") + " 元";
            GetElementById("PayAmount").InnerText = entity.TPPayAmount.ToString("########0.00") + " 元";

            GetElementById("Back").Click += Back_Click;
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }


        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("雅安交警罚没输入决定书编号");
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            CommonData.Amount = (GetBusinessEntity() as YAEntity).TPPayAmount;
            StartActivity("雅安交警罚没金额确认");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
