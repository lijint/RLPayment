using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using YAPayment.Entity;
using Landi.FrameWorks;

namespace YAPayment.Business.PowerCardPay.User
{
    class PowerPayInputUserAmountDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("UserAmount").InnerText = (GetBusinessEntity() as PowerEntity).UserPayAmount.ToString("#######0.00");
            GetElementById("Back").Click += new HtmlElementEventHandler(Back_Click);
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string amount = GetElementById("Amount").GetAttribute("value").Trim();
            if (amount.Length == 0 || double.Parse(amount) == 0)
            {
                GetElementById("ErrMsg").InnerText = "缴费金额不能为0!";
                GetElementById("ErrMsg").Style = "display:block";
                GetElementById("Amount").SetAttribute("value", "");
                return;
            }
            CommonData.Amount = double.Parse(amount);

            //用户缴费 缴费金额必须大于等于应缴金额
            PowerEntity pe = (GetBusinessEntity() as PowerEntity);
            if (CommonData.Amount < pe.UserPayAmount)
            {
                GetElementById("ErrMsg").InnerText = "缴费金额必须大于等于本次应交金额" + pe.UserPayAmount.ToString("#######0.00");
                GetElementById("ErrMsg").Style = "display:block";
                GetElementById("Amount").SetAttribute("value", "");
                return;
            }

            StartActivity("电力支付用户金额确认");
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付用户账单信息");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("ErrMsg").Style = "display:none";
            InputAmount("Amount", keyCode);
            base.OnKeyDown(keyCode);
        }
    }
}
