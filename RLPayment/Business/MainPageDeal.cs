using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Landi.FrameWorks;


namespace RLPayment.Business
{
    class MainPageDeal : FrameActivity, ITimeTick
    {

        private bool _isClick = false;
        protected override void OnEnter()
        {
            try
            {
                _isClick = false;

                //GetElementById("AdImage").SetAttribute("src", mPictures[m_currpicture]);
                //GetElementById("PublicFee").Click += new HtmlElementEventHandler(PublicFee_Click);
                //GetElementById("Gas").Click += new HtmlElementEventHandler(PublicFee_Click);
                //GetElementById("Water").Click += new HtmlElementEventHandler(PublicFee_Click);
                //GetElementById("TV").Click += new HtmlElementEventHandler(PublicFee_Click);
                //GetElementById("Power").Click += new HtmlElementEventHandler(Power_Click);
                //GetElementById("Mobile").Click += new HtmlElementEventHandler(Mobile_Click);
                //GetElementById("CreditCard").Click += new HtmlElementEventHandler(CreditCard_Click);
                //GetElementById("TrafficPolice").Click += new HtmlElementEventHandler(TrafficPolice_Click);
                //GetElementById("CarTicket").Click += CarTicket_Click;
                //SetManageEntryInfo("ManageEntry");
                IsConDisplay(false);
                Log.Info("Version : " + Assembly.GetExecutingAssembly().GetName().Version.ToString());

                INIClass gasCardReaderIni = new INIClass(AppDomain.CurrentDomain.BaseDirectory + "Versionfile.ini");
                gasCardReaderIni.IniWriteValue("Version", "VersionNo", Assembly.GetExecutingAssembly().GetName().Version.ToString());

                GetElementById("btnPay").Click += new HtmlElementEventHandler(btnPay_Click);

                //SetElement(MenuEnable.CreditCard, "CreditCard");
                //SetElement(MenuEnable.TV, "TV");
                //SetElement(MenuEnable.Gas, "Gas");
                //SetElement(MenuEnable.Water, "Water");
                //SetElement(MenuEnable.Power, "Power");
                //SetElement(MenuEnable.Mobile, "Mobile");
                //SetElement(MenuEnable.TrafficPolice, "TrafficPolice");
                //SetElement(MenuEnable.CarTicket, "CarTicket");
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }

        private void btnPay_Click(object sender, HtmlElementEventArgs e)
        {
            if (_isClick) return;
            _isClick = true;
            EnterBusiness(new RLCZ.RLCZStratagy());
            StartActivity("热力充值输入热力号");
            //if (CarPay.HasSignIn)
            //{
            //    StartActivity(ReceiptPrinter.CheckedByManager() ? "车票预订主画面" : "雅安交警罚没打印机故障继续");
            //}
            //else
            //{
            //    StartActivity("正在签到");
            //}
        }

        void CarTicket_Click(object sender, HtmlElementEventArgs e)
        {
            //if (_isClick) return;
            //_isClick = true;
            //EnterBusiness(new CarTicketStratagy());
            //if (CarPay.HasSignIn)
            //{
            //    StartActivity(ReceiptPrinter.CheckedByManager() ? "车票预订主画面" : "雅安交警罚没打印机故障继续");
            //}
            //else
            //{
            //    StartActivity("正在签到");
            //}
        }

        private void SetElement(bool isShow, string eleName)
        {
            if (GetElementById(eleName) == null) return;
            if (isShow)
            {
                if (GetElementById(eleName).Style != null && GetElementById(eleName).Style.Contains("visibility"))
                    GetElementById(eleName).Style.Replace("hidden", "visible");
                else
                    GetElementById(eleName).Style += ";visibility:visible;";
            }
            else
            {
                if (GetElementById(eleName).Style != null && GetElementById(eleName).Style.Contains("visibility"))
                    GetElementById(eleName).Style.Replace("visible", "hidden");
                else
                    GetElementById(eleName).Style += ";visibility:hidden;";
            }
        }

        private void TrafficPolice_Click(object sender, HtmlElementEventArgs e)
        {
            //StartActivity("该业务暂未开通");
            //return;
            //if(_isClick) return;
            //_isClick = true;
            //EnterBusiness(new YATrafficPoliceStratagy());
            //if (YAPaymentPay.HasSignIn)
            //{
            //    StartActivity(ReceiptPrinter.CheckedByManager() ? "雅安交警罚没菜单" : "雅安交警罚没打印机故障继续");
            //}
            //else
            //{
            //    StartActivity("正在签到");
            //}
        }

        private void CreditCard_Click(object sender, HtmlElementEventArgs e)
        {
            //StartActivity("该业务暂未开通");
            //return;
            //if (_isClick) return;
            //_isClick = true;
            //EnterBusiness(new CreditcardStratagy());
            //if (QMPay.HasSignIn)
            //{
            //    StartActivity(ReceiptPrinter.CheckedByManager() ? "信用卡还款温馨提示" : "信用卡打印机故障继续");
            //}
            //else
            //{
            //    StartActivity("正在签到");
            //}
        }

        private void Mobile_Click(object sender, HtmlElementEventArgs e)
        {
            ////StartActivity("该业务暂未开通");
            ////return;
            //if (_isClick) return;
            //_isClick = true;
            //EnterBusiness(new MobileStratagy());
            //if (QMPay.HasSignIn)
            //{
            //    StartActivity(ReceiptPrinter.CheckedByManager() ? "手机充值主界面" : "手机充值打印机故障继续");
            //}
            //else
            //{
            //    StartActivity("正在签到");
            //}
        }

        private void PublicFee_Click(object sender, HtmlElementEventArgs e)
        {
            ////if (_isClick) return;
            ////_isClick = true;
            ////EnterBusiness(new YAPublishPayStratagy());

            ////string id;
            ////if (sender is HtmlElement)
            ////    id = (sender as HtmlElement).Id;
            ////else
            ////    id = (string)sender;

            ////YAEntity entity = GetBusinessEntity() as YAEntity;
            ////switch (id)
            ////{
            ////    case "Gas":
            ////        entity.PublishPayType = YaPublishPayType.Gas;
            ////        break;
            ////    case "Water":
            ////        entity.PublishPayType = YaPublishPayType.Water;
            ////        break;
            ////    case "Power":
            ////        entity.PublishPayType = YaPublishPayType.Power;
            ////        break;
            ////    case "TV":
            ////        entity.PublishPayType = YaPublishPayType.TV;
            ////        break;
            ////}
            ////Log.Info("雅安公共事业缴费：" + entity.PublishPayType.ToString());


            ////if (YAPaymentPay.HasSignIn)
            ////{
            ////    if (ReceiptPrinter.CheckedByManager())
            ////    {
            ////        //StartActivity("雅安支付菜单");
            ////        StartActivity("雅安支付输入用户号");
            ////    }
            ////    else
            ////        StartActivity("雅安支付打印机故障继续");
            ////}
            ////else
            ////{
            ////    StartActivity("正在签到");
            ////}
        }

        private void Power_Click(object sender, HtmlElementEventArgs e)
        {
            //if (_isClick) return;
            //_isClick = true;
            ////Test
            ////StartActivity("该业务暂未开通");
            ////return;
            //EnterBusiness(new PowerPayStratagy());
            //if (PowerPay.HasSignIn)
            //{
            //    if (ReceiptPrinter.CheckedByManager())
            //    {
            //        string temp = "";
            //        if (ValidateOutOfBussinessTime(ref temp))
            //            StartActivity("电力支付菜单");
            //        else
            //            ShowMessageAndGotoMain(temp);  
            //    }
            //    else
            //    {
            //        ShowMessageAndGotoMain("打印机缺纸或故障，服务暂停！");                
            //    }
            //        //StartActivity("电力支付打印机故障继续");
            //}
            //else
            //{
            //    StartActivity("正在签到");
            //}
        }

        /// <summary>
        /// 日切时间验证
        /// </summary>
        /// <returns></returns>
        private bool ValidateOutOfBussinessTime(ref string message)
        {
            try
            {
                DateTime time = DateTime.Now;
                DateTime beginH = ConvertToTime(ConfigFile.ReadConfigAndCreate("Power", "BeginHour", "23:30"));
                DateTime endH = ConvertToTime(ConfigFile.ReadConfigAndCreate("Power", "EndHour", "1:00"));
                message = string.Format("时间段({0}-24:00,0:00-{1})，后台服务暂停！", beginH.ToString("HH:mm"), endH.ToString("HH:mm"));
                if (time >= beginH || time < endH)
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                Log.Error("BeginHour或者EndHour日期配置格式有异常！");
                message = "BeginHour或者EndHour日期配置格式有异常！";
                return false;
            }
        }

        private DateTime ConvertToTime(string value)
        {

            string[] time = value.Split(new char[] { ':' }, StringSplitOptions.None);
            DateTime result = DateTime.Today;
            int hour = int.Parse(time[0]);
            if (hour >= 24)
            {
                Log.Error("小时数不能大于24！");
                throw new Exception("小时数不能大于24");
            }
            result = result.AddHours(hour);
            int min = int.Parse(time[1]);
            if (min >= 60)
            {
                Log.Error("分钟数不能大于60！");
                throw new Exception("分钟数不能大于60");
            }
            result = result.AddMinutes(min);
            return result;
        }

        protected override void OnTimeOut()
        {
            ShowAd();
        }

        public override bool CanQuit()
        {
            return true;
        }

        protected override void OnLeave()
        {

        }

        bool no_service()
        {
            //Test
            //return false;

            bool bRet = false;
            TimeSpan t = DateTime.Now.TimeOfDay;
            if ((t.Hours == 22 && t.Minutes >= 30) ||
                t.Hours == 23 ||
                t.Hours == 0)
            {
                StartActivity("该业务暂未开通");
                bRet = true;
            }

            return bRet;
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            base.OnKeyDown(keyCode);

            //string ID = "";
            //switch (keyCode)
            //{
            //    case Keys.D1:
            //        {
            //            if (MenuEnable.Gas)
            //            {
            //                ID = "Gas";
            //                PublicFee_Click(ID, null);
            //            }
            //        }
            //        break;
            //    case Keys.D2:
            //        {
            //            if (MenuEnable.Water)
            //            {
            //                ID = "Water";
            //                PublicFee_Click(ID, null);
            //            }
            //        }
            //        break;
            //    case Keys.D3:
            //        {
            //            if (MenuEnable.TV)
            //            {
            //                ID = "TV";
            //                PublicFee_Click(ID, null);
            //            }
            //        }
            //        break;
            //    case Keys.D4:
            //        {
            //            if (MenuEnable.Power)
            //            {
            //                Power_Click(null, null);
            //            }
            //        }
            //        break;
            //    case Keys.D5:
            //        {
            //            if (MenuEnable.Mobile)
            //            {
            //                Mobile_Click(null, null);
            //            }
            //            //ID = "Power";
            //            //PublicFee_Click(ID, null);
            //        }
            //        break;
            //    case Keys.D6:
            //        {
            //            if (MenuEnable.CreditCard)
            //            {
            //                CreditCard_Click(null, null);
            //            }
            //        }
            //        break;
            //    case Keys.D7:
            //        {
            //            if (MenuEnable.TrafficPolice)
            //            {
            //                TrafficPolice_Click(null, null);
            //            }
            //        }
            //        break;
            //    case Keys.D8:
            //        {
            //            if (MenuEnable.CarTicket)
            //            {
            //                CarTicket_Click(null, null);
            //            }
            //        }
            //        break;
            //}
        }


        #region ITimeTick 成员

        public void OnTimeTick(int count)
        {
            //string sDate = DateTime.Now.ToString("yyyy年MM月dd日");
            //string sW = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
            //string sTime = DateTime.Now.ToString("HH:mm:ss");

            //string temp = sDate + " " + sW + " " + sTime;
            //GetElementById("DateTime").InnerText = temp;

            //if (count % m_AdSwitchInterval == 0)
            //{
            //    m_currpicture = (m_currpicture + 1) % mPictures.Count;
            //    GetElementById("AdImage").SetAttribute("src", mPictures[m_currpicture]);
            //}
        }

        #endregion

        #region 中间广告
        private List<string> mPictures = new List<string>();
        int m_currpicture = 0;
        int m_AdSwitchInterval = 10;
        /// <summary>
        /// 读取所有图片
        /// </summary>
        /// <param name="dir"></param>
        private void getAllPicture(string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        string ext = Path.GetExtension(d).ToLower();
                        if (ext == ".jpg" || ext == ".bmp")
                        {
                            mPictures.Add(d);
                        }
                    }
                    else
                        getAllPicture(d);
                }
            }
        }
        #endregion
    }
}
#region 
/*
private void CreditCard_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("该业务暂未开通");
            return;
            EnterBusiness(new CreditcardStratagy());
            if (QMPay.HasSignIn)
            {
                if (ReceiptPrinter.CheckedByManager())
                    StartActivity("信用卡还款温馨提示");
                else
                    StartActivity("信用卡打印机故障继续");
            }
            else
            {
                StartActivity("正在签到");
            }
        }

        private void Mobile_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("该业务暂未开通");
            return;
            EnterBusiness(new MobileStratagy());
            if (QMPay.HasSignIn)
            {
                if (ReceiptPrinter.CheckedByManager())
                    StartActivity("手机充值主界面");
                else
                    StartActivity("手机充值打印机故障继续");
            }
            else
            {
                StartActivity("正在签到");
            }
        }

        private void PetroPay_Click(object sender, HtmlElementEventArgs e)
        {
            EnterBusiness(new PetroPayStratagy());
            if (PetroChinaPay.HasSignIn)
            {
                if (ReceiptPrinter.CheckedByManager())
                {
                    if (R80.IsUse)
                        StartActivity(typeof(PetroPayShowUserCardDeal));
                    else
                        StartActivity(typeof(PetroPayUserLoginDeal));
                }
                else
                    StartActivity("中石油支付打印机故障继续");
            }
            else
            {
                StartActivity("正在签到");
            }
        }
 */
#endregion

