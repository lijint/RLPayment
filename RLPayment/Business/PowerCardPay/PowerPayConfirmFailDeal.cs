using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;
using YAPayment.Package;

namespace YAPayment.Business.PowerCardPay
{
    internal class PowerPayConfirmFailDeal : PrinterActivity
    {

        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            if (!ReceiptPrinter.ExistError())
            {
               PrintReceipt(new PowerPay().GetCardReceipt(true));
            }
        }

        protected override void HandleResult(Result result)
        {
           
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
           GotoMain();
        }

    }
}
