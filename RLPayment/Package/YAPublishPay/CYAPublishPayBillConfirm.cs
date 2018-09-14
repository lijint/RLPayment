using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using YAPayment.Package;
using Landi.FrameWorks;

namespace YAPayment.Package.YAPublishPay
{
    class CYAPublishPayBillConfirm : YAPaymentPay
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
            byte[] tmp = HeandField48();

            TLVHandler handler = new TLVHandler();
            handler.AddTag("1F50", "0004");
            handler.AddTag("FF28", GetMerchantNo());
            handler.AddTag("FF29", GetTerminalNo());
            handler.AddTag("BF05", PayEntity.PayFlowNo);
            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length + 1];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);

            Array.Copy(Encoding.Default.GetBytes("#"), 0, result, tmp.Length + content.Length, 1);

            return result;
        }

        private byte[] HeandField48()
        {
            string field48 = "";
            switch (PayEntity.PublishPayType)
            {
                case Entity.YaPublishPayType.Gas:
                    {
                        field48 = "B1V256ZO67700000" + PayEntity.UserID.PadRight(50, ' ') + "000000";
                        break;
                    }
                case Entity.YaPublishPayType.Water:
                    {
                        field48 = "B0V256XO67700000" + PayEntity.UserID.PadRight(50, ' ') + "000000";
                        break;
                    }
                case Entity.YaPublishPayType.TV:
                    {
                        field48 = "B0V258A167700000" + PayEntity.UserID.PadRight(50, ' ') + "000000";
                        break;
                    }
            }

            return Encoding.Default.GetBytes(field48); ;
        }

        

        protected override void OnSucc()
        {
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());
            PayEntity.ConfirmTraceNo = handler.GetStringValue("3F1B");
        }

    }
}
