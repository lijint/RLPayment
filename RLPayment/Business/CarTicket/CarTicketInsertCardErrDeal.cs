using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.CarTicket
{
    class CarTicketInsertCardErrDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Retry").Click += Retry_click;
            GetElementById("Return").Click += Return_Click;
        }

        public void Retry_click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("购票插入银行卡");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GetBusinessEntity<CarEntity>().UnlockMessage = "用户取消,";
            StartActivity("解锁车票");
        }


    }
}
