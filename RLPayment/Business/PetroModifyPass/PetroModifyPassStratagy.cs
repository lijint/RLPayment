using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Business.PetroModifyPass
{
    class PetroModifyPassStratagy : BusinessStratagy
    {
        public override string BusinessName
        {
            get { return "PetroModifyPass"; }
        }

        public override string MessageActivity
        {
            get { return "中石油修改密码通用错误提示"; }
        }
    }
}
