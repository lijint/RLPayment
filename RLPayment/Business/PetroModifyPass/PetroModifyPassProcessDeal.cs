using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package.PetroModifyPass;

namespace YAPayment.Business.PetroModifyPass
{
    class PetroModifyPassProcessDeal:Activity
    {
        protected override void OnEnter()
        {
            CPetroModifyPassProcess PetroModifyPassProcess = new CPetroModifyPassProcess();
            TransResult ret = SyncTransaction(PetroModifyPassProcess);
           
            if (ret == TransResult.E_SUCC)
            {
                StartActivity("中石油修改密码成功");
            }
            else if (ret == TransResult.E_HOST_FAIL)
            {
                if (PetroModifyPassProcess.ReturnCode == "D3")
                    ShowMessageAndGoBack("验证密码失败!请提供正确用户名及密码!");
                else
                    ShowMessageAndGoBack(PetroModifyPassProcess.ReturnCode + "-" + PetroModifyPassProcess.ReturnMessage);
            }
            else if (ret == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGoBack("交易超时，请重试");
            }
            else
            {
                ShowMessageAndGoBack("交易失败，请重试");
            }
        }
    }
}
