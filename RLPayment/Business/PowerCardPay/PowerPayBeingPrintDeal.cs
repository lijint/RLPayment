using Landi.FrameWorks;
using YAPayment.Package;
using YAPayment.Entity;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayBeingPrintDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            if ((GetBusinessEntity() as PowerEntity).PowerBusiness == 1)
                PrintReceipt(new PowerPay().GetCardReceipt());
            else
                PrintReceipt(new PowerPay().GetUserReceipt());
        }

        protected override void HandleResult(Result result)
        {
            if (result == Result.Success || result == Result.PaperFew)
            {
                StartActivity("电力支付交易完成");
            }
            else
            {
                StartActivity("电力支付打印失败");
            }
        }
    }
}
