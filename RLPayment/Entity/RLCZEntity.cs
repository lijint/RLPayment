using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Entity
{
    public class RLCZEntity : BaseEntity
    {
        #region 常量

        public const string SECTION_NAME = "RLCZPayment";
        
        public override string SectionName
        {
            get { return SECTION_NAME; }
        }
        #endregion


        public string CardNO;
        public double Amount;
        public int PayType;             //0---YHK 1---WX 2---ZFB

        public string CspospServerIP
        {
            get { return ReadIniFile("CspospServerIP"); }
        }
        public int CspospServerPort
        {
            get { return int.Parse(ReadIniFile("CspospServerPort")); }
        }
        public string RLServerIP
        {
            get { return ReadIniFile("RLServerIP"); }
        }
        public int RLServerPort
        {
            get { return int.Parse(ReadIniFile("RLServerPort")); }
        }

        public string gBranchNo;        //商户号
        public string gTerminalNo;      //终端号
        public string gBatchNo;         //批次号
        public string gTraceNo;         //流水号

        #region 热力参数
        public string HOTBILLTYPE;      //热力票据类别
        public string HOTBILLNO;        //热力票据号
        public string HOTUSERID;        //热力用户编码
        public string HOTFLOWNO;        //热力流水号
        public string HOTPAYTYPE;       //热力付款类型
        public string BANKCODE;         //银行代码
        public string BUSSINESSCODE;    //营业区码
        public string GUICODE;          //柜员号

        public string ReturnCode;       //返回信息码
        public string Addr;             //地址
        public string UserName;         //用户名称
        public string CompanyCode;      //公司代码
        //public int
        public UserInfo userInfo;       //欠费明细
        #endregion

        #region 威富通参数
        public string ORDERNO;          //威富通订单号
        #endregion

        #region 载入配置
        /// <summary>
        /// 载入配置
        /// </summary>
        public void LoadConfig()
        {
            try
            {
                //BankCardConfig.ConfigTerminal = @"config\TlBankCardConfig.xml";
                XMLConfig xml = new XMLConfig(@"config\TlBankCardConfig.xml");

                //SetBankName("TlBankCard");
                //int iTempValue = 0;
                //if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/LibIsEnabled"), out iTempValue))
                //{
                //    //BankCardConfig.LibIsEnabled = iTempValue;
                //    //AppLog.Write(String.Format("LibIsEnabled:{0}", BankCardConfig.LibIsEnabled));
                //}

                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/PactekDes"), out iTempValue))
                //    {
                //        BankCardConfig.PactekDes = iTempValue;
                //        AppLog.Write(String.Format("PactekDes:{0}", BankCardConfig.PactekDes));
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/SignInMode"), out iTempValue))
                //    {
                //        BankCardConfig.SignInMode = iTempValue;
                //        AppLog.Write(String.Format("SignInMode:{0}", BankCardConfig.SignInMode));
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/LastReverseNum"), out iTempValue))
                //    {
                //        BankCardConfig.LastReverseNum = iTempValue;
                //        AppLog.Write(String.Format("LastReverseNum:{0}", BankCardConfig.LastReverseNum));
                //        if (BankCardConfig.LastReverseNum < 1)
                //        {
                //            //throw new Exception("/Config/LibConfig/LastReverseNum冲正次数不能小于1");
                //        }
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/NextReverseNum"), out iTempValue))
                //    {
                //        BankCardConfig.NextReverseNum = iTempValue;
                //        AppLog.Write(String.Format("NextReverseNum:{0}", BankCardConfig.NextReverseNum));
                //        if (BankCardConfig.NextReverseNum < 1)
                //        {
                //            //throw new Exception("/Config/LibConfig/NextReverseNum冲正次数不能小于1");
                //        }
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/HideCardNo"), out iTempValue))
                //    {
                //        BankCardConfig.HideCardNo = iTempValue;
                //        AppLog.Write(String.Format("HideCardNo:{0}", BankCardConfig.HideCardNo));
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/AllowPinBlank"), out iTempValue))
                //    {
                //        BankCardConfig.AllowPinBlank = iTempValue;
                //        AppLog.Write(String.Format("AllowPinBlank:{0}", BankCardConfig.AllowPinBlank));
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/NeedBatchClear"), out iTempValue))
                //    {
                //        BankCardConfig.NeedBatchClear = iTempValue;
                //        AppLog.Write(String.Format("NeedBatchClear:{0}", BankCardConfig.NeedBatchClear));
                //    }
                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/NeedRevokeClear"), out iTempValue))
                //    {
                //        BankCardConfig.NeedRevokeClear = iTempValue;
                //        AppLog.Write(String.Format("NeedRevokeClear:{0}", BankCardConfig.NeedRevokeClear));
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/SupportIcTrans"), out iTempValue))
                //    {
                //        BankCardConfig.SupportIcTrans = iTempValue;
                //        AppLog.Write(String.Format("SupportIcTrans:{0}", BankCardConfig.SupportIcTrans));
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/SupportPacketFoot"), out iTempValue))
                //    {
                //        BankCardConfig.SupportPacketFoot = iTempValue;
                //        AppLog.Write(String.Format("SupportPacketFoot:{0}", BankCardConfig.SupportPacketFoot));
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/SupportReverse"), out iTempValue))
                //    {
                //        BankCardConfig.SupportReverse = iTempValue;
                //        AppLog.Write(String.Format("SupportReverse:{0}", BankCardConfig.SupportReverse));
                //    }
                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/SupportUpdateTime"), out iTempValue))
                //    {
                //        BankCardConfig.SupportUpdateTime = iTempValue;
                //        AppLog.Write(String.Format("SupportUpdateTime:{0}", BankCardConfig.SupportUpdateTime));
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/AidAndCaDownLoad"), out iTempValue))
                //    {
                //        BankCardConfig.AidAndCaDownLoad = iTempValue;
                //        AppLog.Write(String.Format("AidAndCaDownLoad:{0}", BankCardConfig.AidAndCaDownLoad));
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/HasDownParam"), out iTempValue))
                //    {
                //        if (iTempValue == 0)
                //        {
                //            BankCardConfig.HasDownParam = true;
                //        }
                //        AppLog.Write(String.Format("HasDownParam:{0}", BankCardConfig.HasDownParam));
                //    }
                //    if (int.TryParse(xml.GetNodeValue("/Config/LibConfig/NonInputCheckPin"), out iTempValue))
                //    {
                //        if (iTempValue == 1)
                //        {
                //            BankCardConfig.NonInputCheckPin = true;
                //        }
                //        AppLog.Write(String.Format("NonInputCheckPin:{0}", BankCardConfig.NonInputCheckPin));
                //    }

                //    BankCardConfig.IsNonConnected = xml.GetNodeValue("/Config/TransData/IsNonConnected");
                //    AppLog.Write(String.Format("IsNonConnected:{0}", BankCardConfig.IsNonConnected));

                //    //fileName
                //    BankCardConfig.AidAndCaFileNamePrev = xml.GetNodeValue("/Config/LibConfig/AidAndCaFileName").Trim();

                //    //AllowPinBlank
                //    BankCardConfig.gBranchName = xml.GetNodeValue("/Config/TransData/BranchName");
                //    AppLog.Write(String.Format("gBranchName:{0}", BankCardConfig.gBranchName));

                gBranchNo = xml.GetNodeValue("/Config/TransData/BranchNo");
                if (String.IsNullOrEmpty(gBranchNo) || gBranchNo.Length != 15)
                {
                    throw new Exception(String.Format("商户号长度应该是15位,您输入的是{0}位({1})", gBranchNo.Length, gBranchNo));
                }
                gTerminalNo = xml.GetNodeValue("/Config/TransData/TerminalNo");
                if (String.IsNullOrEmpty(gTerminalNo) || gTerminalNo.Length != 8)
                {
                    throw new Exception(String.Format("终端号长度应该是8位,您输入的是{0}位({1})", gTerminalNo.Length, gTerminalNo));
                }
                gBatchNo = xml.GetNodeValue("/Config/TransData/BatchNo");
                if (String.IsNullOrEmpty(gBatchNo) || gBatchNo.Length != 6)
                {
                    throw new Exception(String.Format("批次号长度应该是6位,您输入的是{0}位({1})", gBatchNo.Length, gBatchNo));
                }

                gTraceNo = xml.GetNodeValue("/Config/TransData/TraceNo");
                if (String.IsNullOrEmpty(gTraceNo) || gTraceNo.Length != 6)
                {
                    throw new Exception(String.Format("流水号长度应该是6位,您输入的是{0}位({1})", gTraceNo.Length, gTraceNo));
                }

                //    BankCardConfig.gHostIP = xml.GetNodeValue("/Config/TransData/HostIp");
                //    AppLog.Write(String.Format("gHostIP:{0}", BankCardConfig.gHostIP));

                //    int iHostPort = 0;
                //    if (int.TryParse(xml.GetNodeValue("/Config/TransData/HostPort"), out iHostPort))
                //    {
                //        BankCardConfig.gHostPort = iHostPort;
                //        AppLog.Write(String.Format("HostPort:{0}", BankCardConfig.gHostPort));
                //    }
                //    else
                //    {
                //        AppLog.Write(String.Format("HostPort:{0}", "配置错误"));
                //        throw new Exception("HostPort配置错误");
                //    }

                //    BankCardConfig.gTpdu = xml.GetNodeValue("/Config/TransData/Tpdu");
                //    AppLog.Write(String.Format("gTpdu:{0}", BankCardConfig.gTpdu));
                //    if (String.IsNullOrEmpty(BankCardConfig.gTpdu) || BankCardConfig.gTpdu.Length != 10)
                //    {
                //        throw new Exception("/Config/TransData/Tpdu配置错误");
                //    }

                //    BankCardConfig.gPackageHead = xml.GetNodeValue("/Config/TransData/PackageHead");
                //    AppLog.Write(String.Format("gPackageHead:{0}", BankCardConfig.gPackageHead));

                //    int iMasterKeyNo = 0;
                //    if (int.TryParse(xml.GetNodeValue("/Config/TransData/MasterKeyNo"), out iMasterKeyNo))
                //    {
                //        BankCardConfig.gMasterKeyNo = iMasterKeyNo;
                //        AppLog.Write(String.Format("gMasterKeyNo:{0}", BankCardConfig.gMasterKeyNo));
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/TransData/SSLFlag"), out iTempValue))
                //    {
                //        BankCardConfig.SSLFlag = iTempValue;
                //        AppLog.Write(String.Format("SSLFlag:{0}", BankCardConfig.SSLFlag));
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/TransData/LocalOpenQuickPass"), out iTempValue))
                //    {
                //        if (iTempValue == 1)
                //        {
                //            BankCardConfig.LocalOpenQuickPass = true;
                //            AppLog.Write(String.Format("LocalOpenQuickPass:{0}", BankCardConfig.LocalOpenQuickPass));
                //        }
                //    }

                //    if (!String.IsNullOrEmpty(xml.GetNodeValue("/Config/LibConfig/AppVersion")))
                //    {
                //        BankCardConfig.AppVersion = xml.GetNodeValue("/Config/LibConfig/AppVersion");
                //    }

                //    PublicFun publicCom = new PublicFun();

                //    /// 都从服务端下载
                //    //BankCardConfig.BaseParamInfo = xml.GetNodeValue("/Config/TransData/BaseParamInfo");
                //    //AppLog.Write(String.Format("BaseParamInfo:{0}", BankCardConfig.BaseParamInfo));
                //    //if (String.IsNullOrEmpty(BankCardConfig.BaseParamInfo))
                //    //{
                //    //    BankCardConfig.HasDownParam = false;
                //    //}
                //    //else
                //    //{
                //    //    BankCardConfig.HasDownParam = true;
                //    //}

                //    //ACCESS
                //    if (!String.IsNullOrEmpty(xml.GetNodeValue("/Config/Application/AccessFile")))
                //    {
                //        BankCardConfig.AccessFile = xml.GetNodeValue("/Config/Application/AccessFile");
                //    }
                //    if (!String.IsNullOrEmpty(xml.GetNodeValue("/Config/Application/AccessPin")))
                //    {
                //        BankCardConfig.AccessPin = xml.GetNodeValue("/Config/Application/AccessPin");
                //    }
                //    if (!String.IsNullOrEmpty(xml.GetNodeValue("/Config/Application/AccessProviderName")))
                //    {
                //        BankCardConfig.AccessProviderName = xml.GetNodeValue("/Config/Application/AccessProviderName");
                //    }
                //    /*-----------------==============================-----------------------*/
                //    XMLConfig xmlTerminal = new XMLConfig(@"config\Terminal.xml");

                //    /// 0初装机
                //    int ApplicationRunCount = 1;
                //    int.TryParse(xmlTerminal.GetNodeValue("/Config/Application/ApplicationRunCount"), out ApplicationRunCount);

                //    BankCardConfig.ApplicationRunCount = ApplicationRunCount;
                //    AppLog.Write(String.Format("ApplicationRunCount:{0}", BankCardConfig.ApplicationRunCount));


                //    if (!String.IsNullOrEmpty(xmlTerminal.GetNodeValue("/Config/Application/RfCardReader")))
                //    {
                //        BankCardConfig.RfCardReader = xmlTerminal.GetNodeValue("/Config/Application/RfCardReader");
                //    }

                //    int isEnable = 0;
                //    int.TryParse(xmlTerminal.GetAttributeValue("/Config/HardWare/CardReader", "Enable"), out isEnable);
                //    if (isEnable == 1)
                //    {
                //        BankCardConfig.CardReaderEnable = true;
                //    }
                //    isEnable = 0;
                //    int.TryParse(xmlTerminal.GetAttributeValue("/Config/HardWare/RfReader", "Enable"), out isEnable);
                //    if (isEnable == 1)
                //    {
                //        BankCardConfig.RfCardReaderEnable = true;
                //    }
                //    isEnable = 0;
                //    int.TryParse(xmlTerminal.GetAttributeValue("/Config/HardWare/CodeScanner", "Enable"), out isEnable);
                //    if (isEnable == 1)
                //    {
                //        BankCardConfig.CodeScannerEnable = true;
                //    }

                //    if (!String.IsNullOrEmpty(xmlTerminal.GetNodeValue("/Config/Application/RfCardReader")))
                //    {
                //        BankCardConfig.RfCardReader = xmlTerminal.GetNodeValue("/Config/Application/RfCardReader");
                //    }

                //    if (int.TryParse(xml.GetNodeValue("/Config/TransData/UseCPosp"), out iTempValue))
                //    {
                //        if (iTempValue == 1)
                //        {
                //            BankCardConfig.UseCPosp = true;

                //            /// 如果是内网，无法解析IP
                //            BankCardConfig.CPospHostIp = xml.GetNodeValue("/Config/TransData/CPospHostIp");

                //            AppLog.Write(String.Format("CPospHostIp:{0}", BankCardConfig.CPospHostIp));

                //            if (int.TryParse(xml.GetNodeValue("/Config/TransData/CPospHostPort"), out iHostPort))
                //            {
                //                BankCardConfig.CPospHostPort = iHostPort;
                //                AppLog.Write(String.Format("CPospHostPort:{0}", BankCardConfig.CPospHostPort));
                //            }
                //            else
                //            {
                //                AppLog.Write(String.Format("CPospHostPort:{0}", "配置错误"));
                //                throw new Exception("CPospHostPort配置错误");
                //            }
                //        }
                //        AppLog.Write(String.Format("UseCPosp:{0}", BankCardConfig.UseCPosp));

                //    }


            }
            catch (Exception ex)
            {
                //OnProcessEnd("EE", ex.Message);
                AppLog.Write("载入业务配置异常", AppLog.LogMessageType.Error, ex);
                throw;
            }
        }
        #endregion

        #region 获取随机字符串
        public string GetRandomString(int length)
        {
            const string key = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            if (length < 1)
                return string.Empty;

            Random rnd = new Random();
            byte[] buffer = new byte[8];

            ulong bit = 31;
            ulong result = 0;
            int index = 0;
            StringBuilder sb = new StringBuilder((length / 5 + 1) * 5);

            while (sb.Length < length)
            {
                rnd.NextBytes(buffer);

                buffer[5] = buffer[6] = buffer[7] = 0x00;
                result = BitConverter.ToUInt64(buffer, 0);

                while (result > 0 && sb.Length < length)
                {
                    index = (int)(bit & result);
                    sb.Append(key[index]);
                    result = result >> 5;
                }
            }
            return sb.ToString();
        }
        #endregion
    }

    public class UserInfo
    {
        public string FeeType;      //费用类别
        public string HeatingPeriod;//采暖期
        public double Area;         //面积
        public double Price;        //单价
        public double Amount;       //应收金额
        public double amountOwed;       //欠费金额

    }

}
