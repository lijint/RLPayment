using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using InputChaIphoneLib;
using Landi.FrameWorks;
using YAPayment.Package;

namespace YAPayment.Business.PetroPay
{
    class PetroPayUserLoginDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
            GetElementById("Password").GotFocus += new HtmlElementEventHandler(Password_OnFocus);
            GetElementById("UserName").Click += new HtmlElementEventHandler(PetroPayUserLoginDeal_Click);
            GetElementById("UserName").LosingFocus += new HtmlElementEventHandler(PetroPayUserLoginDeal_LosingFocus);
            frmInputChaIphone.Instanse.Show();
            GetElementById("UserName").Focus();
            GetElementById("UserName").Focus();
        }

        void PetroPayUserLoginDeal_LosingFocus(object sender, HtmlElementEventArgs e)
        {
            frmInputChaIphone.Instanse.Hide();
        }

        void PetroPayUserLoginDeal_Click(object sender, HtmlElementEventArgs e)
        {
            GetElementById("Notice").Style = "display:none";
            GetElementById("ErrMsg").Style = "display:none";
            frmInputChaIphone.Instanse.Show();
        }

        public void Password_OnFocus(object sender, HtmlElementEventArgs e)
        {
            GetElementById("Notice").Style = "display:block";
            GetElementById("ErrMsg").Style = "display:none";
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string userName = GetElementById("UserName").GetAttribute("value").Trim();
            string passWord = GetElementById("Password").GetAttribute("value").Trim();
            if (userName.Length == 0 || passWord.Length == 0)
            {
                GetElementById("ErrMsg").Style = "display:block";
                return;
            }

            YAPaymentPay.LoginName = userName;
            YAPaymentPay.LoginPsd = Convert.ToBase64String(Encoding.Default.GetBytes(passWord));
            StartActivity("中石油支付用户信息查询");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnLeave()
        {
            frmInputChaIphone.Instanse.Hide();
        }
    }
}
