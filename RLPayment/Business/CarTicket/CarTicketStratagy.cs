using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.CarTicket
{
    class CarTicketStratagy : BusinessStratagy
    {
        public const string BUSINESSNAME = "Car";
        public override string BusinessName
        {
            get { return BUSINESSNAME; }
        }

        public override string MessageActivity
        {
            get { return "购票通用错误"; }
        }

        public override BaseEntity BusinessEntity
        {
            get { return new CarEntity(); }
        }
    }
}
