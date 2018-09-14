using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using YAPayment.Package;
using Landi.FrameWorks;

namespace YAPayment.Package.PetroPay
{
    class CPetroPayBillConfirm : YAPaymentPay
    {
        private byte[] PacketField48()
        {
            string field48 = "B0v22901510000" + UserID.PadLeft(40, ' ') + "000000" + "2";
            byte[] tmp = Encoding.Default.GetBytes(field48);

            TLVHandler handler = new TLVHandler();
            handler.AddTag("BF15", PayFlowNo);
            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);
            return result;
        }

        protected override void Packet()
        {
            SendPackage.SetString(0, "0320");
            SendPackage.SetString(2, CommonData.BankCardNum);
            SendPackage.SetString(3, "290000");
            SendPackage.SetString(4, Utility.AmountToString(CommonData.Amount.ToString()));
            
            SendPackage.SetString(11, GetTraceNo());
            SendPackage.SetString(25, "00"); //服务点条件代码
            SendPackage.SetArrayData(48, PacketField48());
            SendPackage.SetString(49, "156");
            SendPackage.SetString(60, "00" + GetBatchNo() + "362");
        }

        protected override void OnSucc()
        {
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());
            ConfirmTraceNo = handler.GetStringValue("BF05");
            ReceiptID = handler.GetStringValue("BF06");
        }

    }
}
