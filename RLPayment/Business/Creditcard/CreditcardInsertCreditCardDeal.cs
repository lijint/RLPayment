using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;

namespace YAPayment.Business.Creditcard 
{
    class CreditcardInsertCreditCardDeal : LoopReadActivity
    {
        private bool mHandInput;

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
                        (GetBusinessEntity() as QMEntity).CreditcardNum = BankCardNum;
#if DEBUG
                        (GetBusinessEntity() as QMEntity).CreditcardNum = "5268550499102077";
#endif
                        StartActivity("信用卡还款输入还款金额");
                    }
                    break;
                case Result.HardwareError:
                    ShowMessageAndGotoMain("读卡器故障");
                    break;
                case Result.Fail:
                    StartActivity("信用卡读卡错误");
                    break;
                case Result.Cancel:
                    if (mHandInput)
                        StartActivity("信用卡还款输入卡号");
                    else
                        StartActivity("主界面");
                    break;
                case Result.TimeOut:
                    StartActivity("主界面");
                    break;
            }
        }

        protected override Result ReadOnce()
        {
            return DefaultRead3();
        }

        protected override void OnLeave()
        {
            base.OnLeave();
            CardReader.CancelCommand();
            CardReader.CardOut();
            CommonData.BIsCardIn = false;
        }

        protected override void OnEnter()
        {
            mHandInput = false;
            GetElementById("HandInput").Click += HandInput_click;
            base.OnEnter();
        }

        private void HandInput_click(object sender, HtmlElementEventArgs e)
        {
            mHandInput = true;
            UserToQuit = true;
        }
    }
}
