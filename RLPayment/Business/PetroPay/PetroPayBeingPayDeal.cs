using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package;
using Landi.FrameWorks.HardWare;
using YAPayment.Package.PetroPay;
using System.Windows.Forms;
using System.IO;

namespace YAPayment.Business.PetroPay
{
    /// <summary>
    /// 记住每个Activity默认一直保存在内存
    /// </summary>
    class PetroPayBeingPayDeal : Activity
    {
        private EMVTransProcess emv = new EMVTransProcess();
        private bool bemvInit = false;
        private bool bisICCard = false;

        protected override void OnEnter()
        {
            DestroySelf();//设置成自动销毁，每次重新生成
            bemvInit = false;
            bisICCard = false;
            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bisICCard = true;
            
            if (SyncTransaction(new CReverse_YAPaymentPay()) == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("交易超时，请重试");
                return;
            }

            if (bisICCard)//如果是IC卡，或是复合卡
            {
                PostSync(EMVProcess);
                if (!bemvInit)
                {
                    ShowMessageAndGotoMain("IC卡初始化失败，请重试");
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
                    YAPaymentPay.SendField55 = emv.EMVInfo.SendField55;
                    bemvInit = true;
                }
            }
        }

        private void PayProcess()
        {
            TransAccessFactory db = new TransAccessFactory();
            int nTryConfirm = 3;
            CPetroPayBeingPay PetroPayBeingPay = new CPetroPayBeingPay();
            TransResult retPay = SyncTransaction(PetroPayBeingPay);
            CReverse_YAPaymentPay rev = new CReverse_YAPaymentPay(PetroPayBeingPay);

            if (retPay == TransResult.E_SUCC)
            {
                if (bisICCard)
                {
                    int state = emv.EMVTransEnd(YAPaymentPay.RecvField55, YAPaymentPay.RecvField38);
                    if (state != 0)
                    {
                        rev.Reason = "06";
                        SyncTransaction(rev);
                        ShowMessageAndGotoMain("IC确认错误，交易失败，请重试");
                        return;
                    }
                }

                if (GlobalAppData.GetInstance().AccessSwitch)
                {
                    db.PayTraceNo = YAPaymentPay.PayTraceNo;
                    ResponseData rd = new YAPaymentPay().GetResponseData();
                    db.InsertTransLog(rd);
                }
            CONFIRM:
                //缴费成功，发起确认销账
                CPetroPayBillConfirm billConfirm = new CPetroPayBillConfirm();
                TransResult retConfirm = SyncTransaction(billConfirm);
                //Test
                //retConfirm = TransResult.E_RECV_FAIL;
                //PetroChinaPay.PayTraceNo = "111111";
                //PetroChinaPay.PayReferenceNo = "123456789012345678";
                if (retConfirm != TransResult.E_SUCC &&
                    retConfirm != TransResult.E_RECV_FAIL)
                {
                    if (GlobalAppData.GetInstance().AccessSwitch)
                        db.UpdateTransLog(EnumConfirmFlag.E_REVERSE);

                    //失败但不超时才发冲正
                    if (bisICCard)
                        rev.SetField55Value(emv.EMVInfo.EndField55, emv.EMVInfo.EndField55.Length);//处理之后的55域
                    rev.Reason = "06";
                    SyncTransaction(rev);
                    ShowMessageAndGotoMain("交易超时，请重试");
                    return;
                }
                else if (retConfirm == TransResult.E_RECV_FAIL)
                {
                    //超时无响应循环发送确认报文
                    if (nTryConfirm != 0)
                    {
                        nTryConfirm--;
                        goto CONFIRM;
                    }

                    //销账失败
                    //操作成功，后台发生异常，核销失败，请不要继续缴费，
                    //等待系统自动处理。次日下午4:00以后再行查看缴费情况
                    Log.Warn("银行卡扣款成功，但销账失败，需人工处理CardNo=" + CommonData.BankCardNum +
                        " 凭证号=" + YAPaymentPay.PayTraceNo + " 系统参考号=" + YAPaymentPay.PayReferenceNo);
                    rev.ClearReverseFile();//清除冲正文件
                    StartActivity("中石油支付销账失败");
                    
                    //string failPath = Path.Combine(Application.StartupPath, "PetroConfirmFailInfo.dat");
                    //ConfirmFailInfo info = new ConfirmFailInfo();
                    //info.BankCardNo = CommonData.BankCardNum;
                    //info.PayTraceNo = PetroChinaPay.PayTraceNo;
                    //info.PayReferenceNo = PetroChinaPay.PayReferenceNo;
                    //List<ConfirmFailInfo> list = new List<ConfirmFailInfo>();
                    //list.Add(info);
                    //Utility.SaveToFile<ConfirmFailInfo>(failPath, list); 
                }
                else
                {
                    if (GlobalAppData.GetInstance().AccessSwitch)
                        db.UpdateTransLog(EnumConfirmFlag.E_SUCC);
                    
                    rev.ClearReverseFile();//清除冲正文件
                    if (ReceiptPrinter.ExistError())
                        StartActivity("中石油支付交易完成");
                    else
                        StartActivity("中石油支付交易成功是否打印");
                }
            }
            else if (retPay == TransResult.E_HOST_FAIL)
            {
                if (PetroPayBeingPay.ReturnCode == "51")
                {
                    ShowMessageAndGotoMain("温馨提示：抱歉！交易失败！" + "\n" +
                        "您卡内余额不足！");
                }
                else if (PetroPayBeingPay.ReturnCode == "55")
                {
                    StartActivity("中石油支付密码错误");
                }
                else
                {
                    ShowMessageAndGotoMain(PetroPayBeingPay.ReturnCode + "-" + PetroPayBeingPay.ReturnMessage);
                }
            }
            else if (retPay == TransResult.E_RECV_FAIL)
            {
                rev.Reason = "98";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("交易超时，请重试");
                return;
            }
            else
            {
                ShowMessageAndGotoMain("交易失败，请重试");
            }

            rev.ClearReverseFile();//在不发冲正文件的情况下，才清除冲正文件
        }
    }
}
