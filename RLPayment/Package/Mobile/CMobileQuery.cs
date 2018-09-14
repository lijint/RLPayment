using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace PetroChina.Package.Mobile
{
    class CMobileQuery : QMPay
    {
        protected override void Packet()
        {
            SetString(0, "0100");
            SetString(3, "310000");
            SetString(11, GetTraceNo());
            SetString(25, "81");
            SetString(48, "TT" + "CC" + MiddleFlowNo.PadRight(12, ' ') +
                PhoneNo.PadRight(20, ' ') + "#");
            SetString(49, "156");
        }

        protected override void OnSucc()
        {
            //48域 业务数据
            string Field48 = GetString(48);
            byte[] bField48 = Encoding.Default.GetBytes(Field48);
            LeavingAmount = Convert.ToDouble(Utility.StringToAmount(Field48.Substring(56, 20)));
            UserName = Encoding.Default.GetString(bField48, 76, 20).Trim();
        }

    }
}
