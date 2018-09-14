using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;
using YAPayment.Package;
using YAPayment.Package.PowerCardPay;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayBeingPayDeal : Activity
    {
        private PowerEntity m_entity = null;
        private EMVTransProcess emv = null;
        private bool bemvInit = false;
        private bool bisICCard = false;

        protected override void OnEnter()
        {
            DestroySelf();//设置成自动销毁，每次重新生成
            bemvInit = false;
            bisICCard = false;
            m_entity = GetBusinessEntity() as PowerEntity;
            emv = new EMVTransProcess();

            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bisICCard = true;

            if (SyncTransaction(new CReverse_PowerPay()) == TransResult.E_RECV_FAIL)
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
            //int nTryConfirm = 3;

            CPowerPayBeingPay beingPay = new CPowerPayBeingPay();
            TransResult retPay = SyncTransaction(beingPay);
            CReverse_PowerPay rev = new CReverse_PowerPay(beingPay);
            //Test
            //retPay = TransResult.E_HOST_FAIL;
            //beingPay.ReturnCode = "55";
            //rev.Reason = "98";
            //retPay = SyncTransaction(rev);

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
                StartActivity("电力支付退出银行卡");
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
                    StartActivity("电力支付密码错误");
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

        //TransResult retConfirm = TransResult.E_RECV_FAIL;
        //void ConfirmTrans()
        //{
        //    CPowerPayBillConfirm billConfirm = new CPowerPayBillConfirm();
        //    retConfirm = billConfirm.BillConfirm();
        //}
    }
}
