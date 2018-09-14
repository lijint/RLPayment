using System;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Creditcard  
{
    class CreditcardInputAmountDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Confirm_Click;
            GetElementById("Return").Click += Return_Click;
            GetElementById("InputAmountText").GotFocus += InputAmountText_OnFocus;
            GetElementById("InputAmountText").Focus();
        }

        public void InputAmountText_OnFocus(object sender, HtmlElementEventArgs e)
        {
            GetElementById("ErrMsg").Style = "display:none";
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }

        void Confirm_Click(object sender, HtmlElementEventArgs e)
        {
            try
            {
                //CommonData.Amount = 10;
                //StartActivity("信用卡还款插入银行卡");
                //return;
                string amount = GetElementById("InputAmountText").GetAttribute("value");
                if (amount.Length == 0 || double.Parse(amount) == 0)
                {
                    GetElementById("InputAmountText").SetAttribute("value", "");
                    GetElementById("ErrMsg").Style = "display:block";
                    return;
                }
                CommonData.Amount = double.Parse(amount);
                StartActivity("信用卡还款插入银行卡");
            }
            catch (Exception)
            {

            }
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("ErrMsg").Style = "display:none";
            InputAmount("InputAmountText", keyCode);
            switch (keyCode)
            {
                case Keys.Enter:
                    GetElementById("Ok").InvokeMember("Click");
                    break;
                case Keys.Escape:
                    GetElementById("Return").InvokeMember("Click");
                    break;
            }
        }
    }
}
