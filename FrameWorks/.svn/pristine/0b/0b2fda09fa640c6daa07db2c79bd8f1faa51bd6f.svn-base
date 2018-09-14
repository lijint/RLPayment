using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace Landi.FrameWorks.HardWare
{
    public class Esam : HardwareBase<Esam, Esam.Status>, Landi.FrameWorks.IManagedHardware
    {
        #region Esam.dll
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_Open(string port_name, int bps/*= 57600*/);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_Close();
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_GetVersion(string pVersion);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_GetStatus();
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_DownloadKey(byte[] pMainKey);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_DownloadSectKey(int nIndex, byte[] pSectKey, int nKeyLen);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_SetMaxPIN(int nMaxSize);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_SetKeyLen(int nKeyLength);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_ActiveKey(int nMasterKeyNO, int nWorkKeyNO);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_CalcMAC(byte[] pMacKey, byte[] pData, int nDataLen, byte[] MAC);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_GetPIN(byte[] pPinKey, string pszCardNO, int nPwdLen, ref byte pKey, byte[] PIN);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_Encrypt(byte[] pData, int nDataLen, byte[] pEncyptedData);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_Decrypt(byte[] pData, int nDataLen, byte[] pDecyptedData);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_GetWorkMode();
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_SetWorkMode(int nNewValue);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_UpdateWorkKey(int nWorkKeyNO, byte[] pEncryptKeyData);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_CancelCommand();

        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_EncryptData(byte[] pSrcData, int nLen, byte[] pDestData);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_DecryptData(byte[] pSrcData, int nLen, byte[] pDestData);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_GenerateMAC(byte[] pSrcData, int nLen, byte[] pMAC);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_ReadPINSecureANSI(string pszCardNO);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_GetNextKey();
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_GetPINBLOCK(byte[] PIN_BLOCK);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_SetMasterkeyNO(int nMasterKeyNO);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_UserEncrypt(int nKeyID, byte[] pData, int nDataLen, byte[] pEncyptedData);
        [DllImport("ESAM.DLL")]
        protected static extern short ESAM_UserDecrypt(int nKeyID, byte[] pData, int nDataLen, byte[] pDecyptedData);
        #endregion

        /// <summary>
        /// 普通函数返回码
        /// </summary>
        public enum Status
        {
            ESAM_FAIL = 0xFF,                // 通讯错误
            ESAM_NO_RESPONSE = 0xFE,       // 没有接收到数据
            ESAM_BUSY = 0xFC,              // 密码键盘正执行其他指令
            ESAM_SIM_FAIL = 0xA2,          // 与SIM卡通讯失败
            ESAM_SIM_POWEROFF = 0xE3,      // SIM卡未上电
            ESAM_SIM_ERR_INS = 0xE7,       // SIM卡无效指令
            ESAM_SUCC = 0x00,              // 成功
            ESAM_INVALID_INSTR = 0x01,     // 无效指令
            ESAM_INVALID_PARAM = 0x02,     // 指令参数错误
            ESAM_ERROR_VERIFY = 0x03,      // 通讯等待超时或校验错误
            ESAM_TIME_OUT = 0x04,          // 等待按键超时
            ESAM_KEY_PRESSED = 0x05,       // 有效按键（如果是密码输入不返回键值，如果是普通按键输入返回键值）
            ESAM_WAIT_INPUT = 0x10,        // 开始等待按键
            ESAM_ENTER = 0x0D,             // 确认键按下
            ESAM_CLEAR = 0x0E,             // 清除按键（密码键盘）
            ESAM_CANCEL = 0x0F,            // 退出密码输入或退出数据输入
        }
        
        /// <summary>
        /// 以下为函数 ESAM_GetPushKey() 独有的返回码
        /// </summary>
        private enum GetPushKeyRet
        {
            ESAM_DOT = 0x2E,        // '.'键按下
            ESAM_0 = 0x30,          // '0'键按下
            ESAM_9 = 0x39,          // '9'键按下
            ESAM_DOUBLE_0 = 0x3A,   // '00'键按下
            ESAM_PGUP = 0x21,       // '上翻'键按下
            ESAM_PGDN = 0x22,       // '下翻'键按下
        }

        /// <summary>
        /// 以下为设置密码键盘工作模式
        /// </summary>
        public enum WorkMode
        {
            Default = 0, //默认模式
            Encrypt = 1, //加密模式
        }

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        //public static Status Open()
        //{
        //    if (!IsUse) return Status.ESAM_SUCC;
        //    Status ret = (Status)ESAM_Open(Port, Bps);
        //    if (ret != Status.ESAM_SUCC)
        //    {
        //        Log.Warn("[Esam][Open]" + ret.ToString());
        //    }
        //    return ret;
        //}

        /// <summary>
        /// 读取状态
        /// </summary>
        /// <returns></returns>
        public static Status GetStatus()
        {
            if (!IsUse) return Status.ESAM_SUCC;
            try
            {
                Status ret = (Status)ESAM_GetStatus();
                if (ret != Status.ESAM_SUCC && ret != Status.ESAM_BUSY && ret != Status.ESAM_NO_RESPONSE)
                {
                    Log.Warn("[Esam][GetStatus]" + ret);
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[Esam][GetStatus]Error!\n" + e.ToString());
                return Status.ESAM_FAIL;
            }
        }

        /// <summary>
        /// 密码键盘ESAM芯片字节数
        /// </summary>
        /// <param name="nKeyLength"></param>
        /// <returns></returns>
        public static Status SetKeyLen(int nKeyLength)
        {
            if (!IsUse) return Status.ESAM_SUCC;
            try
            {
                Status ret = (Status)ESAM_SetKeyLen(nKeyLength);
                if (ret != Status.ESAM_SUCC)
                {
                    Log.Warn("[Esam][SetKeyLen]" + ret.ToString());
                }
                return ret;
            }
            catch (Exception e)
            {
                Log.Error("[Esam][SetKeyLen]Error!\n" + e.ToString());
                return Status.ESAM_FAIL;
            }
        }

        /// <summary>
        /// 设置使用第几组主密钥
        /// </summary>
        /// <param name="nMasterKeyNO"></param>
        /// <returns></returns>
        public static Status SetMasterkeyNo(int nMasterKeyNo)
        {
            if (!IsUse) return Status.ESAM_SUCC;
            try
            {
                Status ret = (Status)ESAM_ActiveKey(nMasterKeyNo,0);
                if (ret != Status.ESAM_SUCC)
                {
                    Log.Warn("[Esam][SetMasterkeyNo]" + ret.ToString());
                }
                return ret;
            }
            catch (Exception err)
            {
                Log.Error("[Esam][SetMasterkeyNo]Error!\n" + err.ToString());
                return Status.ESAM_FAIL;
            }
        }

        public static Status UserDecrypt(int nKeyID, byte[] pData, int nDataLen, byte[] pDecryptedData)
        {
            if (!IsUse) return Status.ESAM_SUCC;
            try
            {
                System.Threading.Thread.Sleep(200);
                Status ret  = Status.ESAM_SUCC;
                ret = (Status)ESAM_UserDecrypt(nKeyID, pData, nDataLen, pDecryptedData);
                //ret = (Status)ESAM_Decrypt(pData, nDataLen, pDecryptedData);
                if (ret != Status.ESAM_SUCC)
                {
                    AppLog.Write("[Esam][UserDecrypt]" + ret.ToString(), AppLog.LogMessageType.Warn);
                }
                return ret;
            }
            catch (Exception err)
            {
                AppLog.Write("[Esam][UserDecrypt]Error!\n" + err.ToString(), AppLog.LogMessageType.Error);
                return Status.ESAM_FAIL;
            }
        }

        public static Status UserEncrypt(int nKeyID, byte[] pData, int nDataLen, byte[] pDecryptedData)
        {
            if (!IsUse) return Status.ESAM_SUCC;
            try
            {
                System.Threading.Thread.Sleep(200);
                Status ret = (Status)ESAM_UserEncrypt(nKeyID, pData, nDataLen, pDecryptedData);
                if (ret != Status.ESAM_SUCC)
                {
                    AppLog.Write("[Esam][UserEncrypt]" + ret.ToString(), AppLog.LogMessageType.Warn);
                }
                return ret;
            }
            catch (Exception err)
            {
                AppLog.Write("[Esam][UserEncrypt]Error!\n" + err.ToString(), AppLog.LogMessageType.Error);
                return Status.ESAM_FAIL;
            }
        }

        /// <summary>
        /// 计算mac
        /// </summary>
        /// <param name="pMacKey"></param>
        /// <param name="pData"></param>
        /// <param name="nDataLen"></param>
        /// <param name="pMac"></param>
        /// <returns></returns>
        public static Status CalcMac(byte[] pMacKey, byte[] pData, int nDataLen, byte[] pMac)
        {
            if (!IsUse) return Status.ESAM_SUCC;
            try
            {
                Status ret = (Status)ESAM_CalcMAC(pMacKey, pData, nDataLen, pMac);
                if (ret != Status.ESAM_SUCC)
                {
                    Log.Warn("[Esam][CalcMac]" + ret.ToString());
                }
                return ret;
            }
            catch (Exception err)
            {
                Log.Error("[Esam][CalcMac]Error!\n" + err.ToString());
                return Status.ESAM_FAIL;
            }
        }

        /// <summary>
        /// pin输入函数
        /// </summary>
        /// <param name="cardNum"></param>
        /// <param name="len"></param>
        /// <param name="key"></param>
        /// <param name="pin"></param>
        /// <returns></returns>
        public static Status GetX98Pin(byte[] pinKey, string cardNum, int pwdLen, ref byte key, ref string pin)
        {
            if (!IsUse)
                return Status.ESAM_SUCC;
            try
            {
                if (cardNum.Trim() == String.Empty || pwdLen == 0) return Status.ESAM_CANCEL;
                byte[] bPin = new byte[100];
                string tmpCardNum = cardNum;
                if (cardNum.Length < 16)
                {
                    int cardNumLen = cardNum.Length;
                    for (int iPer = 1; iPer <= 16 - cardNumLen; iPer++)
                    {
                        tmpCardNum = tmpCardNum.PadLeft(16, '0');
                    }
                }
                else
                {
                    tmpCardNum = cardNum.Substring(cardNum.Length - 16, 16);
                }
                Status ret = (Status)ESAM_GetPIN(pinKey, tmpCardNum, pwdLen, ref key, bPin);
                if (ret == Status.ESAM_SUCC)
                {
                    pin = Utility.bcd2str(bPin, 8);
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[Esam][GetX98Pin]Error!\n" + e.ToString());
                return Status.ESAM_FAIL;
            }
        }


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="nDataLen"></param>
        /// <param name="pEncyptedData"></param>
        /// <returns></returns>
        public static Status Encrypt(byte[] pData, int nDataLen, byte[] pEncyptedData)
        {
            if (!IsUse) return Status.ESAM_SUCC;
            try
            {
                System.Threading.Thread.Sleep(200);
                Status ret = (Status)ESAM_Encrypt(pData, nDataLen, pEncyptedData);
                if (ret != Status.ESAM_SUCC) 
                {
                    Log.Warn("[Esam][Encrypt]" + ret.ToString());
                }
                return ret;
            }
            catch (Exception err)
            {
                Log.Error("[Esam][Encrypt]Error!\n" + err.ToString());
                return Status.ESAM_FAIL;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="nDataLen"></param>
        /// <param name="pDecryptedData"></param>
        /// <returns></returns>
        public static Status Decrypt(byte[] pData, int nDataLen, byte[] pDecryptedData)
        {
            if (!IsUse) return Status.ESAM_SUCC;
            try
            {
                Status ret = (Status)ESAM_Decrypt(pData, nDataLen, pDecryptedData);
                if (ret != Status.ESAM_SUCC)
                {
                    Log.Warn("[Esam][Decrypt]" + ret.ToString());
                }
                return ret;
            }
            catch (Exception err)
            {
                Log.Error("[Esam][Decrypt]Error!\n" + err.ToString());
                return Status.ESAM_FAIL;
            }
        }

        /// <summary>
        /// 设置工作模式
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Status SetWorkmode(WorkMode mode)
        {
            if (!IsUse) return Status.ESAM_SUCC;
            try
            {
                Status ret = (Status)ESAM_SetWorkMode((int)mode);
                if (ret != Status.ESAM_SUCC)
                {
                    Log.Warn("[Esam][SetWorkmode]" + ret.ToString());
                }
                return ret;
            }
            catch (Exception e)
            {
                Log.Error("[Esam][SetWorkmode]Error!\n" + e.ToString());
                return Status.ESAM_FAIL;
            }
        }

        /// <summary>
        /// 下载主密钥
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="pSectKey"></param>
        /// <param name="nKeyLen"></param>
        /// <returns></returns>
        public static Status DownloadKey(int nIndex, byte[] pSectKey, int nKeyLen)
        {
            if (!IsUse) return Status.ESAM_SUCC;
            try
            {
                Status ret = (Status)ESAM_DownloadSectKey(nIndex, pSectKey, nKeyLen);
                if (ret != Status.ESAM_SUCC)
                {
                    Log.Warn("[Esam][DownloadKey]" + ret.ToString());
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[Esam][DownloadKey]Error!\n" + e.ToString());
                return Status.ESAM_FAIL;
            }
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        /// <returns></returns>
        //public static Status Close()
        //{
        //    if (!IsUse) return Status.ESAM_SUCC;
        //    try
        //    {
        //        Status ret = (Status)ESAM_Close();
        //        if (ret != Status.ESAM_SUCC)
        //        {
        //            Log.Warn("[Esam][Close]" + ret.ToString());
        //        }
        //        return ret;
        //    }
        //    catch (Exception err)
        //    {
        //        Log.Error("[Esam][Close]Error!\n" + err.ToString());
        //        return Status.ESAM_FAIL;
        //    }
        //}

        public static Status GetVersion(ref string version)
        {
            if (!IsUse) return Status.ESAM_SUCC;
            try
            {
                Status ret = (Status)ESAM_GetVersion(version);
                Log.Warn("[Esam][GetVersion]Ret=" + ret + "  version=" + version);
                return ret;
            }
            catch (Exception err)
            {
                Log.Error("[Esam][GetVersion]Error!\n" + err.ToString());
                return Status.ESAM_FAIL;
            }
        }

        public static Status CancelCommand()
        {
            if (!IsUse) return Status.ESAM_SUCC;
            try
            {
                Status ret = (Status)ESAM_CancelCommand();
                Log.Warn("[Esam][CancelCommand] Ret=" + ret);
                return ret;
            }
            catch (Exception err)
            {
                Log.Error("[Esam][CancelCommand]Error!\n" + err.ToString());
                return Status.ESAM_FAIL;
            }
        }

        #region IManagedHardware
        public object Open()
        {
            if (!IsUse) return Status.ESAM_SUCC;
            Status ret = (Status)ESAM_Open(Port, Bps);
            if (ret != Status.ESAM_SUCC)
            {
                Log.Warn("[Esam][Open]" + ret.ToString());
            }
            return ret;
        }

        public object Close()
        {
            if (!IsUse) return Status.ESAM_SUCC;
            Status ret = (Status)ESAM_Close();
            if (ret != Status.ESAM_SUCC)
            {
                Log.Warn("[Esam][Close]" + ret.ToString());
            }
            return ret;
        }

        public object CheckStatus()
        {
            return GetStatus();
        }

        public bool MeansError(object status)
        {
            switch (AsStatus(status))
            {
                case Status.ESAM_SUCC:
                case Status.ESAM_BUSY:
                case Status.ESAM_NO_RESPONSE:
                    return false;
                default:
                    return true;
            }
        }
        #endregion
    }
}