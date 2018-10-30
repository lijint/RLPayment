using Landi.FrameWorks;
using RLPayment.Entity;
using RLPayment.Package;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TerminalLib;
using static Landi.FrameWorks.Package.Other.SocketCommunicate;

namespace RLPayment.Business.RLCZ
{
    class BeingCspospTransDeal : FrameActivity
    {
        private RLCZEntity _entity;
        private CSPospTrans cSPospTrans;
        private TransResult result;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += BeingProcess;
                bw.RunWorkerAsync();
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }

        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (result == TransResult.E_SUCC)
            {
                //StartActivity("热力充值正在缴费通知");
                StartActivity("热力充值正在打印");
            }
            else
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err " + "respcode : " + cSPospTrans.respcode + " respmsg : " + cSPospTrans.respmsg + " RETURNCODE:" + cSPospTrans.RETURNCODE + " MESSAGE:" + cSPospTrans.MESSAGE);
                ShowMessageAndGotoMain("预通知失败|" + cSPospTrans.respcode + " == " + cSPospTrans.RETURNCODE);
            }

        }

        private void BeingProcess(object sender, DoWorkEventArgs e)
        {
            cSPospTrans = new CSPospTrans(_entity);
            cSPospTrans.setIPAndPort(_entity.CspospServerIP, _entity.CspospServerPort);

            result = cSPospTrans.transact();
            return;
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
