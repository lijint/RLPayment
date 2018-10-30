using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Business.RLCZ
{
    class CommonSuccessDeal : FrameActivity
    {
        private RLCZEntity _entity;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;
                GetElementById("Message2").InnerHtml = String.Format("{0:F}", _entity.Amount);
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }

        }
        protected override void FrameReturnClick()
        {
            GotoMain();
        }
    }
}
