using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using Landi.FrameWorks;
using Landi.FrameWorks.Common;
using Landi.FrameWorks.HardWare;
using Landi.FrameWorks.Iso8583;
using Newtonsoft.Json;
using YAPayment.Entity;

namespace YAPayment.Business.CarTicket
{
    class CarCityInqueryDeal:Activity
    {
        private BackgroundWorker worker;//= new BackgroundWorker();
        private CarEntity _carEntity;
        protected override void OnEnter()
        {
            _carEntity = GetBusinessEntity<CarEntity>();
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                ShowMessageAndGoBack("获取购票信息失败，请联系管理员");
            }
            else
            {
               if(e.Result==null)
                StartActivity("购票查询");
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            HttpHelper helper = new HttpHelper();
            string url = ConfigFile.ReadConfig("Car", "FromCity");
            HttpItem item = GetHttpItem(url, "", "utf-8");
            HttpResult result = helper.GetHtml(item);
            
           // AppLog.Write("httpResult2:" + result.Html, AppLog.LogMessageType.Info);
    
            if (result.StatusCode == HttpStatusCode.OK)
            {
                string json = PubFun.Decrypt(result.Html);//, ConfigFile.ReadConfig("AppData", "DesKey"));
                AppLog.Write("json:" + json, AppLog.LogMessageType.Info);
                _carEntity._querySetOutResponse = PubFun.GetJsonObject<QuerySetOutResponse>(json);
                if (_carEntity._querySetOutResponse != null)
                {
                    if (string.IsNullOrEmpty(_carEntity._querySetOutResponse.ErrorCode))
                    e.Cancel = false;
                    else
                    {
                        ShowMessageAndGoBack(_carEntity._querySetOutResponse.ErrorCode + ":" + _carEntity._querySetOutResponse.Msg);
                        e.Cancel = false;
                        e.Result=new object();
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

    }
}
