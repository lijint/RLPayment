using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;

namespace YAPayment.Business.TrafficPolice
{
    /// <summary>
    /// 插入银行卡
    /// </summary>
    class YATrafficPoliceInsertBankCardDeal : LoopReadActivity
    {
        protected override string ReturnId
        {
            get { return "Return"; }
        }

        protected override void HandleResult(Result result)
        {
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
                        StartActivity("雅安交警罚没输入密码");
                    }
                    break;
                case Result.HardwareError:
                    ShowMessageAndGotoMain("读卡器故障");
                    break;
                case Result.Fail:
                    CardReader.CardOut();
                    StartActivity("雅安交警罚没读卡错误");
                    break;
                case Result.Cancel:
                    StartActivity("雅安交警罚没金额确认"); 
                    break;
                case Result.TimeOut:
                    StartActivity("主界面");
                    break;
            }
        }

        protected override Result ReadOnce()
        {
            return DefaultRead4();
        }

        protected override void OnLeave()
        {
            if(!CommonData.BIsCardIn)
                CardReader.CancelCommand();
        }
    }
}
