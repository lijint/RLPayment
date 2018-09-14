using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Management
{
    class ManageModifyPassDeal:Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string oldPass = GetElementById("oldPass").GetAttribute("value");
            string newPass = GetElementById("newPass").GetAttribute("value");
            string newPassConfirm = GetElementById("newPassConfirm").GetAttribute("value");
            if (Encrypt.AESEncrypt(oldPass, GlobalAppData.EncryptKey) != GlobalAppData.GetInstance().EntryPwd)
            {
                GetElementById("info").InnerText = "旧密码输入错误！";
                GetElementById("oldPass").SetAttribute("value", "");
                GetElementById("newPass").SetAttribute("value", "");
                GetElementById("newPassConfirm").SetAttribute("value", "");
                return;
            }
            if (newPass.Length != 6)
            {
                GetElementById("info").InnerText = "新密码长度错误！";
                GetElementById("oldPass").SetAttribute("value", "");
                GetElementById("newPass").SetAttribute("value", "");
                GetElementById("newPassConfirm").SetAttribute("value", "");
                return;
            }
            if (newPass != newPassConfirm)
            {
                GetElementById("info").InnerText = "新密码输入不一致！";
                GetElementById("oldPass").SetAttribute("value", "");
                GetElementById("newPass").SetAttribute("value", "");
                GetElementById("newPassConfirm").SetAttribute("value", "");
                return;
            }
            GlobalAppData.GetInstance().EntryPwd = Encrypt.AESEncrypt(newPass, GlobalAppData.EncryptKey);
            GetElementById("info").InnerText = "密码修改成功！";
            GetElementById("oldPass").SetAttribute("value", "");
            GetElementById("newPass").SetAttribute("value", "");
            GetElementById("newPassConfirm").SetAttribute("value", "");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("管理主界面");
        }
    }
}
