using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using YAPayment.Entity;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayBillInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            PowerEntity entity = GetBusinessEntity() as PowerEntity;
            GetElementById("UserNo").InnerText = entity.UserID.Trim();
            GetElementById("UserName").InnerText = entity.UserName;
            //GetElementById("Address").InnerText = entity.UserAddress;
            GetElementById("PowerCardNo").InnerText = entity.PowerCardNo;
            //GetElementById("PowerNo").InnerText = entity.DBNo;
            GetElementById("PayAmount").InnerText = entity.PayAmount.ToString("####0.00");
            //GetElementById("LimitAmount").InnerText = entity.LimitAmount.ToString("####0.00");

            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }


        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付输入缴费金额");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
