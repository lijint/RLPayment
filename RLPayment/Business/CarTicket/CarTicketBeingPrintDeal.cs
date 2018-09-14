using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package;

namespace YAPayment.Business.CarTicket
{
    class CarTicketBeingPrintDeal : PrinterActivity
    {
        protected override void OnEnter()
        {

            PrintReceipt(new CarPay().GetCarPayReceipt());
        }

        protected override void HandleResult(Result result)
        {
            if (result == Result.Success || result == Result.PaperFew)
            {
                StartActivity("购票交易完成");
            }
            else
            {
                StartActivity("购票打印失败");
            }
        }
    }
}
