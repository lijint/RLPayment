using Landi.FrameWorks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Business.RLCZ
{
    class BeingPrintDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            PrintReceipt(GetTransferReceipt());
        }

        private ArrayList GetTransferReceipt()
        {
            string sTitle = "***济宁热力自助缴费交易凭条***";
            int splitStringLen = Encoding.Default.GetByteCount("--------------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);
            Lprint.Add("  ");
            Lprint.Add(" 交易类型 :  缴费");
            Lprint.Add(" 支付帐号 : " + Utility.GetPrintCardNo(CommonData.BankCardNum));

            Lprint.Add(" 日期/时间: " + System.DateTime.Now.ToString("yyyy") + "/" + System.DateTime.Now.ToString("MM") + "/" + System.DateTime.Now.ToString("dd") + "  " + System.DateTime.Now.ToString("HH") + ":" + System.DateTime.Now.ToString("mm") + ":" + System.DateTime.Now.ToString("ss"));
            Lprint.Add(" ----------------------------------");
            Lprint.Add(" " + sPadLeft + "缴费明细");
            Lprint.Add(" 缴费金额 : " + CommonData.Amount);
            Lprint.Add("   ");
            Lprint.Add("   ");
            Lprint.Add(" " + sPadLeft + "*** 中国兴业银行 ***");
            //Lprint.Add(" " + sPadLeft + " 客服电话: 023-63086110");
            Lprint.Add("   ");
            Lprint.Add("   ");

            return Lprint;
        }

        protected override void HandleResult(Result result)
        {
            if (result == Result.Success || result == Result.PaperFew)
            {
                StartActivity("热力充值通用成功");
            }
            else
            {
                ShowMessageAndGotoMain("失败|打印错误！");
            }
        }

    }
}
