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
    internal class LockTicketProcessDeal : Activity
    {
        private BackgroundWorker worker;// = new BackgroundWorker();
        private CarEntity _carEntity;

        protected override void OnEnter()
        {
            GetElementById("showmsg").InnerText = "正在锁定车票";
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
                string url = ConfigFile.ReadConfig("Car", "LockTicket");

                string postData = PubFun.GetJsonString(_carEntity._BstLockTicketRequest);
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
                    _carEntity._BstLockTicketResponse = PubFun.GetJsonObject<BstLockTicketResponse>(json);
                    if (_carEntity._BstLockTicketResponse != null)
                    {
                        if (string.IsNullOrEmpty(_carEntity._BstLockTicketResponse.ErrorCode))
                        {
                            e.Cancel = false;
                            _carEntity.TicketId = _carEntity._BstLockTicketResponse.Data.TicketCode;
                        }
                        else
                        {
                            e.Cancel = false;
                            ShowMessageAndGoBack( _carEntity._BstLockTicketResponse.ErrorCode + ":" + _carEntity._BstLockTicketResponse.Msg);
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
                AppLog.Write("LockTicketProcessDeal http Err:" + ex.Message, AppLog.LogMessageType.Error);
                e.Cancel = true;
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                ShowMessageAndGotoMain("锁票失败");
            }
            else
            {
                if (e.Result == null)
                {
                    double amount = 0;
                    for (int i = 0; i < _carEntity._BstLockTicketResponse.Data.TicketList.Count; i++)
                    {
                        amount += double.Parse(_carEntity._BstLockTicketResponse.Data.TicketList[i].RealPrice);
                    }
                    CommonData.Amount = amount;
                    StartActivity("购票插入银行卡");
                }
            }
        }
    }
}