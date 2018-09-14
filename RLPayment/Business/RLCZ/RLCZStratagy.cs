using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Business.RLCZ
{
    class RLCZStratagy : BusinessStratagy
    {
        public const string BUSINESSNAME = "RLCZ";

        public override string BusinessName
        {
            get { return BUSINESSNAME; }

        }

        public override string MessageActivity
        {
            get { return "热力充值通用错误"; }
        }


        public override BaseEntity BusinessEntity
        {
            get { return new RLCZEntity(); }
        }

    }
}
