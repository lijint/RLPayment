using Landi.FrameWorks;
using YAPayment.Package;

namespace YAPayment.Business.Creditcard 
{
    class CreditcardBeingPrintDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            PrintReceipt(new QMPay().GetCreditCardReceipt());
        }

        protected override void HandleResult(Result result)
        {
            if (result == Result.Success || result == Result.PaperFew)
            {
                StartActivity("信用卡还款打印成功");
            }
            else
            {
                StartActivity(typeof(CreditcardPrintErrorDeal));
            }
        }
    }
}
