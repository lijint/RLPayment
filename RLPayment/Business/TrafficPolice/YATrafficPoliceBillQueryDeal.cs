using Landi.FrameWorks;
using YAPayment.Package.TrafficPolice;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceBillQueryDeal : Activity
    {
        protected override void OnEnter()
        {
            CYATrafficPoliceBillQuery infoQuery = new CYATrafficPoliceBillQuery();
            TransResult ret = SyncTransaction(infoQuery);

            if (ret == TransResult.E_SUCC)
            {
                StartActivity("雅安交警罚没违章显示");
            }
            else if (ret == TransResult.E_HOST_FAIL)
            {
                if (infoQuery.ReturnCode == "D3")
                    ShowMessageAndGotoMain("验证密码失败!请提供正确决定书编号！");
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
