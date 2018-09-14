using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InputChaIphoneLib;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;

namespace YAPayment.Business.CarTicket
{
    class CarPassengerInfoDeal:Activity
    {
        private CarEntity _carEntity;
        private List<PassangerData> _passangerList;
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
            _passangerList = new List<PassangerData>();
            _carEntity = GetBusinessEntity<CarEntity>();
            TicketLines line = _carEntity.SelectLine;
            GetElementById("bus_type_name").InnerText = line.BusTypeName;
            GetElementById("sch_id").InnerText = line.SchId;
            GetElementById("service_price").InnerText = line.ServicePrice;
            GetElementById("mile").InnerText = line.Mile;
            GetElementById("full_price").InnerText = line.FullPrice;
            GetElementById("riding_date").InnerText = _carEntity._BstTicketByCityRequest.RidingDate;
            GetElementById("city").InnerText = line.City;
            GetElementById("carry_sta_name").InnerText = line.CarryStaName;
            GetElementById("drv_date_time").InnerText = line.DrvDateTime;
            GetElementById("sch_type_name").InnerText = line.SchTypeName;
            GetElementById("stop_name").InnerText = line.StopName;
            InitPassagerView();
            GetElementById("confirm").Click += Confirm_Click;
            GetElementById("cancel").Click += Cancel_Click;

            GetElementById("passName").GotFocus += frmInput_GotFocus;
            GetElementById("passName").LostFocus += frmInput_LostFocus;
            GetElementById("cerNumValue").GotFocus += numberInput_GotFocus;
            GetElementById("cerNumValue").LostFocus += numberInput_LostFocus;
            GetElementById("passTel").GotFocus += numberInput_GotFocus;
            GetElementById("passTel").LostFocus += numberInput_LostFocus;
        }

        void frmInput_LostFocus(object sender, HtmlElementEventArgs e)
        {
            frmInputCha.Instanse.Hide();
        }

        void frmInput_GotFocus(object sender, HtmlElementEventArgs e)
        {
            frmInputCha.Instanse.SelectLanguage("拼音");
            frmInputCha.Instanse.Show();
        }
        void numberInput_LostFocus(object sender, HtmlElementEventArgs e)
        {
            numberInput.Instanse.Hide();
        }
        void numberInput_GotFocus(object sender, HtmlElementEventArgs e)
        {
            numberInput.Instanse.Show();
        }
        protected override void OnLeave()
        {
            base.OnLeave();
            frmInputCha.Instanse.Hide();
            numberInput.Instanse.Hide();
        }
        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            _carEntity._BstLockTicketRequest = new BstLockTicketRequest();
            _carEntity._BstLockTicketRequest.CarryStaId = _carEntity.CarryStaId;
            _carEntity._BstLockTicketRequest.StrDate = _carEntity.SelectLine.DrvDateTime;
            _carEntity._BstLockTicketRequest.SignId = _carEntity.SelectLine.SignId;
            _carEntity._BstLockTicketRequest.StopName = _carEntity.SelectLine.StopName;
            _carEntity._BstLockTicketRequest.OpenId = "";
            _carEntity._BstLockTicketRequest.BuyTicketInfo = GetBuyTicketInfo();
            if (_passangerList.Count == 0)
            {
                GetElementById("message").InnerText = "请添加乘客，最多5人！";
                return;
            }
            StartActivity("正在锁票");
        }

        private string GetBuyTicketInfo()
        {
            string result = "";
            for (int i = 0; i < _passangerList.Count; i++)
            {
                result += string.Format("{0}|{1}|{2}|{3}|{4}$", _passangerList[i].IdCard, _passangerList[i].Name,
                    _passangerList[i].TelNum, _passangerList[i].TicketType, _passangerList[i].CardType);
            }
            if (!string.IsNullOrEmpty(result))
                result = result.Substring(0, result.Length - 1);
            return result;
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("车票预订主画面");
        }

        void Cancel_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            InvokeScript("hideElement", new object[] { "addPassenger" });
            InvokeScript("hideElement", new object[] { "cover" });
        }
        void Confirm_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {



            PassangerData pass = new PassangerData();
            pass.TicketType = GetElementById("passType").GetAttribute("value");
            pass.TelNum = GetElementById("passTel").GetAttribute("value");
            pass.Name = GetElementById("passName").GetAttribute("value");
            if (string.IsNullOrEmpty(pass.Name) || string.IsNullOrEmpty(pass.TelNum))
            {
                GetElementById("msg_pas").InnerText = "请输入全部数据！";
                return;
            }
            if (pass.TicketType == "0")
            {
                pass.CardType = GetElementById("cerTypeValue").GetAttribute("value");
                pass.IdCard = GetElementById("cerNumValue").GetAttribute("value");
                if (string.IsNullOrEmpty(pass.IdCard))
                {
                    GetElementById("msg_pas").InnerText = "请输入全部数据！";
                    return;
                }
            }


            GetElementById("cerNumValue").InnerText = "";
            GetElementById("passTel").InnerText = "";
            GetElementById("passName").InnerText = "";

            _passangerList.Add(pass);
            InvokeScript("hideElement", new object[] { "addPassenger" });
            InvokeScript("hideElement", new object[] { "cover" });

            InitPassagerView();
        }

        private void InitPassagerView()
        {
            int count = _passangerList.Count;
            string html = "";
            if (count == 0)
            {
                html =
                    "<table class=\"table_passager\"><tr><td>XXXXX</td><td>XXXXX</td><td>XXXXX</td><td>XXXXX</td><td id=\"addP\" class=\"addP\"></td><td id=\"delP0\" class=\"delP\"></td></tr></table>";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                if (count == 5)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        string format =
                            "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td></td><td id=\"delP{5}\" class=\"delP\"></td></tr>";
                        sb.Append(string.Format(format, _passangerList[i].Name, _passangerList[i].TelNum,
                            _passangerList[i].TicketTypeName, _passangerList[i].CardTypeName, _passangerList[i].IdCard,
                            i));
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        string format = i == (count - 1)
                            ? "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td id=\"addP\" class=\"addP\"></td><td id=\"delP{5}\" class=\"delP\"></td></tr>"
                            : "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td></td><td id=\"delP{5}\" class=\"delP\"></td></tr>";
                        sb.Append(string.Format(format, _passangerList[i].Name, _passangerList[i].TelNum,
                            _passangerList[i].TicketTypeName, _passangerList[i].CardTypeName, _passangerList[i].IdCard,
                            i));
                    }
                }
                html = "<table class=\"table_passager\">" + sb.ToString() + "</table>";
            }
            GetElementById("passengerDetial").InnerHtml = html;
            if (GetElementById("addP") != null)
                GetElementById("addP").Click += AddPassanger_Click;
            for (int i = 0; i < count; i++)
            {
                GetElementById("delP"+i).Click += DelPassanger_Click;
            }
        }

        private void DelPassanger_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            try
            {
                HtmlElement ele = sender as HtmlElement;
                if (ele == null) return;
                int delIndex = int.Parse(ele.Id.Replace("delP", ""));
                _passangerList.RemoveAt(delIndex);
                InitPassagerView();
            }
            catch (Exception ex)
            {
                AppLog.Write("[DelPassanger_Click] Err." + ex.Message, AppLog.LogMessageType.Error);
            }
        }

        void AddPassanger_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            InvokeScript("showElement", new object[] { "addPassenger" });
            InvokeScript("showElement", new object[] { "cover" });
        }

        protected override void OnKeyDown(Keys keyCode)
        {
        }
    }

    class PassangerData
    {
        public string IdCard;
        private string _cardType;

        public string CardType
        {
            get { return _cardType; }
            set
            {
                switch (value)
                {
                    case "0":
                        CardTypeName = "身份证";
                        break;
                    case "1":
                        CardTypeName = "护照";
                        break;
                    case "2":
                        CardTypeName = "军官证";
                        break;
                    case "3":
                        CardTypeName = "无证件号（半票）";
                        break;
                }
                _cardType = value;
            }
        }

        public string CardTypeName;

        public string Name;
        public string TelNum;

        private string _ticketType;
        public string TicketType
        {
            get { return _ticketType; }
            set
            {
                switch (value)
                {
                    case "0":
                        TicketTypeName = "全票"; break;
                    case "1":
                        TicketTypeName = "半票";break;
                }
                _ticketType= value;
            }
        }
        public string TicketTypeName;

    }
}
