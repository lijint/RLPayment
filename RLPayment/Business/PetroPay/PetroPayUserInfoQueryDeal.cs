using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package.PetroPay;

namespace YAPayment.Business.PetroPay
{
    class PetroPayUserInfoQueryDeal : Activity
    {
        protected override void OnEnter()
        {
            CPetroPayUserInfoQuery infoQuery = new CPetroPayUserInfoQuery();
            TransResult ret = SyncTransaction(infoQuery);

            if (ret == TransResult.E_SUCC)
            {
                StartActivity("中石油支付用户信息显示");
            }
            else if (ret == TransResult.E_HOST_FAIL)
            {
                if (infoQuery.ReturnCode == "D3")
                    ShowMessageAndGotoMain("验证密码失败!请提供正确用户名及密码!");
                else
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
