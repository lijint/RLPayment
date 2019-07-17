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
    class BeingQueryDeal : FrameActivity
    {
        private RLCZEntity _entity;
        private TransResult result;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;
                result = TransResult.E_INVALID;
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += querymsg;
                bw.RunWorkerAsync();
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            }
            catch(Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (result == TransResult.E_SUCC)
            {
                if (_entity.CompanyCode == "01")
                {
                    //01-济宁新东供热有限责任公司 821370849000006
                    if (_entity.gBranchNo!= "821370849000006")
                    {
                        ShowMessageAndGoBack("查询出错|非本公司用户");
                        return;
                    }
                }
                else if (_entity.CompanyCode == "02")
                {
                    //02-济宁高新公用事业发展股份有限公司 821370849000005
                    if (_entity.gBranchNo != "821370849000005")
                    {
                        ShowMessageAndGoBack("查询出错|非本公司用户");
                        return;
                    }
                }
                else
                {
                    ShowMessageAndGoBack("查询出错|返回公司类型错误" + _entity.CompanyCode);
                    return;
                }
                StartActivity("热力充值查询结果");
            }
            else
            {
                ShowMessageAndGoBack("查询出错|" + _entity.ReturnCode + _entity.ReturnMsg);
            }

        }

        private void querymsg(object sender, DoWorkEventArgs e)
        {
            try
            {
                CRLQueryTrans cSPospTrans = new CRLQueryTrans(_entity);
                cSPospTrans.setIPAndPort(_entity.RLServerIP, _entity.RLServerPort);
                result = cSPospTrans.transact();
            }
            catch(Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
            return;
        }

    }
}
