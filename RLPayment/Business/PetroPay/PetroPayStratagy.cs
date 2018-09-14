using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package;

namespace YAPayment.Business.PetroPay
{
    class PetroPayStratagy:BusinessStratagy
    {
        public override string BusinessName
        {
            get { return "PetroPay"; }
        }

        public override string MessageActivity
        {
            get { return "中石油支付通用错误提示"; }
        }
    }
}
