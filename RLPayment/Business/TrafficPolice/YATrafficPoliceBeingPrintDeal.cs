using Landi.FrameWorks;
using YAPayment.Package;

namespace YAPayment.Business.TrafficPolice
{
    /// <summary>
    /// 正在打印
    /// </summary>
    class YATrafficPoliceBeingPrintDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            PrintReceipt(new YAPaymentPay().GetTPReceipt());
        }

        protected override void HandleResult(Result result)
        {
            if (result ==Result.Success || result == Result.PaperFew)
            {
                StartActivity("雅安交警罚没交易完成");
            }
            else
            {
                StartActivity("雅安交警罚没打印失败");
            }
        }
    }
}
