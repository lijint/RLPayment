using Landi.FrameWorks;
using YAPayment.Package.TrafficPolice;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceBeingConfirmDeal : Activity
    {
        protected override void OnEnter()
        {
            CommonData.Amount = 0;

            CYATrafficPoliceBillConfirm billConfirm = new CYATrafficPoliceBillConfirm();
            TransResult retConfirm = SyncTransaction(billConfirm);

            if (retConfirm == TransResult.E_SUCC)
            {
                StartActivity("雅安交警罚核销成功");
            }
            else if (retConfirm == TransResult.E_HOST_FAIL)
            {
                ShowMessageAndGotoMain(billConfirm.ReturnCode + "-" + billConfirm.ReturnMessage);
            }
            else if (retConfirm == TransResult.E_RECV_FAIL)
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
