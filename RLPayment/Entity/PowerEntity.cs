using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package.PowerCardPay;
using YAPayment.Business.PowerCardPay;

namespace YAPayment.Entity
{
    class PowerEntity : BaseEntity
    {
        #region 常量

        public const string SECTION_NAME = "Power";

        #endregion

        public PowerCardInfo PowerCardData = new PowerCardInfo();

        /// <summary>
        /// 电力缴费类型 默认 1=购电卡充值 2=用户号缴费
        /// </summary>
        public int PowerBusiness = 0;

        #region 查询
        
        /// <summary>
        /// 电卡用户号
        /// </summary>
        public string UserID = "";
        /// <summary>
        /// 账单查询流水
        /// </summary>
        public string QueryTraceNo = "";
        /// <summary>
        /// 用户购电次数
        /// </summary>
        public int PowerPayCount = 0;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName = "";
        /// <summary>
        /// 电力资金编号
        /// </summary>
        public string EleFeeNum = "";
        /// <summary>
        /// 电力资金单位结算编号
        /// </summary>
        public string EleFeeAccountNum = "";
        /// <summary>
        /// 电力资金单位结算单位
        /// </summary>
        public string EleFeeAccountName = "";
        /// <summary>
        /// 电力区域编号
        /// </summary>
        public string PowerAreaNum = "";
        /// <summary>
        /// 电卡卡号
        /// </summary>
        public string PowerCardNo = "";
        /// <summary>
        /// 余额
        /// </summary>
        public double PayAmount = 0;
        /// <summary>
        /// 电能表标识
        /// </summary>
        public string PowerIdentity = "";
        /// <summary>
        /// 用户供电单位名称
        /// </summary>
        public string PowerAdvName = "";
        /// <summary>
        /// 电力响应说明
        /// </summary>
        public string PowerReturnMsg = "";

        /// <summary>
        /// 用户号(后付费）
        /// </summary>
        public string DBNo = "";
        /// <summary>
        /// 用户地址
        /// </summary>
        public string UserAddress = "";
        /// <summary>
        /// 用户余额
        /// </summary>
        public double UserAmount = 0;
        /// <summary>
        /// 本次应交金额
        /// </summary>
        public double UserPayAmount = 0;
        /// <summary>
        /// 结果集
        /// </summary>
        public List<UserQueryInfo> UserQueryInfo;


        /// <summary>
        /// 所在城市电业局编码
        /// </summary>
        public string CityPowerNo
        {
            get { return ReadIniFile("CityPowerNo"); }
        }

        #endregion

        #region 支付

        /// <summary>
        /// 账单支付流水
        /// </summary>
        public string PayFlowNo = "";
        #endregion

        #region 确认
        ///<summary>
        /// 账单确认流水
        /// </summary>
        public string ConfirmTraceNo = "";
        ///<summary>
        /// 消息标志位
        /// </summary>
        public string MsgFlag = "";
        ///<summary>
        /// 抄表段号
        /// </summary>
        public string PowerSegNo = "";
        ///<summary>
        /// 本次购电金额
        /// </summary>
        public double PowerPayAmount = 0;
        ///<summary>
        /// 用电类别
        /// </summary>
        public string PowerUseType = "";
        ///<summary>
        /// 电力返回说明
        /// </summary>
        public string PowerRetMsg = ""; 
        ///<summary>
        /// 银联商务订单号
        /// </summary>
        public string PowerPayConfirmCode = "";
        ///<summary>
        /// 写卡金额
        /// </summary>
        public double WriteCardAmount = 0;

        #endregion

        #region 补写卡

        ///<summary>
        /// 补写卡新一代流水号(第一次返回)
        /// </summary>
        public string ReWriteCardTraceNo1 = "";
        ///<summary>
        /// 补写卡新一代流水号(第二次返回)
        /// </summary>
        public string ReWriteCardTraceNo2 = "";
        ///<summary>
        /// 补写卡新一代流水号(第三次返回)
        /// </summary>
        public string ReWriteCardTraceNo3 = "";
        ///<summary>
        /// 补写卡电力返回说明信息
        /// </summary>
        public string ReWriteCardReturnMsg = "";
        ///<summary>
        /// 补写卡写卡金额
        /// </summary>
        public string ReWriteCardAmount = "";
        ///<summary>
        /// 补写卡用户余额
        /// </summary>
        public string ReWriteUserAmount = "";
        #endregion

        #region 校验
        ///<summary>
        /// 校验返回新一代流水号(第一次返回)
        /// </summary>
        public string CheckTraceNo = "";
        ///<summary>
        /// 校验返回写卡金额
        /// </summary>
        public string CheckWriteAmount = "";
        ///<summary>
        /// 校验返回用户余额
        /// </summary>
        public string CheckUserAmount = "";
        ///<summary>
        /// 校验返回订单状态
        /// </summary>
        public string CheckOrderStatus = "";
        ///<summary>
        /// 校验返回充值金额
        /// </summary>
        public string CheckRechargeAmount = "";
        ///<summary>
        /// 校验返回酬金
        /// </summary>
        public string CheckRemuneration = "";
        ///<summary>
        /// 校验返回购电次数
        /// </summary>
        public string CheckBuyEleTimes = "";
        ///<summary>
        /// 校验返回电力返回说明信息
        /// </summary>
        public string CheckReturnMsg = ""; 
        ///<summary>
        /// 校验返回File1
        /// </summary>
        public string CheckBF1 = "";
        ///<summary>
        /// 校验返回File2
        /// </summary>
        public string CheckBF2 = "";
        ///<summary>
        /// 校验返回File31
        /// </summary>
        public string CheckBF31 = "";
        ///<summary>
        /// 校验返回File32
        /// </summary>
        public string CheckBF32 = "";
        ///<summary>
        /// 校验返回File41
        /// </summary>
        public string CheckBF41 = "";
        ///<summary>
        /// 校验返回File42
        /// </summary>
        public string CheckBF42 = "";
        ///<summary>
        /// 校验返回File5
        /// </summary>
        public string CheckBF5 = "";
        ///<summary>
        /// 校验返回sKey1
        /// </summary>
        public string Check57sKey1 = "";
        ///<summary>
        /// 校验返回sKey2
        /// </summary>
        public string Check57sKey2 = "";
        ///<summary>
        /// 校验返回sKey3
        /// </summary>
        public string Check57sKey3 = "";
        #endregion


        public PowerEntity()
        {

#if DEBUG
            QueryTraceNo = "000000123456";
            UserID = "12345678";
            UserName = "兰平品";
            UserAddress = "福建省宁德市霞浦县崇儒乡上水村26号";
            PowerCardNo = "12345678";
            DBNo = "N12345678";
            UserAmount = 1.23;
            UserPayAmount = 13.00;

            PayFlowNo = "000000123478";
            ConfirmTraceNo = "00000012347800000012347800000000";
            PayReferenceNo = "000000123478";
            RecvField38 = "";
            RecvField55 = new byte[0];
            PayAmount = 20;
#endif
        }

        #region 电力业务返回码
        private void PowerRespMessage(string code, ref string mean, ref string show)
        {
            switch (code)
            {
                case "M3": { mean = "已销账"; show = "已销账"; } break;
                case "N0": { mean = "网络故障，请重试"; show = "网络故障，请重试"; } break;
                case "N1": { mean = "22:50-00:10不能缴费"; show = "22:50-00:10不能缴费"; } break;
                case "N2": { mean = "内部异常，请重试"; show = "内部异常，请重试"; } break;
            }
        }

        public override void ParseRespMessage(string code, ref string mean, ref string show)
        {
            switch (BusinessName)
            {
                case PowerPayStratagy.BUSINESSNAME: { PowerRespMessage(code, ref mean, ref show); } break;
            }
        }

        #endregion

        public override string SectionName
        {
            get { return SECTION_NAME; }
        }
    }
}
