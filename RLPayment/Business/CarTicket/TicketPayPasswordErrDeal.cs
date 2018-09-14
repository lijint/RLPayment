using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.CarTicket
{
    class TicketPayPasswordErrDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Retry").Click += Retry_click;
            GetElementById("Return").Click += Return_click;
        }

        public void Retry_click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("购票输入密码");
        }

        public void Return_click(object sender, HtmlElementEventArgs e)
        {
             GetBusinessEntity<CarEntity>().UnlockMessage = "用户返回，";
            StartActivity("解锁车票");
        }
    }
}
