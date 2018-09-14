using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Landi.FrameWorks.HardWare
{
    public class CardIssuer2: HardwareBase<CardIssuer2, CardIssuer2.Status>,Landi.FrameWorks.IManagedHardware
    {
        #region CardIssuer2.dll
        [DllImport("CardIssuer2.dll")]
        protected static extern short Card_Init(string port_name, int bps, int capture);
        [DllImport("CardIssuer2.dll")]
        protected static extern short Card_GetVersion(byte[] version);
        [DllImport("CardIssuer2.dll")]
        protected static extern short Card_GetStatus(out int cardPos, out int boxNum);
        [DllImport("CardIssuer2.dll")]
        protected static extern short Card_Pass(int passPos);
        [DllImport("CardIssuer2.dll")]
        protected static extern short Card_Close();
        [DllImport("CardIssuer2.dll")]
        protected static extern short Card_Eject();
        [DllImport("CardIssuer2.dll")]
        protected static extern short Card_InsertMSCard(byte[] track1, byte[] track2, byte[] track3);
        [DllImport("CardIssuer2.dll")]
        protected static extern short Card_ReadAll(byte[] track1, byte[] track2, byte[] track3);
        [DllImport("CardIssuer2.dll")]
        protected static extern short Card_Capture();
        [DllImport("CardIssuer2.dll")]
        protected static extern short Card_CancelCommand();
        #endregion

        public enum Status
        {
            CARD_SUCC = 0, //正确执行
            CARD_FAIL = 1, //通讯错误
            CARD_ERR_INS = 2, //命令处理错误,卡座返回 'N'
            CARD_WAIT = 3, //等待插卡
            CARD_ERR_PARAM = 4, //错误参数
            CARD_NOT_POWERUP = 5, //卡未上电
            CARD_NORESPONE = 6, //返回的resp长度不和要求
            CARD_UNSUPPORTED = 7, //不支持的IC卡类型
            CARD_ERR = 8,         //命令处理成功但返回码不为9000
            Null = 9,        //卡箱无卡
            CardFew = 10,     //卡箱卡片少，需要加卡
            CardEnough = 11,  //卡箱卡片充足
        }

        public enum InitCapture
        {
            EjectCard = 0,     //弹卡
            CaptureCard = 1,   //吞卡
            None = 2,          //不动作
        }

        public enum CardPos
        {
            CardOnChannel = 0,       //通道有卡
            CardOnTrackReadPos = 1,  //读磁卡位置有卡
            CardOnICReadPos = 2,     //IC卡位置有卡
            CardOnFrontNipPos = 3,   //前端夹卡位置有卡
            CardOnFrontNoNipPos = 4, //前端不夹卡位置有卡
            CardOnErrorPos = 5,      //卡不在标准位置
            CardOnTransmission = 6,  //卡在传动过程中
            CardOnMifReadPos = 7,    //射频卡位置有卡
            CardNoCardOnChannel = 8,      //通道无卡
        }

        public enum BoxCardNum
        {
            Null = 0,        //卡箱无卡
            CardFew = 1,     //卡箱卡片少，需要加卡
            CardEnough = 2,  //卡箱卡片充足
        }

        public enum CardPassPos
        {
            TransOnTrackReadPos = 0,      //将卡传动到读磁卡位置
            TransOnICReadPos = 1,         //将卡片传动到IC卡位置
            TransOnFrontNipPos = 2,       //将卡片传动到前端夹卡位置
            EjectCard = 3,                //弹出卡片
            RecoverCard = 4,              //回收卡片到回收箱
            TransOnMifReadPox = 5,        //将卡片传动到射频卡位置
            EjectCardShake = 6,           //将卡片抖动弹出
        }

        private static InitCapture iniMode;
        public CardIssuer2()
        {
            string modeStr = ReadIniFile("InitMode");
            if (modeStr == "")
            {
                modeStr = "0";
                WriteIniFile("InitMode", modeStr);
            }
            int mode = 0;
            int.TryParse(modeStr, out mode);
            iniMode = (InitCapture)mode;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        //public static Status Open(InitCapture capture)
        //{
        //    if (!IsUse) return Status.CARD_SUCC;
        //    try
        //    {
        //        Status ret = (Status)Card_Init(Port, Bps, (int)capture);
        //        if (ret != Status.CARD_SUCC)
        //        {
        //            AppLog.Write("[CardIssuer2][Open]" + ret.ToString(), AppLog.LogMessageType.Warn);
        //        }
        //        return ret;
        //    }
        //    catch (System.Exception e)
        //    {
        //        AppLog.Write("[CardIssuer2][Open]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
        //        return Status.CARD_FAIL;
        //    }
        //}

        ///// <summary>
        ///// 初始化
        ///// </summary>
        ///// <returns></returns>
        //public static Status Close()
        //{
        //    if (!IsUse) return Status.CARD_SUCC;
        //    try
        //    {
        //        Status ret = (Status)Card_Close();
        //        return ret;
        //    }
        //    catch (System.Exception e)
        //    {
        //        AppLog.Write("[CardIssuer2][Close]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
        //        return Status.CARD_FAIL;
        //    }
        //}

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        public static Status GetVersion(ref string version)
        {
            if (!IsUse) return Status.CARD_SUCC;
            try
            {
                byte[] bVer = new byte[100];
                Status ret = (Status)Card_GetVersion(bVer);
                if (ret != Status.CARD_SUCC)
                {
                    AppLog.Write("[CardIssuer2][GetVersion]" + ret.ToString(), AppLog.LogMessageType.Warn);
                }
                else
                    version = Encoding.Default.GetString(bVer);
                return ret;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CardIssuer2][GetVersion]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.CARD_FAIL;
            }
        }

        /// <summary>
        /// 获取设备状态
        /// </summary>
        /// <param name="cardPos">卡片位置</param>
        /// <param name="boxNum">回收箱卡数量</param>
        /// <returns></returns>
        public static Status GetStatus(ref CardPos cardPos, ref BoxCardNum boxNum)
        {
            if (!IsUse) return Status.CARD_SUCC;
            try
            {
                int iCardPos, iBoxNum;
                Status ret = (Status)Card_GetStatus(out iCardPos, out iBoxNum);
                if (ret != Status.CARD_SUCC)
                {
                    AppLog.Write("[CardIssuer2][GetStatus]" + ret.ToString(), AppLog.LogMessageType.Warn);
                }
                else
                {
                    cardPos = (CardPos)iCardPos;
                    boxNum = (BoxCardNum)iBoxNum;
                }
                return ret;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CardIssuer2][GetStatus]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.CARD_FAIL;
            }
        }

        /// <summary>
        /// 将卡传动到某位置
        /// </summary>
        /// <param name="passType">位置参数</param>
        /// <returns></returns>
        public static Status Pass(CardPassPos passType)
        {
            if (!IsUse) return Status.CARD_SUCC;
            try
            {
                int iPassType = (int)passType;
                Status ret = (Status)Card_Pass(iPassType);
                if (ret != Status.CARD_SUCC)
                {
                    AppLog.Write("[CardIssuer2][Pass]" + ret.ToString(), AppLog.LogMessageType.Warn);
                }
                return ret;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CardIssuer2][Pass]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.CARD_FAIL;
            }
        }

        /// <summary>
        /// 读取此道信息
        /// </summary>
        /// <param name="track1"></param>
        /// <param name="track2"></param>
        /// <param name="track3"></param>
        /// <returns></returns>
        public static Status ReadCardTrack(ref string track1, ref string track2, ref string track3)
        {
            if (!IsUse) return Status.CARD_SUCC;
            try
            {
                CardPos cardPos = CardPos.CardOnErrorPos;
                BoxCardNum boxNum = BoxCardNum.Null;
                Status ret = (Status)GetStatus(ref cardPos, ref boxNum);
                if (ret == Status.CARD_SUCC && cardPos == CardPos.CardOnTrackReadPos)
                {
                    byte[] bTrack1 = new byte[128];
                    byte[] bTrack2 = new byte[128];
                    byte[] bTrack3 = new byte[128];
                    ret = (Status)Card_ReadAll(bTrack1, bTrack2, bTrack3);
                    if (ret == Status.CARD_SUCC)
                    {
                        track1 = Encoding.Default.GetString(bTrack1).Trim();
                        track2 = Encoding.Default.GetString(bTrack2).Trim();
                        track3 = Encoding.Default.GetString(bTrack3).Trim();
                    }
                }
                else
                {
                    ret = Status.CARD_FAIL;
                }
                return ret;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CardIssuer2][ReadCardTrack]Error!", AppLog.LogMessageType.Error, e);
                return Status.CARD_FAIL;
            }
        }

        /// <summary>
        /// 等待插卡并读磁道信息
        /// </summary>
        /// <param name="track1"></param>
        /// <param name="track2"></param>
        /// <param name="track3"></param>
        /// <returns></returns>
        public static Status InsertMSCard(ref string track1, ref string track2, ref string track3)
        {
            if (!IsUse) return Status.CARD_SUCC;
            try
            {
                byte[] bTrack1 = new byte[128];
                byte[] bTrack2 = new byte[128];
                byte[] bTrack3 = new byte[128];
                Status ret = (Status)Card_InsertMSCard(bTrack1, bTrack2, bTrack3);
                if (ret == Status.CARD_SUCC)
                {
                    track1 = Encoding.Default.GetString(bTrack1).Trim();
                    track2 = Encoding.Default.GetString(bTrack2).Trim();
                    track3 = Encoding.Default.GetString(bTrack3).Trim();
                }
                return ret;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CardIssuer2][InsertMSCard]Error!", AppLog.LogMessageType.Error, e);
                return Status.CARD_FAIL;
            }
        }

        #region IManagedHardware
        public object Open()
        {
            if (!IsUse) return Status.CARD_SUCC;
            Status ret = (Status)Card_Init(Port, Bps, (int)iniMode);
            if (ret != Status.CARD_SUCC)
            {
                AppLog.Write("[CardIssuer2][Open]" + ret.ToString(), AppLog.LogMessageType.Warn);
            }
            return ret;
        }

        public object Close()
        {
            if (!IsUse) return Status.CARD_SUCC;
            Status ret = (Status)Card_Close();
            return ret;
        }

        public object CheckStatus()
        {
            BoxCardNum num = BoxCardNum.Null;
            CardPos pos = CardPos.CardOnErrorPos;
            Status ret = GetStatus(ref pos, ref num);
            if (ret == Status.CARD_SUCC)
            {
                if (num == BoxCardNum.Null)
                    ret = Status.Null;
                else if (num == BoxCardNum.CardFew)
                    ret = Status.CardFew;
                else if (num == BoxCardNum.CardEnough)
                    ret = Status.CardEnough;
            }
            return ret;
        }

        public bool MeansError(object status)
        {
            switch (AsStatus(status))
            {
                case Status.CARD_SUCC:
                case Status.CARD_WAIT:
                    return false;
                default:
                    return true;
            }
        }
        #endregion
    }
}
