using Landi.FrameWorks;
using YAPayment.Entity;
using YAPayment.Package.PowerCardPay;

namespace YAPayment.Business.PowerCardPay.User
{
    class PowerPayUserBillQueryDeal : Activity
    {
        protected override void OnEnter()
        {
            PowerEntity entity = GetBusinessEntity() as PowerEntity;
            CPowerCardUserBillQuery infoQuery = new CPowerCardUserBillQuery();
            TransResult ret = SyncTransaction(infoQuery);
            if (ret == TransResult.E_SUCC)
            {
                StartActivity("电力支付用户账单信息");
            }
            else if (ret == TransResult.E_HOST_FAIL)
            {
                ShowMessageAndGotoMain(infoQuery.ReturnCode + "-" + infoQuery.ReturnMessage);
            }
            else if (ret == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("交易超时，请重试");
            }
            else
            {
                ShowMessageAndGotoMain("交易失败，请重试");
            }
        }
    }
}
