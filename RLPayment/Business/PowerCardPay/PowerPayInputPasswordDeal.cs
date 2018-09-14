using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayInputPasswordDeal : EsamActivity
    {
        protected override void OnErrorLength()
        {
            InvokeScript("showBankPassLenError");
        }

        protected override string InputId
        {
            get { return "pin"; }
        }

        protected override void OnClearNotice()
        {
            InvokeScript("hideBankPassLenError");
        }

        protected override void HandleResult(Result result)
        {
            if (result == Result.Success)
            {
                CommonData.BankPassWord = Password;
                if ((GetBusinessEntity() as PowerEntity).PowerBusiness == 1)
                    StartActivity("电力支付正在交易");
                else
                    StartActivity("电力支付用户正在交易");
            }
            else if (result == Result.Cancel || result == Result.TimeOut)
                GotoMain();
            else if (result == Result.HardwareError)
                ShowMessageAndGotoMain("密码键盘故障");
        }

        protected override string SectionName
        {
            get { return GetBusinessEntity().SectionName; }
        }
    }
}
