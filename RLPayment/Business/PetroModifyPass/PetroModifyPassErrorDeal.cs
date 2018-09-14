using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace YAPayment.Business.PetroModifyPass
{
    class PetroModifyPassErrorDeal : MessageActivity
    {
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }

        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("中石油修改密码主界面");
        }

        protected override void DoMessage(object message)
        {
            GetElementById("Message").InnerText = (string)message;
            if (IsBack)
            {
                GetElementById("Ok").Style = "display:block;";
                GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            }
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);

        }
    }
}
