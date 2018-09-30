using Landi.FrameWorks;
using RLPayment.Entity;
using RLPayment.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalLib;

namespace RLPayment.Business.RLCZ
{
    class BeingProcessDeal: FrameActivity
    {
        //private RLCZEntity _entity;
        //protected override void OnEnter()
        //{
        //    base.OnEnter();
        //    try
        //    {
        //        _entity = GetBusinessEntity() as RLCZEntity;
        //        if (BeingProcess() != 0)
        //        {
        //            ShowMessageAndGoBack("交易出错|请返回！");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
        //    }

        //}
        //private int BeingProcess()
        //{
        //    int ret = -1;
        //    CSPospTrans cSPospTrans = new CSPospTrans(_entity);
        //    cSPospTrans.setIPAndPort(_entity.CspospServerIP, _entity.CspospServerPort);
        //    cSPospTrans.transact();
        //    ret = 0;
        //    return ret;
        //}

        protected override void PayCallback(ResponseData ResponseEntity)
        {
            if (ResponseEntity.StepCode == "ProceduresEnd")
            {
                if (ResponseEntity.returnCode == "00")
                {
                    //交易成功
                    StartActivity("热力充值通用成功");
                }
                else if(ResponseEntity.returnCode == "82")
                {
                    ShowMessageAndGoBack("交易失败|" + ResponseEntity.args);
                }
                else
                {
                    ShowMessageAndGotoMain("交易失败|" + ResponseEntity.args);
                }
            }
        }

    }
}
