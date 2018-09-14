using Landi.FrameWorks;
using YAPayment.Package;

namespace YAPayment.Business.YAPublishPay
{
    /// <summary>
    /// 正在打印
    /// </summary>
    class YAPublishPayBeingPrintDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            PrintReceipt(new YAPaymentPay().GetReceipt());
        }

        protected override void HandleResult(Result result)
        {
            if (result ==Result.Success || result == Result.PaperFew)
            {
                StartActivity("雅安支付交易完成");
            }
            else
            {
                StartActivity("雅安支付打印失败");
            }
        }
    }
}
