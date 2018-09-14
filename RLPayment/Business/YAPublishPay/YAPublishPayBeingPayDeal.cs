using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;
using YAPayment.Package;
using YAPayment.Package.YAPublishPay;

namespace YAPayment.Business.YAPublishPay
{
    /// <summary>
    /// 记住每个Activity默认一直保存在内存
    /// </summary>
    class YAPublishPayBeingPayDeal : Activity
    {
        private YAEntity m_entity;
        private EMVTransProcess emv;
        private bool bemvInit;
        private bool bisICCard;

        protected override void OnEnter()
        {
            DestroySelf();//设置成自动销毁，每次重新生成
            bemvInit = false;
            bisICCard = false;
            m_entity = GetBusinessEntity() as YAEntity;
            emv = new EMVTransProcess();

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
                    m_entity.SendField55 = emv.EMVInfo.SendField55;
                    bemvInit = true;
                }
            }
        }

        private void PayProcess()
        {
            int nTryConfirm = 3;
            if (m_entity.PublishPayType == YaPublishPayType.TV)
                nTryConfirm = 0;
            
            CYAPublishPayBeingPay beingPay = new CYAPublishPayBeingPay();
            TransResult retPay = SyncTransaction(beingPay);
            CReverse_YAPaymentPay rev = new CReverse_YAPaymentPay(beingPay);
            //Test
            //retPay = TransResult.E_RECV_FAIL;
            if (retPay == TransResult.E_SUCC)
            {
                if (bisICCard)
                {
                    int state = emv.EMVTransEnd(m_entity.RecvField55, m_entity.RecvField38);
                    if (state != 0)
                    {
                        rev.Reason = "06";
                        SyncTransaction(rev);
                        ShowMessageAndGotoMain("IC确认错误，交易失败，请重试");
                        return;
                    }
                }
                
                rev.ClearReverseFile();//缴费成功之后进入销账流程，不在发冲正报文，清除冲正文件

            CONFIRM:
                //缴费成功，发起确认销账
                CYAPublishPayBillConfirm billConfirm = new CYAPublishPayBillConfirm();
                TransResult retConfirm = SyncTransaction(billConfirm);
                //Test
                //retConfirm = TransResult.E_RECV_FAIL;
                if (retConfirm != TransResult.E_SUCC)
                {
                    //超时无响应循环发送确认报文
                    if (nTryConfirm != 0 && (retConfirm == TransResult.E_RECV_FAIL || billConfirm.ReturnCode == "E1"))
                    {
                        nTryConfirm--;
                        goto CONFIRM;
                    }

                    //销账失败
                    //缴费确认未成功，48小时内系统会自动处理，请耐心等待，不要重复缴费
                    string temp0 = "银行卡扣款成功，但销账失败，由系统自行处理CardNo={0}，凭证号={1}，系统参考号={2}，缴费流水号={3}";
                    string temp = string.Format(temp0, CommonData.BankCardNum, m_entity.PayTraceNo, m_entity.PayReferenceNo, m_entity.PayFlowNo);
                    Log.Warn(temp);
                    
                    StartActivity("雅安支付销账失败");

                }
                else
                {
                    if (ReceiptPrinter.ExistError())
                        StartActivity("雅安支付交易完成");
                    else
                        StartActivity("雅安支付交易成功是否打印");
                }
            }
            else if (retPay == TransResult.E_HOST_FAIL)
            {
                if (beingPay.ReturnCode == "51")
                {
                    ShowMessageAndGotoMain("温馨提示：抱歉！交易失败！" + "\n" +
                        "您卡内余额不足！");
                }
                else if (beingPay.ReturnCode == "55")
                {
                    StartActivity("雅安支付密码错误");
                }
                else
                {
                    ShowMessageAndGotoMain(beingPay.ReturnCode + "-" + beingPay.ReturnMessage);
                }
            }
            else if (retPay == TransResult.E_RECV_FAIL)
            {
                rev.Reason = "98";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("交易超时，请重试");
                return;
            }
            else if (retPay == TransResult.E_UNPACKET_FAIL)
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
