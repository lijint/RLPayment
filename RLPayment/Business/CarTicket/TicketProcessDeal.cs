using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using Landi.FrameWorks;
using Landi.FrameWorks.Common;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;

namespace YAPayment.Business.CarTicket
{
    class TicketProcessDeal : Activity
    {
        private BackgroundWorker worker;// = new BackgroundWorker();
        private CarEntity _carEntity;
        protected override void OnEnter()
        {
            GetElementById("showmsg").InnerText = "正在查询车票信息";
            _carEntity = GetBusinessEntity<CarEntity>();
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                HttpHelper helper = new HttpHelper();
                string url = ConfigFile.ReadConfig("Car", "TicketQuery");

                string postData = PubFun.GetJsonString(_carEntity._BstTicketByCityRequest);
                Log.Info("PostData:" + postData);
                postData = PubFun.Encrypt(postData);

                url += "&data=" + postData.Replace("+", "%2b");

                HttpItem item = GetHttpItem(url, "", "utf-8");
                HttpResult result = helper.GetHtml(item);

                AppLog.Write("httpResult2:" + result.Html, AppLog.LogMessageType.Info);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    string json = PubFun.Decrypt(result.Html); //, ConfigFile.ReadConfig("AppData", "DesKey"));
                    AppLog.Write("json:" + json, AppLog.LogMessageType.Info);
                    _carEntity._BstTicketByCityResponse = PubFun.GetJsonObject<BstTicketByCityResponse>(json);
                    if (_carEntity._BstTicketByCityResponse != null)
                    {
                        if (string.IsNullOrEmpty(_carEntity._BstTicketByCityResponse.ErrorCode))
                        e.Cancel = false;
                        else
                        {
                            ShowMessageAndGoBack(_carEntity._BstTicketByCityResponse.ErrorCode + ":" + _carEntity._BstTicketByCityResponse.Msg);
                            e.Cancel = false;
                            e.Result = new object();
                            return;
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                AppLog.Write("TicketProcessDeal http Err:" + ex.Message, AppLog.LogMessageType.Error);
                e.Cancel = true;
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                StartActivity("购票查询");
            }
            else
            {
                if (e.Result==null)
                StartActivity("车票信息");
            }
        }
    }
}
