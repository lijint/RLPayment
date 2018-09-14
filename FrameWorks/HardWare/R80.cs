using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Landi.FrameWorks.HardWare
{
    public class R80 : HardwareBase<R80, R80.Status>,IManagedHardware
    {
        [DllImport("R80.dll")]
        protected static extern short EA_mifare_sOpenport(string pcPort, int uiBps);
        [DllImport("R80.dll")]
        protected static extern short EA_mifare_sCloseport();
        [DllImport("R80.dll")]
        protected static extern short EA_mifare_sICActive(int iDelaytime, byte[] pucType, byte[] pucUIDLen, 
								   byte[] pucCardUID, byte[] pucATRLen, 
								   byte[] pucATRData);
        [DllImport("R80.dll")]
        protected static extern short EA_mifare_sICHalt(int iDelaytime);

        [DllImport("R80.dll")]
        protected static extern short EA_mifare_sICPowerup(int iDelaytime, byte[] pucPTL,
									byte[] pucATRLen, byte[] pucATRData);

        [DllImport("R80.dll")]
        protected static extern short EA_Alarm(int nTime);

        [DllImport("R80.dll")]
        protected static extern long GetCommHandle();

        public enum Status
        {
            EM_MIFARE_SUCC = 0,	// 通讯成功
            EM_MIFARE_FAIL = 1,// 通讯失败
            EM_MIFARE_ERR = 2,	// 通讯错误或参数错误	
        }

        public enum ActivateResult
        {
            ET_SETSUCCESS = 0X00,  // 激活成功
            ET_NOSUPPORTMIF = 0x3001,  // 不支持非接触用户卡
            ET_MIFACTIVEFAIL = 0x3005, // 非接触用户卡激活失败
            ET_MIFOVERTIME = 0x3006, // 等待卡进入感应区超时
            ET_SETHALTFAIL = 0x3008, // 设置卡halt状态失败
            ET_MIFCOLLERR = 0x3009, // 有多张卡在感应区
            ET_NORE = 0x3007,  // 操作非接触用户卡数据无回应========
            ET_LEFTOVERTIME = 0x3002, //等待卡拿离感应区超时============
        }

        public static Status PowerUp(int iDelaytime, string cardno)
        {
            if (!IsUse) return Status.EM_MIFARE_SUCC;
            byte[] ucCardUID = new byte[128];//卡序列号
            byte[] ucATRLen = new byte[2];//ATR数据长度
            byte[] ucATRDataBuf = new byte[512];//卡片复位应答协议和历史字符（激活成功才返回）
            byte[] apo = new byte[512];
            byte[] msg = new byte[128];

            Status nRet = (Status)EA_mifare_sICPowerup(10, ucCardUID, ucATRLen, ucATRDataBuf);
            return nRet;
        }

        public static ActivateResult ICActive(int iDelaytime, ref string cardUID)
        {
            if (!IsUse) return ActivateResult.ET_SETSUCCESS;
            byte[] ucType = new byte[2];//卡类型
            byte[] ucDLen = new byte[2];//卡序列号长度
            byte[] ucCardUID = new byte[128];//卡序列号
            byte[] ucATRLen = new byte[2];//ATR数据长度
            byte[] ucATRDataBuf = new byte[512];//卡片复位应答协议和历史字符（激活成功才返回）
            byte[] apo = new byte[512];
            byte[] msg = new byte[128];

            ActivateResult nRet = (ActivateResult)EA_mifare_sICActive(iDelaytime, ucType, ucDLen, ucCardUID, ucATRLen, ucATRDataBuf);
            if (nRet == ActivateResult.ET_SETSUCCESS)
            {
                EA_Alarm(100);
                int len = ucDLen[0] + 16 * ucDLen[1];
                cardUID = Utility.bcd2str(ucCardUID, len);
            }
            return nRet;
        }

        /// <summary>
        ///获取句炳
        /// </summary>
        /// <returns></returns>
        public static long GetHandle()
        {
            if (!IsUse)
            {
                return 0;
            }

            try
            {
                long s = GetCommHandle();
                return s;

            }
            catch (Exception err)
            {
                Log.Error("GetHandle Error");
                Log.Error("****Err.Description =" + err.Message);
            }
            return 0;
        }

        #region IManagedHardware
        public object Open()
        {
            if (!IsUse) return Status.EM_MIFARE_SUCC;
            Status ret = (Status)EA_mifare_sOpenport(Port, Bps);
            if (ret != Status.EM_MIFARE_SUCC)
            {
                AppLog.Write("[R80][Open]" + ret.ToString(), AppLog.LogMessageType.Warn);
            }
            return ret;
        }

        public object Close()
        {
            if (!IsUse) return Status.EM_MIFARE_SUCC;
            Status ret = (Status)EA_mifare_sCloseport();
            if (ret != Status.EM_MIFARE_SUCC)
            {
                AppLog.Write("[R80][Close]" + ret.ToString(), AppLog.LogMessageType.Warn);
            }
            return ret;
        }

        public object CheckStatus()
        {
            return Status.EM_MIFARE_SUCC;
        }

        public bool MeansError(object status)
        {
            switch (AsStatus(status))
            {
                case Status.EM_MIFARE_SUCC:
                    return false;
                default:
                    return true;
            }
        }
        #endregion
    }
}
