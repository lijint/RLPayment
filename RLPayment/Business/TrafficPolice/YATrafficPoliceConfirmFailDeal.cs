using System.Collections;
using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Package;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceConfirmFailDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            GetElementById("Return").Click += Return_Click;
            if (!ReceiptPrinter.ExistError())
            {
                ArrayList al = new YAPaymentPay().GetTPReceipt();
                al.Add("   ");
                al.Add("   银联扣款成功，交警确认失败，");
                al.Add("   请使用自助核销功能进行核销。");
                al.Add("   或耐心等待24小时，系统会自动处理。");
                PrintReceipt(al);
            }
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void HandleResult(Result result)
        {

        }
    }
}
