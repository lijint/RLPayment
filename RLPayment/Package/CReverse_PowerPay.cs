using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using YAPayment.Package;
using Landi.FrameWorks;

namespace YAPayment.Package
{
    class CReverse_PowerPay : PowerPay
    {
        public string Reason;
        public CReverse_PowerPay()
        {
            RestorePackageFromFile();
        }

        public CReverse_PowerPay(PackageBase pb)
            : base(pb) 
        {
        }

        public void SetField55Value(byte[] field55, int len)
        {
            if (len != 0)
                SendPackage.SetArrayData(55, field55, 0, len);
        }

        public void CreateReverseFile(string reason)
        {
            if (string.IsNullOrEmpty(reason))
                reason = "06";
            Reason = reason;
            SavePackageToFile();
        }

        public void ClearReverseFile()
        {
            DeletePackageFile();
        }

        //protected override void Packet()
        //{
        //    if (SendPackage.ExistBit(55))
        //    {
        //        byte[] bSend55 = SendPackage.GetArrayData(55);
        //        byte[] field55 = GetICAutoField55(bSend55, bSend55.Length);//IC卡55域有变动
        //        SendPackage.SetArrayData(55, field55, 0, field55.Length);
        //    }
        //    PackReverse(Reason);
        //}

        protected override void Packet()
        {
            bool bIsIC = false;
            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bIsIC = true;

            SendPackage.SetString(0, "0400");
            if (!string.IsNullOrEmpty(CommonData.BankCardNum) && CommonData.BankCardNum.Length != 0)
            {
                SendPackage.SetString(2, CommonData.BankCardNum);
            }
            SendPackage.SetString(3, "190000");
            SendPackage.SetString(4, Utility.AmountToString(CommonData.Amount.ToString()));
            //PayEntity.PayTraceNo = GetTraceNo();
            //SendPackage.SetString(11, PayEntity.PayTraceNo);
            if (!string.IsNullOrEmpty(CommonData.BankCardExpDate) && CommonData.BankCardExpDate.Length != 0)//卡有效期
            {
                SendPackage.SetString(14, CommonData.BankCardExpDate);
            }
            if (bIsIC)//22
                SendPackage.SetString(22, "051");
            else
                SendPackage.SetString(22, "021");
            //if (!string.IsNullOrEmpty(CommonData.BankCardSeqNum) && CommonData.BankCardSeqNum.Length != 0)//卡序列号
            //{
            //    SendPackage.SetString(23, CommonData.BankCardSeqNum);
            //}
            SendPackage.SetString(25, "81"); //服务点条件代码
            //SendPackage.SetString(26, "06");
            if (!string.IsNullOrEmpty(CommonData.Track2) && CommonData.Track2.Length != 0)
            {
                SendPackage.SetString(35, CommonData.Track2.Replace('=', 'D'));
            }
            if (!string.IsNullOrEmpty(CommonData.Track3) && CommonData.Track3.Length != 0)
            {
                SendPackage.SetString(36, CommonData.Track3.Replace('=', 'D'));
            }
            SendPackage.SetString(41, GetTerminalNo());
            SendPackage.SetString(42, GetMerchantNo());
            SendPackage.SetArrayData(48, PacketField48());
            SendPackage.SetString(49, "156");
            if (SendPackage.ExistBit(55))
            {
                byte[] bSend55 = SendPackage.GetArrayData(55);
                byte[] field55 = GetICAutoField55(bSend55, bSend55.Length);//IC卡55域有变动
                SendPackage.SetArrayData(55, field55, 0, field55.Length);
            }
            PackReverse(Reason);

            //SendPackage.SetArrayData(52, Utility.str2Bcd(CommonData.BankPassWord));
            //switch (DType)
            //{
            //    case DesType.Des:
            //        SendPackage.SetString(53, "2000000000000000");
            //        break;
            //    case DesType.TripleDes:
            //        SendPackage.SetString(53, "2600000000000000");
            //        break;
            //}
            ////55
            //if (bIsIC && PayEntity.SendField55 != null && PayEntity.SendField55.Length != 0)
            //{
            //    SendPackage.SetArrayData(55, PayEntity.SendField55);
            //}
            //if (bIsIC)
            //    SendPackage.SetString(60, "22" + GetBatchNo() + "00050");
            //else
            //    SendPackage.SetString(60, "22" + GetBatchNo());

            ////创建冲正文件 98 96 06
            //CReverse_PowerPay cr = new CReverse_PowerPay(this);
            //cr.CreateReverseFile("98");
        }

        private byte[] PacketField48()
        {
            string temp = "U0V2560265200000" + PayEntity.PowerCardNo.PadRight(50, ' ') + "000000";
            byte[] tmp = Encoding.Default.GetBytes(temp);

            TLVHandler handler = new TLVHandler();
            handler.AddTag("1F50", "0004");
            handler.AddTag("FF28", GetMerchantNo());
            handler.AddTag("FF29", GetTerminalNo());
            //handler.AddTag("FF54",PayEntity.EleFeeNum);
            //handler.AddTag("FF55",PayEntity.EleFeeAccountNum);
            //handler.AddTag("FF61",PayEntity.PowerIdentity);//新增2个tag ，提供银商对账用 2015-11-2
            handler.AddTag("BF05", PayEntity.QueryTraceNo);

            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length + 1];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);

            Array.Copy(Encoding.Default.GetBytes("#"), 0, result, tmp.Length + content.Length, 1);

            return result;
        }


        protected override void OnRecvFail()
        {
            SavePackageToFile();
        }

        protected override void OnHostFail(string returnCode, string returnMessage)
        {
            DeletePackageFile();
        }

        protected override void OnSucc()
        {
            DeletePackageFile();
        }

        protected override void OnOtherResult()
        {
            SavePackageToFile();
        }
    }
}
