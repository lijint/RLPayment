using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;
using YAPayment.Package.PowerCardPay;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayInsertPowerCardDeal : LoopReadActivity
    {
        protected override string ReturnId
        {
            get { return "Return"; }
        }

        protected override void HandleResult(Result result)
        {
            PowerEntity entity = GetBusinessEntity() as PowerEntity;
            //Test
            //result = Result.Fail;
            switch (result)
            {
                case Result.Success:
                    {
                        ReportSync("PowerReadCard");
                        if(!new CPowerCard().ReadPowerCard(entity.PowerCardData))
                            goto case Result.Fail;
                        StartActivity("电力支付账单查询");
                    }
                    break;
                case Result.HardwareError:
                    ShowMessageAndGotoMain("读卡器故障");
                    break;
                case Result.Fail:
                    CardReader.CardOut();
                    StartActivity("电力支付读电卡错误");
                    break;
                case Result.Cancel:
                    StartActivity("主界面");
                    break;
                case Result.TimeOut:
                    StartActivity("主界面");
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
            if (msg == "PowerReadCard")
            {
                GetElementById("Msg").InnerText = "正在读取购电卡信息，请稍等";
            }
        }

        protected override void OnLeave()
        {
            if (!CommonData.BIsCardIn)
                CardReader.CancelCommand();
        }
    }
}
