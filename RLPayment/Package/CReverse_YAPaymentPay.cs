using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using YAPayment.Package;
using Landi.FrameWorks;

namespace YAPayment.Package
{
    class CReverse_YAPaymentPay : YAPaymentPay
    {
        public string Reason;
        public CReverse_YAPaymentPay()
        {
            RestorePackageFromFile();
        }

        public CReverse_YAPaymentPay(PackageBase pb)
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

        protected override void Packet()
        {
            if (SendPackage.ExistBit(55))
            {
                byte[] bSend55 = SendPackage.GetArrayData(55);
                byte[] field55 = GetICAutoField55(bSend55, bSend55.Length);//IC卡55域有变动
                SendPackage.SetArrayData(55, field55, 0, field55.Length);
            }
            PackReverse(Reason);
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
