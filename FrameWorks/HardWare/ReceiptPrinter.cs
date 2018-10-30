using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;

namespace Landi.FrameWorks.HardWare
{
    public class ReceiptPrinter : HardwareBase<ReceiptPrinter, ReceiptPrinter.Status>, Landi.FrameWorks.IManagedHardware
    {
        #region epson532.dll
        [DllImport("epson532.dll")]
        protected static extern short Epson532OpenPrinter(string port_name, int bps);
        [DllImport("epson532.dll")]
        protected static extern short Epson532CPrint(string sPrintData);
        [DllImport("epson532.dll")]
        protected static extern short Epson532Cut();
        [DllImport("epson532.dll")]
        protected static extern short Epson532GetPrinterStatus();
        [DllImport("epson532.dll")]
        protected static extern short Epson532BlackMark();
        [DllImport("epson532.dll")]
        protected static extern short Epson532CloseCom();
        [DllImport("epson532.dll")]
        protected static extern short Epson532FeedLine(int nLineNum);
        [DllImport("epson532.dll")]
        protected static extern short Epson532PrintImageFromFileName(string filename);
        [DllImport("epson532.dll")]
        protected static extern short Epson532SetPrintPosition(int mode);
        [DllImport("epson532.dll")]
        protected static extern short Epson532SetAbsPrintPosition(int mode);
        #endregion

        /// <summary>
        /// 打印机状态
        /// </summary>
        public enum Status
        {
            SUCC = 0,               //成功
            FAIL = -1,              //通讯错误,请检查端口是否存在或是否连线正确
            OUT_OF_POW = 1,         //打印板处于下电状态
            HEADER_ERR = 2,         //打印头没有压下
            HARDW_ERR = 3,          //硬件错误
            OUT_OF_PAPER = 4,       //缺纸
            BUSY_OFFLINE_ERROR = 5, //打印板可能忙或离线或出错
            PAPER_FEW = 8,          //纸量少
            BUSY = 6,               //忙
            OFFLINE = 7,            //离线
        }

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        public static Status OpenPrint()
        {
            if (!IsUse) return Status.SUCC;
            try
            {
                Status ret = (Status)Epson532OpenPrinter(Port, Bps);
                if (ret != Status.SUCC)
                {
                    Log.Warn("[ReceiptPrinter][Open]" + ret.ToString());
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[ReceiptPrinter][Open]Error!\n" + e.ToString());
                return Status.FAIL;
            }
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="lsMsg"></param>
        public static Status PrintString(ArrayList lsMsg)
        {
            if (!IsUse)
            {
                if (lsMsg != null)
                {
                    for (int i = 0; i < lsMsg.Count; i++)
                        Log.Debug(lsMsg[i]);
                }
                return Status.SUCC;
            }
            try
            {
                Status ret = GetStatus();
                if (ret != Status.SUCC && ret != Status.PAPER_FEW)
                {
                    Log.Warn("打印机故障!ret=" + ret.ToString());
                    return ret;
                }

                foreach (string msInfo in lsMsg)
                {
                    Log.Debug("PrintString:" + msInfo);
                    Epson532CPrint(msInfo);
                }
                return Status.SUCC;
            }
            catch (System.Exception e)
            {
                Log.Error("[PrintString]Error!\n" + e.ToString());
                return Status.HARDW_ERR;
            }
        }

        public static Status SetPrintPosition(int pos)
        {
            if (!IsUse) return Status.SUCC;
            try
            {
                Status ret = (Status)Epson532SetAbsPrintPosition(pos);
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[PrintString]Error!\n" + e.ToString());
                return Status.HARDW_ERR;
            }
        }

        /// <summary>
        /// 打印图片
        /// </summary>
        /// <param name="lsMsg"></param>
        public static Status PrintImage(string filename)
        {
            if (!IsUse) return Status.SUCC;
            try
            {
                Status ret = GetStatus();
                if (ret != Status.SUCC && ret != Status.PAPER_FEW)
                {
                    Log.Warn("打印机故障!ret=" + ret.ToString());
                    return ret;
                }
                Epson532PrintImageFromFileName(filename);
                return Status.SUCC;
            }
            catch (System.Exception e)
            {
                Log.Error("[PrintImage]Error!\n" + e.ToString());
                return Status.HARDW_ERR;
            }
        }

        /// <summary>
        /// 得到打印机状态
        /// </summary>
        /// <returns></returns>
        public static Status GetStatus()
        {
            if (!IsUse) return Status.SUCC;
            try
            {
                Status ret = (Status)Epson532GetPrinterStatus();
                if (ret != Status.SUCC)
                {
                    Log.Warn("[ReceiptPrinter][GetStatus]" + ret.ToString());
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[ReceiptPrinter][GetStatus]Error!\n" + e.ToString());
                return Status.FAIL;
            }
        }

        /// <summary>
        /// 切纸
        /// </summary>
        /// <returns></returns>
        public static Status CutPage()
        {
            if (!IsUse) return Status.SUCC;
            try
            {
                Status ret = (Status)Epson532Cut();
                if (ret != Status.SUCC)
                {
                    Log.Warn("[ReceiptPrinter][CutPage]" + ret.ToString());
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[ReceiptPrinter][CutPage]Error!\n" + e.ToString());
                return Status.HARDW_ERR;
            }
        }

        /// <summary>
        /// 查找黑标
        /// </summary>
        /// <returns></returns>
        public static Status BlackMark()
        {
            if (!IsUse) return Status.SUCC;
            try
            {
                Status ret = (Status)Epson532BlackMark();
                if (ret != Status.SUCC)
                {
                    Log.Warn("[ReceiptPrinter][BlackMark]" + ret.ToString());
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[ReceiptPrinter][BlackMark]Error!\n" + e.ToString());
                return Status.FAIL;
            }
        }

        /// <summary>
        /// 进nLineNum行
        /// </summary>
        /// <param name="nLineNum"></param>
        /// <returns></returns>
        public static Status FeedLine(int nLineNum)
        {
            if (!IsUse) return Status.SUCC;
            try
            {
                Status ret = (Status)Epson532FeedLine(nLineNum);
                if (ret != Status.SUCC)
                {
                    Log.Warn("[ReceiptPrinter][FeedLine]" + ret.ToString());
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[ReceiptPrinter][FeedLine]Error!\n" + e.ToString());
                return Status.FAIL;
            }
        }

        /// <summary>
        /// 关闭打印机
        /// </summary>
        /// <returns>打印机状态</returns>
        public static Status ClosePrint()
        {
            if (!IsUse) return Status.SUCC;
            try
            {
                Status ret = (Status)Epson532CloseCom();
                if (ret != Status.SUCC)
                {
                    Log.Warn("[ReceiptPrinter][Close]" + ret.ToString());
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[ReceiptPrinter][Close]Error!\n" + e.ToString());
                return Status.FAIL;
            }
        }

        #region IManagedHardware
        public object Open()
        {
            if (!IsUse) return Status.SUCC;
            Log.Info("open receiptPrinter isuse:"+IsUse);
            Status ret = (Status)Epson532OpenPrinter(Port, Bps);
            if (ret != Status.SUCC)
            {
                Log.Warn("[ReceiptPrinter][Open]" + ret.ToString());
            }
            return ret;
        }

        public object Close()
        {
            if (!IsUse) return Status.SUCC;
            Status ret = (Status)Epson532CloseCom();
            if (ret != Status.SUCC)
            {
                Log.Warn("[ReceiptPrinter][Close]" + ret.ToString());
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
