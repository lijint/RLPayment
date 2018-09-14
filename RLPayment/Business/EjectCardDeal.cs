using Landi.FrameWorks;
using System;
using TerminalLib;

namespace RLPayment.Business
{
    class EjectCardDeal : Activity, ITimeTick
    {
        private bool mUserTakeCard;
        protected override void OnEnter()
        {
            mUserTakeCard = false;
            PostAsync(OnResult);
        }

        private void OnResult()
        {
            while (!mUserTakeCard)
            {
                if (TimeIsOut)
                {
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

        private void EjectCard()
        {
            //RequestData _request = new RequestData();
            //terminalPay.BusinessLib = String.Format("{0}.CardReaderService", bankCardLibName);
            //terminalPay.EjectCard(_request);

            //ResponseData ResponseEntity = terminalPay.ResponseEntity;
            //string strHtml = ResponseEntity.args;
            //if (ResponseEntity.StepCode == "ProceduresEnd")
            //{
            //    if (ResponseEntity.returnCode == "00")
            //    {
            //        strHtml = "已退卡";
            //    }
            //    else
            //    {
            //        strHtml = "退卡失败";
            //    }
            //    ShowAppMessage(strHtml);
            //}
        }

        #region ITimeTick 成员

        public void OnTimeTick(int count)
        {
            //CardReader.CardStatus cs = CardReader.CardStatus.CARD_POS_GATE;
            //CardReader.Status s = CardReader.GetStatus(ref cs);
            //if (cs == CardReader.CardStatus.CARD_POS_OUT)
            //{
            //    mUserTakeCard = true;
            //}
        }
        #endregion
    }
}
