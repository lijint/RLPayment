using System.Collections;
using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Package;

namespace YAPayment.Business.PowerCardPay.User
{
    class PowerPayUserConfirmFailDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            GetElementById("Return").Click += Return_Click;
            if (!ReceiptPrinter.ExistError())
            {
                ArrayList al = new PowerPay().GetUserReceipt();
                al.Add("   ");
                al.Add("   缴费确认未成功，48小时内系统会自动处理，");
                al.Add("   请耐心等待，不要重复缴费");
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
