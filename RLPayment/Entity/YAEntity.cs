using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Business.YAPublishPay;
using YAPayment.Business.TrafficPolice;

namespace YAPayment.Entity
{
    enum YaPublishPayType : int
    {
        Water,
        Gas,
        Power,
        TV,
        None,
    }

    class YAEntity : BaseEntity
    {
        #region 常量

        public const string SECTION_NAME = "YAPayment";

        #endregion

        public YaPublishPayType PublishPayType = YaPublishPayType.None;

        #region 查询
        
        /// <summary>
        /// 用户号
        /// </summary>
        public string UserID = "";
        /// <summary>
        /// 账单查询流水
        /// </summary>
        public string QueryTraceNo = "";
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName = "";
        /// <summary>
        /// 用户地址
        /// </summary>
        public string UserAddress = "";
        /// <summary>
        /// 查询欠费金额
        /// </summary>
        public double QueryAmount = 0;
        /// <summary>
        /// 使用开始日期
        /// </summary>
        public string QueryDateStart = "";
        /// <summary>
        /// 使用结束日期
        /// </summary>
        public string QueryDateEnd = "";

        //Water
        /// <summary>
        /// 水费滞纳金
        /// </summary>
        public double WaterFee = 0;
        /// <summary>
        /// 水费总金额
        /// </summary>
        public double WaterTotalAmount = 0;

        //TV
        /// <summary>
        /// 资费1
        /// </summary>
        public double Price1 = 0;
        /// <summary>
        /// 资费2
        /// </summary>
        public double Price2 = 0;
        /// <summary>
        /// 资费信息
        /// </summary>
        public string PriceInfo = "";
        /// <summary>
        /// 选择的资费
        /// </summary>
        public double SelectPrice = 0;
        /// <summary>
        /// 选择的时长
        /// </summary>
        public int SelectMonth = 0;

        #endregion

        #region 支付

        /// <summary>
        /// 账单支付流水
        /// </summary>
        public string PayFlowNo = "";
        /// <summary>
        /// 账单销账流水
        /// </summary>
        public string ConfirmTraceNo = "";

        #endregion

        #region 交警罚没变量

        #region 认罚
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicensePlant = "";
        /// <summary>
        /// 车架号（末6位）
        /// </summary>
        public string CarId = "";
        /// <summary>
        /// 驾驶证号
        /// </summary>
        public string LicenseNo = "";
        /// <summary>
        /// 车辆类别
        /// </summary>
        public string CarType = "";
        /// <summary>
        /// 请求页数
        /// </summary>
        public string CurrentIndex = "1";//第一次为1
        /// <summary>
        /// 违法详细汇总
        /// </summary>
        public string InquiryInfo = "";//序号|违法时间|扣分|罚款金额|违法地址&序号|违法时间|扣分|罚款金额|违法地址&序号|违法时间|扣分|罚款金额|违法地址&（按每条信息循环显示）
        #endregion


        #region 核销
        /// <summary>
        /// 核销支付流水号
        /// </summary>
        public string TPPayFlowNo2 = "";

        #endregion

        #region 查询
        /// <summary>
        /// 决定书编号
        /// </summary>
        public string TPDecisionNo = "";
        /// <summary>
        /// 查询流水号
        /// </summary>
        public string TPQueryTraceNo = "";
        /// <summary>
        /// 用户名
        /// </summary>
        public string TPUserName = "";
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string TPUserID = "";
        /// <summary>
        /// 罚款本金
        /// </summary>
        public double TPPrinAmount = 0;
        /// <summary>
        /// 滞纳金
        /// </summary>
        public double TPFeeAmount = 0;
        /// <summary>
        /// 缴纳总金额
        /// </summary>
        public double TPPayAmount = 0;

        #endregion

        #region 支付
        /// <summary>
        /// 支付流水
        /// </summary>
        public string TPPayFlowNo = "";
        #endregion

        #region 确认
        ///<summary>
        /// 账单销账流水
        /// </summary>
        public string TPConfirmTraceNo = "";
        #endregion

        #region 交警业务返回码
        private void TPRespMessage(string code, ref string mean, ref string show)
        {
            switch (code)
            {
                case "M1": { mean = "已缴费完毕"; show = "已缴费完毕,请不要重新缴费"; } break;
                case "M3": { mean = "缴费未完毕"; show = "缴费未完毕，请稍后再试"; } break;
                case "M4": { mean = "决定书编号不存在"; show = "决定书编号不存在，请检查输入的决定书编号"; } break;
                case "M5": { mean = "交易失败"; show = "交易失败"; } break;
                case "N1": { mean = "该罚单已缴费，请等待交警处理结果"; show = "该罚单已缴费，请等待交警处理结果"; } break;
                case "N2": { mean = "决定书号长度有误"; show = "决定书号长度有误"; } break;
                case "N3": { mean = "决定书号必须以51开头"; show = "决定书号必须以51开头"; } break;
                case "N4": { mean = "22:30 - 01:30停止服务"; show = "22:30 - 01:30停止服务"; } break;
                case "EE": { mean = "交易失败"; show = "交易失败"; } break;
            }
        }
        #endregion

        #endregion

        public YAEntity()
        {

#if DEBUG
            QueryTraceNo = "000000123456";
            UserName = "张三";
            UserAddress = "福建省福州市鼓楼区";
            QueryAmount = 12;
            QueryDateStart = "201406";
            QueryDateEnd = "201407";

            Price1 = 25.00;
            Price2 = 30.00;

            WaterFee = 1;
            WaterTotalAmount = 12;

            PayFlowNo = "000000123478";
            ConfirmTraceNo = "00000012347800000012347800000000";
            PayReferenceNo = "000000123478";
            RecvField38 = "";
            RecvField55 = new byte[0];
#endif
        }

        public override string SectionName
        {
            get { return SECTION_NAME; }
        }

        public override void ParseRespMessage(string code, ref string mean, ref string show)
        {
            switch (BusinessName)
            {
                case YATrafficPoliceStratagy.BUSINESSNAME: { TPRespMessage(code, ref mean, ref show); } break;
            }
        }
    }
}
