using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.YAPublishPay
{
    class YAPublishPayConfirmPayInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("PayAMount").InnerText = CommonData.Amount.ToString("########0.00");
  
            GetElementById("Back").Click += Back_Click;
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            switch ((GetBusinessEntity() as YAEntity).PublishPayType)
            {
                case YaPublishPayType.Gas:
                    StartActivity("雅安气费账单信息");
                    break;
                case YaPublishPayType.Water:
                    StartActivity("雅安水费账单信息");
                    break;
                case YaPublishPayType.Power:
                    break;
                case YaPublishPayType.TV:
                    StartActivity("雅安广电费选择包月类型");
                    break;
            }
            
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("雅安支付插入银行卡");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
