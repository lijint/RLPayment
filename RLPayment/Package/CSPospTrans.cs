using Landi.FrameWorks;
using Landi.FrameWorks.Package.Other;
using Newtonsoft.Json;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RLPayment.Package
{
    public class CSPospTrans : SocketCommunicate
    {
        public RLCZEntity _entity;

        public string respcode;
        public string respmsg;
        public string RETURNCODE;
        public string MESSAGE;

        public CSPospTrans(RLCZEntity entity)
        {
            if (entity != null)
                _entity = entity;
            HeadLength = 4;

            _entity.HOTBILLTYPE = "电子凭证";
            _entity.HOTBILLNO = _entity.OrderNumber;
            if (_entity.PayType == 0)
            {
                _entity.HOTPAYTYPE = "网上银行";
            }
            else if (_entity.PayType == 1)
            {
                _entity.HOTPAYTYPE = "微信";
            }
            else if (_entity.PayType == 2)
            {
                _entity.HOTPAYTYPE = "支付宝";
            }
        }
        protected override byte[] Packet()
        {
            PacketData packetData = new PacketData();

            packetData.version = "1.0.0";
            packetData.msgsender = "TERMINAL";
            packetData.msgrecv = "POSP";
            packetData.imei = _entity.gTerminalNo;
            packetData.transno = _entity.gTraceNo;
            packetData.transtype = "101";
            packetData.transdate = DateTime.Now.ToString("yyyyMMdd");
            packetData.transtime = DateTime.Now.ToString("hhmmss");
            packetData.guidcode = _entity.GetRandomString(32);
            packetData.encrypttype = "0";
            packetData.postdata = packetAdditiondata();
            packetData.postlength = packetData.postdata.Length;

            SendPackage = JsonConvert.SerializeObject(packetData);

            if (SendPackage == null)
                return null;
            return Encoding.UTF8.GetBytes(SendPackage);

        }

        private string getNoticePacket()
        {
            CNoticeTrans cNotice = new CNoticeTrans(_entity);
            return cNotice.GetPacket();
        }

        private string packetAdditiondata()
        {
            string retAdditionStr = "";
            Postdata postdata = new Postdata();

            postdata.POSTDATA = getNoticePacket();

            postdata.ADDITIONDATA.BANKCARDNO = CommonData.BankCardNum;
            postdata.ADDITIONDATA.AMOUNT = _entity.Amount.ToString();
            postdata.ADDITIONDATA.TRANSDATE = _entity.bBankBackTransDateTime;
            postdata.ADDITIONDATA.FLOWNO = _entity.gTraceNo;
            postdata.ADDITIONDATA.BATCHNO = _entity.gBatchNo;
            postdata.ADDITIONDATA.BANKTERMINALNO = _entity.gTerminalNo;
            postdata.ADDITIONDATA.BANKBRANCHNO = _entity.gBranchNo;
            postdata.ADDITIONDATA.BANKREFNO = _entity.bHostSerialNumber;
            postdata.ADDITIONDATA.MEMO = "预上送交易";

            postdata.ADDITIONDATA.HOTBILLTYPE = _entity.HOTBILLTYPE;
            postdata.ADDITIONDATA.HOTBILLNO = _entity.HOTBILLNO;
            postdata.ADDITIONDATA.HOTUSERID = _entity.CardNO;             //取消用户编号
            postdata.ADDITIONDATA.HOTFLOWNO = _entity.OrderNumber;
            postdata.ADDITIONDATA.HOTPAYTYPE = _entity.HOTPAYTYPE;
            postdata.ADDITIONDATA.BANKCODE = _entity.BANKCODE;
            postdata.ADDITIONDATA.BUSSINESSCODE = _entity.BUSSINESSCODE;
            postdata.ADDITIONDATA.GUICODE = _entity.GUICODE;

            if (_entity.PayType == 0)
            {
                postdata.ADDITIONDATA.PAYTYPE = "1";
            }
            else if (_entity.PayType == 1 || _entity.PayType == 2)
            {
                postdata.ADDITIONDATA.ORDERNO = _entity.OrderNumber;
                postdata.ADDITIONDATA.PAYTYPE = "2";
            }

            retAdditionStr = JsonConvert.SerializeObject(postdata);
            return retAdditionStr;
        }

        protected override bool UnPacket(byte[] recv_all)
        {
            RecvPackage = Encoding.UTF8.GetString(recv_all);
            Log.Info("recv packet : " + RecvPackage);

            try
            {
                if (string.IsNullOrEmpty(RecvPackage))
                    return false;
                RecvPacketData packetData = new RecvPacketData();
                packetData = JsonConvert.DeserializeObject<RecvPacketData>(RecvPackage);
                respcode = packetData.respcode;
                respmsg = packetData.respmsg;
                if (respcode == "00")
                {
                    CNoticeTrans cNotice = new CNoticeTrans(_entity);
                    return cNotice.getUnpacket(packetData.postdata);
                    //RecvPostdata postdata = new RecvPostdata();
                    //postdata = JsonConvert.DeserializeObject<RecvPostdata>(packetData.postdata);
                    //RETURNCODE = postdata.ADDITIONDATA.RETURNCODE;
                    //MESSAGE = postdata.ADDITIONDATA.MESSAGE;
                    //if (postdata.ADDITIONDATA.RETURNCODE == "00")

                    //return true;

                    //else
                    //    return false;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
                return false;
            }
        }

        protected override byte[] PacketHead(byte[] SendBytes)
        {
            int headLength = HeadLength;
            int totalLength = SendBytes.Length;
            byte[] head = new byte[headLength];
            byte[] send_all = new byte[headLength + totalLength];

            for (int i = headLength; i > 0; i--)
            {
                head[i - 1] = (byte)(totalLength % 256);
                totalLength = totalLength / 256;
            }
            Array.Copy(head, send_all, headLength);
            Array.Copy(SendBytes, 0, send_all, headLength, SendBytes.Length);
            return send_all;
        }

        protected override int UnPacketHead(byte[] recHead)
        {
            int recvPacketLength = 0;
            for (int i = 0; i < HeadLength; i++)
            {
                recvPacketLength = recvPacketLength * 256 + recHead[i];
            }
            Log.Info("recv packet len : " + recvPacketLength);
            return recvPacketLength;
        }
    }

    public class PacketData
    {
        public string version;          // String     Y Y   1.0.0              版本号
        public string msgsender;        // String(10) Y Y   TERMINAL/TPOS/POSP 发送方
        public string msgrecv;          // String(10) Y Y   TERMINAL/TPOS/POSP 接收方
        public string imei;             // String(32) Y     终端号              设备唯一标识
        public string transno;          // String(6)  Y Y	6位自增数           交易流水号
        public string transtype;        // String(16) Y Y   交易类型           
        public string transdate;        // String(8)  Y Y   YYYYMMDD           交易日期
        public string transtime;        // String(6)  Y Y   hhmmss             交易时间
        public string guidcode;         // String(32) Y Y   应答时需要重新生成     随机字符串
        //public string requestkey;       // String(32)       上送的内容原样返回     请求方保留域
        public string encrypttype;      // String(6)  Y     0-不加密            加密方式
        public int postlength;          // Int        Y     源字符串长度         加密长度
        public string postdata;         // String     Y     加密内容           
        public string macvalue;         // String(16) Y Y   mac校验值         
        //public string respcode;         // String(6)    Y   应答码            
        //public string respmsg;          // String(128)  Y   应答描述           
        //public string ip;               // String(32) C     第三方通信地址        目的IP地址 
        //public int port;                // Int        C                        目的端口号
    }

    public class RecvPacketData
    {
        public string version;          // String     Y Y   1.0.0              版本号
        public string msgsender;        // String(10) Y Y   TERMINAL/TPOS/POSP 发送方
        public string msgrecv;          // String(10) Y Y   TERMINAL/TPOS/POSP 接收方
        public string imei;             // String(32) Y     终端号              设备唯一标识
        public string transno;          // String(6)  Y Y	6位自增数           交易流水号
        public string transtype;        // String(16) Y Y   交易类型           
        public string transdate;        // String(8)  Y Y   YYYYMMDD           交易日期
        public string transtime;        // String(6)  Y Y   hhmmss             交易时间
        public string guidcode;         // String(32) Y Y   应答时需要重新生成     随机字符串
        //public string requestkey;       // String(32)       上送的内容原样返回     请求方保留域
        public string encrypttype;      // String(6)  Y     0-不加密            加密方式
        public int postlength;          // Int        Y     源字符串长度         加密长度
        public string postdata;         // String     Y     加密内容           
        public string macvalue;         // String(16) Y Y   mac校验值         
        public string respcode;         // String(6)    Y   应答码            
        public string respmsg;          // String(128)  Y   应答描述           
        public string ip;               // String(32) C     第三方通信地址        目的IP地址 
        public int port;                // Int        C                        目的端口号

    }
    public class Postdata
    {
        public Postdata()
        {
            ADDITIONDATA = new AddData();
        }
        public string POSTDATA;
        public AddData ADDITIONDATA;
    }

    public class RecvPostdata
    {
        public RecvPostdata()
        {
            ADDITIONDATA = new RecvAddData();
        }
        public string POSTDATA;
        public RecvAddData ADDITIONDATA;
    }

    public class AddData
    {
        public string BANKCARDNO;       //交易卡号
        public string AMOUNT;           //交易金额（单位 元）
        public string TRANSDATE;        //交易日期(yyyyMMddHHmmss)
        public string FLOWNO;           //终端流水号
        public string BATCHNO;          //终端批次号
        public string BANKTERMINALNO;   //银行终端号
        public string BANKBRANCHNO;     //银行商户号
        public string BANKREFNO;        //银行系统参考号
        public string MEMO;             //备注
        public string HOTBILLTYPE;      //热力票据类别
        public string HOTBILLNO;        //热力票据号
        public string HOTUSERID;        //热力用户编码
        public string HOTFLOWNO;        //热力流水号
        public string HOTPAYTYPE;       //热力付款类型
        public string BANKCODE;         //银行代码
        public string BUSSINESSCODE;    //营业区码
        public string GUICODE;          //柜员号
        public string ORDERNO;          //威富通订单号
        public string PAYTYPE;          //1、通联，2威富通
        //public string RETURNCODE;       //前置处理响应结果
        //public string MESSAGE;          //处理结果说明

    }
    public class RecvAddData
    {
        public string BANKCARDNO;       //交易卡号
        public string AMOUNT;           //交易金额（单位 元）
        public string TRANSDATE;        //交易日期(yyyyMMddHHmmss)
        public string FLOWNO;           //终端流水号
        public string BATCHNO;          //终端批次号
        public string BANKTERMINALNO;   //银行终端号
        public string BANKBRANCHNO;     //银行商户号
        public string BANKREFNO;        //银行系统参考号
        public string MEMO;             //备注
        public string HOTBILLTYPE;      //热力票据类别
        public string HOTBILLNO;        //热力票据号
        public string HOTUSERID;        //热力用户编码
        public string HOTFLOWNO;        //热力流水号
        public string HOTPAYTYPE;       //热力付款类型
        public string BANKCODE;         //银行代码
        public string BUSSINESSCODE;    //营业区码
        public string GUICODE;          //柜员号
        public string ORDERNO;          //威富通订单号
        public string PAYTYPE;          //1、通联，2威富通
        public string RETURNCODE;       //前置处理响应结果
        public string MESSAGE;          //处理结果说明

    }


    enum ReturnCode
    {
        E_SUCC = 0,               // 成功返回
        E_SEND_FAIL = 2,          // 
        E_RECV_FAIL = 3,                // 需要冲正
        E_PARAM_ERR = 7,                //上送参数有误
        E_UNPACKET_FAIL = 11,           // 解包失败
        E_TRANS_FAIL = 13,              // 交易失败
        E_BANK_SEND_FAIL = 22,          // 不需要冲正
        E_BANK_RECV_FAIL = 23,          // 需要冲正
        E_GETMASTERKEY_FAIL = 24,       //获取主密钥异常
        E_MACVERIFY_FAI = 25,           //获取Mac密钥异常
        E_DECRYPT_FAIL = 26,            //解密数据失败
        E_PARAMCREATE_FAIL = 27,        //平台获取参数异常
        E_SOCKET_NULL = 28,             //通信地址不存在
        E_REQUESTDATA_FORMAT_ERR = 29,  //请求参数格式异常
        E_SOCKET_ERR = 30,              //通信请求异常
        E_SUIT_ERR = 31,                //套件不合法
        E_MACVERIFY_FAIL = 32,
        E_MACCALC_ERROR = 33,
        E_SUITMANAGER_SENDFAIL = 34,
        E_SOCKETSSL_ERR = 35,
        E_SOCKETSSL_CONERR = 36,
        E_SSLCER_FAIL = 37,
        E_ADDERR = 99,
         [Description("数据库操作失败")]
        E_DATABASE_FAIL = 30,
         [Description("终端号不合法")]
        E_VARIFY_TER_FAIL = 81,
         [Description("操作员不合法")]
        E_VARIFY_OPERATOR_FAIL = 82,     //校验mac值
         [Description("操作员不合法")]
        E_CUSTOMER_ERR = 83,

    }

}
