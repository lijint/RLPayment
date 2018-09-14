using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Business.RLCZ
{
    class CommonSuccessDeal : FrameActivity
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {

            }
            catch(Exception ex)
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
