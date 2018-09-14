using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using YAPayment.Package;

namespace YAPayment.Business.PetroPay
{
    class PetroPayUserInputPasswordDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string passWord = GetElementById("Password").GetAttribute("value").Trim();
            if (string.IsNullOrEmpty(passWord))
            {
                GetElementById("ErrMsg").Style = "display:block;";
                return;
            }
            YAPaymentPay.LoginPsd = Convert.ToBase64String(Encoding.Default.GetBytes(passWord));
            StartActivity("中石油支付用户信息查询");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("ErrMsg").Style = "display:none;";
            InputNumber("Password", keyCode);
        }
    }
}
