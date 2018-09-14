using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.YAPublishPay
{
    class YAPublishPayPrintErrorDeal : Activity
    {
        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("雅安支付交易完成");
        }

        protected override void OnEnter()
        {
            GetElementById("Message").InnerText = "凭条打印失败";
            GetElementById("Ok").Click += Ok_Click;
        }
    }
}
