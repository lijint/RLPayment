using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.CarTicket
{
    class CarTicketPaySucessThankDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Return").Click += Return_Click;
        }
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }


    }
}
