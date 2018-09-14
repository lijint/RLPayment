using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using YAPayment.Package;
using InputChaIphoneLib;
using Landi.FrameWorks.HardWare;

namespace YAPayment.Business.PetroModifyPass
{
    class PetroModifyPassMainDeal : Activity, ITimeTick
    {
        protected override void OnEnter()
        {
            GetElementById("UserName").LostFocus += new HtmlElementEventHandler(PetroModifyPassMainDeal_LostFocus);
            GetElementById("UserName").GotFocus += new HtmlElementEventHandler(PetroModifyPassMainDeal_GotFocus);
            GetElementById("OldPassword").GotFocus += new HtmlElementEventHandler(PetroModifyPassMainDeal_GotFocus);
            GetElementById("NewPassword").GotFocus += new HtmlElementEventHandler(PetroModifyPassMainDeal_GotFocus);
            GetElementById("NewPasswordConfirm").GotFocus += new HtmlElementEventHandler(PetroModifyPassMainDeal_GotFocus);
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        void PetroModifyPassMainDeal_GotFocus(object sender, HtmlElementEventArgs e)
        {
            HtmlElement ele = (HtmlElement)sender;
            if (ele.Id == "UserName")
            {
                frmInputChaIphone.Instanse.Show();
                GetElementById("Notice").Style = "display:none";
            }
            else
                GetElementById("Notice").Style = "display:block";
            GetElementById("ErrMsg").InnerText = "";
        }

        void PetroModifyPassMainDeal_LostFocus(object sender, HtmlElementEventArgs e)
        {
            GetElementById("ErrMsg").InnerText = "";
            frmInputChaIphone.Instanse.Hide();
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string userName = GetElementById("UserName").GetAttribute("value").Trim(); ;
            string oldPass = GetElementById("OldPassword").GetAttribute("value").Trim();
            string newPass = GetElementById("NewPassword").GetAttribute("value").Trim();
            string newPassC = GetElementById("NewPasswordConfirm").GetAttribute("value").Trim();
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(oldPass) ||
                string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(newPassC))
            {
                GetElementById("ErrMsg").InnerText = "用户名或新旧密码不能为空！";
                GetElementById("Notice").Style = "display:none";
                return;
            }
            if (newPass != newPassC)
            {
                GetElementById("ErrMsg").InnerText = "新密码输入不一致！";
                GetElementById("Notice").Style = "display:none";
                GetElementById("NewPassword").SetAttribute("value", "");
                GetElementById("NewPasswordConfirm").SetAttribute("value", "");
                return;
            }

            YAPaymentPay.LoginName = userName;
            YAPaymentPay.LoginPsd = Convert.ToBase64String(Encoding.Default.GetBytes(oldPass));
            YAPaymentPay.LoginNewPsd = Convert.ToBase64String(Encoding.Default.GetBytes(newPass));
            StartActivity("中石油修改密码正在交易");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }

        protected override void OnLeave()
        {
            frmInputChaIphone.Instanse.Hide();
        }

        #region ITimeTick 成员

        public void OnTimeTick(int count)
        {
            lock (this)
            {
                try
                {
                    string mCardNo = "";
                    R80.ActivateResult ret = R80.ICActive(0, ref mCardNo);
                    if (ret == R80.ActivateResult.ET_SETSUCCESS &&
                        mCardNo.Length > 0)
                    {
                        GetElementById("UserName").SetAttribute("value", "0000" + mCardNo);
                        GetElementById("OldPassword").Focus();
                        GetElementById("OldPassword").Focus();
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("[PetroModifyPassMainDeal][OnTimeTick]Error", ex);
                }
            }
        }

        #endregion
    }
}
