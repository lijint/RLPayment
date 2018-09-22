using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Business.RLCZ
{
    class BeingQueryDeal : FrameActivity
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                if (querymsg() == 0)
                {
                    StartActivity("热力充值查询结果");
                }
                else
                {
                    ShowMessageAndGoBack("查询出错|请返回！");
                }
            }
            catch(Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }

        private int querymsg()
        {
            int ret = -1;
            ret = 0;
            return ret;
        }

    }
}
