using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Business.PetroPay
{
    class PetroPaySelectWayDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Brush").Click += new System.Windows.Forms.HtmlElementEventHandler(Brush_Click);
            GetElementById("Hand").Click += new System.Windows.Forms.HtmlElementEventHandler(Hand_Click);
            GetElementById("Return").Click += new System.Windows.Forms.HtmlElementEventHandler(Return_Click);
        }

        void Brush_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            StartActivity("中石油支付出示用户卡");
        }

        void Hand_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            StartActivity("中石油支付用户登录");
        }

        void Return_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
