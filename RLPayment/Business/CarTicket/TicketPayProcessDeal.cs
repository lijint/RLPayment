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
using YAPayment.Package;
using YAPayment.Package.CarTicket;
using YAPayment.Package.Mobile;

namespace YAPayment.Business.CarTicket
{
    class TicketPayProcessDeal : Activity
    {
        private EMVTransProcess emv = new EMVTransProcess();
        private bool bemvInit;
        private bool bisICCard;
        private CarEntity entity;
        private BackgroundWorker worker;
        protected override void OnEnter()
        {
            DestroySelf();//设置成自动销毁，每次重新生成
            bemvInit = false;
            bisICCard = false;
            entity = GetBusinessEntity<CarEntity>();
            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bisICCard = true;

            if (SyncTransaction(new CReverse_YAPaymentPay()) == TransResult.E_RECV_FAIL)
            {
                entity.UnlockMessage = "交易超时，请重试,";
                StartActivity("解锁车票");
                return;
            }

            entity.SendField55 = null;
            if (bisICCard)//如果是IC卡，或是复合卡
            {
                PostSync(EMVProcess);
                if (!bemvInit)
                {
                    entity.UnlockMessage = "IC卡初始化失败，请重试,";
                    StartActivity("解锁车票");
                    return;
                }
            }

            PayProcess();
        }


        private void EMVProcess()
        {
            int state = emv.EMVTransInit(CommonData.Amount, EMVTransProcess.PbocTransType.PURCHASE);
            if (state == 0)
            {
                if (emv.EMVTransDeal() == 0)
                {
                    CommonData.BankCardNum = emv.EMVInfo.CardNum;
                    CommonData.BankCardSeqNum = emv.EMVInfo.CardSeqNum;
                    CommonData.BankCardExpDate = emv.EMVInfo.CardExpDate;
                    CommonData.Track2 = emv.EMVInfo.Track2;
                    entity.SendField55 = emv.EMVInfo.SendField55;
                    bemvInit = true;
                }
            }
        }

        private CReverse_CarPay rev;
        private void PayProcess()
        {
            CPayTicket mobileRecharge = new CPayTicket();
            TransResult retPay = SyncTransaction(mobileRecharge);
            rev = new CReverse_CarPay(mobileRecharge);

            if (retPay == TransResult.E_SUCC)
            {
                if (bisICCard)
                {
                    int state = emv.EMVTransEnd(entity.RecvField55, entity.RecvField38);
                    if (state != 0)
                    {
                        rev.Reason = "06";
                        SyncTransaction(rev);
                        entity.UnlockMessage = "IC确认错误，交易失败，请重试,";
                        StartActivity("解锁车票");
                        return;
                    }
                }

                worker = new BackgroundWorker();
                worker.DoWork += worker_DoWork;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                worker.RunWorkerAsync();
                return;
            }
            else if (retPay == TransResult.E_HOST_FAIL)
            {
                if (mobileRecharge.ReturnCode == "51")
                {
                    entity.UnlockMessage = "交易失败,您卡内余额不足！";
                    StartActivity("解锁车票");
                }
                else if (mobileRecharge.ReturnCode == "55")
                {
                    StartActivity("购票密码错误");
                }
                else
                {
                    entity.UnlockMessage = mobileRecharge.ReturnCode + "-" +
                        mobileRecharge.ReturnMessage;
                    StartActivity("解锁车票");
                }

            }
            else if (retPay == TransResult.E_RECV_FAIL)
            {
                rev.Reason = "98";
                SyncTransaction(rev);
                entity.UnlockMessage = "交易失败";
                StartActivity("解锁车票");
                return;
            }
            else if (retPay == TransResult.E_CHECK_FAIL)
            {
                rev.Reason = "96";
                SyncTransaction(rev);
                entity.UnlockMessage = "系统异常，请稍后再试";
                StartActivity("解锁车票");
                return;
            }
            else
            {
                entity.UnlockMessage = "交易失败";
                StartActivity("解锁车票");
            }
            rev.ClearReverseFile();//有做冲正直接return，否则最后要清除冲正文件
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                return;
                HttpHelper helper = new HttpHelper();
                string url = ConfigFile.ReadConfig("Car", "BuyTicket");
                entity._BstBuyTicketRequest = new BstBuyTicketRequest();

                entity._BstBuyTicketRequest.TicketIds = entity.TicketId;
                string postData = PubFun.GetJsonString(entity._BstBuyTicketRequest);
                Log.Info("PostData:" + postData);
                postData = PubFun.Encrypt(postData);
                url += "&data=" + postData.Replace("+", "%2b");

                HttpItem item = GetHttpItem(url, "", "utf-8");
                HttpResult result = helper.GetHtml(item);

                // AppLog.Write("httpResult2:" + result.Html, AppLog.LogMessageType.Info);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    string json = PubFun.Decrypt(result.Html); //, ConfigFile.ReadConfig("AppData", "DesKey"));
                    AppLog.Write("json:" + json, AppLog.LogMessageType.Info);
                    entity._BstBuyTicketResponse = PubFun.GetJsonObject<BstBuyTicketResponse>(json);
                    if (entity._BstBuyTicketResponse != null)
                    {
                        if (string.IsNullOrEmpty(entity._BstBuyTicketResponse.ErrorCode))
                        {
                            e.Cancel = false;
                        }
                        else
                        {
                            entity.UnlockMessage = entity._BstBuyTicketResponse.ErrorCode + ":" + entity._BstBuyTicketResponse.Msg;
                            e.Cancel = true;
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

            catch(Exception ex)
            {
                AppLog.Write("TicketProcessDeal http Err:" + ex.Message, AppLog.LogMessageType.Error);
                e.Cancel = true;
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                if (string.IsNullOrEmpty(entity.UnlockMessage))
                    entity.UnlockMessage = "渠道购票失败！";
                StartActivity("解锁车票");
            }
            else
            {
                rev.ClearReverseFile();//有做冲正直接return，否则最后要清除冲正文件
                if (ReceiptPrinter.ExistError())
                    StartActivity("购票交易成功是否打印");
                else
                    StartActivity("购票交易完成");
            }
        }

    }
}
