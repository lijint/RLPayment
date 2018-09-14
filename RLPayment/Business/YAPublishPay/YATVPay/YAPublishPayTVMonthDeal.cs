using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;
using System.Windows.Forms;

namespace YAPayment.Business.YAPublishPay.YATVPay
{
    class YAPublishPayTVMonthDeal :Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Amount").InnerText = (GetBusinessEntity() as YAEntity).QueryAmount.ToString("######0.00") + "元";
            GetElementById("Money").InnerText = (GetBusinessEntity() as YAEntity).SelectPrice.ToString("######0.00") + "/月";

            GetElementById("Month1").Click += new HtmlElementEventHandler(Month_Click);
            GetElementById("Month2").Click += new HtmlElementEventHandler(Month_Click);
            GetElementById("Month3").Click += new HtmlElementEventHandler(Month_Click);

            GetElementById("Back").Click += new HtmlElementEventHandler(Back_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Month_Click(object sender, HtmlElementEventArgs e)
        {
            string ID = "";
            if (sender is HtmlElement)
                ID = (sender as HtmlElement).Id;
            else
                ID = (string)sender;
            YAEntity entity = GetBusinessEntity() as YAEntity;
            switch (ID)
            {
                case "Month1":
                    entity.SelectMonth = 6;
                    break;
                case "Month2":
                    entity.SelectMonth = 9;
                    break;
                case "Month3":
                    entity.SelectMonth = 12;
                    break;
            }
            Log.Info("广电缴费选择预包月类型：" + entity.SelectMonth.ToString() + "个月");

            CommonData.Amount = entity.SelectMonth * entity.SelectPrice + entity.QueryAmount;
            StartActivity("雅安支付金额确认");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("雅安广电费选择缴费类型");
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            base.OnKeyDown(keyCode);

            string ID = "";
            switch (keyCode)
            {
                case Keys.D1:
                    {
                        ID = "Month1";
                        Month_Click(ID, null);
                    }
                    break;
                case Keys.D2:
                    {
                        ID = "Month2";
                        Month_Click(ID, null);
                    }
                    break;
                case Keys.D3:
                    {
                        ID = "Month3";
                        Month_Click(ID, null);
                    }
                    break;
            }
        }
    }
}
