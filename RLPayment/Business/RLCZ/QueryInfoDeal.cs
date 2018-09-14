using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RLPayment.Business.RLCZ
{
    class QueryInfoDeal : FrameActivity
    {

        private RLCZEntity _entity;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;
                GetElementById("ok").Click += new HtmlElementEventHandler(OKClick);
            }
            catch(Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }

        private void OKClick(object sender, HtmlElementEventArgs e)
        {
            if (_entity.Amount == 0)
                return;
            StartActivity("热力充值查入银行卡");
        }

        protected override void FrameReturnClick()
        {
            StartActivity("热力充值输入热力号");
        }
    }
}
