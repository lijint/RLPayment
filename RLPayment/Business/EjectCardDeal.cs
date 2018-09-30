using Landi.FrameWorks;
using System;
using TerminalLib;

namespace RLPayment.Business
{
    class EjectCardDeal : FrameActivity, ITimeTick
    {

        protected override void FrameReturnClick()
        {
            GotoMain();
        }


        protected override void OnTimeOut()
        {
            GotoMain();
        }

        #region ITimeTick 成员

        public void OnTimeTick(int count)
        {
            //RequestData _request = new RequestData();
            //Global.gTerminalPay.BusinessLib = String.Format("{0}.CardReaderService", Global.gBankCardLibName);
            //Global.gTerminalPay.EjectCard(_request);
            //ResponseData ResponseEntity = Global.gTerminalPay.ResponseEntity;
            //string strHtml = ResponseEntity.args;
            //if (ResponseEntity.StepCode == "ProceduresEnd")
            //{
            //    if (ResponseEntity.returnCode == "00")
            //    {
            //        StartActivity("主界面");
            //        //strHtml = "已退卡";
            //    }
            //    else
            //    {
            //        Log.Error("退卡失败" + strHtml);
            //    }
            //}
        }
        #endregion
    }
}
