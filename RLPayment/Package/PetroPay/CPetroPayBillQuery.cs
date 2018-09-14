using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using YAPayment.Package;
using Landi.FrameWorks;

namespace YAPayment.Package.PetroPay
{
    class CPetroPayBillQuery : YAPaymentPay
    {
        private byte[] PacketField48()
        {
            string field48 = "C0v22901510000" + UserID.PadLeft(40, ' ') + "000000" + "2";
            byte[] tmp = Encoding.Default.GetBytes(field48);
            TLVHandler handler = new TLVHandler();
            handler.AddTag("BF12", SelectRecordInfo[0]);
            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);
            return result;
        }

        protected override void Packet()
        {
            SendPackage.SetString(0, "0100");
            SendPackage.SetString(3, "310000");
            SendPackage.SetString(11, GetTraceNo());
            SendPackage.SetString(25, "92"); //服务点条件代码
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
            //54域
            CommonData.Amount = double.Parse(RecvPackage.GetString(54)) / 100;
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());
            QueryTraceNo = handler.GetStringValue("BF05");
            QueryCount = handler.GetStringValue("BF06");
            QueryContent = handler.GetStringValue("FF48");
        }
    }
}
