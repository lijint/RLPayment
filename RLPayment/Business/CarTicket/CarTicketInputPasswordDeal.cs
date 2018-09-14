using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.CarTicket
{
    class CarTicketInputPasswordDeal : EsamActivity
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
                StartActivity("购票正在交易");
            }
            else if (result == Result.Cancel || result == Result.TimeOut)
            {
                GetBusinessEntity<CarEntity>().UnlockMessage = "用户取消,";
                StartActivity("解锁车票");
            }
            else if (result == Result.HardwareError)
            {
                GetBusinessEntity<CarEntity>().UnlockMessage = "免密键盘故障,";
                StartActivity("解锁车票"); ;
            }
        }

        protected override string SectionName
        {
            get { return GetBusinessEntity().SectionName; }
        }
    }
}
