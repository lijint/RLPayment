using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.CarTicket
{
    class TicketRevokeInputDeal:Activity
    {
        private CarEntity _carEntity;

        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
            _carEntity = GetBusinessEntity<CarEntity>();
        }

        private void Ok_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            _carEntity.TicketId = GetElementById("InputText").GetAttribute("value");
            if (string.IsNullOrEmpty(_carEntity.TicketId))
            {
                GetElementById("ErrMsg").InnerText = "车票ID不能为空！";
            }
        }

        private void Return_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            StartActivity("车票预订主画面");
        }
    }
}
