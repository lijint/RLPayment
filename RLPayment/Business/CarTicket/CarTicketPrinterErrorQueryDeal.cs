using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.CarTicket
{
    class CarTicketPrinterErrorQueryDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("车票预订主画面");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

    }
}
