using Landi.FrameWorks;
using System;
using YAPayment.Entity;
using YAPayment.Package.PowerCardPay;

namespace YAPayment.Business.PowerCardPay
{
  
    class PowerPayBillQueryDeal : Activity
    {
        private TransResult ret = TransResult.E_RECV_FAIL;
        private CPowerCardBillQuery infoQuery;
        protected override void OnEnter()
        {
            PowerEntity entity = GetBusinessEntity() as PowerEntity;
            //TransResult result1 = SyncTransaction(infoQuery);
            PostSync(Inquery);
            //TransResult result = SyncTransaction(infoQuery);
            if (ret == TransResult.E_SUCC)
            {
                StartActivity("电力支付账单信息");
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

        private void Inquery()
        {
            try
            {
                infoQuery = new CPowerCardBillQuery();

                ret = infoQuery.BillQuery();
            }
            catch (Exception ex)
            {
                Log.Error("inquery error:", ex);
            }
        }
   }
}
