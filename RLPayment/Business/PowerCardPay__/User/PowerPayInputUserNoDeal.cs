using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using YAPayment.Entity;
using Landi.FrameWorks;

namespace YAPayment.Business.PowerCardPay.User
{
    class PowerPayInputUserNoDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string DecisionNo = GetElementById("UserNo").GetAttribute("value").Trim();

            if (DecisionNo.Length == 0)
            {
                GetElementById("ErrMsg").InnerText = "用户号不能为空";
                GetElementById("ErrMsg").Style = "display:block";
                return;
            }


            (GetBusinessEntity() as PowerEntity).DBNo = DecisionNo;//"4446318730";//DecisionNo;//Test
            StartActivity("电力支付用户账单查询");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("ErrMsg").Style = "display:none";
            InputNumber("UserNo", keyCode);
            base.OnKeyDown(keyCode);
        }
    }
}
