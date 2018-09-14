using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Landi.FrameWorks.HardWare
{
    public class ScanIS4225 : HardwareBase<ScanIS4225, ScanIS4225.Status>, Landi.FrameWorks.IManagedHardware
    {
        #region Scanbar.dll
        [DllImport("Scanbar.dll")]
        protected static extern short EA_Scan_Openport(string portName);
        [DllImport("Scanbar.dll")]
        protected static extern short EA_Scan_ScanPage(StringBuilder code, long waitTime);
        [DllImport("Scanbar.dll")]
        protected static extern short EA_Scan_GetStatus();
        [DllImport("Scanbar.dll")]
        protected static extern short EA_Scan_Closeport();
        #endregion

        public enum Status
        {
            Success = 0,
            Fail = 1,
            OpenPortFail = 2,
            OutOfTime = 3,
            NotConnect = 4, 
        }

        //public static Status Open()
        //{
        //    if (!IsUse) return Status.Success;
        //    try
        //    {
        //        Status ret = (Status)EA_Scan_Openport(Port);
        //        AppLog.Write("[Scan][Open]" + ret.ToString(), AppLog.LogMessageType.Warn);
        //        return ret;
        //    }
        //    catch (System.Exception e)
        //    {
        //        AppLog.Write("[Scan][Open]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
        //        return Status.Fail;
        //    }
        //}

        //public static Status Close()
        //{
        //    if (!IsUse) return Status.Success;
        //    try
        //    {
        //        Status ret = (Status)EA_Scan_Closeport();
        //        AppLog.Write("[Scan][Close]" + ret.ToString(), AppLog.LogMessageType.Warn);
        //        return ret;
        //    }
        //    catch (System.Exception e)
        //    {
        //        AppLog.Write("[Scan][Close]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
        //        return Status.Fail;
        //    }
        //}

        public static Status GetStatus()
        {
            if (!IsUse) return Status.Success;
            try
            {
                Status ret = (Status)EA_Scan_GetStatus();
                AppLog.Write("[Scan][GetStatus]" + ret.ToString(), AppLog.LogMessageType.Warn);
                return ret;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[Scan][GetStatus]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.Fail;
            }
        }

        /// <summary>
        /// 扫描条码(配合条码API每次的扫描时间为3秒，该函数500毫秒执行结束)
        /// </summary>
        /// <param name="strCode"></param>
        /// <returns></returns>
        public static Status ScanPage(ref string strCode)
        {
            if (!IsUse) return Status.Success;
            try
            {
                StringBuilder code = new StringBuilder();
                Status ret = (Status)EA_Scan_ScanPage(code, 500);
                AppLog.Write("[Scan][ScanPage]" + ret.ToString(), AppLog.LogMessageType.Info);
                return ret;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[Scan][ScanPage]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.Fail;
            }
        }

        #region IManagedHardware
        public object Open()
        {
            if (!IsUse) return Status.Success;
            Status ret = (Status)EA_Scan_Openport(Port);
            if (ret != Status.Success)
                AppLog.Write("[Scan][Open]" + ret.ToString(), AppLog.LogMessageType.Warn);
            return ret;
        }

        public object Close()
        {
            if (!IsUse) return Status.Success;
            Status ret = (Status)EA_Scan_Closeport();
            if (ret != Status.Success)
                AppLog.Write("[Scan][Close]" + ret.ToString(), AppLog.LogMessageType.Warn);
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
                case Status.Success:
                    return false;
                default:
                    return true;
            }
        }
        #endregion
    }
}
