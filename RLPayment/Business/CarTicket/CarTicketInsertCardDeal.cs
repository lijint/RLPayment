using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;

namespace YAPayment.Business.CarTicket
{
    class CarTicketInsertCardDeal : LoopReadActivity
    {
        private CarEntity _carEntity;
        protected override string ReturnId
        {
            get { return "Return"; }
        }

        protected override void HandleResult(Result result)
        {
            _carEntity = GetBusinessEntity<CarEntity>();
            switch (result)
            {
                case Result.Success:
                    {
                        CommonData.BankCardNum = BankCardNum;
                        CommonData.BankCardSeqNum = CardSeqNum;
                        CommonData.BankCardExpDate = ExpDate;
                        CommonData.Track1 = Track1;
                        CommonData.Track2 = Track2;
                        CommonData.Track3 = Track3;
                        CommonData.UserCardType = BankCardType;
                        StartActivity("购票输入密码");
                    }
                    break;
                case Result.HardwareError:
                    //ShowMessageAndGotoMain("读卡器故障");
                    _carEntity.UnlockMessage = "读卡器故障,";
                    StartActivity("解锁车票");
                    break;
                case Result.Fail:
                    CardReader.CardOut();
                    StartActivity("购票读卡错误");
                    break;
                case Result.Cancel:
                case Result.TimeOut:
                    _carEntity.UnlockMessage = "用户取消,";
                    StartActivity("解锁车票");
                    break;
            }
        }

        protected override Result ReadOnce()
        {
            return DefaultRead4();
        }

        protected override void OnLeave()
        {
            if (!CommonData.BIsCardIn)
                CardReader.CancelCommand();
        }
    }
}
