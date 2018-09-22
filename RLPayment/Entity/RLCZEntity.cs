using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Entity
{
    class RLCZEntity : BaseEntity
    {
        #region 常量

        public const string SECTION_NAME = "RLCZPayment";

        #endregion

        public string CardNO;
        public decimal Amount;
        public int PayType;             //0---YHK 1---WX 2---ZFB

        public override string SectionName
        {
            get { return SECTION_NAME; }
        }
    }
}
