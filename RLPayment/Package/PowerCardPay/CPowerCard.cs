using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks.HardWare;
using Landi.FrameWorks;

namespace YAPayment.Package.PowerCardPay
{
    class PowerCardInfo
    {
        public string CardNo = "";
        public string CardInfo = "";
        public string Random = "";
        public string EF1 = "";
        public string EF2 = "";
        public string EF3 = "";
        public string EF4 = "";
        //public string EF31 = "";
        //public string EF32 = "";
        //public string EF41 = "";
        //public string EF42 = "";
        public string EF5 = "";
        public string W_EF1 = "";
        public string W_EF2 = "";
        public string W_EF31 = "";
        public string W_EF32 = "";
        public string W_EF41 = "";
        public string W_EF42 = "";
        public string W_EF5 = "";
        public string Key1 = "";
        public string Key2 = "";
        public string Key3 = "";
        public string Key4 = "";
        public string CertDes = "";
        public string LimitDes = "";
        public string ExtDes = "";
        public string ErrorMsg = "";
    }

    class CPowerCard
    {
        private const string RET_STRING = "9000";
        private const string RET_STRING1 = "610C";
        private const string RET_STRING2 = "6108";
        /// <summary>
        /// 新卡
        /// </summary>
        private const string RET_CARDTYPE_0 = "6B00";
        /// <summary>
        /// 开户卡
        /// </summary>
        private const string RET_CARDTYPE_1 = "6B01";
        /// <summary>
        /// 购电卡
        /// </summary>
        private const string RET_CARDTYPE_2 = "6B02";
        /// <summary>
        /// 补卡
        /// </summary>
        private const string RET_CARDTYPE_3 = "6B03";

        private byte[] GET_CARD_INFO ={ 0x00, 0xB0, 0x9F, 0x05, 0x80 };
        private byte[] SELECT_FILE = { 0x00, 0xA4, 0x00, 0x00, 0x02, 0xDF, 0x01 };
        private byte[] GET_CARD_TYPE = { 0x00, 0xB0, 0x81, 0x2A, 0x01 };
        private byte[] GET_RANDOM = { 0x00, 0x84, 0x00, 0x00, 0x08 };
        private byte[] GET_EF1 = { 0x00, 0xB0, 0x81, 0x00, 0x2D };
        private byte[] GET_EF2 = { 0x00, 0xB0, 0x82, 0x00, 0x08 };
        private readonly byte[] GET_EF3 = { 0x00, 0xB0, 0x83, 0x00, 0x00 };
        private readonly byte[] GET_EF4 = { 0x00, 0xB0, 0x84, 0x00, 0x00 };
        //private byte[] GET_EF31 = { 0x00, 0xB0, 0x83, 0x00, 0xAA };
        //private byte[] GET_EF32 = { 0x00, 0xB0, 0x83, 0xAA, 0x58 };
        //private byte[] GET_EF41 = { 0x00, 0xB0, 0x84, 0x00, 0xAA };
        //private byte[] GET_EF42 = { 0x00, 0xB0, 0x84, 0xAA, 0x58 };
        private byte[] GET_EF5 = { 0x00, 0xB0, 0x85, 0x00, 0x31 };
        private byte[] CERT_AUTH = { 0x00, 0x88, 0x00, 0x01, 0x08 };//身份认证 +随机数
        //private byte[] CERT_AUTH_GET = { 0x00,0xC0,0x00,0x00,0x08 };//身份认证 或许响应数据
        private byte[] LIMIT_AUTH = { 0x00, 0x82, 0x00, 0x03, 0x08 };//权限认证 + 密钥
        private byte[] EXT_AUTH = { 0x00, 0x82, 0x00, 0x04, 0x08 };//外部认证 + 密钥
           

        /// <summary>
        /// 获取电卡信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool GetCardInfo(ref string info)
        {
            bool bRet = false;
            if (!CardReader.IsUse)
            {
                info = "5DDC19FC1287506465BE2C70D5080817C28C009B181DA1C46ADC7285F5D1FCCD7CD2E7CAE3EA664F5754B8AC2C6DF4189B14A56696A950B915BE6794C7F49E41849C8756619EA31922FE5D04326128B03A32FB122697F077F932274EF10E320790EAF47E1304D7BF33DC2176D9FFE4E2F29069BF5C8E3D47ED9CAC426C38ADC69000";
                return true;
            }
            try
            {
                byte[] bOut = new byte[2048];
                int nOut = 0;
                CardReader.Status sRet = CardReader.CardChipIO(GET_CARD_INFO, GET_CARD_INFO.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    Log.Debug("GetCardInfo:" + info);
                    if (info.Substring(info.Length - 4) == RET_STRING)
                    {
                        bRet = true;
                        info = info.Substring(0, info.Length - 4);
                    }
                    else
                    {
                        Log.Warn("GetCardInfo:" + info);
                    }
                }
            }
            catch (Exception ex)
            {
            	Log.Error("GetCardInfo:" + ex);
            }
            return bRet;
        }

        /// <summary>
        /// 选择电卡文件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool SelectDirectory(ref string info)
        {
            bool bRet = false;
            if (!CardReader.IsUse)
            {
                info = "6F0A8400A50688009F0801029000";
                return true;
            }

            try
            {
                byte[] bOut = new byte[2048];
                int nOut = 0;
                CardReader.Status sRet = CardReader.CardChipIO(SELECT_FILE, SELECT_FILE.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    Log.Debug("SelectDirectory:" + info);
                    string temp = info.Substring(info.Length - 4);
                    if (temp == RET_STRING || temp == RET_STRING1)
                    {
                        bRet = true;
                        info = info.Substring(0, info.Length - 4);
                    }
                    else
                    {
                        Log.Warn("SelectDirectory:" + info);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("SelectDirectory:" + ex);
            }
            return bRet;
        }

        /// <summary>
        /// 判断电卡类型
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool GetCardType(ref string info)
        {
            bool bRet = false;
            try
            {
                byte[] bOut = new byte[2048];
                int nOut = 0;
                CardReader.Status sRet = CardReader.CardChipIO(GET_CARD_TYPE, GET_CARD_TYPE.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    Log.Debug("GetCardType:" + info);
                    if (info == RET_CARDTYPE_2)
                    {
                        bRet = true;
                    }
                    else
                    {
                        Log.Warn("GetCardType:" + info);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetCardType:" + ex);
            }
            return bRet;
        }

        /// <summary>
        /// 上电获取卡片序列号
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool GetCardNo(ref string info)
        {
            bool bRet = false;

            if (!CardReader.IsUse)
            {
                info = "10110800000F82CA";
                return true;
            }
   
            try
            {
                byte[] bAnswer = new byte[512];
                int nLen = 0;
                int nChip = 0;
                CardReader.Status status = CardReader.CardPowerUp(bAnswer, ref nLen, ref nChip);
                Log.Error("GetCardResult:" + Enum.GetName(typeof(CardReader.Status), status));
                if (status == CardReader.Status.CARD_SUCC)
                {
                    bRet = true;
                    info = PubFunc.ByteArrayToHexString(bAnswer, nLen);
                    Log.Debug("GetCardNO:" + info);
                    info = info.Substring(info.Length - 16);
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetCardNo:" + ex);
            }
            return bRet;
        }

        /// <summary>
        /// 随机数
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool GetRandom(ref string info)
        {
            bool bRet = false;
            if (!CardReader.IsUse)
            {
                info = "C97FFA2376FD9E43";
                return true;
            }
            try
            {
                byte[] bOut = new byte[2048];
                int nOut = 0;
                CardReader.Status sRet = CardReader.CardChipIO(GET_RANDOM, GET_RANDOM.Length, bOut, ref nOut);
                Log.Debug("GetRandom Status:" + Enum.GetName(typeof(CardReader.Status), sRet));
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    Log.Debug("GetRandom:" + info);
                    if (info.Substring(info.Length - 4) == RET_STRING)
                    {
                        info = info.Substring(0, info.Length - 4);
                        bRet = true;
                    }
                    else
                    {
                        Log.Warn("GetRandom:" + info);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetRandom:" + ex);
            }
            return bRet;
        }

        #region 读取电卡文件

        /// <summary>
        /// 用户信息文件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool GetEF1(ref string info)
        {
            bool bRet = false;
            if (!CardReader.IsUse)
            {
                info = "68010027028F00000001000000000004000050000000000000000100000100000370598100005458666401D4169000";
                return true;
            }
            try
            {
                byte[] bOut = new byte[2048];
                int nOut = 0;
                CardReader.Status sRet = CardReader.CardChipIO(GET_EF1, GET_EF1.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    Log.Debug("GetEF1:" + info);
                    if (info.Substring(info.Length - 4) == RET_STRING)
                    {
                        bRet = true;
                        info = info.Substring(0, info.Length - 4);
                    }
                    else
                    {
                        Log.Warn("GetEF1:" + info);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetEF1:" + ex);
            }
            return bRet;
        }

        /// <summary>
        /// 读钱包文件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool GetEF2(ref string info)
        {
            bool bRet = false;
            if (!CardReader.IsUse)
            {
                info = "0000283C000000019000";
                return true;
            }
            try
            {
                byte[] bOut = new byte[2048];
                int nOut = 0;
                CardReader.Status sRet = CardReader.CardChipIO(GET_EF2, GET_EF2.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    Log.Debug("GetEF2:" + info);
                    if (info.Substring(info.Length - 4) == RET_STRING)
                    {
                        bRet = true;
                        info = info.Substring(0, info.Length - 4);
                    }
                    else
                    {
                        Log.Warn("GetEF2:" + info);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetEF2:" + ex);
            }
            return bRet;
        }

        /// <summary>
        /// 4.	读费率文件1的部分,截取前256位
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool GetEF3(ref string info)
        {
            bool bRet = false;
            if (!CardReader.IsUse)
            {
                info = "680100FC00005224000052240000522400002535000052240000522400005224000017500000522400005224000052240000253500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
                return true;
            }
            try
            {
                byte[] bOut = new byte[2048];
                int nOut = 0;
                CardReader.Status sRet = CardReader.CardChipIO(GET_EF3, GET_EF3.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    Log.Debug("GetEF3:" + info);
                    if (info.Substring(info.Length - 4) == RET_STRING)
                    {
                        bRet = true;
                        //info = info.Substring(0, info.Length - 4);
                        //if (info.Length > 256)
                        //    info = info.Substring(0, 256);
                    }
                    else
                    {
                        Log.Warn("GetEF3:" + info);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetEF3:" + ex.ToString());
            }
            return bRet;
        }

        /// <summary>
        /// 5.	读费率文件2的部分,截取前256位
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool GetEF4(ref string info)
        {
            bool bRet = false;
            if (!CardReader.IsUse)
            {
                info = "680100FC00005224000052240000522400002535000052240000522400005224000017500000522400005224000052240000253500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
                return true;
            }
            try
            {
                byte[] bOut = new byte[2048];
                int nOut = 0;
                CardReader.Status sRet = CardReader.CardChipIO(GET_EF4, GET_EF4.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    Log.Debug("GetEF4:" + info);
                    if (info.Substring(info.Length - 4) == RET_STRING)
                    {
                        bRet = true;
                        info = info.Substring(0, info.Length - 4);
                        if (info.Length > 256)
                            info = info.Substring(0, 256);
                    }
                    else
                    {
                        Log.Warn("GetEF4:" + info);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetEF4:" + ex.ToString());
            }
            return bRet;
        }

        /// <summary>
        /// 8.	读返写区文件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool GetEF5(ref string info)
        {
            bool bRet = false;
            if (!CardReader.IsUse)
            {
                info = "6811002B02000001000001000000629276002013007233000017E80000001700000000010004010000001510051026FE169000";
                return true;
            }
            try
            {
                byte[] bOut = new byte[2048];
                int nOut = 0;
                CardReader.Status sRet = CardReader.CardChipIO(GET_EF5, GET_EF5.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    Log.Debug("GetEF5:" + info);
                    if (info.Substring(info.Length - 4) == RET_STRING)
                    {
                        bRet = true;
                        info = info.Substring(0, info.Length - 4);
                    }
                    else
                    {
                        Log.Warn("GetEF5:" + info);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetEF5:" + ex);
            }
            return bRet;
        }
        #endregion

        #region 设置电卡文件信息

        /// <summary>
        /// 设置用户信息文件
        /// </summary>
        /// <returns></returns>
        public bool SetEF1(string ef1)
        {
            bool bRet = false;
            try
            {
                byte[] bOut = new byte[2048];
                string sCommand = "04D6810031";
                int nOut = 0;
                string info = "";
                byte[] bCommand = PubFunc.HexStringToByteArray(sCommand + ef1);
                CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    if (info == RET_STRING)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("SetEF1:" + ex);
            }
            return bRet;
        }

        /// <summary>
        /// 写钱包文件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool SetEF2(string ef2)
        {
            bool bRet = false;
            try
            {
                byte[] bOut = new byte[2048];
                string sCommand = "04D682000C";
                int nOut = 0;
                string info = "";
                byte[] bCommand = PubFunc.HexStringToByteArray(sCommand + ef2);
                CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    if (info == RET_STRING)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("SetEF2:" + ex);
            }
            return bRet;
        }
        /// <summary>
        /// 4.	写费率文件1的第一部分
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool SetEF31(string ef31)
        {
            bool bRet = false;
            try
            {
                byte[] bOut = new byte[2048];
                string sCommand = "04D6830084";
                int nOut = 0;
                string info = "";
                byte[] bCommand = PubFunc.HexStringToByteArray(sCommand + ef31);
                CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    if (info == RET_STRING)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("SetEF31:" + ex);
            }
            return bRet;
        }

        /// <summary>
        /// 5.	写费率文件1的第二部分
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool SetEF32(string ef32)
        {
            bool bRet = false;
            try
            {
                byte[] bOut = new byte[2048];
                string sCommand = "04D6838086";
                int nOut = 0;
                string info = "";
                byte[] bCommand = PubFunc.HexStringToByteArray(sCommand + ef32);
                CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    if (info == RET_STRING)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("SetEF32:" + ex);
            }
            return bRet;
        }
        /// <summary>
        /// 6.	写费率文件2的第一部分
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool SetEF41(string ef41)
        {
            bool bRet = false;
            try
            {
                byte[] bOut = new byte[2048];
                string sCommand = "04D6840084";
                int nOut = 0;
                string info = "";
                byte[] bCommand = PubFunc.HexStringToByteArray(sCommand + ef41);
                CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    if (info == RET_STRING)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("SetEF41:" + ex);
            }
            return bRet;
        }
        /// <summary>
        /// 7.	写费率文件2的第二部分
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool SetEF42(string ef42)
        {
            bool bRet = false;
            try
            {
                byte[] bOut = new byte[2048];
                string sCommand = "04D6848086";
                int nOut = 0;
                string info = "";
                byte[] bCommand = PubFunc.HexStringToByteArray(sCommand + ef42);
                CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    if (info == RET_STRING)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("SetEF42:" + ex);
            }
            return bRet;
        }
        /// <summary>
        /// 8.	写返写区文件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool SetEF5(string ef5)
        {
            bool bRet = false;
            try
            {
                byte[] bOut = new byte[2048];
                string sCommand = "04D6850035";
                int nOut = 0;
                string info = "";
                byte[] bCommand = PubFunc.HexStringToByteArray(sCommand + ef5);
                CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    if (info == RET_STRING)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("SetEF5:" + ex);
            }
            return bRet;
        }
        #endregion

        //#region 购电认证 old

        //public bool CertAuth(string random,string certPsd)
        //{
        //    bool bRet = false;
        //    try
        //    {
        //        byte[] bOut = new byte[2048];
        //        byte[] bOut1 = new byte[2048];
        //        int nOut = 0;
        //        string info = "";
        //        byte[] bRandom = PubFunc.HexStringToByteArray(random);
        //        byte[] bCommand = new byte[CERT_AUTH.Length + bRandom.Length];
        //        Array.Copy(CERT_AUTH, bCommand, CERT_AUTH.Length);
        //        Array.Copy(bRandom, 0, bCommand, CERT_AUTH.Length, bRandom.Length);
        //        //计算随机数
        //        CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
        //        if (sRet == CardReader.Status.CARD_SUCC)
        //        {
        //            info = PubFunc.ByteArrayToHexString(bOut, nOut);
        //            if (info != RET_STRING && info == RET_STRING2)
        //                return false;
        //            nOut = 0;
        //            //校验身份验证数据
        //            sRet = CardReader.CardChipIO(CERT_AUTH_GET, CERT_AUTH_GET.Length, bOut1, ref nOut);
        //            if (sRet == CardReader.Status.CARD_SUCC)
        //            {
        //                info = PubFunc.ByteArrayToHexString(bOut1, nOut);
        //                if (info.Substring(info.Length - 4) == RET_STRING)
        //                {
        //                    info = info.Substring(0, info.Length - 4);
        //                    bRet = string.Compare(info, certPsd, true) == 0;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("CertAuth:" + ex);
        //    }
        //    return bRet;
        //}

        //public bool LimitAuth(string limitPsd)
        //{
        //    bool bRet = false;
        //    try
        //    {
        //        byte[] bOut = new byte[2048];
        //        int nOut = 0;
        //        string info = "";
        //        byte[] bLimit = PubFunc.HexStringToByteArray(limitPsd);
        //        byte[] bCommand = new byte[LIMIT_AUTH.Length + bLimit.Length];
        //        Array.Copy(LIMIT_AUTH, bCommand, LIMIT_AUTH.Length);
        //        Array.Copy(bLimit, 0, bCommand, LIMIT_AUTH.Length, bLimit.Length);
        //        CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
        //        if (sRet == CardReader.Status.CARD_SUCC)
        //        {
        //            info = PubFunc.ByteArrayToHexString(bOut, nOut);
        //            if (info == RET_STRING)
        //            {
        //                bRet = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("LimitAuth:" + ex);
        //    }
        //    return bRet;
        //}

        //public bool ExtAuth(string extPsd)
        //{
        //    bool bRet = false;
        //    try
        //    {
        //        byte[] bOut = new byte[2048];
        //        int nOut = 0;
        //        string info = "";
        //        byte[] bExt = PubFunc.HexStringToByteArray(extPsd);
        //        byte[] bCommand = new byte[EXT_AUTH.Length + bExt.Length];
        //        Array.Copy(EXT_AUTH, bCommand, EXT_AUTH.Length);
        //        Array.Copy(bExt, 0, bCommand, EXT_AUTH.Length, bExt.Length);
        //        CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
        //        if (sRet == CardReader.Status.CARD_SUCC)
        //        {
        //            info = PubFunc.ByteArrayToHexString(bOut, nOut);
        //            if (info == RET_STRING)
        //            {
        //                bRet = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("ExtAuth:" + ex);
        //    }
        //    return bRet;
        //}

        //#endregion

        #region 购电认证

        public bool CertAuth(string random, string certPsd)
        {
            bool bRet = false;
            try
            {
                Log.Debug("身份认证");
                byte[] bOut = new byte[2048];
                //byte[] bOut1 = new byte[2048];
                int nOut = 0;
                string info = "";
                byte[] bRandom = PubFunc.HexStringToByteArray(random);
                byte[] bCommand = new byte[CERT_AUTH.Length + bRandom.Length];
                Array.Copy(CERT_AUTH, bCommand, CERT_AUTH.Length);
                Array.Copy(bRandom, 0, bCommand, CERT_AUTH.Length, bRandom.Length);
                Log.Debug("身份认证命令:" + PubFunc.ByteArrayToHexString(bCommand, bCommand.Length));
                //计算随机数
                CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    if (info != RET_STRING && info == RET_STRING2)
                        return false;
                    //nOut = 0;
                    ////校验身份验证数据
                    //sRet = CardReader.CardChipIO(CERT_AUTH_GET, CERT_AUTH_GET.Length, bOut1, ref nOut);
                    //if (sRet == CardReader.Status.CARD_SUCC)
                    //{
                    //    info = PubFunc.ByteArrayToHexString(bOut1, nOut);
                    //    if (info.Substring(info.Length - 4) == RET_STRING)
                    //    {
                    info = info.Substring(0, info.Length - 4);
                    Log.Info("CertAuth:" + info + " CertPsd:" + certPsd);
                    bRet = String.Compare(info, certPsd, StringComparison.OrdinalIgnoreCase) == 0;
                    //}
                    //}
                }
            }
            catch (Exception ex)
            {
                Log.Error("CertAuth:" + ex.ToString());
            }
            return bRet;
        }

        public bool LimitAuth(string limitPsd)
        {
            bool bRet = false;
            try
            {
                Log.Debug("写权限认证:" + limitPsd);
                byte[] bOut = new byte[2048];
                int nOut = 0;
                string info = "";
                byte[] bLimit = PubFunc.HexStringToByteArray(limitPsd);
                byte[] bCommand = new byte[LIMIT_AUTH.Length + bLimit.Length];
                Array.Copy(LIMIT_AUTH, bCommand, LIMIT_AUTH.Length);
                Array.Copy(bLimit, 0, bCommand, LIMIT_AUTH.Length, bLimit.Length);
                //bCommand[bCommand.Length - 1] = 0x00;
                Log.Debug("写权限命令:" + PubFunc.ByteArrayToHexString(bCommand, bCommand.Length));
                CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    Log.Debug("写权限：" + info);
                    if (info == RET_STRING)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("LimitAuth:" + ex.ToString());
            }
            return bRet;
        }

        public bool ExtAuth(string extPsd)
        {
            bool bRet = false;
            try
            {
                Log.Debug("购电外部认证" + extPsd);
                byte[] bOut = new byte[2048];
                int nOut = 0;
                string info = "";
                byte[] bExt = PubFunc.HexStringToByteArray(extPsd);
                byte[] bCommand = new byte[EXT_AUTH.Length + bExt.Length];
                Array.Copy(EXT_AUTH, bCommand, EXT_AUTH.Length);
                Array.Copy(bExt, 0, bCommand, EXT_AUTH.Length, bExt.Length);
                Log.Debug("购电外部命令:" + PubFunc.ByteArrayToHexString(bCommand, bCommand.Length));
                CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    Log.Debug("购电外部：" + info);
                    if (info == RET_STRING)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("ExtAuth:" + ex.ToString());
            }
            return bRet;
        }

        #endregion


        #region 更新密钥
        public bool UpdateKey(string key)
        {
            bool bRet = false;
            try
            {
                byte[] bOut = new byte[2048];
                string sKey = "84D401FF20";
                int nOut = 0;
                string info = "";
                byte[] bCommand = PubFunc.HexStringToByteArray(sKey + key);
                CardReader.Status sRet = CardReader.CardChipIO(bCommand, bCommand.Length, bOut, ref nOut);
                if (sRet == CardReader.Status.CARD_SUCC)
                {
                    info = PubFunc.ByteArrayToHexString(bOut, nOut);
                    if (info == RET_STRING)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("UpdateKey:" + ex);
            }
            return bRet;
        }
        #endregion

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="EF1"></param>
        /// <param name="EF2"></param>
        /// <param name="EF3"></param>
        /// <param name="EF4"></param>
        /// <param name="EF5"></param>
        /// <returns></returns>
        public bool GetFileInfo(PowerCardInfo cardEFInfo)
        {
            bool bRet = false;
            try
            {
                bRet = GetEF1(ref cardEFInfo.EF1) ? GetEF2(ref cardEFInfo.EF2) : false;
                if (!bRet)
                    return false;

                bRet = GetEF3(ref cardEFInfo.EF3);
                if (!bRet)
                    return false;

                bRet = GetEF4(ref cardEFInfo.EF4);
                if (!bRet)
                    return false;

                bRet = GetEF5(ref cardEFInfo.EF5);
            }
            catch (Exception ex)
            {
                Log.Error("GetFileInfo:" + ex);
            }
            return bRet;
        }

        /// <summary>
        /// 授权验证
        /// </summary>
        /// <param name="random"></param>
        /// <param name="certPsd"></param>
        /// <param name="limitPsd"></param>
        /// <param name="extPsd"></param>
        /// <returns></returns>
        public bool Authenticate(PowerCardInfo authInfo)
        {
            bool bRet = false;
            try
            {
                bRet = CertAuth(authInfo.Random, authInfo.CertDes) && LimitAuth(authInfo.LimitDes);
                if (!bRet)
                    return false;
                bRet = ExtAuth(authInfo.ExtDes);
            }
            catch (Exception ex)
            {
                Log.Error("Authenticate:" + ex);
            }
            return bRet;
        }

        /// <summary>
        /// 更新密钥
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="key3"></param>
        /// <param name="key4"></param>
        /// <returns></returns>
        public bool UpdateKey(string key1, string key2,
            string key3, string key4)
        {
            bool bRet = false;
            try
            {
                bRet = UpdateKey(key1) ? UpdateKey(key2) : false;
                if (!bRet)
                    return false;
                bRet = UpdateKey(key3) ? UpdateKey(key4) : false;
            }
            catch (Exception ex)
            {
                Log.Error("UpdateKey:" + ex);
            }
            return bRet;
        }

        /// <summary>
        /// 写电卡文件
        /// </summary>
        /// <param name="EF1"></param>
        /// <param name="EF2"></param>
        /// <param name="EF31"></param>
        /// <param name="EF32"></param>
        /// <param name="EF41"></param>
        /// <param name="EF42"></param>
        /// <param name="EF5"></param>
        /// <returns></returns>
        public bool SetFileInfo(PowerCardInfo efInfo)
        {
            bool bRet = false;
            try
            {
                string temp = efInfo.W_EF1.Substring(10);
                byte bUpdateFlag = PubFunc.HexStringToByteArray(temp)[0];
                if (((bUpdateFlag >> 7) & 1) == 1)
                {
                    if (!SetEF1(efInfo.W_EF1))
                        return false;
                }

                if (((bUpdateFlag >> 0) & 1) == 1)
                {
                    bRet = SetEF31(efInfo.W_EF31) ? SetEF32(efInfo.W_EF32) : false;
                    if (!bRet)
                        return false;
                }

                if (((bUpdateFlag >> 1) & 1) == 1)
                {
                    bRet = SetEF41(efInfo.W_EF41) ? SetEF42(efInfo.W_EF42) : false;
                    if (!bRet)
                        return false;
                }

                bRet = SetEF2(efInfo.W_EF2) ? SetEF5(efInfo.W_EF5) : false;
            }
            catch (Exception ex)
            {
                Log.Error("SetFileInfo:" + ex);
            }
            return bRet;
        }

        /// <summary>
        /// 读购电卡
        /// </summary>
        /// <returns></returns>
        public bool ReadPowerCard(PowerCardInfo cardInfo)
        {
            bool bRet = false;
            try
            {
                //Test
                //return true;
                string powerCardNo = "", powerCardRandom = "", powerCardInfo = "";
                string temp = "";
                if (cardInfo == null)
                {
                    Log.Warn("PowerCardInfo is null");
                    return false;
                }
                cardInfo.ErrorMsg = "请确认您的购电卡是否正确插入，谢谢";

                    if (!GetCardNo(ref powerCardNo))
                    {
                        Log.Debug("GetCardNo Err!");
                        return false;
                    }

                    if (!GetCardInfo(ref powerCardInfo))
                        return false;

                    if (!SelectDirectory(ref temp))
                        return false;

                    if (!GetRandom(ref powerCardRandom))
                        return false;

                    if (!GetFileInfo(cardInfo))
                        return false;
                cardInfo.CardNo = powerCardNo;
                cardInfo.Random = powerCardRandom;
                cardInfo.CardInfo = powerCardInfo;
                bRet = true;
                Log.Debug("ReadPowerCard Success! Random="+powerCardRandom+";CardNo="+powerCardNo);
            }
            catch (Exception ex)
            {
                Log.Error("ReadPowerCard:" + ex);
            }
            return bRet;
        }

        public bool WritePowerCard(PowerCardInfo cardInfo)
        {
            bool bRet = false;
            try
            {
                //Test
                //return true;
                string powerCardNo = "";
                string temp = "";
                if (cardInfo == null)
                {
                    Log.Warn("PowerCardInfo is null");
                    return false;
                }
                cardInfo.ErrorMsg = "请确认您的购电卡是否正确插入，谢谢";

                //if (!GetCardNo(ref powerCardNo))
                //    return false;

                //if (!GetCardType(ref temp))
                //    return false;

                //if (!SelectDirectory(ref temp))
                //    return false;

                if (!Authenticate(cardInfo))
                {
                    Log.Debug("写卡权限验证失败");
                    cardInfo.ErrorMsg = "写卡权限验证失败";
                    return false;
                }

                if (!SetFileInfo(cardInfo))
                {
                    Log.Debug("写文件失败");
                    cardInfo.ErrorMsg = "写文件失败";
                    return false;
                }
                bRet = true;
            }
            catch (Exception ex)
            {
                Log.Error("WritePowerCard:" + ex);
            }
            finally
            {
                CardReader.CardPowerDown();
            }
            return bRet;
        }
        


    }
}
