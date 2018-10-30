using Landi.FrameWorks;
using RLPayment.Entity;
using RLPayment.Package;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using static Landi.FrameWorks.Package.Other.SocketCommunicate;

namespace RLPayment.Business.RLCZ
{
    class BeingNoticeDeal : FrameActivity
    {
        private RLCZEntity _entity;
        private TransResult result;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += querymsg;
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
                StartActivity("热力充值正在打印");
                //StartActivity("热力充值通用成功");
            }
            else
            {
                ShowMessageAndGoBack("缴费通知出错|" + _entity.ReturnCode + _entity.ReturnMsg);
            }
        }

        private void querymsg(object sender, DoWorkEventArgs e)
        {
            try
            {
                CNoticeTrans cSPospTrans = new CNoticeTrans(_entity);
                cSPospTrans.setIPAndPort(_entity.RLServerIP, _entity.RLServerPort);
                result = cSPospTrans.transact();
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
            return;
        }

    }
}
