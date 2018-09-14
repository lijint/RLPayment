using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PetroPay
{
    /// <summary>
    /// 交易成功是否打印
    /// </summary>
    class PetroPaySuccessDeal : Activity
    {
        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("中石油支付用户信息显示");
        }

        void Print_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("中石油支付正在打印");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnEnter()
        {
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Print").Click += new HtmlElementEventHandler(Print_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }
    }
}
