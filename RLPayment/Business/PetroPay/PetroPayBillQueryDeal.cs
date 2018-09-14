using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package.PetroPay;
using System.Collections;
using YAPayment.Package;

namespace YAPayment.Business.PetroPay
{
    class PetroPayBillQueryDeal : Activity
    {
        protected override void OnEnter()
        {
#if DEBUG
            CommonData.Amount = 195.5;
#endif
            CPetroPayBillQuery PetroPayBillQuery = new CPetroPayBillQuery();
            TransResult ret = SyncTransaction(PetroPayBillQuery);
            if (ret == TransResult.E_SUCC)
            {
                YAPaymentPay.ListQueryInfo.Clear();
                int nQueryCount = 0;
                int.TryParse(YAPaymentPay.QueryCount, out nQueryCount);
                if (nQueryCount != 0)
                {
                    YAPaymentPay.ListQueryInfo.AddRange(YAPaymentPay.QueryContent.Split(';'));
                }

                if (YAPaymentPay.ListQueryInfo.Count != nQueryCount)
                {
                    Log.Warn("缴费账单查询的条数不一致");
                    ShowMessageAndGoBack("查询失败");
                }
                else
                {
                    StartActivity("中石油支付账单费用显示");
                }

            }
            else if (ret == TransResult.E_HOST_FAIL)
            {
                if (PetroPayBillQuery.ReturnCode == "D3")
                    ShowMessageAndGoBack("该账户已经缴费，无缴费记录");//该账户无欠费记录
                else
                    ShowMessageAndGoBack(PetroPayBillQuery.ReturnCode + "-" + PetroPayBillQuery.ReturnMessage);
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
