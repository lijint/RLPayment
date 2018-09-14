using System;
using System.Collections.Generic;
using System.Text;
using InputChaIphoneLib;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.TrafficPolice
{
    class InputInqueryParamDeal : Activity
    {
        private YAEntity _entity = null;

        protected override void OnEnter()
        {
            _entity = GetBusinessEntity() as YAEntity;
            GetElementById("license").InnerText = ConfigFile.ReadConfigAndCreate("AppData", "LicensePlate", "川");
            GetElementById("Ok").Click += new System.Windows.Forms.HtmlElementEventHandler(OK_Click);
            GetElementById("Return").Click += new System.Windows.Forms.HtmlElementEventHandler(Return_Click);
            GetElementById("carNO").Focus();
            GetElementById("carNO").GotFocus += new System.Windows.Forms.HtmlElementEventHandler(GotFocusString);
            GetElementById("carID").GotFocus += new System.Windows.Forms.HtmlElementEventHandler(GotFocusInt);
            GetElementById("licenseNo").GotFocus += new System.Windows.Forms.HtmlElementEventHandler(GotFocusString);
            
            GetElementById("carID").LostFocus += new System.Windows.Forms.HtmlElementEventHandler(LostFocus);
            GetElementById("carNO").LostFocus += new System.Windows.Forms.HtmlElementEventHandler(LostFocus);
            GetElementById("licenseNo").LostFocus += new System.Windows.Forms.HtmlElementEventHandler(LostFocus);
        }

        void LostFocus(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            frmInputCha.Instanse.Hide();
            numberInput.Instanse.Hide();
        }

        void GotFocusString(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            frmInputCha.Instanse.Show();
            GetElementById("ErrMsg").InnerText = "";//清空错误提示信息
        }


        void GotFocusInt(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            numberInput.Instanse.Show();
            GetElementById("ErrMsg").InnerText = "";//清空错误提示信息
        }

        void Return_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            GotoMain();
            frmInputCha.Instanse.Hide();
            numberInput.Instanse.Hide();
        }

        void OK_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            string licensePlant = GetElementById("carNO").GetAttribute("value");
            string carID = GetElementById("carID").GetAttribute("value");
            string typeValue = GetElementById("carType").GetAttribute("value");
            string licenseNo = GetElementById("licenseNo").GetAttribute("value");
            string errmessage = "";
            if (string.IsNullOrEmpty(licensePlant))
                errmessage += "/车牌号不能为空";
            if (string.IsNullOrEmpty(carID))
                errmessage += "/车架号不能为空";
            if (string.IsNullOrEmpty(licenseNo))
                errmessage += "/驾驶证号不能为空";
            if (!string.IsNullOrEmpty(errmessage))
            {
                GetElementById("ErrMsg").InnerText = errmessage.Substring(1)+"!";
                return;
            }

            _entity.LicensePlant = GetElementById("license").InnerText	+ licensePlant;
            _entity.CarId = carID;
            _entity.CarType = typeValue;
            _entity.LicenseNo = licenseNo;
            StartActivity("雅安交警认罚正在查询");
        }

        protected override void OnLeave()
        {
            frmInputCha.Instanse.Hide();
            numberInput.Instanse.Hide();
            base.OnLeave();
        }
    }
}
