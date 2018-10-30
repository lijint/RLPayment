using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Landi.FrameWorks.HardWare;

namespace Landi.FrameWorks
{
    public abstract class PrinterActivity : Activity, IHandleMessage
    {
        protected enum Result
        {
            Success,
            PaperFew,
            OutOfPaper,
            Fail,
        }

        private const int PRINT = 1;
        private ArrayList mContent;
        private bool mPrinting;

        protected void PrintReceipt(ArrayList content)
        {
            if (content != null && content.Count > 0 && !mPrinting)
            {
                mPrinting = true;
                mContent = content;
                SendMessage(PRINT, "PrintReceipt");
            }
        }

        protected void PrintInvoice(ArrayList content)
        {
            if (content != null && content.Count > 0 && !mPrinting)
            {
                mPrinting = true;
                mContent = content;
                SendMessage(PRINT, "PrintInvoice");
            }
        }

        public void HandleMessage(Message message)
        {
            if (message.what == PRINT)
            {
                Result ret = Result.Fail;
                switch ((string)message.obj)
                {
                    case "PrintReceipt":
                        ret = OnPrintReceipt(mContent);
                        break;
                    case "PrintInvoice":
                        ret = OnPrintInvoice(mContent);
                        break;
                }
                if (ret == Result.Fail || ret == Result.OutOfPaper || ret == Result.PaperFew)
                    Log.Warn((string)message.obj + " : " + ret.ToString());
                else
                    Log.Debug((string)message.obj + " : " + ret.ToString());

                HandleResult(ret);
            }
        }

        protected abstract void HandleResult(Result result);

        protected virtual Result OnPrintReceipt(ArrayList content)
        {
            ReceiptPrinter.Status ret = ReceiptPrinter.OpenPrint();
            if (ret != ReceiptPrinter.Status.SUCC && ret != ReceiptPrinter.Status.PAPER_FEW)
            {
                Log.Warn("打开打印机故障!ret=" + ret.ToString());
                return Result.Fail;
            }

            ReceiptPrinter.SetPrintPosition(0);
            ret = ReceiptPrinter.PrintString(content);
            if (ret == ReceiptPrinter.Status.SUCC || ret == ReceiptPrinter.Status.PAPER_FEW)
            {
                ReceiptPrinter.FeedLine(8);
                ReceiptPrinter.CutPage();
                ReceiptPrinter.ClosePrint();

                if (ret == ReceiptPrinter.Status.SUCC)
                    return Result.Success;
                else
                    return Result.PaperFew;
            }
            else if (ret == ReceiptPrinter.Status.OUT_OF_PAPER)
            {
                return Result.OutOfPaper;
            }
            else
            {
                return Result.Fail;
            }
        }

        protected virtual Result OnPrintInvoice(ArrayList content)
        {
            InvoicePrinter.Status ret = InvoicePrinter.PrintString(content);
            if (ret == InvoicePrinter.Status.SUCC || ret == InvoicePrinter.Status.PAPER_FEW)
            {
                InvoicePrinter.CutPaper();
                if (ret == InvoicePrinter.Status.SUCC)
                    return Result.Success;
                else
                    return Result.PaperFew;
            }
            else if (ret == InvoicePrinter.Status.OUT_OF_PAPER)
            {
                return Result.OutOfPaper;
            }
            else
            {
                return Result.Fail;
            }
 
        }

        protected override void OnLeave()
        {
            mPrinting = false;
        }
    }
}
