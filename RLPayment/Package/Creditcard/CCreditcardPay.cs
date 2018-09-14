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
    class CCreditcardPay : QMPay
    {
        protected override void Packet()
        {
            bool bIsIC = false;
            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bIsIC = true;

            SendPackage.SetString(0, "0200");
            SendPackage.SetString(3, "190000");
            SendPackage.SetString(4, Utility.AmountToString(PayEntity.TotalAmount.ToString()));
            PayEntity.PayTraceNo = GetTraceNo();
            SendPackage.SetString(11, PayEntity.PayTraceNo);
            if (!string.IsNullOrEmpty(CommonData.BankCardExpDate) && CommonData.BankCardExpDate.Length != 0)//卡有效期
            {
                SendPackage.SetString(14, CommonData.BankCardExpDate);
            }
            if (bIsIC)//22
                SendPackage.SetString(22, "051");
            else
                SendPackage.SetString(22, "021");
            if (!string.IsNullOrEmpty(CommonData.BankCardSeqNum) && CommonData.BankCardSeqNum.Length != 0)//卡序列号
            {
                SendPackage.SetString(23, CommonData.BankCardSeqNum);
            }
            SendPackage.SetString(25, "00");
            SendPackage.SetString(26, "06");
            if (!string.IsNullOrEmpty(CommonData.Track2) && CommonData.Track2.Length != 0)
            {
                SendPackage.SetString(35, CommonData.Track2.Replace('=', 'D'));
            }
            if (!string.IsNullOrEmpty(CommonData.Track3))
            {
                SendPackage.SetString(36, CommonData.Track3.Replace('=', 'D'));
            }
            //SendPackage.SetString(48, "PA" + "08" + PayEntity.CreditcardNum.PadRight(20, ' ') + "N" + "0".PadRight(11, '0') + "#");
            string f48 = "PA" + "08" + PayEntity.CreditcardNum.PadRight(20, ' ') + "N" + "0".PadRight(11, '0') + "#";
            SendPackage.SetArrayData(48, Encoding.Default.GetBytes(f48));
            SendPackage.SetArrayData(52, Utility.str2Bcd(CommonData.BankPassWord));
            SendPackage.SetString(49, "156");
            switch (DType)
            {
                case DesType.Des:
                    SendPackage.SetString(53, "2000000000000000");
                    break;
                case DesType.TripleDes:
                    SendPackage.SetString(53, "2600000000000000");
                    break;
            }
            //55
            if (bIsIC && PayEntity.SendField55 != null && PayEntity.SendField55.Length != 0)
            {
                SendPackage.SetArrayData(55, PayEntity.SendField55);
            }
            if (PayEntity.UseICCard)
                SendPackage.SetString(60, "00" + GetBatchNo() + "00050");
            else
                SendPackage.SetString(60, "00" + GetBatchNo() + "000");

            //创建冲正文件 98 96 06
            CReverse_YAPaymentPay cr = new CReverse_YAPaymentPay(this);
            cr.CreateReverseFile("98");
        }

        protected override void OnSucc()
        {
            //37域 系统参考号
            PayEntity.PayReferenceNo = RecvPackage.GetString(37);
            //38域
            PayEntity.RecvField38 = RecvPackage.ExistValue(38) ? RecvPackage.GetString(38) : "";
            //55域
            PayEntity.RecvField55 = RecvPackage.ExistValue(55) ? RecvPackage.GetArrayData(55) : new byte[0];
        }
    }
}
