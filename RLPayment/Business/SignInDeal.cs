using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package;
using Landi.FrameWorks.HardWare;
using YAPayment.Business.Creditcard;
using YAPayment.Business.Mobile;

namespace YAPayment.Business
{
    class SignInDeal:Activity
    {
        protected override void OnEnter()
        {
            bool succ = false;
            string businessName = GetBusinessName();
            if (businessName == "PowerPay")
            {
                SyncTransaction(new CSignIn_PowerPay());
                succ = PowerPay.HasSignIn;
            }
            else if (businessName == "Car")
            {
                SyncTransaction(new CSignIn_CarPay());
                succ = CarPay.HasSignIn;
            }
            else 
            {
                SyncTransaction(new CSignIn_YAPaymentPay());
                succ = YAPaymentPay.HasSignIn;
                QMPay.HasSignIn = YAPaymentPay.HasSignIn;
            }
   
            if (!succ)
            {
                ShowMessageAndGotoMain("签到失败，该业务暂时不能使用");
            }
            else
            {
                switch (GetBusinessName())
                {
                    case "CreditCard":
                        StartActivity(ReceiptPrinter.CheckedByManager() ? "信用卡还款温馨提示" : "信用卡打印机故障继续");
                        break;
                    case "Mobile":
                        StartActivity(ReceiptPrinter.CheckedByManager() ? "手机充值主界面" : "手机充值打印机故障继续");
                        break;
                    case "YAPublishPay":
                        StartActivity(ReceiptPrinter.CheckedByManager() ? "雅安支付输入用户号" : "雅安支付打印机故障继续");
                        break;
                    case "PowerPay":
                        StartActivity(ReceiptPrinter.CheckedByManager() ? "电力支付菜单" : "电力支付打印机故障继续");
                        break;
                    case "YATrafficPolice":
                        StartActivity(ReceiptPrinter.CheckedByManager() ? "雅安交警罚没菜单" : "雅安交警罚没打印机故障继续");
                        break;
                    case "Car":
                        StartActivity(ReceiptPrinter.CheckedByManager() ? "车票预订主画面" : "雅安交警罚没打印机故障继续");
                        break;
                }
            }
        }
    }
}
