using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using System.Collections;
using YAPayment.Package;

namespace YAPayment.Business.PetroPay
{
    /// <summary>
    /// 正在打印
    /// </summary>
    class PetroPayBeingPrintDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            PrintReceipt(new YAPaymentPay().GetReceipt());
        }

        protected override void HandleResult(Result result)
        {
            if (result ==Result.Success || result == Result.PaperFew)
            {
                StartActivity("中石油支付交易完成");
            }
            else
            {
                StartActivity("中石油支付打印失败");
            }
        }
    }
}
