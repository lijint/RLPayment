using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalLib;

namespace RLPayment.Business.RLCZ
{
    class BeingBackDeal : FrameActivity
    {
        public int FlagCancel { get; private set; }

        protected override void OnEnter()
        {
            base.OnEnter();
            FlagCancel = 0;
        }

        protected override void InsertCardCancel()
        {
            if (Global.gTerminalPay.ResponseEntity.returnCode == "03")
            {
                FlagCancel = 1;
            }
        }

        protected override void PayCallback(ResponseData ResponseEntity)
        {
            //if (ResponseEntity.StepCode == "ProceduresEnd")
            //{
            //    if (ResponseEntity.returnCode != "00")
            //    {
                    StartActivity("热力充值缴费方式选择");
            //    }
            //}
        }
    }
}
