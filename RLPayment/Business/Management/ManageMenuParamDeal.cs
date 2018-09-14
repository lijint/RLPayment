using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Management
{
    class ManageMenuParamDeal:Activity
    {
        private const string Hide= "display:none;";
        private const string Show = "display:block;";
        private MenuParam _param;
        protected override void OnEnter()
        {
            GetElementById("Return").Click += Return_Click;
            GetElementById("Ok").Click += OK_Click;
            GetElementById("gas").Click += ManageMenuParamDeal_Click;
            GetElementById("water").Click += ManageMenuParamDeal_Click;
            GetElementById("TV").Click += ManageMenuParamDeal_Click;
            GetElementById("EleFee").Click += ManageMenuParamDeal_Click;
            GetElementById("TelFee").Click += ManageMenuParamDeal_Click;
            GetElementById("CreditCard").Click += ManageMenuParamDeal_Click;
            GetElementById("TrafficFee").Click += ManageMenuParamDeal_Click;
            GetElementById("CarTicket").Click += ManageMenuParamDeal_Click;


            _param = new MenuParam();
            _param.Gas = ConfigFile.ReadConfigAndCreate("AppData", "Gas", "1");
            _param.Water = ConfigFile.ReadConfigAndCreate("AppData", "Water", "1");
            _param.TV = ConfigFile.ReadConfigAndCreate("AppData", "TV", "1");
            _param.Power = ConfigFile.ReadConfigAndCreate("AppData", "Power", "1");
            _param.Mobile = ConfigFile.ReadConfigAndCreate("AppData", "Mobile", "1");
            _param.CreditCard = ConfigFile.ReadConfigAndCreate("AppData", "CreditCard", "1");
            _param.TrafficPolice = ConfigFile.ReadConfigAndCreate("AppData", "TrafficPolice", "1");
            _param.CarTicket = ConfigFile.ReadConfigAndCreate("AppData", "CarTicket", "1");

            SetSelectState("gas", _param.Gas);
            SetSelectState("water", _param.Water);
            SetSelectState("TV", _param.TV);
            SetSelectState("EleFee", _param.Power);
            SetSelectState("TelFee", _param.Mobile);
            SetSelectState("CreditCard", _param.CreditCard);
            SetSelectState("TrafficFee", _param.TrafficPolice);
            SetSelectState("CarTicket", _param.CarTicket);
        }

        void ManageMenuParamDeal_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            HtmlElement ele = sender as HtmlElement;
            if(ele==null) return;
            if (ele.Style.ToLower().Contains("silver"))
            {
                ele.Style=ele.Style.Replace("silver", "orange");
                SetValue(ele, "1");
            }
            else
            {
                ele.Style = ele.Style.Replace("orange", "silver");
                SetValue(ele, "0");
            }
        }

        private void SetValue(HtmlElement ele, string value)
        {
            switch (ele.Id)
            {
                case "gas":
                    _param.Gas = value;
                    break;
                case "water":
                    _param.Water = value;
                    break;
                case "TV":
                    _param.TV = value;
                    break;
                case "EleFee":
                    _param.Power = value;
                    break;
                case "TelFee":
                    _param.Mobile = value;
                    break;
                case "CreditCard":
                    _param.CreditCard = value;
                    break;
                case "TrafficFee":
                    _param.TrafficPolice = value;
                    break;
                case "CarTicket":
                    _param.CarTicket = value;
                    break;
            }
        }



        void OK_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            ConfigFile.WriteConfig("AppData", "Gas", _param.Gas);
            ConfigFile.WriteConfig("AppData", "Water", _param.Water);
            ConfigFile.WriteConfig("AppData", "TV", _param.TV);
            ConfigFile.WriteConfig("AppData", "Power", _param.Power);
            ConfigFile.WriteConfig("AppData", "Mobile", _param.Mobile);
            ConfigFile.WriteConfig("AppData", "CreditCard", _param.CreditCard);
            ConfigFile.WriteConfig("AppData", "TrafficPolice", _param.TrafficPolice);
            ConfigFile.WriteConfig("AppData", "CarTicket", _param.CarTicket);
            MenuEnable.MenuEnableClear();
            GetElementById("info").InnerText = "保存成功！";
        }

        protected void SetSelectState(string Id, string SelectValue)
        {
            if (SelectValue == "1")
                GetElementById(Id).Style = GetElementById(Id).Style.ToLower().Replace("silver", "orange");
            else
                GetElementById(Id).Style = GetElementById(Id).Style.ToLower().Replace("orange", "silver");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("管理主界面");
        }
    }

    public class MenuParam
    {
        public string Gas;
        public string Water;
        public string TV;
        public string Power;
        public string Mobile;
        public string CreditCard;
        public string TrafficPolice;
        public string CarTicket;
    }
}
