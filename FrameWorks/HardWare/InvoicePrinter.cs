using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

namespace Landi.FrameWorks.HardWare
{
    public class InvoicePrinter : HardwareBase<InvoicePrinter, InvoicePrinter.Status>, Landi.FrameWorks.IManagedHardware
    {
        #region EpsonPrinter.dll
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonOpenPrinter(string port_name, int bps);
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonCPrint(string szPrintData);
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonEPrint(string szPrintData);
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonPrinterBeep();
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonSetLineWidth(int nWidth);
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonUnderLine(int nEnable);
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonRedPrint();
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonDoubleWidth();
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonCancelDoubleWidth();
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonCut();
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonSemiCut();
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonFeedLine(int nLineNum);
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonBackLine(int nLineNum);
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonGetPrinterStatus();
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonBlackMark();
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonCloseCom();
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonGetSensorStatus();
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonSetBlackMark(BlackPosition position,int len);
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonSetBlackMark(int iInn);
        [DllImport("EpsonPrinter.dll")]
        protected static extern short EpsonSetRowDistance(int iInDistance);
        #endregion

        public enum Status
        { 
            SUCC = 0,           //成功
            FAIL = 1,           //失败
            ERR = 2,            //错误
            OUT_OF_PAPER = 3,    //缺纸    
            PAPER_FEW = 4      //纸量少
        }

        public enum BlackPosition
        {
            Print = 1,          //相对于打印位置
            CUT = 2             //相对于切纸位置
        }

        //public static Status Open()
        //{
        //    if (!IsUse)
        //    {
        //        return Status.SUCC;
        //    }

        //    try
        //    {
        //        Status ret = (Status)EpsonOpenPrinter(Port, Bps);
        //        if (ret != Status.SUCC)
        //        {
        //            AppLog.Write("[InvoicePrinter][Open]" + ret.ToString(), AppLog.LogMessageType.Warn);
        //        }
        //        return ret;
        //    }
        //    catch (System.Exception e)
        //    {
        //        AppLog.Write("[InvoicePrinter][Open]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
        //        return Status.FAIL;
        //    }
        //}

        //public static Status Close()
        //{
        //    if (!IsUse)
        //    {
        //        return Status.SUCC;
        //    }

        //    try
        //    {
        //        Status ret = (Status)EpsonCloseCom();
        //        if (ret != Status.SUCC)
        //        {
        //            AppLog.Write("[InvoicePrinter][Close]" + ret.ToString(), AppLog.LogMessageType.Warn);
        //        }
        //        return ret;
        //    }
        //    catch (System.Exception e)
        //    {
        //        AppLog.Write("[InvoicePrinter][Close]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
        //        return Status.FAIL;
        //    }
        //}

        public static Status PrintString(ArrayList printlist)
        {
            if (!IsUse)
            {
                if (printlist != null)
                {
                    for (int i = 0; i < printlist.Count; i++)
                        Log.Debug(printlist[i]);
                }
                return Status.SUCC;
            }

            try
            {
                Status nRet = (Status)EpsonGetPrinterStatus();
                if (nRet == Status.SUCC)
                {
                    foreach (string info in printlist)
                    {
                        AppLog.Write("Invoice PrintString  info =" + info, AppLog.LogMessageType.Info);
                        EpsonCPrint(info);
                        System.Threading.Thread.Sleep(200);
                    }
                }
                else
                {
                    AppLog.Write("[InvoicePrinter][PrintString]" + nRet.ToString(), AppLog.LogMessageType.Warn);
                }


                return nRet;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[InvoicePrinter][PrintString]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.FAIL;
            }

        }

        public static Status CutPaper()
        { 
            if (!IsUse)
            {
                return Status.SUCC;
            }

            try
            {
                Status nRet = (Status)EpsonCut();
                if (nRet != Status.SUCC)
                {
                    AppLog.Write("[InvoicePrinter][CutPage]" + nRet.ToString(), AppLog.LogMessageType.Warn);
                }
                return nRet;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[InvoicePrinter][CutPage]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.FAIL;
            }
        }

        public static Status FeedLine(int linenum)
        {
            if (!IsUse)
            {
                return Status.SUCC;
            }

            try
            {
                Status nRet = (Status)EpsonFeedLine(linenum);
                if (nRet != Status.SUCC)
                {
                    AppLog.Write("[InvoicePrinter][FeedLine]" + nRet.ToString(), AppLog.LogMessageType.Warn);
                }
                return nRet;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[InvoicePrinter][FeedLine]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.FAIL;
            }
        }

        public static Status BackLine(int linenum)
        {
            if (!IsUse)
            {
                return Status.SUCC;
            }

            try
            {
                Status nRet = Status.FAIL;
                //一次性退纸太多，容易造成卡纸
                for (int i=0;i<linenum;i++)
                {
                    nRet = (Status)EpsonBackLine(1);
                    if (nRet!=Status.SUCC)
                    {
                        AppLog.Write("[InvoicePrinter][BackLine][" + i.ToString("00#") + "]" + nRet.ToString(), AppLog.LogMessageType.Warn);
                    }
                }

                return Status.SUCC;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[InvoicePrinter][BackLine]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.FAIL;
            }
        }

        public static Status SetBackPosition(BlackPosition pos,int len)
        {
            if (!IsUse)
            {
                return Status.SUCC;
            }

            try
            {
                Status nRet = (Status)EpsonSetBlackMark(pos,len);
                if (nRet != Status.SUCC)
                {
                    AppLog.Write("[InvoicePrinter][SetBackPosition]" + nRet.ToString(), AppLog.LogMessageType.Warn);
                }
                return nRet;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[InvoicePrinter][SetBackPosition]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.FAIL;
            } 
        }

        public static Status FindBlack()
        { 
            if (!IsUse)
            {
                return Status.SUCC;
            }
            
            try
            {
                Status nRet = (Status)EpsonBlackMark();
                if (nRet!=Status.SUCC)
                {
                    AppLog.Write("[InvoicePrinter][FindBlack]" + nRet.ToString(), AppLog.LogMessageType.Warn);
                }
                return nRet;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[InvoicePrinter][FindBlack]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.FAIL;
            } 
        }

        /// <summary>
        /// 获取纸量传感器状态
        /// </summary>
        /// <returns></returns>
        public static Status GetStatus()
        {
            if (!IsUse)
            {
                return Status.SUCC;
            }

            try
            {
                Status nRet = (Status)EpsonGetSensorStatus();
                if (nRet != Status.SUCC)
                {
                    AppLog.Write("[InvoicePrinter][GetStatus]" + nRet.ToString(), AppLog.LogMessageType.Warn);
                }
                return nRet;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[InvoicePrinter][GetStatus]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.FAIL;
            } 
        }

        public static Status SetCutBlackMark(int iInn)
        {
            if (!IsUse)
            {
                return Status.SUCC;
            }

            try
            {
                //AppLog.Write("[InvoicePrinter][GetSensorStatus]1!\n", AppLog.LogMessageType.Error);
                EpsonSetBlackMark(iInn);
                //AppLog.Write("[InvoicePrinter][GetSensorStatus]2!\n", AppLog.LogMessageType.Error);
                return Status.SUCC;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[InvoicePrinter][SetCutBlackMark]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.FAIL;
            }
        }

        public static Status SetRowDistance(int iInn)
        {
            if (!IsUse)
            {
                return Status.SUCC;
            }

            try
            {
                Status nRet = (Status)EpsonSetRowDistance(iInn);
                if (nRet != Status.SUCC)
                {
                    AppLog.Write("[InvoicePrinter][SetRowDistance]" + nRet.ToString(), AppLog.LogMessageType.Warn);
                }
                return nRet;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[InvoicePrinter][SetRowDistance]Error!\n" + e.ToString(), AppLog.LogMessageType.Error);
                return Status.FAIL;
            }
        }

        #region IManagedHardware
        public object Open()
        {
            if (!IsUse)
                return Status.SUCC;
            Status ret = (Status)EpsonOpenPrinter(Port, Bps);
            if (ret != Status.SUCC)
            {
                AppLog.Write("[InvoicePrinter][Open]" + ret.ToString(), AppLog.LogMessageType.Warn);
            }
            return ret;
        }

        public object Close()
        {
            if (!IsUse)
                return Status.SUCC;
            Status ret = (Status)EpsonCloseCom();
            if (ret != Status.SUCC)
            {
                AppLog.Write("[InvoicePrinter][Close]" + ret.ToString(), AppLog.LogMessageType.Warn);
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
                case Status.SUCC:
                case Status.PAPER_FEW:
                    return false;
                default:
                    return true;
            }
        }
        #endregion
    }
}
