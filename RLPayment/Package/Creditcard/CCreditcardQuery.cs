using System;
using System.Collections.Generic;
using System.Text;
using YAPayment.Package;
using Landi.FrameWorks;

namespace YAPayment.Package.Creditcard
{
    /// <summary>
    /// 信用卡还款还款
    /// </summary>
    class CCreditcardQuery : QMPay
    {
        protected override void Packet()
        {
            SendPackage.SetString(0, "0100");
            SendPackage.SetString(2, PayEntity.CreditcardNum);
            SendPackage.SetString(3, "310000");
            SendPackage.SetString(4, Utility.AmountToString(CommonData.Amount.ToString()));
            SendPackage.SetString(11, GetTraceNo());
            SendPackage.SetString(22, "012");
            SendPackage.SetString(25, "92");
            //SendPackage.SetString(48, "PA" + "08" + PayEntity.CreditcardNum.PadRight(19, ' ') + "0".PadRight(11, '0') + "#");
            string f48 = "PA" + "08" + PayEntity.CreditcardNum.PadRight(19, ' ') + "0".PadRight(11, '0') + "#";
            SendPackage.SetArrayData(48, Encoding.Default.GetBytes(f48));
            
            SendPackage.SetString(49, "156");
        }

        protected override void OnSucc()
        {
            //48域 业务数据
            string Field48 = Encoding.Default.GetString(RecvPackage.GetArrayData(48));
            string totalAmount = Field48.Substring(4, 12).Trim();
            PayEntity.TotalAmount = double.Parse(totalAmount) / 100;

            string fee = Field48.Substring(16, 12).Trim();
            PayEntity.Fee = double.Parse(fee) / 100;
            string payAmount = Field48.Substring(28, 12);
            CommonData.Amount = double.Parse(payAmount) / 100;
        }
    }
}
