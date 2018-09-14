using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.Mobile
{
    class MobileConfirmRechargeInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("PhoneNo").InnerText = (GetBusinessEntity() as QMEntity).PhoneNo;
            GetElementById("Amount").InnerText = CommonData.Amount.ToString();

            GetElementById("Ok").Click += Confirm_Click;
            GetElementById("Return").Click += Return_Click;
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        void Confirm_Click(object sender, HtmlElementEventArgs e)
        {
           StartActivity("手机充值插入银行卡");
        }
    }
}
