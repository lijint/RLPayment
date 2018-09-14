using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;
using YAPayment.Package;
using YAPayment.Package.Creditcard;

namespace YAPayment.Business.Creditcard
{
    class CreditcardProcessDeal : Activity
    {
        private EMVTransProcess emv = new EMVTransProcess();
        private bool bemvInit;
        private bool bisICCard;
        private QMEntity entity;

        protected override void OnEnter()
        {

            DestroySelf();//设置成自动销毁，每次重新生成
            bemvInit = false;
            bisICCard = false;
            entity = (GetBusinessEntity() as QMEntity);

            if (SyncTransaction(new CReverse_YAPaymentPay()) == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("交易超时，请重试");
                return;
            }

            if (QueryProcess() != TransResult.E_SUCC)
                return;

            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bisICCard = true;

            entity.SendField55 = null;
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
            //传入支付金额
            int state = emv.EMVTransInit(entity.TotalAmount, EMVTransProcess.PbocTransType.PURCHASE);
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

        private TransResult QueryProcess()
        {
            CCreditcardQuery query = new CCreditcardQuery();
            TransResult result = SyncTransaction(query);
            if (result == TransResult.E_SUCC)
            {
            }
            else if (result == TransResult.E_HOST_FAIL)
            {
                ShowMessageAndGotoMain(query.ReturnCode + "-" +
                            query.ReturnMessage);
            }
            else if (result == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("交易超时，请重试");
            }
            else
            {
                ShowMessageAndGotoMain("交易失败，请重试");
            }

            return result;
        }

        private void PayProcess()
        {
            CCreditcardPay pay = new CCreditcardPay();
            TransResult result = SyncTransaction(pay);
            CReverse_YAPaymentPay rev = new CReverse_YAPaymentPay(pay);

            if (result == TransResult.E_SUCC)
            {
                if (bisICCard)
                {
                    int state = emv.EMVTransEnd(entity.RecvField55, entity.RecvField38);
                    if (state != 0)
                    {
                        rev.Reason = "06";
                        SyncTransaction(rev);
                        ShowMessageAndGotoMain("IC确认错误，交易失败，请重试");
                        return;
                    }
                }

                if (ReceiptPrinter.ExistError())
                    StartActivity("信用卡还款打印成功");
                else
                    StartActivity("信用卡还款交易成功");
            }
            else if (result == TransResult.E_HOST_FAIL)
            {
                if (pay.ReturnCode == "51")
                    ShowMessageAndGotoMain("温馨提示：抱歉！交易失败！" + "\n" +
                        "您卡内余额不足！");
                else if (pay.ReturnCode == "55")
                    StartActivity("信用卡还款密码错误");
                else
                    ShowMessageAndGotoMain(pay.ReturnCode + "-" +
                        pay.ReturnMessage);
            }
            else if (result == TransResult.E_RECV_FAIL)
            {
                rev.Reason = "98";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("交易超时，请重试");
                return;
            }
            else if (result == TransResult.E_CHECK_FAIL)
            {
                rev.Reason = "96";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("系统异常，请稍后再试");
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
