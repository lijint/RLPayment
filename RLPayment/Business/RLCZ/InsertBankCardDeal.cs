﻿using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Business.RLCZ
{
    class InsertBankCardDeal : FrameActivity
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
            StartActivity("热力充值缴费方式选择");
        }
    }
}