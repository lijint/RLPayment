using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Landi.FrameWorks.HardWare;

namespace Landi.FrameWorks
{
    #region 工具类
    public static class PubFunc
    {
        public static string ByteArrayToHexString(byte[] data, int dataLen)
        {
            StringBuilder sb = new StringBuilder(dataLen * 2);
            for (int i = 0; i < dataLen; i++)
            {
                sb.Append(Convert.ToString(data[i], 16).PadLeft(2, '0'));
            }
            return sb.ToString().ToUpper();
        }

        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            s = s.Replace("=", "D");
            if (s.Length % 2 != 0) s = "0" + s;
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
    }
    #endregion

    public class EMVTransProcess
    {
        /// <summary>
        /// IC卡交易
        /// </summary>
        public enum PbocTransType : int
        {
            //IC卡交易
            /// <summary>
            /// 电子现金查询
            /// </summary>
            EC_INQUERY = 80,//电子现金查询
            /// <summary>
            /// IC卡日志查询
            /// </summary>
            EC_LOGINQ = 83,//IC卡日志查询
            /// <summary>
            /// 脱机消费
            /// </summary>
            EC_PURCHASE = 81,//脱机消费
            /// <summary>
            /// 指定帐户圈存
            /// </summary>
            EC_LOAD = 82,//指定帐户圈存
            /// <summary>
            /// 非指定帐户圈存
            /// </summary>
            EC_LOAD_U = 84,//非指定帐户圈存
            /// <summary>
            /// 消费
            /// </summary>
            PURCHASE = 1,//消费
            /// <summary>
            /// 余额查询
            /// </summary>
            INQUERY = 2,//余额查询
        }

        /// <summary>
        /// 冲正接口
        /// </summary>
        public interface IReverse
        {
            /// <summary>
            /// 创建冲正文件
            /// </summary>
            void CreateReverseFile();
            /// <summary>
            /// 发送冲正文件
            /// </summary>
            void DoReverseFile();
            /// <summary>
            /// 清除冲正文件
            /// </summary>
            void ClearReverseFile();
        }

        public class EMVData
        {
            public string CardNum; //银卡卡号
            public string CardSeqNum;//银行卡序列号 IC
            public string CardExpDate;//银行卡有效日期
            public string Track2; //磁道2
            public string Track3; //磁道3
            public string CommonErrorMessage; //错误说明

            public byte[] SendField55;
            public byte[] EndField55;
            public byte[] AutoField55;
            public string RecvField38;

            public void Clear()
            {
                CardNum = null;
                CardSeqNum = null;
                CardExpDate = null;
                Track2 = null;
                Track3 = null;
                CommonErrorMessage = "请确认插入的卡片是否为银联卡.";
                SendField55 = null;
                EndField55 = null;
                AutoField55 = null;
                RecvField38 = null;
            }
        }
        /// <summary>
        /// EMV信息
        /// </summary>
        public EMVData EMVInfo = null;

        /// <summary>
        /// 支付模式：0:IC接触 1:IC感应/R80
        /// </summary>
        public int PayType = 0;

        private EC_PBOC pboc = new EC_PBOC();

        public EMVTransProcess() { }

        /// <summary>
        /// AID和CA文件前缀
        /// </summary>
        /// <param name="AidAndCAFileName">AID和CA文件前缀</param>
        public EMVTransProcess(string AidAndCAFileName)
        {
            byte[] fileName = new byte[0];
            fileName = Encoding.Default.GetBytes(AidAndCAFileName);
            pboc.App_EMVSetAidAndCAFileName(fileName, fileName.Length);
        }

        public static void EMVSetAidAndCAFileName(string AidAndCAFileName)
        {
            byte[] fileName = new byte[0];
            fileName = Encoding.Default.GetBytes(AidAndCAFileName);
            new EC_PBOC().App_EMVSetAidAndCAFileName(fileName, fileName.Length);
        }

        /// <summary>
        /// EMV初始化
        /// </summary>
        /// <param name="dInAmount"></param>
        /// <param name="pbocType"></param>
        /// <returns></returns>
        public int EMVTransInit(double dInAmount, PbocTransType pbocType)
        {
            int state = 0;
            byte[] answer = new byte[128];
            int pnLen = 0;
            int pnChipProtocol = 0;
            EMVInfo = new EMVData();
            long hand = 0;
            try
            {
                #region 卡片上电初始化
                if (PayType == 1)
                {
                    hand = R80.GetHandle();
                    R80.Status cRet = R80.PowerUp(10, "");
                    if (cRet != R80.Status.EM_MIFARE_SUCC)
                    {
                        Log.Warn("上电失败");
                        return -1;
                    }
                }
                else
                {
                    hand = CardReader.GetHandle();
                    CardReader.Status cRet = CardReader.CardPowerUp(answer, ref pnLen, ref pnChipProtocol);
                    if (cRet != CardReader.Status.CARD_SUCC)
                    {
                        CardReader.CardPowerDown();
                        Log.Warn("上电失败");
                        return -1;
                    }
                }
                
                state = pboc.App_EMVLInit(PayType, hand);
                if (state != 0)
                {
                    Log.Warn("卡片初始化失败");
                    return -1;
                }
                #endregion

                #region 获取卡片应用

                byte[] appList = new byte[256];
                int nListNum = 0;
                pboc.App_EMVL2SelectApp(pnChipProtocol, appList, ref nListNum);
                string[] strEmvList = System.Text.Encoding.Default.GetString(appList).Trim().Replace("\0", "").Split('|');
                if (nListNum < 1)
                {
                    CardReader.CardPowerDown();
                    Log.Warn("卡片无可用的应用");
                    return -1;
                }

                byte[] inTrace = Encoding.Default.GetBytes("000000");
                byte[] inDay = Encoding.Default.GetBytes(DateTime.Now.ToString("yyMMdd"));
                byte[] inTime = Encoding.Default.GetBytes(DateTime.Now.ToString("HHmmss"));
                byte[] inAmount = Encoding.Default.GetBytes(Utility.AmountToString(dInAmount.ToString())); ;
                byte[] inOtherAmount = Encoding.Default.GetBytes("000000000000");
                int iAppId = 0;
                bool bEmvOk = false;
                foreach (string tempEmv in strEmvList)
                {
                    //A000000333010101|银联
                    //if (!String.IsNullOrEmpty(tempEmv) && tempEmv.StartsWith("A000000333"))
                    //{
                    //    state = pboc.App_EMVStartEmvApp(iAppId, pnChipProtocol, (int)PbocTransType.PURCHASE, inTrace, inDay, inTime, inAmount, inOtherAmount);
                    //    if (state == 0)
                    //    {
                    //        bEmvOk = true;
                    //        break;
                    //    }
                    //}
                    state = pboc.App_EMVStartEmvApp(iAppId, pnChipProtocol, (int)pbocType, inTrace, inDay, inTime, inAmount, inOtherAmount);
                    if (state == 0)
                    {
                        bEmvOk = true;
                        break;
                    }
                    iAppId++;
                }
                if (!bEmvOk)
                {
                    CardReader.CardPowerDown();
                    Log.Warn("没有支持的应用");
                    return -1;
                }

                #endregion

                #region 获取卡片卡号信息

                byte[] cardNo = new byte[21];
                int cardNoLen = 0;
                byte[] track2 = new byte[38];
                int track2Len = 0;
                byte[] expData = new byte[5];
                int expLen = 0;
                byte[] cardSeqNum = new byte[2];

                pboc.App_EMVGetCardNo(cardNo, ref cardNoLen, track2, ref track2Len, expData, ref expLen, cardSeqNum);
                EMVInfo.CardNum = Encoding.Default.GetString(cardNo).Trim('\0');
                EMVInfo.Track2 = Encoding.Default.GetString(track2).Trim('\0');
                EMVInfo.CardSeqNum = Convert.ToString(cardSeqNum[0]).Trim('\0');
                EMVInfo.CardExpDate = Encoding.Default.GetString(expData).Trim('\0');
                if (String.IsNullOrEmpty(EMVInfo.CardNum))
                {
                    CardReader.CardPowerDown();
                    Log.Warn("IC:读卡号失败");
                    return -1;
                }
                else
                    EMVInfo.CardNum = EMVInfo.CardNum.Replace('\0', ' ').Trim();
                #endregion
            }
            catch(Exception ex)
            {
                CardReader.CardPowerDown();
                Log.Error("IC:初始化异常", ex);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// EMV交易，获取交易55域
        /// </summary>
        /// <returns></returns>
        public int EMVTransDeal()
        {
            int state = 0;
            try
            {

                #region 银行卡判定

                //3 App_EMVTermRiskManageProcessRestrict
                state = pboc.App_EMVTermRiskManageProcessRestrict();
                if (state != 0)
                {
                    CardReader.CardPowerDown();
                    Log.Warn("App_EMVTermRiskManageProcessRestrict:银行卡无效 state=" + state);
                    return -1;
                }

                //4 App_EMVCardHolderValidate
                int cTime = 0;
                state = pboc.App_EMVCardHolderValidate(ref cTime);
                if (state != 0)
                {
                    state = pboc.App_EMVContinueCardHolderValidate(1, ref cTime);//无论state为何值，直接提示联机pin以成功输入。内核对联机PIN的处理，只需要告诉是否完成PIN即可
                }

                //5 App_EMVActionAnalysis
                state = pboc.App_EMVActionAnalysis();
                if (state == -1)
                {
                    CardReader.CardPowerDown();
                    Log.Warn("App_EMVActionAnalysis:系统故障 state=" + state);
                    return -1;
                }
                else if (state == 0)
                {
                    CardReader.CardPowerDown();
                    Log.Warn("App_EMVActionAnalysis:脱机交易成功 state=" + state);
                    return -1;
                }
                else if (state == 1)
                {
                    CardReader.CardPowerDown();
                    Log.Warn("App_EMVActionAnalysis:卡片拒绝交易 state=" + state);
                    return -1;
                }
                else if (state == 2)
                {
                    byte[] field55 = new byte[512];
                    int field55Len = 0;
                    //6 App_EMVGetField55
                    pboc.App_EMVGetField55(field55, ref field55Len);
                    EMVInfo.SendField55 = new byte[field55Len];
                    Array.Copy(field55, EMVInfo.SendField55, field55Len);
                }
                else
                {
                    CardReader.CardPowerDown();
                    Log.Warn("App_EMVCardHolderValidate:交易失败 state=" + state);
                    return -1;
                }
                #endregion
            }
            catch (System.Exception ex)
            {
                CardReader.CardPowerDown();
                Log.Error("EMV交易失败", ex);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// EMV卡片交互
        /// </summary>
        /// <param name="field55"></param>
        /// <param name="AuthNo"></param>
        /// <returns></returns>
        public int EMVTransEnd(byte[] field55,string AuthNo)
        {
            int ret = 0;
            try
            {
                string temp = PubFunc.ByteArrayToHexString(field55, field55.Length);
                ret = pboc.App_EMVOnlineDataProcess(0, Encoding.Default.GetBytes(temp),temp.Length, 
                    PubFunc.HexStringToByteArray(AuthNo),AuthNo.Length);
                if (ret == 0)
                {
                    byte[] field551 = new byte[1024];
                    int field55Len = 0;
                    pboc.App_EMVGetField55(field551, ref field55Len);
                    EMVInfo.EndField55 = new byte[field55Len];
                    Array.Copy(field551, EMVInfo.EndField55, field55Len);
                }
            }
            catch (System.Exception ex)
            {
                Log.Error("App_EMVOnlineDataProcess Error!", ex);
                ret = -1;
            }
            finally
            {
                CardReader.CardPowerDown();
            }
            return ret;
        }

        /// <summary>
        /// 脚本通知的55域
        /// </summary>
        private string GetField55Script(string _field55)
        {
            //9F33 95 9F37 9F1E 9F10 9F26 9F36 82 DF31 9F1A 9A
            string scriptField55 = "";
            //TLVHandler tlv = new TLVHandler();
            //Hashtable hs = new Hashtable();
            //TLVHandler handler = new TLVHandler();
            //handler.ParseTLV();
            //tlv.UnPacket(_field55, ref hs);

            //#region 打包Field55

            //if (hs.ContainsKey("9F33"))
            //{
            //    string s0 = hs["9F33"].ToString().Trim();
            //    tlv.Packet("9F33", s0.Length, s0, ref scriptField55);
            //}
            //if (hs.ContainsKey("95"))
            //{
            //    string s0 = hs["95"].ToString().Trim();
            //    tlv.Packet("95", s0.Length, s0, ref scriptField55);
            //}
            //if (hs.ContainsKey("9F37"))
            //{
            //    string s0 = hs["9F37"].ToString().Trim();
            //    tlv.Packet("9F37", s0.Length, s0, ref scriptField55);
            //}
            //if (hs.ContainsKey("9F1E"))
            //{
            //    string s0 = hs["9F1E"].ToString().Trim();
            //    tlv.Packet("9F1E", s0.Length, s0, ref scriptField55);
            //}
            //if (hs.ContainsKey("9F26"))
            //{
            //    string s0 = hs["9F26"].ToString().Trim();
            //    tlv.Packet("9F26", s0.Length, s0, ref scriptField55);
            //}
            //if (hs.ContainsKey("9F36"))
            //{
            //    string s0 = hs["9F36"].ToString().Trim();
            //    tlv.Packet("9F36", s0.Length, s0, ref scriptField55);
            //}
            //if (hs.ContainsKey("82"))
            //{
            //    string s0 = hs["82"].ToString().Trim();
            //    tlv.Packet("82", s0.Length, s0, ref scriptField55);
            //}
            //if (hs.ContainsKey("DF31"))
            //{
            //    string s0 = hs["DF31"].ToString().Trim();
            //    tlv.Packet("DF31", s0.Length, s0, ref scriptField55);
            //}
            //if (hs.ContainsKey("9F1A"))
            //{
            //    string s0 = hs["9F1A"].ToString().Trim();
            //    tlv.Packet("9F1A", s0.Length, s0, ref scriptField55);
            //}
            //if (hs.ContainsKey("9A"))
            //{
            //    string s0 = hs["9A"].ToString().Trim();
            //    tlv.Packet("9A", s0.Length, s0, ref scriptField55);
            //}
            //#endregion

            return scriptField55;
        }

        /// <summary>
        /// 脚本通知的55域
        /// </summary>
        private byte[] GetField55Script(byte[] _field55)
        {
            //9F33 95 9F37 9F1E 9F10 9F26 9F36 82 DF31 9F1A 9A
            TLVHandler tlv = new TLVHandler();
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(_field55);
            byte[] value = new byte[0];

            #region 打包Field55

            if ((value = handler.GetBytesValue("9F33")) != null)
            {
                tlv.AddTag("9F33", value);
            }
            if ((value = handler.GetBytesValue("95")) != null)
            {
                tlv.AddTag("95", value);
            }
            if ((value = handler.GetBytesValue("9F37")) != null)
            {
                tlv.AddTag("9F37", value);
            }
            if ((value = handler.GetBytesValue("9F1E")) != null)
            {
                tlv.AddTag("9F1E", value);
            }
            if ((value = handler.GetBytesValue("9F10")) != null)
            {
                tlv.AddTag("9F10", value);
            }
            if ((value = handler.GetBytesValue("9F26")) != null)
            {
                tlv.AddTag("9F26", value);
            }
            if ((value = handler.GetBytesValue("9F36")) != null)
            {
                tlv.AddTag("9F36", value);
            }
            if ((value = handler.GetBytesValue("82")) != null)
            {
                tlv.AddTag("82", value);
            }
            if ((value = handler.GetBytesValue("DF31")) != null)
            {
                tlv.AddTag("DF31", value);
            }
            if ((value = handler.GetBytesValue("9F1A")) != null)
            {
                tlv.AddTag("9F1A", value);
            }
            if ((value = handler.GetBytesValue("9A")) != null)
            {
                tlv.AddTag("9A", value);
            }
            #endregion

            return tlv.GetTLV();
        }


        /// <summary>
        /// 冲正使用的55域
        /// </summary>
        private string GetICAutoField55(string _field55)
        {
            //95 9F1E 9F10 9F36 DF31
            string autoField55 = "";
            //TLVDef tlv = new TLVDef();
            //Hashtable hs = new Hashtable();
            //tlv.UnPacket(_field55, ref hs);

            //#region 打包Field55

            //if (hs.ContainsKey("95"))
            //{
            //    string s0 = hs["95"].ToString().Trim();
            //    tlv.Packet("95", s0.Length, s0, ref autoField55);
            //}
            //if (hs.ContainsKey("9F1E"))
            //{
            //    string s0 = hs["9F1E"].ToString().Trim();
            //    tlv.Packet("9F1E", s0.Length, s0, ref autoField55);
            //}
            //if (hs.ContainsKey("9F10"))
            //{
            //    string s0 = hs["9F10"].ToString().Trim();
            //    tlv.Packet("9F10", s0.Length, s0, ref autoField55);
            //}
            //if (hs.ContainsKey("9F36"))
            //{
            //    string s0 = hs["9F36"].ToString().Trim();
            //    tlv.Packet("9F36", s0.Length, s0, ref autoField55);
            //}
            //if (hs.ContainsKey("DF31"))
            //{
            //    string s0 = hs["DF31"].ToString().Trim();
            //    tlv.Packet("DF31", s0.Length, s0, ref autoField55);
            //}
            //#endregion

            return autoField55;
        }

        /// <summary>
        /// 冲正使用的55域
        /// </summary>
        private byte[] GetICAutoField55(byte[] _field55, int fieldLen)
        {
            //95 9F1E 9F10 9F36 DF31
            byte[] field55 = new byte[fieldLen];
            Array.Copy(_field55, field55, fieldLen);
            TLVHandler tlv = new TLVHandler();
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(field55);
            byte[] value = new byte[0];

            #region 打包Field55

            if ((value = handler.GetBytesValue("95")) != null)
            {
                tlv.AddTag("95", value);
            }
            if ((value = handler.GetBytesValue("9F1E")) != null)
            {
                tlv.AddTag("9F1E", value);
            }
            if ((value = handler.GetBytesValue("9F10")) != null)
            {
                tlv.AddTag("9F10", value);
            }
            if ((value = handler.GetBytesValue("9F36")) != null)
            {
                tlv.AddTag("9F36", value);
            }
            if ((value = handler.GetBytesValue("DF31")) != null)
            {
                tlv.AddTag("DF31", value);
            }
            #endregion

            return tlv.GetTLV();
        }
    }
}
