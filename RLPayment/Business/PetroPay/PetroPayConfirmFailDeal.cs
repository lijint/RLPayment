using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using YAPayment.Package;
using Landi.FrameWorks.HardWare;
using System.Collections;

namespace YAPayment.Business.PetroPay
{
    class PetroPayConfirmFailDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
            if (!ReceiptPrinter.ExistError())
            {
                ArrayList al = new YAPaymentPay().GetReceipt();
                al.Add("   ");
                al.Add("   操作成功，后台发生异常，核销失败，请不要继续缴费");
                al.Add("   等待系统自动处理。次日下午4:00以后再行查看缴费情况");
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
