using Landi.FrameWorks;
using YAPayment.Entity;
using YAPayment.Package.YAPublishPay;

namespace YAPayment.Business.YAPublishPay
{
    class YAPublishPayBillQueryDeal : Activity
    {
        protected override void OnEnter()
        {
            YAEntity entity = GetBusinessEntity() as YAEntity;
            CYAPublishPayBillQuery infoQuery = new CYAPublishPayBillQuery();
            TransResult ret = SyncTransaction(infoQuery);

            if (ret == TransResult.E_SUCC)
            {
                switch (entity.PublishPayType)
                {
                    case YaPublishPayType.Gas:
                        StartActivity("雅安气费账单信息");
                        break;
                    case YaPublishPayType.Water:
                        StartActivity("雅安水费账单信息");
                        break;
                    case YaPublishPayType.Power:
                        break;
                    case YaPublishPayType.TV:
                        StartActivity("雅安广电费账单信息");
                        break;
                }
            }
            else if (ret == TransResult.E_HOST_FAIL)
            {
                if (infoQuery.ReturnCode == "D3")
                    ShowMessageAndGotoMain("验证密码失败!请提供正确用户名!");
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
