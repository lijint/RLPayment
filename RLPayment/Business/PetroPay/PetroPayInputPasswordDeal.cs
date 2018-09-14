using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks.HardWare;
using YAPayment.Package;
using Landi.FrameWorks;

namespace YAPayment.Business.PetroPay
{
    /// <summary>
    /// 输入密码
    /// </summary>
    class PetroPayInputPasswordDeal : EsamActivity
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
                StartActivity("中石油支付正在交易");
            }
            else if (result == Result.Cancel || result == Result.TimeOut)
                GotoMain();
            else if (result == Result.HardwareError)
                ShowMessageAndGotoMain("密码键盘故障");
        }

        protected override string SectionName
        {
            get { return "YAPayment"; }
        }
    }
}
