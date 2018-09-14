using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceInputPayTraceNoDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string TraceNo = GetElementById("TraceNo").GetAttribute("value").Trim();


            //if (DecisionNo.Length != 12)
            //{
            //    GetElementById("ErrMsg").Style = "display:block";
            //    return;
            //}

            (GetBusinessEntity() as YAEntity).TPPayFlowNo = TraceNo;
            StartActivity("雅安交警罚正在核销");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("ErrMsg").Style = "display:none";
            InputNumber("TraceNo", keyCode);
            base.OnKeyDown(keyCode);
        }
    }
}
