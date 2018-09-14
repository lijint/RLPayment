using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using YAPayment.Entity;
using Landi.FrameWorks;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayInputAmountDeal : Activity
    {
        private PowerEntity _entity;
        protected override void OnEnter()
        {
            _entity = (PowerEntity)GetBusinessEntity();
            GetElementById("Amount").InnerText = _entity.PayAmount.ToString();
            GetElementById("Back").Click += new HtmlElementEventHandler(Back_Click);
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string amount = GetElementById("Amount").GetAttribute("value").Trim();
            if (amount.Length == 0 || double.Parse(amount) == 0)
            {
                GetElementById("ErrMsg").Style = "display:block";
                GetElementById("Amount").SetAttribute("value", "");
                return;
            }
            CommonData.Amount = double.Parse(amount);

            StartActivity("电力支付金额确认");
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("电力支付账单信息");  
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
