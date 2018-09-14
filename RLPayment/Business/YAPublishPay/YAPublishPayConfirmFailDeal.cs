using System.Collections;
using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Package;

namespace YAPayment.Business.YAPublishPay
{
    class YAPublishPayConfirmFailDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            GetElementById("Return").Click += Return_Click;
            if (!ReceiptPrinter.ExistError())
            {
                ArrayList al = new YAPaymentPay().GetReceipt();
                al.Add("   ");
                al.Add("   �ɷ�ȷ��δ�ɹ���48Сʱ��ϵͳ���Զ������");
                al.Add("   �����ĵȴ�����Ҫ�ظ��ɷ�");
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
