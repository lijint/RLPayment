using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;
using YAPayment.Package.PowerCardPay;
using Landi.FrameWorks.HardWare;
using YAPayment.Package;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayInsertPowerCardAgainDeal : LoopReadActivity
    {
        protected override string ReturnId
        {
            get { return "Return"; }
        }

        TransResult retConfirm = TransResult.E_RECV_FAIL;

        private void ConfirmTrans()
        {
            CPowerPayBillConfirm billConfirm = new CPowerPayBillConfirm();
            retConfirm = billConfirm.BillConfirm();
        }

        protected override void HandleResult(Result result)
        {
            int nTryConfirm = 3;
            PowerEntity entity = GetBusinessEntity() as PowerEntity;

            CPowerCard powerCard = new CPowerCard();
            if (!powerCard.ReadPowerCard(entity.PowerCardData))
            {
                StartActivity("电力支付写卡失败");//
                return;
            }
            CONFIRM:
            //缴费成功，发起确认销账
            retConfirm = TransResult.E_RECV_FAIL;
            //PostAsync(ConfirmTrans);
            ConfirmTrans();
            if (retConfirm != TransResult.E_SUCC)//如果返回失败结果，超时有重复3次
            {
                //超时无响应循环发送确认报文
                if (nTryConfirm != 0 && (retConfirm == TransResult.E_RECV_FAIL))
                {
                    nTryConfirm--;
                    goto CONFIRM;
                }

                //销账失败
                //缴费确认未成功，48小时内系统会自动处理，请耐心等待，不要重复缴费
                string temp0 = "银行卡扣款成功，但销账失败，由系统自行处理CardNo={0}，凭证号={1}，系统参考号={2}，缴费流水号={3}";
                string temp = string.Format(temp0, CommonData.BankCardNum, entity.PayTraceNo,
                    entity.PayReferenceNo, entity.PayFlowNo);
                Log.Warn(temp);
                StartActivity("电力支付销账失败");//
            }
            else
            {
                Log.Info("用户号：" + entity.PowerCardNo + "银联商户订单号：" + entity.PowerPayConfirmCode);
                ReportSync("PowerWriteCard");
                //System.Threading.Thread.Sleep(5000);
                //if (!new CPowerCard().WritePowerCard(entity.PowerCardData))
                //{
                    //自动调用补写卡操作
                    //ReportSync("PowerWriteCardAgain");
                System.Threading.Thread.Sleep(10000);
                for (int i = 0; i < 4; i++)
                {
                    TransResult transRet = new CPowerCardWriteAgain().WritePowerCardAgain();
                    if (transRet != TransResult.E_SUCC && i==2)
                    {
                        StartActivity("电力支付写卡失败");
                        return;
                    }
                    if (transRet == TransResult.E_SUCC)
                    {
                        if (ReceiptPrinter.ExistError())
                            StartActivity("电力支付交易完成");
                        else
                            StartActivity("电力支付交易成功是否打印");
                        break;
                    }
                    System.Threading.Thread.Sleep(500);
                }
                //}
                //if (!powerCard.ReadPowerCard(entity.PowerCardData))
                //{
                //    StartActivity("电力支付写卡失败");//
                //    return;
                //}
                //写卡成功
                //for (int i = 0; i < 3; i++)
                //{
                //    //购电校验
                //    ReportSync("PowerCheckAmount");
                //    retConfirm = ConfirmBillProcess();
                //    if (retConfirm == TransResult.E_SUCC)
                //    {
                //        Log.Info("entity.CheckBuyAmount:" + double.Parse(Utility.StringToAmount(entity.CheckWriteAmount)) + " entity.PowerPayAmount:" + CommonData.Amount + " entity.CheckWriteCardAmount:" + double.Parse(Utility.StringToAmount(entity.CheckRechargeAmount)));
                //        if ((double.Parse(Utility.StringToAmount(entity.CheckWriteAmount)) == CommonData.Amount) || (double.Parse(Utility.StringToAmount(entity.CheckRechargeAmount)) == CommonData.Amount))
                //        {
                //            //校验成功
                //            if (ReceiptPrinter.ExistError())
                //                StartActivity("电力支付交易完成");
                //            else
                //                StartActivity("电力支付交易成功是否打印");
                //            break;
                //        }
                //        else
                //        {
                //            //校验失败，进行补写卡
                //            if (i < 2)
                //            {
                //                ReportSync("PowerWriteCardAgain");
                //                //System.Threading.Thread.Sleep(5000);
                //                TransResult transR = new CPowerCardWriteAgain().WritePowerCardAgain();
                //                if (transR != TransResult.E_SUCC)
                //                {
                //                    StartActivity("电力支付写卡失败");
                //                    break;
                //                }
                //                continue;
                //            }
                //            else
                //            {
                //                ShowMessageAndGotoMain("购电校验失败, 请到柜台处理");
                //                break;
                //            }
                //        }
                //    }
                //    if (retConfirm == TransResult.E_RECV_FAIL)
                //    {
                //        //交易超时，再次校验
                //        if (i < 2)
                //        {
                //            continue;
                //        }
                //        else
                //        {
                //            ShowMessageAndGotoMain("购电校验超时, 请到柜台处理");
                //            break;
                //        }
                //    }
                //    else
                //        break;
                //}
            }
            
        }

        protected override Result ReadOnce()
        {
            return InsertICCard();
        }

        protected override void OnReport(object progress)
        {
            base.OnReport(progress);
            string msg = (string)progress;
            if (msg == "PowerWriteCard")
            {
                GetElementById("Msg").InnerText = "正在写购电卡信息，请稍等";
            }
            else if (msg == "PowerWriteCardAgain")
            {
                GetElementById("Msg").InnerText = "补写卡中，请稍等";
            }

            else if (msg == "PowerCheckAmount")
            {
                GetElementById("Msg").InnerText = "写卡完成，金额校验中，请稍等";
            }
        }

        /// <summary>
        /// 购电完成后，购电确认交易
        /// </summary>
        /// <returns></returns>
        private CPowerCardBillCheck billConfirm = new CPowerCardBillCheck();

        private TransResult ConfirmBillProcess()
        {
            TransResult retConfirm = TransResult.E_RECV_FAIL;

            PostAsync(billcheck);
            if (retConfirm != TransResult.E_SUCC && retConfirm != TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain(billConfirm.ReturnCode + "-" + billConfirm.ReturnMessage);
            }
            return retConfirm;
        }

        private void billcheck()
        {
            try
            {
                retConfirm = billConfirm.BillCheck();
            }
            catch (Exception ex)
            {
                Log.Debug("bullcheck err:", ex);
            }
        }

        protected override void OnLeave()
        {
            if (!CommonData.BIsCardIn)
                CardReader.CancelCommand();
        }
    }
}
