using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Package.PetroModifyPass
{
    class CPetroModifyPassProcess : YAPaymentPay
    {
        private byte[] PacketField48()
        {
            string field48 = "C3v22901510000" + "".PadLeft(40, ' ') + "000000" + "2";
            byte[] tmp = Encoding.Default.GetBytes(field48);

            string loginName = LoginName;
            string loginPsd = LoginPsd;
            string loginNewPsd = LoginNewPsd;
#if DEBUG
            loginName = "FW010101Z00501010" + LoginName;
#endif
            TLVHandler handler = new TLVHandler();
            handler.AddTag("FF44", loginName);
            handler.AddTag("FF45", loginPsd);
            handler.AddTag("FF46", loginNewPsd);
            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);
            return result;
        }

        protected override void Packet()
        {
            SendPackage.SetString(0, "0100");
            SendPackage.SetString(3, "340000");
            SendPackage.SetString(11, GetTraceNo());
            SendPackage.SetString(25, "00"); //服务点条件代码
            SendPackage.SetArrayData(48, PacketField48());

            SendPackage.SetString(49, "156");
            switch (DType)
            {
                case DesType.Des:
                    SendPackage.SetString(53, "20");
                    break;
                case DesType.TripleDes:
                    SendPackage.SetString(53, "26");
                    break;
            }
            SendPackage.SetString(60, "00" + GetBatchNo() + "000");
        }

        protected override void OnSucc()
        {
        }
    }
}
