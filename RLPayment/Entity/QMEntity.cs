using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Entity
{
    class QMEntity : BaseEntity
    {
        #region 常量

        public const string SECTION_NAME = "YAPayment";

        #endregion

        #region 信用卡还款
        /// <summary>
        /// 信用卡号
        /// </summary>
        public string CreditcardNum;
        /// <summary>
        /// 还款总金额
        /// </summary>
        public double TotalAmount;
        /// <summary>
        /// 还款手续费
        /// </summary>
        public double Fee;
        /// <summary>
        /// 清算日期
        /// </summary>
        public string Field15;
        #endregion
        
        #region 手机话费直充

        /// <summary>
        /// 运营商类别(01-移动 02-联通 03-电信)
        /// </summary>
        public string MobileType; //运营商类别(01-移动 02-联通 03-电信)
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNo; //手机号码
        /// <summary>
        /// 中间业务流水号
        /// </summary>
        public string MiddleFlowNo; //中间业务流水号

        #endregion

        public QMEntity()
        {
#if DEBUG
            Fee = 2;
            TotalAmount = CommonData.Amount + Fee;

            MiddleFlowNo = "00000012347800000012347800000000";
            PayReferenceNo = "000000123478";
            RecvField38 = "";
            RecvField55 = new byte[0];
#endif
        }

        public override string SectionName
        {
            get { return SECTION_NAME; }
        }
    }
}
