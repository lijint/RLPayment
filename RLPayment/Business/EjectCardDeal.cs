using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;

namespace YAPayment.Business
{
    class EjectCardDeal : Activity, ITimeTick
    {
        private bool mUserTakeCard;
        protected override void OnEnter()
        {
            mUserTakeCard = false;
            CardReader.CardOut();
            PostAsync(OnResult);
        }

        private void OnResult()
        {
            while (!mUserTakeCard)
            {
                if (TimeIsOut)
                {
                    CardReader.CardCapture();
                    Log.Warn("TakeCard TimeOut Capture Card.");
                    break;
                }
                Sleep(200);
            }
            StartActivity("主界面");
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
