using Landi.FrameWorks;
using RLPayment.Entity;
using RLPayment.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalLib;
using static Landi.FrameWorks.Package.Other.SocketCommunicate;

namespace RLPayment.Business.RLCZ
{
    class BeingCspospTransDeal : FrameActivity
    {
        private RLCZEntity _entity;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;
                if (BeingProcess() == 0)
                {
                    StartActivity("热力充值正在缴费通知");
                }
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }

        }
        private int BeingProcess()
        {
            int ret = -1;
            CSPospTrans cSPospTrans = new CSPospTrans(_entity);
            cSPospTrans.setIPAndPort(_entity.CspospServerIP, _entity.CspospServerPort);
            TransResult result = cSPospTrans.transact();
            if (result == TransResult.E_SUCC)
            {
                ret = 0;
            }
            else
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err " + "respcode : " + cSPospTrans.respcode + " respmsg : " + cSPospTrans.respmsg + "RETURNCODE:" + cSPospTrans.RETURNCODE + " MESSAGE" + cSPospTrans.MESSAGE);
                ShowMessageAndGotoMain("预通知失败|" + cSPospTrans.respcode + " == " + cSPospTrans.RETURNCODE);
            }
            return ret;
        }

        protected override void InsertCardStart()
        {
            _entity.OrderNumber = _entity.gTerminalNo + DateTime
                .Now.ToString("yyyyMMddhhmmss")+ _entity.gTraceNo+(new Random()).Next(1000,10000).ToString();
            if (BeingProcess() == 0)
                StartActivity("热力充值插入银行卡");
        }

        protected override void PreCreateSucc()
        {
            _entity.OrderNumber = Global.gTerminalPay.ResponseEntity.PosTraceNumber;
            if (BeingProcess() == 0)
                StartActivity("热力充值二维码显示");
            else
            {
                Global.gTerminalPay.WaitInsertCardCancel();
            }
        }

        protected override void PayCallback(ResponseData ResponseEntity)
        {
            if (ResponseEntity.StepCode == "ProceduresEnd")
            {
                if (ResponseEntity.returnCode != "00")
                {
                    ShowMessageAndGotoMain("预通知失败|" + ResponseEntity.returnCode);
                }
            }
        }

    }
}
