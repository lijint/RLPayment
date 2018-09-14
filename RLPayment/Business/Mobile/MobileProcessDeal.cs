using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;
using YAPayment.Package;
using YAPayment.Package.Mobile;

namespace YAPayment.Business.Mobile
{
    class MobileProcessDeal : Activity
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
            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bisICCard = true;

            if (SyncTransaction(new CReverse_YAPaymentPay()) == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("交易超时，请重试");
                return;
            }

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

        private void PayProcess()
        {
            CMobileRecharge mobileRecharge = new CMobileRecharge();
            TransResult retPay = SyncTransaction(mobileRecharge);
            CReverse_YAPaymentPay rev = new CReverse_YAPaymentPay(mobileRecharge);
            //Test
            //retPay = TransResult.E_RECV_FAIL;
            if (retPay == TransResult.E_SUCC)
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

                rev.ClearReverseFile();//有做冲正直接return，否则最后要清除冲正文件
                if (ReceiptPrinter.ExistError())
                    StartActivity("手机充值打印成功");
                else
                    StartActivity("手机充值交易成功");

                #region no use
                //缴费成功，发起确认交易
                //CMobileConfirm mobileConfirm = new CMobileConfirm();
                //TransResult retConfirm = SyncTransaction(mobileConfirm);
                //if (retConfirm == TransResult.E_SUCC)
                //{
                
                //}
                //else if (retConfirm == TransResult.E_HOST_FAIL)
                //{
                //    if (mobileConfirm.ReturnCode.ToUpper() == "ET")
                //    {
                //        //冲正
                //        CReverse_QMPay rev = new CReverse_QMPay(mobileRecharge);
                //        rev.Reason = "06";
                //        SyncTransaction(rev);
                //        ShowMessageAndGotoMain("交易失败");
                //    }
                //    else
                //    {
                //        ShowPringPage();
                //    }
                //}
                //else if (retConfirm == TransResult.E_RECV_FAIL)
                //{
                //    //检查查询交易返回结果
                //    CMobileQuery mobileQuery = new CMobileQuery();
                //    TransResult retQuery = TransResult.E_SUCC;
                //    for (int iPer = 0; iPer < 3; iPer++)
                //    {
                //        retQuery = SyncTransaction(mobileQuery);
                //        if (retQuery == TransResult.E_SUCC)
                //        {
                //            break;
                //        }
                //        else if (retQuery == TransResult.E_HOST_FAIL)
                //        {
                //            if (mobileQuery.ReturnCode.ToUpper() == "ET")
                //            {
                //                //冲正
                //                CReverse_QMPay rev = new CReverse_QMPay(mobileRecharge);
                //                rev.Reason = "06";
                //                SyncTransaction(rev);
                //            }
                //            break;
                //        }
                //        else if (retQuery != TransResult.E_RECV_FAIL)
                //        {
                //            break;
                //        }
                //    }

                //    if (retQuery == TransResult.E_SUCC)
                //    {
                //        ShowPringPage();
                //    }
                //    else if (retQuery == TransResult.E_HOST_FAIL)
                //    {
                //        if (mobileQuery.ReturnCode.ToUpper() == "ET")
                //        {
                //            ShowMessageAndGotoMain("交易失败");
                //        }
                //        else
                //        {
                //            ShowPringPage();
                //        }
                //    }
                //    else //查询交易其他情况
                //    {
                //        ShowPringPage();
                //    }
                //}
                //else //确认交易其他情况
                //{
                //    ShowPringPage();
                //}
                #endregion
            }
            else if (retPay == TransResult.E_HOST_FAIL)
            {
                if (mobileRecharge.ReturnCode == "51")
                {
                    ShowMessageAndGotoMain("温馨提示：抱歉！交易失败！" + "\n" +
                        "您卡内余额不足！");
                }
                else if (mobileRecharge.ReturnCode == "55")
                {
                    StartActivity("手机充值密码错误");
                }
                else
                {
                    ShowMessageAndGotoMain(mobileRecharge.ReturnCode + "-" +
                        mobileRecharge.ReturnMessage);
                }

            }
            else if (retPay == TransResult.E_RECV_FAIL)
            {
                rev.Reason = "98";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("交易失败");
                return;
            }
            else if (retPay == TransResult.E_CHECK_FAIL)
            {
                rev.Reason = "96";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("系统异常，请稍后再试");
                return;
            }
            else
            {
                ShowMessageAndGotoMain("交易失败");
            }
            rev.ClearReverseFile();//有做冲正直接return，否则最后要清除冲正文件
        }
    }
}