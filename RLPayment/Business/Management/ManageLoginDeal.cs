using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Management
{
    class ManageLoginDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string passWord = GetElementById("Password").GetAttribute("value").Trim();
            if (passWord.Length < 6)
            {
                GetElementById("Password").SetAttribute("value", "");
                return;
            }

            //校验密码
            if (Encrypt.AESEncrypt(passWord, GlobalAppData.EncryptKey) == GlobalAppData.GetInstance().EntryPwd)
            {
                StartActivity("管理主界面");
            }
            else
            {
                GetElementById("Password").SetAttribute("value", "");
            }
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            base.OnKeyDown(keyCode);
            InputNumber("Password", keyCode);
        }

        protected override void OnTimeOut()
        {
            StartActivity("初始化");
        }
    }
}
