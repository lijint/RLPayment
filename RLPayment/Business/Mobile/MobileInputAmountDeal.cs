using System;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Mobile
{
    /// <summary>
    ///输入金额
    /// </summary>
    class MobileInputAmountDeal : Activity
    {
        protected override void OnEnter()
        {
            //GetElementById("Ok").Click += new HtmlElementEventHandler(Confirm_Click);
            GetElementById("Money1").Click += Money_Click;
            GetElementById("Money2").Click += Money_Click;
            GetElementById("Money3").Click += Money_Click;
            GetElementById("Return").Click += Return_Click;
            //GetElementById("InputAmountText").GotFocus += new HtmlElementEventHandler(InputAmountText_OnFocus);
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        private void Money_Click(object sender, HtmlElementEventArgs e)
        {
            string ID = "", amount = "";
            if (sender is HtmlElement)
                ID = (sender as HtmlElement).Id;
            else
                ID = (string)sender;
            switch (ID)
            {
                case "Money1":
                    amount = "50";
                    break;
                case "Money2":
                    amount = "100";
                    break;
                case "Money3":
                    amount = "200";
                    break;
            }

            CommonData.Amount = Convert.ToDouble(amount);
            Log.Info("话费充值金额：" + CommonData.Amount.ToString() + "元");

            if (CommonData.Amount > 0)
            {
                StartActivity("手机充值确认充值信息");
            }
        }

        void Confirm_Click(object sender, HtmlElementEventArgs e)
        {
            string amount = GetElementById("InputAmountText").GetAttribute("value");
            if (amount.Length == 0 || double.Parse(amount) == 0 || double.Parse(amount) % 50 != 0)
            {
                GetElementById("InputAmountText").SetAttribute("value", "");
                GetElementById("ErrMsg").Style = "display:block";
                return;
            }

            CommonData.Amount = Convert.ToDouble(amount);
            if (CommonData.Amount > 0)
            {
                StartActivity("手机充值确认充值信息");
            }
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            base.OnKeyDown(keyCode);

            string ID = "";
            switch (keyCode)
            {
                case Keys.D1:
                    {
                        ID = "Money1";
                        Money_Click(ID, null);
                    }
                    break;
                case Keys.D2:
                    {
                        ID = "Money2";
                        Money_Click(ID, null);
                    }
                    break;
                case Keys.D3:
                    {
                        ID = "Money3";
                        Money_Click(ID, null);
                    }
                    break;
            }
        }

    }
}
