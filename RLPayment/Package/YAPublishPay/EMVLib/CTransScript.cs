using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Landi.FrameWorks;
using YAPayment.Package;

namespace YAPayment.Package
{
    class CTransScript : YAPaymentPay
    {
        public CTransScript(PackageBase pb)
            : base(pb)
        {

        }

        public byte[] BField62 = new byte[0];
        public byte[] BAID = new byte[0];

        protected override void Packet()
        {
            SendPackage.SetString(0, "0620");
            SendPackage.SetString(60, "00" + GetBatchNo() + "951");

            SendPackage.ClearBitAndValue(14);
            SendPackage.ClearBitAndValue(26);
            SendPackage.ClearBitAndValue(35);
            SendPackage.ClearBitAndValue(36);
            SendPackage.ClearBitAndValue(48);
            SendPackage.ClearBitAndValue(52);
            SendPackage.ClearBitAndValue(53);
            SendPackage.ClearBitAndValue(64);
        }
    }
}
