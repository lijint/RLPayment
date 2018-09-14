using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.Mobile
{
    class MobileInputPhoneAgainDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Confirm_Click;
            GetElementById("Return").Click += Return_Click;
            GetElementById("InputText").GotFocus += InputText_OnFocus;
        }

        public void InputText_OnFocus(object sender, HtmlElementEventArgs e)
        {
            GetElementById("InputText").Focus();
            GetElementById("ErrMsg").Style = "display:none";
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        void Confirm_Click(object sender, HtmlElementEventArgs e)
        {
            string phone = GetElementById("InputText").GetAttribute("value");
            if (phone.Length != 11)
            {
                GetElementById("InputText").SetAttribute("value", "");
                GetElementById("ErrMsg").Style = "display:block";
                return;
            }
            if (phone != (GetBusinessEntity() as QMEntity).PhoneNo)
            {
                StartActivity("手机充值两次手机输入不一样");
            }
            else
            {
                StartActivity("手机充值输入充值金额");
            }
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("ErrMsg").Style = "display:none";
            InputNumber("InputText", keyCode);
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
