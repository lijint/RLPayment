using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Business.RLCZ
{
    class BeingQueryQRCodeDeal : FrameActivity
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                if(queryQRCodeDeal()==0)
                {
                     
                }
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }

        private int queryQRCodeDeal()
        {
            int ret = -1;
            ret = 0;
            return ret;
        }
    }
}
