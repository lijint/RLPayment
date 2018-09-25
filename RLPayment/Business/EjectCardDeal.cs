using Landi.FrameWorks;
using System;
using TerminalLib;

namespace RLPayment.Business
{
    class EjectCardDeal : FrameActivity, ITimeTick
    {
        //private bool mUserTakeCard;
        //protected override void OnEnter()
        //{
        //    mUserTakeCard = false;
        //    EjectCard();
        //    //PostAsync(OnResult);
        //}

        //private void OnResult()
        //{
        //    while (!mUserTakeCard)
        //    {
        //        if (TimeIsOut)
        //        {
        //            Log.Warn("TakeCard TimeOut Capture Card.");
        //            break;
        //        }
        //        Sleep(200);
        //    }
        //    StartActivity("主界面");
        //}

        //protected override void OnTimeOut()
        //{
        //}

        //private void EjectCard()
        //{
        //    RequestData _request = new RequestData();

        //    Global.gTerminalPay.BusinessLib = String.Format("{0}.CardReaderService", Global.gBankCardLibName);
        //    Global.gTerminalPay.EjectCard(_request);

        //    ResponseData ResponseEntity = Global.gTerminalPay.ResponseEntity;
        //    string strHtml = ResponseEntity.args;
        //    if (ResponseEntity.StepCode == "ProceduresEnd")
        //    {
        //        if (ResponseEntity.returnCode == "00")
        //        {
        //            StartActivity("主界面");
        //            //strHtml = "已退卡";
        //        }
        //        else
        //        {
        //            Log.Error("退卡失败" + strHtml);
        //        }
        //    }
        //}

        #region ITimeTick 成员

        public void OnTimeTick(int count)
        {
            ResponseData ResponseEntity = Global.gTerminalPay.ResponseEntity;
            string strHtml = ResponseEntity.args;
            if (ResponseEntity.StepCode == "ProceduresEnd")
            {
                if (ResponseEntity.returnCode == "00")
                {
                    StartActivity("主界面");
                    //strHtml = "已退卡";
                }
                else
                {
                    Log.Error("退卡失败" + strHtml);
                }
            }
        }
        #endregion
    }
}
