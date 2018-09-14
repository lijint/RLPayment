using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace YAPayment.Package
{
    class CEndAID : YAPaymentPay
    {
        public byte[] BField62 = new byte[0];
        public byte[] BAID = new byte[0];
        protected override void Packet()
        {
            SendPackage.SetString(0, "0800");
            SendPackage.SetString(60, "00" + GetBatchNo() + "381");
        }

        protected override bool NeedCalcMac()
        {
            return false;
        }
        protected override void OnSucc()
        {
            SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //记录批次号
        }
    }
}
