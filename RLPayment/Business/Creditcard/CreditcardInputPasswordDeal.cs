using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.Creditcard
{
    /// <summary>
    /// 输入密码
    /// </summary>
    class CreditcardInputPasswordDeal : EsamActivity
    {
        protected override void OnEnter()
        {
            GetElementById("Amount").InnerText = CommonData.Amount.ToString("#######0.00");
            GetElementById("Account").InnerText = (GetBusinessEntity() as QMEntity).CreditcardNum;
            base.OnEnter();
        }

        protected override void OnClearNotice()
        {
            InvokeScript("hideBankPassLenError");
        }

        protected override string InputId
        {
            get { return "pin"; }
        }

        protected override void OnErrorLength()
        {
            InvokeScript("showBankPassLenError");
        }

        protected override void HandleResult(Result result)
        {
            if (result == Result.Success)
            {
                CommonData.BankPassWord = Password;
                StartActivity("信用卡还款正在交易");
            }
            else if (result == Result.Cancel || result == Result.TimeOut)
                GotoMain();
            else if (result == Result.HardwareError)
                ShowMessageAndGotoMain("密码键盘故障");
        }

        protected override string SectionName
        {
            get { return GetBusinessEntity().SectionName; }
        }
    }
}
