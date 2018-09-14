using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace YAPayment.Business.PetroModifyPass
{
    class PetroModifyPassSuccessDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("Ö÷½çÃæ");
        }
    }
}
