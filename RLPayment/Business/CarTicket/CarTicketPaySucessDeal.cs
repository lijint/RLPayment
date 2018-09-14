using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.CarTicket
{
    class CarTicketPaySucessDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Print").Click += Yes_Click;
            GetElementById("Return").Click += No_Click;
        }

        void No_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        void Yes_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("购票正在打印");
        }
    }
}
