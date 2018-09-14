using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using YAPayment.Package;
using Landi.FrameWorks;

namespace YAPayment.Package.TrafficPolice
{
    class CYATrafficPoliceBillConfirm : YAPaymentPay
    {
        protected override void Packet()
        {
            SendPackage.SetString(0, "0320");
            //SendPackage.SetString(2, CommonData.BankCardNum);
            SendPackage.SetString(3, "290000");
            SendPackage.SetString(4, Utility.AmountToString(CommonData.Amount.ToString()));

            SendPackage.SetString(11, GetTraceNo());
            SendPackage.SetString(25, "00"); //服务点条件代码
            SendPackage.SetArrayData(48, PacketField48());
            SendPackage.SetString(49, "156");
            SendPackage.SetString(60, "00" + GetBatchNo() + "362");
        }

        private byte[] PacketField48()
        {
            string temp = "B0V2571165000000" + "".PadLeft(50, ' ') + "000000";
            byte[] tmp = Encoding.Default.GetBytes(temp);

            TLVHandler handler = new TLVHandler();
            handler.AddTag("1F50", "0004");
            handler.AddTag("FF28", GetMerchantNo());
            handler.AddTag("FF29", GetTerminalNo());
            handler.AddTag("BF05", PayEntity.TPPayFlowNo);
            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length + 1];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);

            Array.Copy(Encoding.Default.GetBytes("#"), 0, result, tmp.Length + content.Length, 1);

            return result;
        }

        protected override void OnSucc()
        {
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());
            PayEntity.TPConfirmTraceNo = handler.GetStringValue("BF05");
        }

    }
}
