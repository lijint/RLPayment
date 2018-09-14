using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;
using YAPayment.Package.PowerCardPay;
using Landi.FrameWorks.HardWare;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayInsertPowerCardAgainDeal : LoopReadActivity
    {
        protected override string ReturnId
        {
            get { return "Return"; }
        }

        protected override void HandleResult(Result result)
        {
            PowerEntity entity = GetBusinessEntity() as PowerEntity;
            switch (result)
            {
                case Result.Success:
                    {
                        ReportSync("PowerWriteCard");
                        //System.Threading.Thread.Sleep(5000);
                        if (!new CPowerCard().WritePowerCard(entity.PowerCardData))
                        {
                            //自动调用补写卡操作
                            ReportSync("PowerWriteCardAgain");
                            //System.Threading.Thread.Sleep(5000);
                            TransResult transR = new CPowerCardWriteAgain().WritePowerCardAgain();
                            if (transR != TransResult.E_SUCC)
                                goto case Result.Fail;
                        }

                        if (ReceiptPrinter.ExistError())
                            StartActivity("电力支付交易完成");
                        else
                            StartActivity("电力支付交易成功是否打印");
                    }
                    break;
                case Result.HardwareError:
                case Result.Fail:
                case Result.Cancel:
                case Result.TimeOut:
                    StartActivity("电力支付写卡失败");
                    break;
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
                GetElementById("Msg").InnerText = "写卡失败，自动补写卡中，请稍等";
            }
        }

        protected override void OnLeave()
        {
            if (!CommonData.BIsCardIn)
                CardReader.CancelCommand();
        }
    }
}
