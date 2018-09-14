using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.CarTicket
{
    class CarTicketMenuDeal:Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Buy").Click += Buy_Click;
            InvokeScript("hideElement", new object[] { "Revoke" });
            GetElementById("Revoke").Click += Revoke_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Buy_Click(object sender, HtmlElementEventArgs e)
        {
            Log.Info("购票查询跳转画面");
            StartActivity("购票查询跳转画面");
        }
        private void Revoke_Click(object sender, HtmlElementEventArgs e)
        {

        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
