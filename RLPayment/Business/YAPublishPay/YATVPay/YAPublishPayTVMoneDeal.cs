using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;
using System.Windows.Forms;

namespace YAPayment.Business.YAPublishPay.YATVPay
{
    class YAPublishPayTVMoneDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Money1").Click += new HtmlElementEventHandler(Money_Click);
            GetElementById("Money2").Click += new HtmlElementEventHandler(Money_Click);

            GetElementById("Back").Click += new HtmlElementEventHandler(Back_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Money_Click(object sender, HtmlElementEventArgs e)
        {
            string ID = "";
            if (sender is HtmlElement)
                ID = (sender as HtmlElement).Id;
            else
                ID = (string)sender;
            YAEntity entity = GetBusinessEntity() as YAEntity;
            switch (ID)
            {
                case "Money1":
                    entity.SelectPrice = 25.00;
                    break;
                case "Money2":
                    entity.SelectPrice = 30.00;
                    break;
            }
            Log.Info("广电缴费选择预缴费类型：" + entity.SelectPrice.ToString("#####0.00") + "/月");
            StartActivity("雅安广电费选择包月类型");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("雅安广电费账单信息");
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
            }
        }
    }
}
