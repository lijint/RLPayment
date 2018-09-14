using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;

namespace Landi.FrameWorks.HardWare
{
    public class CashCode : HardwareBase<CashCode, CashCode.Status>, Landi.FrameWorks.IManagedHardware
    {
        #region CashCode.dll
        [DllImport("CashCode.dll")]
        protected static extern short EA_CASHCODE_Reset();
        [DllImport("CashCode.dll")]
        protected static extern short EA_CASHCODE_Getversion(ref string version);
        [DllImport("CashCode.dll")]
        protected static extern short EA_CASHCODE_Getbilltype();
        [DllImport("CashCode.dll")]
        protected static extern short EA_CASHCODE_EnableBill(byte type, byte escrow);
        [DllImport("CashCode.dll")]
        protected static extern short EA_CASHCODE_Poll();
        [DllImport("CashCode.dll")]
        protected static extern short EA_CASHCODE_ReturnBill();
        [DllImport("CashCode.dll")]
        protected static extern short EA_CASHCODE_StackBill();
        [DllImport("CashCode.dll")]
        protected static extern short EA_CASHCODE_HoldBill();
        [DllImport("CashCode.dll")]
        protected static extern short EA_CASHCODE_SetComm(string portName, int bps);
        [DllImport("CashCode.dll")]
        protected static extern short EA_CASHCODE_ACK();
        [DllImport("CashCode.dll")]
        protected static extern short EA_CASHCODE_Close();
        [DllImport("CashCode.dll")]
        protected static extern long EA_CASHCODE_GetStatus();
        [DllImport("CashCode.dll")]
        protected static extern string EA_CASHCODE_GetErrorMessage();
        [DllImport("CashCode.dll")]
        protected static extern byte EA_CASHCODE_GetErrorCode();
        #endregion

        /// <summary>
        /// 函数返回值
        /// </summary>
        public enum Status
        {
            Success = 1,
            Failed = 0,
            ParamError = -1,
            OpenFailed = -2,
        }

        public enum PollRet : int
        {
            One = 1,
            Two = 2,
            Five = 5,
            Ten = 10,
            Twenty = 20,
            Fifty = 50,
            Hundred = 100,
            InspectionFail = 0, //检测错误
            HappenError = -1, //发生错误
            IgnoreRet = -3, //忽略值
        }

        public const int YY = 0x01;
        public const int EY = 0x02;
        public const int WY = 0x04;
        public const int SY = 0x08;
        public const int ES = 0x10;
        public const int WS = 0x20;
        public const int YB = 0x40;

        public static bool PollStopOrder = false; //接收纸币停止命令标志位

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        //public static Status Open()
        //{
        //    if (!IsUse) return Status.Success;
        //    try
        //    {
        //        EA_CASHCODE_Close();
        //        Status cRet = (Status)EA_CASHCODE_SetComm(Port, Bps);
        //        if (cRet != Status.Success)
        //        {
        //            AppLog.Write("[CashCode][Open]" + cRet.ToString(), AppLog.LogMessageType.Error);
        //        }
        //        return cRet;
        //    }
        //    catch (System.Exception e)
        //    {
        //        AppLog.Write("[CashCode][Open]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
        //        return Status.Failed;
        //    }
        //}

        /// <summary>
        /// 重设识别器
        /// </summary>
        /// <returns></returns>
        public static Status CashReset()
        {
            if (!IsUse) return Status.Success;
            try
            {
                Status cRet = (Status)EA_CASHCODE_Reset();
                if (cRet != Status.Success)
                {
                    AppLog.Write("[CashReset][ResetCash]cRet = " + cRet.ToString(), AppLog.LogMessageType.Error);
                }
                return cRet;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CashReset][ResetCash]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.Failed;
            }
        }

        /// <summary>
        /// 获取识别器状态值
        /// </summary>
        /// <returns></returns>
        public static Status GetStatus()
        {
            if (!IsUse) return Status.Success;
            try
            {
                Status cRet = (Status)EA_CASHCODE_GetStatus();
                if (cRet == Status.OpenFailed || cRet == Status.ParamError || cRet == Status.Failed)
                {
                    AppLog.Write("[CashCode][GetStatus]cRet = " + cRet.ToString(), AppLog.LogMessageType.Warn);
                }
                return cRet;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CashCode][GetStatus]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.Failed;
            }
        }

        public static Status SetEnableBillType(byte ctype)
        {
            if (!IsUse) return Status.Success;
            try
            {
                byte escrow = YY | EY | WY | SY | ES | WS | YB;
                Status cRet = (Status)EA_CASHCODE_EnableBill(ctype, escrow);
                if (cRet != Status.Success)
                {
                    cRet = (Status)EA_CASHCODE_Reset();
                    if (cRet != Status.Success)
                    {
                        return Status.Failed;
                    }
                    if ((Status)EA_CASHCODE_EnableBill(ctype, escrow) != Status.Success)
                    {
                        return Status.Failed;
                    }
                }
                return Status.Success;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CashCode][SetEnableBillType]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.Failed;
            }
        }
        
        /// <summary>
        /// 无限循环等待接收纸币
        /// </summary>
        /// <param name="cType">允许接收纸币范围指令</param>
        /// <param name="retValue">返回的结果</param>
        /// <returns></returns>
        public static Status CashPoll(ref PollRet retValue)
        {
            if (!IsUse) return Status.Success;
            try
            {
                short iRet = EA_CASHCODE_Poll();
                EA_CASHCODE_ACK();
                switch (iRet)
                {
                    case 1:
                        retValue = PollRet.One;
                        return Status.Success;
                    case 2:
                        retValue = PollRet.Two;
                        return Status.Success;
                    case 5:
                        retValue = PollRet.Five;
                        return Status.Success;
                    case 10:
                        retValue = PollRet.Ten;
                        return Status.Success;
                    case 20:
                        retValue = PollRet.Twenty;
                        return Status.Success;
                    case 50:
                        retValue = PollRet.Fifty;
                        return Status.Success;
                    case 100:
                        retValue = PollRet.Hundred;
                        return Status.Success;
                    case 0:
                        retValue = PollRet.InspectionFail;
                        return Status.Failed;
                    case -1:
                        retValue = PollRet.HappenError;
                        return Status.Failed;
                    case -3:
                        retValue = PollRet.IgnoreRet;
                        return Status.Failed;
                    default:
                        retValue = PollRet.InspectionFail;
                        return Status.Failed;
                }
            }
            catch (System.Exception e)
            {
            	AppLog.Write("[CashReset][CashPoll]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.Failed;
            }
        }

        /// <summary>
        /// 关闭纸币识别，使用完识别器后必须关闭
        /// </summary>
        public static void ClosePoll()
        {
            try
            {
                Status cRet = (Status)EA_CASHCODE_EnableBill(0, 0);
                if (cRet != Status.Success)
                {
                    AppLog.Write("[CashCode][StopPoll]Failed!", AppLog.LogMessageType.Warn);
                }
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CashCode][StopPoll]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
            }
        }

        /// <summary>
        /// 关闭识别器
        /// </summary>
        //public static void Close()
        //{
        //    try
        //    {
        //        EA_CASHCODE_Close();
        //    }
        //    catch (System.Exception e)
        //    {
        //        AppLog.Write("[CashCode][Close]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
        //    }
        //}

        /// <summary>
        /// 获取错误码和错误信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public static void GetErrorInfo(ref string code, ref string message)
        {
            try
            {
                byte bCode = EA_CASHCODE_GetErrorCode();
                message = EA_CASHCODE_GetErrorMessage();
                code = Convert.ToString(bCode);
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CashCode][GetErrorInfo]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
            }
        }

        /// <summary>
        /// 进钞
        /// </summary>
        /// <returns></returns>
        public static Status StackBill()
        {
            if (!IsUse) return Status.Success;
            try
            {
                Status cRet = (Status)EA_CASHCODE_StackBill();
                if (cRet != Status.Success)
                {
                    AppLog.Write("[CashCode][StackBill]Failed!", AppLog.LogMessageType.Warn);
                }
                return cRet;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CashCode][StackBill]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.Failed;
            }
        }

        /// <summary>
        /// 吐钞
        /// </summary>
        /// <returns></returns>
        public static Status ReturnBill()
        {
            if (!IsUse) return Status.Success;
            try
            {
                Status cRet = (Status)EA_CASHCODE_ReturnBill();
                if (cRet != Status.Success)
                {
                    AppLog.Write("[CashCode][ReturnBill]Failed!", AppLog.LogMessageType.Warn);
                }
                return cRet;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CashCode][ReturnBill]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.Failed;
            }
        }

        #region IManagedHardware
        public object Open()
        {
            if (!IsUse) return Status.Success;
            Status cRet = (Status)EA_CASHCODE_SetComm(Port, Bps);
            if (cRet != Status.Success)
            {
                AppLog.Write("[CashCode][Open]" + cRet.ToString(), AppLog.LogMessageType.Error);
            }
            return cRet;
        }

        public object Close()
        {
            return (Status)EA_CASHCODE_Close();
        }

        public object CheckStatus()
        {
            return GetStatus();
        }

        public bool MeansError(object status)
        {
            switch (AsStatus(status))
            {
                case Status.Success:
                    return false;
                default:
                    return true;
            }
        }
        #endregion
    }
}