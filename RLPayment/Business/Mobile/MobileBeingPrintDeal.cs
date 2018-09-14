using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using YAPayment.Package;

namespace YAPayment.Business.Mobile
{
    /// <summary>
    /// 正在打印
    /// </summary>
    class MobileBeingPrintDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            PrintReceipt(new QMPay().GetMobileReceipt());
        }

        protected override void HandleResult(Result result)
        {
            if (result == Result.Success || result == Result.PaperFew)
            {
                StartActivity("手机充值打印成功");
            }
            else
            {
                StartActivity("手机充值打印失败");
            }
        }
    }
}
