using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;
using System.Windows.Forms;

namespace YAPayment.Business.PowerCardPay.User
{
    class PowerPayUserBillInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            PowerEntity entity = GetBusinessEntity() as PowerEntity;
            GetElementById("UserName").InnerText = entity.UserName;
            GetElementById("Address").InnerText = entity.UserAddress;
            GetElementById("PayDate").InnerText = entity.PowerDate;
            GetElementById("PayAmount").InnerText = entity.UserPayAmount.ToString("######0.00") + " 元";
            GetElementById("PayCount").InnerText = entity.PowerPayCount.ToString() + " 笔";

            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }


        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            CommonData.Amount = (GetBusinessEntity() as PowerEntity).UserPayAmount;
            StartActivity("电力支付用户金额确认");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
