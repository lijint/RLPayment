using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using YAPayment.Package;
using Landi.FrameWorks;

namespace YAPayment.Package.PetroPay
{
    class CPetroPayBeingPay:YAPaymentPay
    {
        private byte[] PacketField48()
        {
            string field48 = "P0v22901510000" + UserID.PadLeft(40, ' ') + "000000" + "2";
            byte[] tmp = Encoding.Default.GetBytes(field48);
            TLVHandler handler = new TLVHandler();
            handler.AddTag("BF15", QueryTraceNo);
            handler.AddTag("FF44", SelectRecordInfo[3]);
            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);
            return result;
        }

        protected override void Packet()
        {
            bool bIsIC = false;
            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bIsIC = true;

            SendPackage.SetString(0, "0200");
            if (!string.IsNullOrEmpty(CommonData.BankCardNum) && CommonData.BankCardNum.Length != 0)
            {
                SendPackage.SetString(2, CommonData.BankCardNum);
            }
            SendPackage.SetString(3, "190000");
            SendPackage.SetString(4, Utility.AmountToString(CommonData.Amount.ToString()));
            PayTraceNo = GetTraceNo();
            SendPackage.SetString(11, PayTraceNo);
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
            SendPackage.SetString(25, "81"); //服务点条件代码
            SendPackage.SetString(26, "06");
            if (!string.IsNullOrEmpty(CommonData.Track2) && CommonData.Track2.Length != 0)
            {
                SendPackage.SetString(35, CommonData.Track2.Replace('=', 'D'));
            }
            if (!string.IsNullOrEmpty(CommonData.Track3) && CommonData.Track3.Length != 0)
            {
                SendPackage.SetString(36, CommonData.Track3.Replace('=', 'D'));
            }
            SendPackage.SetArrayData(48, PacketField48());
            SendPackage.SetString(49, "156");
            SendPackage.SetArrayData(52, Utility.str2Bcd(CommonData.BankPassWord));
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
            if (SendField55 != null && SendField55.Length != 0)
            {
                SendPackage.SetArrayData(55, SendField55, 0, SendField55.Length);
            }
            if (bIsIC)
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
            PayReferenceNo = RecvPackage.GetString(37);
            //38域
            if (RecvPackage.ExistValue(38))
            {
                RecvField38 = RecvPackage.GetString(38);
            }
            //55域
            if (RecvPackage.ExistValue(55))
            {
                RecvField55 = RecvPackage.GetArrayData(55);
            }
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());
            PayFlowNo = handler.GetStringValue("BF05");
        }
    }
}
