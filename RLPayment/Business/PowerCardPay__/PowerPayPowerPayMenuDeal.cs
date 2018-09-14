using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using YAPayment.Entity;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayPowerPayMenuDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Card").Click += new HtmlElementEventHandler(Pay_Click);
            GetElementById("User").Click += new HtmlElementEventHandler(WriteCard_Click);

            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Pay_Click(object sender, HtmlElementEventArgs e)
        {
            (GetBusinessEntity() as PowerEntity).PowerBusiness = 1;
            StartActivity("电力支付插入电卡");
        }

        private void WriteCard_Click(object sender, HtmlElementEventArgs e)
        {
            (GetBusinessEntity() as PowerEntity).PowerBusiness = 0;
            StartActivity("电力支付输入用户号");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            base.OnKeyDown(keyCode);

            switch (keyCode)
            {
                case Keys.D1:
                    {
                        WriteCard_Click(null, null);
                    }
                    break;
                case Keys.D2:
                    {
                        Pay_Click(null, null);
                    }
                    break;
            }
        }
    }
}
