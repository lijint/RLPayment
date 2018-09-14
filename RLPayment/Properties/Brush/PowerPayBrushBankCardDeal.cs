using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;

namespace YAPayment.Business.PowerCardPay.Brush
{
    class PowerPayBrushBankCardDeal : Activity, ITimeTick
    {
        private bool mUserPutCard = false;
        protected override void OnEnter()
        {
            mUserPutCard = false;
            PostAsync(OnResult);
        }

        private void OnResult()
        {
            while (!mUserPutCard)
            {
                if (TimeIsOut)
                {
                    Log.Warn("PutCard TimeOut");
                    StartActivity("主界面");
                    return;
                }
                Sleep(200);
            }
            StartActivity("电力支付感应正在交易");
        }

        protected override void OnTimeOut()
        {
        }

        #region ITimeTick 成员

        public void OnTimeTick(int count)
        {
            lock (this)
            {
                try
                {
                    if (mUserPutCard)
                        return;
                    
                    string mCardNo = "";
                    R80.ActivateResult ret = R80.ICActive(0, ref mCardNo);
                    if (ret == R80.ActivateResult.ET_SETSUCCESS)
                    {
                        mUserPutCard = true;
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("[MainPageDeal][OnTimeTick]Error", ex);
                }
            }
        }

        #endregion
    }
}
