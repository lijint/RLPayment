using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using PetroChina.Package;
using Landi.FrameWorks.HardWare;
using PetroChina.Package.PetroPay;

namespace PetroChina.Business.PetroPay
{
    /// <summary>
    /// 记住每个Activity默认一直保存在内存
    /// </summary>
    class PetroPayBeingPayDeal : Activity,EMVTransProcess.IReverse
    {
        EMVTransProcess emv = new EMVTransProcess();
        private void EMVProcess()
        {
            emv.CR = this;
            emv.Trans += new TransHanlde(EMV_ISO8583Trans);
            emv.EMVTransDeal(CommonData.Amount, EMVTransProcess.PbocTransType.PURCHASE);
            mRetPay = emv.EmvRet;
        }

        TransResult EMV_ISO8583Trans()
        {
            CommonData.BankCardNum = emv.EMVInfo.CardNum;
            CommonData.BankCardSeqNum = emv.EMVInfo.CardSeqNum;
            CommonData.BankCardExpDate = emv.EMVInfo.CardExpDate;
            CommonData.Track2 = emv.EMVInfo.Track2;
            PetroChinaPay.SendField55 = emv.EMVInfo.SendField55;
            TransResult ret = PetroPayBeingPay.Communicate();
            if (ret == TransResult.E_SUCC)
            {
                emv.EMVInfo.RecvField55 = PetroChinaPay.RecvField55;
                emv.EMVInfo.RecvField38 = PetroChinaPay.RecvField38;
            }

            return ret;
        }

        private TransResult mRetPay = TransResult.E_SEND_FAIL;
        CPetroPayBeingPay PetroPayBeingPay = new CPetroPayBeingPay();
        protected override void OnEnter()
        {
            DestroySelf();//设置成自动销毁，每次重新生成
            if (SyncTransaction(new CReverse_PetroChinaPay()) == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("交易超时，请重试");
                return;
            }
            if (RestoreBoolean("UseICCard"))//如果使用IC卡
                PostSync(EMVProcess);
            else
                mRetPay = SyncTransaction(PetroPayBeingPay);

            PayResult(mRetPay);
        }

        private void PayResult(TransResult retPay)
        {
            if (retPay == TransResult.E_SUCC)
            {
                //缴费成功，发起确认销账
                CPetroPayBillConfirm billConfirm = new CPetroPayBillConfirm();
                TransResult retConfirm = SyncTransaction(billConfirm);
                if (retConfirm != TransResult.E_SUCC)
                {
                    CReverse_PetroChinaPay rev = new CReverse_PetroChinaPay(PetroPayBeingPay);
                    rev.Reason = "06";
                    rev.Field55 = emv.EMVInfo.AutoField55;
                    SyncTransaction(rev);
                    ShowMessageAndGotoMain("交易超时，请重试");
                }
                else
                {
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
                CReverse_PetroChinaPay rev = new CReverse_PetroChinaPay(PetroPayBeingPay);
                rev.Reason = "98";
                rev.Field55 = emv.EMVInfo.AutoField55;
                SyncTransaction(rev);
                ShowMessageAndGotoMain("交易超时，请重试");
            }
            else
            {
                ShowMessageAndGotoMain("交易失败，请重试");
            }
        }

        #region IReverse 成员

        public void CreateReverseFile()
        {
            CReverse_PetroChinaPay rev = new CReverse_PetroChinaPay(PetroPayBeingPay);
            rev.Reason = "06";
            if (emv.EMVInfo.AutoField55 != null && emv.EMVInfo.AutoField55.Length != 0)
            {
                rev.Field55 = emv.EMVInfo.AutoField55;
            }
            Save("Reverse", rev);
        }

        public void ClearReverseFile()
        {
            Save("Reverse", null);
        }

        public void DoReverseFile()
        {
            CReverse_PetroChinaPay rev = Restore("Reverse") as CReverse_PetroChinaPay;
            if (rev != null)
            {
                rev.Communicate();
                ClearReverseFile();
            }
        }
        #endregion
    }
}
