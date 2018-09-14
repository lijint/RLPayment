using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceInputDecisionNoDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string DecisionNo = GetElementById("DecisionNo").GetAttribute("value").Trim();

            if (!DecisionNo.StartsWith("51"))
            {
                GetElementById("ErrMsg").InnerText = "决定书编号必须是以51开头!";
                GetElementById("ErrMsg").Style = "display:block";
                return;
            }

            if (DecisionNo.Length != 15)
            {
                GetElementById("ErrMsg").InnerText = "决定书编号长度不足15位!";
                GetElementById("ErrMsg").Style = "display:block";
                return;
            }

            (GetBusinessEntity() as YAEntity).TPDecisionNo = DecisionNo;
            StartActivity("雅安交警罚没违章查询");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("ErrMsg").Style = "display:none";
            InputNumber("DecisionNo", keyCode);
            base.OnKeyDown(keyCode);
        }
    }
}
