using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.YAPublishPay
{
    class YAPublishPayMenuDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Gas").Click += Ele_Click;
            GetElementById("Water").Click += Ele_Click;
            //GetElementById("Power").Click += new HtmlElementEventHandler(Ele_Click);
            GetElementById("TV").Click += Ele_Click;

            GetElementById("Return").Click += Return_Click;
        }

        private void Ele_Click(object sender, HtmlElementEventArgs e)
        {
            HtmlElement ele = (HtmlElement)sender;
            YAEntity entity = GetBusinessEntity() as YAEntity;
            switch (ele.Id)
            {
                case "Gas":
                    entity.PublishPayType = YaPublishPayType.Gas;
                    break;
                case "Water":
                    entity.PublishPayType = YaPublishPayType.Water;
                    break;
                case "Power":
                    entity.PublishPayType = YaPublishPayType.Power;
                    break;
                case "TV":
                    entity.PublishPayType = YaPublishPayType.TV;
                    break;
            }
            Log.Info("雅安公共事业缴费：" + entity.PublishPayType);
            StartActivity("雅安支付输入用户号");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
