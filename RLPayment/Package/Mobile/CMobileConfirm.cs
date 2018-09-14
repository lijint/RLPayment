using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace PetroChina.Package.Mobile
{
    class CMobileConfirm : QMPay
    {
        protected override void Packet()
        {
            SetString(0, "0320");
            SetString(3, "290000");
            SetString(11, GetTraceNo());
            SetString(15, Field15);
            SetString(37, PayReferenceNo);
            SetString(48, "QR" + "CC" +
                MobileType + PhoneNo.PadRight(20, ' ') + MiddleFlowNo.PadRight(12, ' ') + "#");
            SetString(60, "00" + GetBatchNo() + "362");
        }

        protected override void OnSucc()
        {
            //37域 系统参考号
            //RecvData.Field37 = TransDownPack.GetString(37);
            //48域 业务数据
            string Field48 = GetString(48);
            byte[] bField48 = Encoding.Default.GetBytes(Field48);
            LeavingAmount = Convert.ToDouble(Utility.StringToAmount(Field48.Substring(56, 20)));
            UserName = Encoding.Default.GetString(bField48, 76, 20).Trim();
        }
    }
}
