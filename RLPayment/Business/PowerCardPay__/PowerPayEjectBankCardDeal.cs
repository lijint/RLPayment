using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayEjectBankCardDeal : Activity, ITimeTick
    {
        private bool mUserTakeCard = false;
        protected override void OnEnter()
        {
            mUserTakeCard = false;
            CardReader.CardOut();
            CommonData.BIsCardIn = false;
            PostAsync(OnResult);
        }

        private void OnResult()
        {
            while (!mUserTakeCard)
            {
                if (TimeIsOut)
                {
                    CardReader.CardCapture();
                    Log.Warn("TakeCard TimeOut Capture Power Card.");
                    StartActivity("主界面");
                    return;
                }
                Sleep(200);
            }
            StartActivity("电力支付再次插入电卡");
        }

        protected override void OnTimeOut()
        {
        }

        #region ITimeTick 成员

        public void OnTimeTick(int count)
        {
            CardReader.CardStatus cs = CardReader.CardStatus.CARD_POS_GATE;
            CardReader.Status s = CardReader.GetStatus(ref cs);
            if (cs == CardReader.CardStatus.CARD_POS_OUT)
            {
                mUserTakeCard = true;
            }
        }

        #endregion
    }
}
