using System;
using System.Collections.Generic;
using System.Text;
using log4net.Appender;
using Landi.FrameWorks;
using Newtonsoft.Json;

namespace YAPayment.Entity
{
    class CarEntity : BaseEntity
    {
        public const string SECTION_NAME = "Car";
        public override string SectionName
        {
            get { return SECTION_NAME; }
        }

        public SetOutCityItem SelectStartCity;
        /// <summary>
        /// 选择的目的地站名
        /// </summary>
        public string SelectEndCityName;
        /// <summary>
        /// 选择的乘车站编号
        /// </summary>
        public string CarryStaId;

        public TicketLines SelectLine;

        /// <summary>
        /// 锁票后返回的车票ID
        /// </summary>
        public string TicketId;

        public string UnlockMessage="";
        #region
        /// <summary>
        /// 始发站信息
        /// </summary>
        public QueryStationResponse _QueryStationResponse;
        /// <summary>
        /// 根据关键字获取始发城市请求
        /// </summary>
        public QueryStationByCityRequest _queryStationByCityRequest;

        /// <summary>
        /// 根据关键字获取始发城市响应
        /// </summary>
        public QueryStationByCityResponse _queryStationByCityResponse;

        /// <summary>
        /// 获取所有联网的出发城市
        /// </summary>
        public QuerySetOutResponse _querySetOutResponse;

        /// <summary>
        /// 根据简拼或关键字查询目地站
        /// </summary>
        public QueryTerminusByCityRequest _QueryTerminusByCityRequest;
        /// <summary>
        /// 根据简拼或关键字查询目地站
        /// </summary>
        public QueryTerminusByCityResponse _QueryTerminusByCityResponse;

        /// <summary>
        /// 1.2.1	根据城市查询车票信息
        /// </summary>
        public BstTicketByCityRequest _BstTicketByCityRequest;
        public BstTicketByCityResponse _BstTicketByCityResponse;

        public BstLockTicketRequest _BstLockTicketRequest;
        public BstLockTicketResponse _BstLockTicketResponse;

        public BstBuyTicketRequest _BstBuyTicketRequest;
        public BstBuyTicketResponse _BstBuyTicketResponse;

        public UnLockTicketRequest _UnLockTicketRequest;
        public UnLockTicketResponse _UnLockTicketResponse;

        #endregion
    }

    #region 获取始发站
    public class QueryStationResponse
    {
        [JsonProperty("status")]
        public string Status;
        [JsonProperty("error_code")]
        public string Errorcode;
        [JsonProperty("msg")]
        public string Msg;
        [JsonProperty("passenger_terminal")]
        public List<PassengerTerminal> PassengerTerminalList;
    }

    public class PassengerTerminal
    {
        [JsonProperty("province")]
        public string Province;
        [JsonProperty("city")]
        public string City;
        [JsonProperty("code")]
        public string Code;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("num_code")]
        public string Numcode;

    }
    #endregion

    #region 根据关键字获取始发城市

    public class QueryStationByCityRequest
    {
        [JsonProperty("city_name")]
        public string CityName;

    }

    public class QueryStationByCityResponse
    {
        [JsonProperty("status")]
        public string Status;
        [JsonProperty("msg")]
        public string Msg;
        [JsonProperty("error_code")]
        public string ErrorCode;
        [JsonProperty("start_city")]
        public List<StratCityItem> StartCity;

    }

    public class StratCityItem
    {
        [JsonProperty("en_name")]
        public string EnName;
        [JsonProperty("full_name")]
        public string FullName;
        [JsonProperty("short_name")]
        public string ShortName;
        [JsonProperty("city_id")]
        public string CityId;
        [JsonProperty("is_connected")]
        public string IsConnected;
        [JsonProperty("parent_id")]
        public string ParentId;
        [JsonProperty("is_pre_sell")]
        public string IsPreSell;
        [JsonProperty("city_name")]
        public string CityName;
        [JsonProperty("alias_name")]
        public string AliasName;
        [JsonProperty("provider")]
        public string Provider;

    }
    #endregion

    #region 获取所有联网的出发城市

        public class QuerySetOutResponse
        {
            [JsonProperty("status")]
            public string Status;
            [JsonProperty("msg")]
            public string Msg;
            [JsonProperty("error_code")]
            public string ErrorCode;
            [JsonProperty("start_city")]
            public List<SetOutCityItem> StartCity;
        }

        public class SetOutCityItem
        {
            [JsonProperty("en_name")]
            public string EnName;
            [JsonProperty("full_name")]
            public string FullName;
            [JsonProperty("short_name")]
            public string ShortName;
            [JsonProperty("city_id")]
            public string CityId;
            [JsonProperty("is_connected")]
            public string IsConnected;
            [JsonProperty("parent_id")]
            public string ParentId;
            [JsonProperty("is_pre_sell")]
            public string IsPreSell;
            [JsonProperty("city_name")]
            public string CityName;
            [JsonProperty("alias_name")]
            public string AliasName;
            [JsonProperty("level")]
            public string Level;
            [JsonProperty("children")]
            public List<SetOutCityItem> Children;
        }
        #endregion

    #region 1.1.4	根据简拼或关键字查询目地站

    public class QueryTerminusByCityRequest
    {
        [JsonProperty("city_name")] public string CityName;
        [JsonProperty("city_id")] public string CityId;
        [JsonProperty("stop_name")] public string StopName;
    }

    public class QueryTerminusByCityResponse
    {
        [JsonProperty("status")]
        public string Status;
        [JsonProperty("msg")]
        public string Msg;
        [JsonProperty("error_code")]
        public string ErrorCode;
        [JsonProperty("target_cityB")]
        public List<TargetCity> TargetCityB;
    }

    public class TargetCity
    {
        [JsonProperty("starting_city_id")]
        public string StartingCityId;
        [JsonProperty("carry_sta_id")]
        public string CarryStaId;
        [JsonProperty("carry_sta_name")]
        public string CarryStaName;
        [JsonProperty("stop_name")]
        public string StopName;
        [JsonProperty("short_name")]
        public string ShortName;
        [JsonProperty("full_name")]
        public string FullName;
        [JsonProperty("en_name")]
        public string EnName;
    }
    #endregion

    #region 1.2.1	根据城市查询车票信息

    public class BstTicketByCityRequest
    {
        [JsonProperty("city_id")]
        public string CityId;
        [JsonProperty("carry_sta_id")]
        public string CarryStaId;
        [JsonProperty("city_name")]
        public string CityName;
        [JsonProperty("stop_name")]
        public string StopName;
        [JsonProperty("riding_date")]
        public string RidingDate;
    }

    public class BstTicketByCityResponse
    {
        [JsonProperty("status")]
        public string Status;
        [JsonProperty("msg")]
        public string Msg;
        [JsonProperty("error_code")]
        public string ErrorCode;
        [JsonProperty("ticket_lines_query")]
        public List<TicketLines> TicketLinesQuery;
        [JsonProperty("type")]
        public string Type;
    }

    public class TicketLines
    {
        [JsonProperty("carry_sta_id")]
        public string CarryStaId;
        [JsonProperty("carry_sta_name")]
        public string CarryStaName;
        [JsonProperty("city")]
        public string City;
        [JsonProperty("sign_id")]
        public string SignId;
        [JsonProperty("sch_id")]
        public string SchId;
        [JsonProperty("drv_date_time")]
        public string DrvDateTime;
        [JsonProperty("end_sta_name")]
        public string EndStaName;
        [JsonProperty("full_price")]
        public string FullPrice;
        [JsonProperty("half_price")]
        public string HalfPrice;
        [JsonProperty("amount")]
        public string Amount;
        [JsonProperty("child_amount")]
        public string ChildAmount;
        [JsonProperty("mile")]
        public string Mile;
        [JsonProperty("extra_flag")]
        public string ExtraFlag;
        [JsonProperty("bus")]
        public string Bus;
        [JsonProperty("sch_type_name")]
        public string SchTypeName;
        [JsonProperty("bus_type_name")]
        public string BusTypeName;
        [JsonProperty("pass_id")]
        public string PassId;
        [JsonProperty("mot_name")]
        public string MotName;
        [JsonProperty("stop_name")]
        public string StopName;
        [JsonProperty("stop_code")]
        public string StopCode;
        [JsonProperty("stop_alias_name")]
        public string StopAliasName;
        [JsonProperty("booking_url")]
        public string BookingUrl;
        [JsonProperty("stop_num")]
        public string StopNum;
        [JsonProperty("max_sell_num")]
        public string MaxSellNum;
        [JsonProperty("is_insure")]
        public string IsInsure;
        [JsonProperty("service_price")]
        public string ServicePrice;

    }

    #endregion

    #region 锁票
    public class BstLockTicketRequest
    {
        [JsonProperty("carry_sta_id")]
        public string CarryStaId;
        [JsonProperty("str_date")]
        public string StrDate;
        [JsonProperty("sign_id")]
        public string SignId;
        [JsonProperty("stop_name")]
        public string StopName;
        [JsonProperty("buy_ticket_info")]
        public string BuyTicketInfo;
        [JsonProperty("open_id")]
        public string OpenId;

    }

    public class BstLockTicketResponse
    {
        [JsonProperty("status")]
        public string Status;
        [JsonProperty("msg")]
        public string Msg;
        [JsonProperty("error_code")]
        public string ErrorCode;
        [JsonProperty("data")]
        public LockDataModel Data;
    }

    public class LockDataModel
    {
        [JsonProperty("expire_time")]
        public string ExpireTime;
        [JsonProperty("code")]
        public string Code;
        [JsonProperty("ticket_code")]
        public string TicketCode;
        [JsonProperty("pay_order_id")]
        public string PayOrderId;
        [JsonProperty("ticket_list")]
        public List<TicketModel> TicketList;
        [JsonProperty("ticket_lines")]
        public TicketLineModel TicketLines;
        [JsonProperty("ticket_ids")]
        public List<string> TicketIds;
        [JsonProperty("order_ods")]
        public List<string> OrderOds;
        [JsonProperty("ticket_price_list")]
        public List<string> TicketPriceList;
        [JsonProperty("ticket_type")]
        public List<string> TicketType;
        [JsonProperty("web_order_id")]
        public List<string> WebOrderId;
        [JsonProperty("seat_number_list")]
        public List<string> SeatNumberList;
        [JsonProperty("lock_data")]
        public string LockData;
    }

    public class TicketModel
    {
        [JsonProperty("order_id")]
        public string OrderId;
        [JsonProperty("ticket_id")]
        public string TicketId;
        [JsonProperty("server_price")]
        public string ServerPrice;
        [JsonProperty("real_price")]
        public string RealPrice;
    }

    public class TicketLineModel
    {
        [JsonProperty("carry_sta_id")]
        public string CarryStaId;
        [JsonProperty("carry_sta_name")]
        public string CarryStaName;
        [JsonProperty("sign_id")]
        public string SignId;
        [JsonProperty("drv_date_time")]
        public string DrvDateTime;
        [JsonProperty("sch_id")]
        public string SchId;
        [JsonProperty("end_sta_name")]
        public string EndStaName;
        [JsonProperty("full_price")]
        public string FullPrice;
        [JsonProperty("half_price")]
        public string HalfPrice;
        [JsonProperty("amount")]
        public string Amount;
        [JsonProperty("child_amount")]
        public string ChildAmount;
        [JsonProperty("sch_type_id")]
        public string SchTypeId;
        [JsonProperty("mile")]
        public string Mile;
        [JsonProperty("extra_flag")]
        public string ExtraFlag;
        [JsonProperty("bus")]
        public string Bus;
        [JsonProperty("sch_type_name")]
        public string SchTypeName;
        [JsonProperty("bus_type_name")]
        public string BusTypeName;
        [JsonProperty("pass_id")]
        public string PassId;
        [JsonProperty("mot_name")]
        public string MotName;
        [JsonProperty("stop_name")]
        public string StopName;
        [JsonProperty("booking_url")]
        public string BookingUrl;

    }
    #endregion

    #region    解锁票

    public class UnLockTicketRequest
    {
        [JsonProperty("ticket_ids")]
        public string TicketIds;
    }

    public class UnLockTicketResponse
    {
        [JsonProperty("status")]
        public string Status;
        [JsonProperty("msg")]
        public string Msg;
        [JsonProperty("error_code")]
        public string ErrorCode;
        [JsonProperty("ticket_id")]
        public string TicketId;
    }

    #endregion

    #region 购票

    public class BstBuyTicketRequest
    {
        [JsonProperty("ticket_ids")]
        public string TicketIds;
        [JsonProperty("code")]
        public string Code;
        [JsonProperty("open_id")]
        public string OpenId;
        [JsonProperty("phone_order_id")]
        public string PhoneOrderId;
        [JsonProperty("pay_type")]
        public string PayType;
        [JsonProperty("serial_number")]
        public string SerialNumber;
    }

    public class BstBuyTicketResponse
    {
        [JsonProperty("status")]
        public string Status;
        [JsonProperty("msg")]
        public string Msg;
        [JsonProperty("error_code")]
        public string ErrorCode;
        [JsonProperty("code")]
        public string Code;
        [JsonProperty("pay_order_id")]
        public string PayOrderId;
        [JsonProperty("succeeded_at")]
        public string SucceededAt;
    }

    #endregion
}
