using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Business.RLCZ
{
    class BeingPrintDeal : PrinterActivity
    {
        private RLCZEntity _entity;
        protected override void OnEnter()
        {
            base.OnEnter();
            _entity = GetBusinessEntity() as RLCZEntity;
            GetElementById("Message1").InnerHtml = "正在打印，请稍后... ...";
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
            switch(_entity.PayType)
            {
                case 0:
                    Lprint.Add(" 交易方式 : 银行卡");
                    Lprint.Add(" 支付帐号 : " + Utility.GetPrintCardNo(CommonData.BankCardNum));
                    break;
                case 1:
                    Lprint.Add(" 交易方式 : 微信");
                    break;
                case 2:
                    Lprint.Add(" 交易方式 : 支付宝");
                    break;
            }
            //Lprint.Add(" 支付帐号 : " + Utility.GetPrintCardNo(CommonData.BankCardNum));

            Lprint.Add(" 日期/时间: " + System.DateTime.Now.ToString("yyyy") + "/" + System.DateTime.Now.ToString("MM") + "/" + System.DateTime.Now.ToString("dd") + "  " + System.DateTime.Now.ToString("HH") + ":" + System.DateTime.Now.ToString("mm") + ":" + System.DateTime.Now.ToString("ss"));

            Lprint.Add(" 用户卡号 : " + _entity.CardNO.Trim());
            Lprint.Add(" 用户姓名 : " + _entity.UserName.Trim());
            Lprint.Add(" 地   址 : " + _entity.Addr.Trim());
            Lprint.Add(" ----------------------------------");
            Lprint.Add(" " + sPadLeft + "缴费明细");
            foreach (UserInfo ui in _entity.userInfoList)
            {
                Lprint.Add(" 费用类别 : " + ui.FeeType.Trim());
                Lprint.Add(" 采 暖 期 : " + ui.HeatingPeriod.Trim());
                Lprint.Add(" 面   积 : " + ui.Area + "m²");
                Lprint.Add(" 应收金额 : " + ui.ReceivableAmount + "元");
            }
            Lprint.Add("   ");
            Lprint.Add("   ");
            if(_entity.CompanyCode=="01")
                Lprint.Add("     " + "*** 济宁新东供热有限责任公司 ***");
            else if(_entity.CompanyCode=="02")
                Lprint.Add("   " + "*** 济宁高新公用事业发展股份有限公司 ***");

            Lprint.Add(" " + sPadLeft + "***      中国兴业银行      ***");
            Lprint.Add(" " + sPadLeft + "***        通联支付       ***");
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
