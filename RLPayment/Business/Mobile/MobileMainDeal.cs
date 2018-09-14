using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.Mobile
{
    /// <summary>
    /// 水电煤业务主菜单
    /// </summary>
    class MobileMainDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Cmcc").Click += Ele_Click;
            GetElementById("Unicom").Click += Ele_Click;
            GetElementById("TeleCom").Click += Ele_Click;

            GetElementById("Return").Click += Return_Click;
        }

        private void Ele_Click(object sender, HtmlElementEventArgs e)
        {
            string ID = "";
            if (sender is HtmlElement)
                ID = (sender as HtmlElement).Id;
            else
                ID = (string)sender;
            QMEntity entity = GetBusinessEntity() as QMEntity;
            switch (ID)
            {
                case "Cmcc":
                    entity.MobileType = "01";
                    break;
                case "Unicom":
                    entity.MobileType = "02";
                    break;
                case "TeleCom":
                    entity.MobileType = "03";
                    break;
            }
            Log.Info("手机充值：" + ID);
            StartActivity("手机充值输入手机号");
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            base.OnKeyDown(keyCode);

            string ID = "";
            switch (keyCode)
            {
                case Keys.D1:
                    {
                        ID = "Cmcc";
                        Ele_Click(ID, null);
                    }
                    break;
                case Keys.D2:
                    {
                        ID = "Unicom";
                        Ele_Click(ID, null);
                    }
                    break;
                case Keys.D3:
                    {
                        ID = "TeleCom";
                        Ele_Click(ID, null);
                    }
                    break;
            }
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

    }
}
