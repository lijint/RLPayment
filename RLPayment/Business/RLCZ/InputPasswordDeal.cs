using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RLPayment.Business.RLCZ
{
    class InputPasswordDeal : FrameActivity
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {

            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }
        protected override void FrameReturnClick()
        {
            StartActivity("退卡");
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            base.OnKeyDown(keyCode);
            switch (keyCode)
            {
                case Keys.Enter://确定
                    StartActivity("热力充值正在交易");
                    break;
            }

        }
    }
}
