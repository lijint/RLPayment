using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.Common;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;

namespace YAPayment.Business.CarTicket
{
    class CarQueryInputDeal:Activity
    {
        private CarEntity _carEntity;
        private Dictionary<string, SetOutCityItem> _startCityDic;
        private bool _clickFlag = false;
        private BackgroundWorker _worker;
        protected override void OnEnter()
        {
            _carEntity = GetBusinessEntity<CarEntity>();
            GetElementById("Ok").Click += Confirm_Click;
            GetElementById("Return").Click += Return_Click;
            GetElementById("city1").Click += City1_Click;
            GetElementById("city2").Click += City2_Click;
            List<SetOutCityItem> cityItems = _carEntity._querySetOutResponse.StartCity;
            _startCityDic = new Dictionary<string, SetOutCityItem>();
            InitStarDic(cityItems);
            
            _worker=new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    GetElementById("info").InnerText = "获取目的地异常，请联系管理员或重试！";
                    InvokeScript("hideElement", new object[] { "endSel" });
                    InvokeScript("hideElement", new object[] { "cover" });
                    return;
                }
                else
                {
                    InitEndCity();

                }
            }
            catch (Exception ex)
            {
                AppLog.Write("Query EndCity Err" + ex.Message, AppLog.LogMessageType.Error);

            }
            finally
            {
      
            }
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Cancel= ProcessEndCity();
        }

        void Confirm_Click(object sender, HtmlElementEventArgs e)
        {
            try
            {
                //string startCity = GetElementById("city1").GetAttribute("value");
                string startCityId = GetElementById("city1").GetAttribute("alt");
                string endCity = GetElementById("city2").GetAttribute("value");

                string date = GetElementById("departTime").GetAttribute("value");

                if (string.IsNullOrEmpty(startCityId) || string.IsNullOrEmpty(endCity) || string.IsNullOrEmpty(date))
                {
                    GetElementById("message").InnerText = "数据不能为空！";
                    return;
                }
                SetOutCityItem startCityItem = _startCityDic[startCityId];
                _carEntity._BstTicketByCityRequest = new BstTicketByCityRequest();
                _carEntity._BstTicketByCityRequest.CityId = startCityItem.CityId;
                _carEntity._BstTicketByCityRequest.CityName = startCityItem.CityName;
                _carEntity._BstTicketByCityRequest.StopName = endCity;
                _carEntity._BstTicketByCityRequest.RidingDate = date;
                _carEntity._BstTicketByCityRequest.CarryStaId = _carEntity.CarryStaId;
                StartActivity("正在查询车票信息");
            }
            catch (Exception)
            {

            }
        }

        void City1_Click(object sender, HtmlElementEventArgs e)
        {
            InitStartSel(_carEntity._querySetOutResponse.StartCity);
            InvokeScript("showElement", new object[] { "startSel" });
            InvokeScript("showElement", new object[] { "cover" });
        }

        private void City2_Click(object sender, HtmlElementEventArgs e)
        {
            if (_clickFlag) return;
            _clickFlag = true;
            try
            {

                if (_carEntity.SelectStartCity == null)
                {
                    GetElementById("info").InnerText = "请先选择出发城市！";
                    _clickFlag = false;
                    return;
                }

                InvokeScript("showElement", new object[] {"endSel"});
                InvokeScript("showElement", new object[] {"cover"});
    
                _worker.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                InvokeScript("hideElement", new object[] { "endSel" });
                InvokeScript("hideElement", new object[] { "cover" });
                throw ex;
            }
            finally
            {
                _clickFlag = false;
            }
        }




        private void InitStarDic(List<SetOutCityItem> items)
        {
            foreach (SetOutCityItem item in items)
            {
                _startCityDic.Add(item.CityId, item);
                if (item.Children != null && item.Children.Count > 0)
                    InitStarDic(item.Children);
            }
        }


        private void InitStartSel(List<SetOutCityItem> items)
        {
            //string value = "<table style="width:100%;"><tr ><td id="0" class="landi_selectTr">aaaaaaaa</td><td id="1" class="landi_selectTr">aaaaaaaa</td><td id="2" class="landi_selectTr">aaaaaaaa</td><td id="2"lass="landi_selectTr">aaaaaaaa</d></tr></table>";

            string starStr = "<td id=\"{0}\" class=\"landi_selectTr\">{1}</td>";
            StringBuilder allSb = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < items.Count; i++)
            {
                SetOutCityItem item = items[i];
                sb.Append(string.Format(starStr, item.CityId, item.CityName));

                //if ((i+1)%4 == 0)
                //{
                    string temp = string.Format("<tr>" + sb.ToString() + "</tr>");
                    sb = new StringBuilder();
                    allSb.Append(temp);
               // }
            }
            string res = sb.ToString();
            if(!string.IsNullOrEmpty(res))
                allSb.Append(string.Format("<tr>" + res + "</tr>"));
            starStr = "<div style=\" position:absolute;width:50px;height:407px; left: 814px; top: 1px;\">	<div class=\"img_pre\" onclick=\"Scorll('start',-50)\"></div><div  class=\"img_next\" class=\"img_next\" onclick=\"Scorll('start',50)\"/>></div></div><div id=\"Ret\" style=\"height:40px; background-color:#2275BA; width:100%\" >	<img src=\"../images/car/left.png\" alt=\"\" style=\"height:30px; width:30px; padding-left:10px; padding-top:5px;\"/></div><div id=\"start\" style=\"overflow-y:hidden; overflow-x:hidden;height: 367px;\"><table style=\"width:100%;  \">" + allSb.ToString() + "</table><div style=\"float:right; padding-top:30px; padding-right:30px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td onclick=\" var arr=new Array('endSel','startSel','cover'); hideElement(arr);\"></td></tr></table></div></div>";
            GetElementById("startSel").InnerHtml = starStr;

            HtmlElementCollection eles = GetElementById("startSel").GetElementsByTagName("td");
            foreach (HtmlElement ele in eles)
            {
                ele.Click+=ele_Click;
            }
            GetElementById("Ret").Click += CarQueryInputDeal_Click;
        }

        void CarQueryInputDeal_Click(object sender, HtmlElementEventArgs e)
        {
            InvokeScript("hideElement", new object[] { "startSel" });
            InvokeScript("hideElement", new object[] { "endSel" });
            InvokeScript("hideElement", new object[] { "cover" });
        }

        private void ele_Click(object sender, HtmlElementEventArgs e)
        {
            HtmlElement ele = sender as HtmlElement;
            if(ele==null) return;
            SetOutCityItem item = _startCityDic[ele.Id];
            if(item==null) return;
            if (item.Children == null || item.Children.Count == 0)
            {
               // GetElementById("city1").InnerText = item.CityName;
                InvokeScript("SelectSuccess", new object[] { item.CityId, item.CityName, "city1", "startSel" });
                InvokeScript("SelectSuccess", new object[] { "", "", "city2", "endSel" });
                _carEntity.SelectStartCity = item;
            }
            else
            {
                InitStartSel(item.Children);
            }
        }


        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        /// <summary>
        /// 没取到值返回true
        /// </summary>
        /// <returns></returns>
        private bool ProcessEndCity()
        {
            try
            {

                // Thread.Sleep(6000);
                HttpHelper helper = new HttpHelper();
                string url = ConfigFile.ReadConfig("Car", "ToCity");

                QueryTerminusByCityRequest request = new QueryTerminusByCityRequest();
                request.CityName = _carEntity.SelectStartCity.CityName;
                request.CityId = _carEntity.SelectStartCity.CityId;
                request.StopName = "";
                string postData = PubFun.GetJsonString(request);
                Log.Info("PostData:" + postData);
                postData = PubFun.Encrypt(postData);

                url += "&data=" + postData.Replace("+", "%2b");

                HttpItem item = GetHttpItem(url, "", "utf-8");
                HttpResult result = helper.GetHtml(item);
                Log.Info("httpResult2:" + result.Html);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    string json = PubFun.Decrypt(result.Html);
                    Log.Info("json:" + json);
                    _carEntity._QueryTerminusByCityResponse = PubFun.GetJsonObject<QueryTerminusByCityResponse>(json);
                    if (_carEntity._QueryTerminusByCityResponse != null)
                    {
                        if (string.IsNullOrEmpty(_carEntity._QueryTerminusByCityResponse.ErrorCode))
                        return false;
                        else
                        {
                            ShowMessageAndGoBack(_carEntity._QueryTerminusByCityResponse.ErrorCode + ":" + _carEntity._QueryTerminusByCityResponse.Msg);
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.Write("[ProcessEndCity Err]"+ex.Message, AppLog.LogMessageType.Error);
                return true;
            }

        }


        private void InitEndCity()
        {

            string starStr = "<td id=\"{0}\" class=\"landi_selectTr\">{1}</td>";
            StringBuilder allSb = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            List<TargetCity> listCitys= _carEntity._QueryTerminusByCityResponse.TargetCityB;
            for (int i = 0; i < listCitys.Count; i++)
            {
                TargetCity item = listCitys[i];
                sb.Append(string.Format(starStr, item.CarryStaId, item.StopName));

                //if ((i+1)%4 == 0)
                //{
                string temp = string.Format("<tr>" + sb.ToString() + "</tr>");
                sb = new StringBuilder();
                allSb.Append(temp);
                // }
            }
            string res = sb.ToString();
            if (!string.IsNullOrEmpty(res))
                allSb.Append(string.Format("<tr>" + res + "</tr>"));
            starStr = "<div style=\" position:absolute;width:50px;height:407px; left: 814px; top: 1px;\">	<div class=\"img_pre\" onclick=\"Scorll('end',-50)\"></div><div  class=\"img_next\" class=\"img_next\" onclick=\"Scorll('end',50)\"/>></div></div><div id=\"Ret1\" style=\"height:40px; background-color:#2275BA; width:100%\" >	<img src=\"../images/car/left.png\" alt=\"\" style=\"height:30px; width:30px; padding-left:10px; padding-top:5px;\"/></div><div id=\"end\" style=\"overflow-y:hidden; overflow-x:hidden;height: 367px;\"><table style=\"width:100%;  \">" + allSb.ToString() + "</table><div style=\"float:right; padding-top:30px; padding-right:30px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td onclick=\" var arr=new Array('endSel','startSel','cover'); hideElement(arr);\"></td></tr></table></div></div>";
            GetElementById("endSel").InnerHtml = starStr;

            HtmlElementCollection eles = GetElementById("endSel").GetElementsByTagName("td");
            foreach (HtmlElement ele in eles)
            {
                ele.Click += ele_Click2;
            }
            GetElementById("Ret1").Click += CarQueryInputDeal_Click;
        }

        private void ele_Click2(object sender, HtmlElementEventArgs e)
        {
            HtmlElement ele = sender as HtmlElement;
            if (ele == null) return;
            InvokeScript("SelectSuccess", new object[] { ele.Id, ele.InnerText, "city2", "endSel" });
            _carEntity.SelectEndCityName = ele.InnerText;
            _carEntity.CarryStaId = ele.Id;

        }
    }
}
