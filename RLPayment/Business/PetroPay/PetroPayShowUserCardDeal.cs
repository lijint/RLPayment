using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks.HardWare;
using Landi.FrameWorks;
using YAPayment.Package;

namespace YAPayment.Business.PetroPay
{
    class PetroPayShowUserCardDeal : LoopReadActivity
    {
        private bool mHandInput;
        private string mCardNo;
        protected override void HandleResult(Result result)
        {
            switch (result)
            {
                case Result.Cancel:
                    if (!mHandInput)
                        GotoMain();
                    else
                        StartActivity(typeof(PetroPayUserLoginDeal));
                    break;
                case Result.TimeOut:
                    GotoMain();
                    break;
                case Result.Success:
                    YAPaymentPay.LoginName = "0000" + mCardNo;
                    StartActivity("中石油支付输入用户密码");
                    break;
            }
        }

        protected override Result ReadOnce()
        {
            R80.ActivateResult ret = R80.ICActive(0, ref mCardNo);
            if (ret == R80.ActivateResult.ET_SETSUCCESS)
                return Result.Success;
            return Result.Again;
        }

        protected override string ReturnId
        {
            get { return "Return"; }
        }

        protected override void OnEnter()
        {
            mCardNo = "";
            mHandInput = false;
            GetElementById("HandInput").Click += new HtmlElementEventHandler(PetroPayShowUserCardDeal_Click);
            base.OnEnter();
        }

        void PetroPayShowUserCardDeal_Click(object sender, HtmlElementEventArgs e)
        {
            mHandInput = true;
            UserToQuit = true;
        }
    }
}
