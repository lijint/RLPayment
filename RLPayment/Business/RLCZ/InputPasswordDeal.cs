using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TerminalLib;

namespace RLPayment.Business.RLCZ
{
    class InputPasswordDeal : FrameActivity
    {
        private RLCZEntity _entity;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;
                if (!string.IsNullOrEmpty(CommonData.BankCardNum))
                    GetElementById("cardNo").InnerHtml = CommonData.BankCardNum;
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }
        protected override void FrameReturnClick()
        {
            StartActivity("退卡");
        }

        //protected override void OnKeyDown(Keys keyCode)
        //{
        //    base.OnKeyDown(keyCode);
        //    switch (keyCode)
        //    {
        //        case Keys.Enter://确定
        //            StartActivity("热力充值正在交易");
        //            break;
        //    }
        //}

        protected override void InputPassword()
        {
            GetElementById("pin").InnerText = Global.gTerminalPay.ResponseEntity.args;
        }
        protected override void EEError()
        {
            ShowMessageAndGotoMain("密码输入错误|" + Global.gTerminalPay.ResponseEntity.args);
        }
        protected override void GotoProcess()
        {
            StartActivity("热力充值正在交易");
        }
        protected override void InputPasswordErr()
        {
            ShowMessageAndGotoMain("密码输入错误|" + Global.gTerminalPay.ResponseEntity.args);
        }
        protected override void PayCallback(ResponseData ResponseEntity)
        {
            if (ResponseEntity.StepCode == "ProceduresEnd")
            {
                if (ResponseEntity.returnCode == "04")
                {
                    //用户取消输入
                    StartActivity("退卡");
                }
            }
        }
    }
}
