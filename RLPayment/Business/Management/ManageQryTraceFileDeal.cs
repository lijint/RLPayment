using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Package;

namespace YAPayment.Business.Management
{
    class ManageQryTraceFileDeal:Activity
    {
        protected override void OnEnter()
        {
            getconfirminfo();
            GetElementById("Return").Click += Return_Click;
        }

        private void getconfirminfo()
        {
            try
            {
                string failPath = Path.Combine(Application.StartupPath, "PetroConfirmFailInfo.dat");
                List<ConfirmFailInfo> list = new List<ConfirmFailInfo>();
                Utility.RestoreFromFile(failPath, list);
                string temp = "";
                foreach (ConfirmFailInfo item in list)
                    temp += item.ToString();
                if (string.IsNullOrEmpty(temp))
                    temp = "无";

                GetElementById("info").SetAttribute("value", temp);
            }
            catch (Exception ex)
            {
                Log.Error("[ManageQryTraceFileDeal][getconfirminfo]Error", ex);
            }
        }


        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("管理主界面");
        }
    }
}
