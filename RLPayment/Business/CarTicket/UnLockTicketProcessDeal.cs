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
    class UnLockTicketProcessDeal : Activity
    {
        private BackgroundWorker worker;
        private CarEntity _carEntity;

        protected override void OnEnter()
        {
     
            _carEntity = GetBusinessEntity<CarEntity>();
            GetElementById("showmsg").InnerText = _carEntity.UnlockMessage + " 正在解锁车票!";
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
                string url = ConfigFile.ReadConfig("Car", "UnLockTicket");

                _carEntity._UnLockTicketRequest = new UnLockTicketRequest();
                _carEntity._UnLockTicketRequest.TicketIds = _carEntity.TicketId;

                string postData = PubFun.GetJsonString(_carEntity._UnLockTicketRequest);
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
                    _carEntity._UnLockTicketResponse = PubFun.GetJsonObject<UnLockTicketResponse>(json);
                    if (_carEntity._UnLockTicketResponse != null)
                    {
                        if (string.IsNullOrEmpty(_carEntity._UnLockTicketResponse.ErrorCode))
                        {
                            e.Cancel = false;
                        }
                        else
                        {
                            e.Cancel = false;
                            ShowMessageAndGoBack(_carEntity._UnLockTicketResponse.ErrorCode + ":" + _carEntity._UnLockTicketResponse.Msg);
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
                AppLog.Write("UnLockTicketProcessDeal http Err:" + ex.Message, AppLog.LogMessageType.Error);
                e.Cancel = true;
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                ShowMessageAndGotoMain("解锁失败！");
            }
            else
            {
                if(e.Result==null)
                ShowMessageAndGotoMain("已成功解锁车票！");
            }
        }
    }
}
