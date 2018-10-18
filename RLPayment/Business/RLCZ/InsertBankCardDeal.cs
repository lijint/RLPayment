using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalLib;

namespace RLPayment.Business.RLCZ
{
    class InsertBankCardDeal : FrameActivity
    {
        private int FlagCancel;
        private RLCZEntity _entity;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                FlagCancel = 0;
                _entity = GetBusinessEntity() as RLCZEntity;
                RequestData request = new RequestData();

                /// 消费金额
                request.Amount = _entity.Amount;

                Global.gTerminalPay.BusinessLib = String.Format("{0}.PayService", Global.gBankCardLibName);
                Global.gTerminalPay.RequestEntity = request;
                Global.gTerminalPay.Pay(request);

            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }
        protected override void FrameReturnClick()
        {
            Global.gTerminalPay.WaitInsertCardCancel();
            StartActivity("热力充值正在返回");
        }

        protected override void InsertCardEnd()
        {
            CommonData.BankCardNum = Global.gTerminalPay.ResponseEntity.CardNumber;
            CommonData.BankCardExpDate = Global.gTerminalPay.ResponseEntity.ExpireDate;
        }

        protected override void GotoNextActivity()
        {
            StartActivity("热力充值输入密码");
        }
        protected override void HasCardInside()
        {
            GetElementById("insertcardmsg").InnerHtml = "正在读卡，请稍后...";
            setRetBtnDisplay(false);
        }

        protected override void EEError()
        {
            ShowMessageAndGotoMain("读卡出错|" + Global.gTerminalPay.ResponseEntity.args);
        }

        //protected override void InsertCardCancel()
        //{
        //    if (Global.gTerminalPay.ResponseEntity.returnCode == "03")
        //    {
        //        FlagCancel = 1;
        //    }
        //}
        //protected override void PayCallback(ResponseData ResponseEntity)
        //{
        //    if (ResponseEntity.StepCode == "ProceduresEnd" && ResponseEntity.returnCode == "24" && FlagCancel == 1)
        //    {
        //        StartActivity("热力充值缴费方式选择");
        //    }
        //}

    }
}
