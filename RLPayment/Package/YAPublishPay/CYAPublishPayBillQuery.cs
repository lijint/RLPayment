using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Package.YAPublishPay
{
    class CYAPublishPayBillQuery : YAPaymentPay
    {
        protected override void Packet()
        {
            SendPackage.SetString(0, "0100");
            SendPackage.SetString(3, "310000");
            SendPackage.SetString(11, GetTraceNo());
            SendPackage.SetString(25, "92"); //服务点条件代码
            SendPackage.SetArrayData(48, PacketField48());
            SendPackage.SetString(49, "156");
            SendPackage.SetString(60, "00" + GetBatchNo() + "000");
        }

        private byte[] PacketField48()
        {
            byte[] tmp = HeandField48();
            TLVHandler handler = new TLVHandler();
            handler.AddTag("1F50", "0004");
            handler.AddTag("FF28", GetMerchantNo());
            handler.AddTag("FF29", GetTerminalNo());

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
                        field48 = "C1V256ZO67700000" + PayEntity.UserID.PadRight(50, ' ') + "000000";
                        break;
                    }
                case Entity.YaPublishPayType.Water:
                    {
                        field48 = "C0V256XO67700000" + PayEntity.UserID.PadRight(50, ' ') + "000000";
                        break;
                    }
                case Entity.YaPublishPayType.TV:
                    {
                        field48 = "C0V258A167700000" + PayEntity.UserID.PadRight(50, ' ') + "000000";
                        break;
                    }
                case Entity.YaPublishPayType.Power:
                    {
                        field48 = "C2V2560265000000" + PayEntity.UserID.PadRight(50, ' ') + "000000";
                        break;
                    }
            }

            return Encoding.Default.GetBytes(field48); ;
        }


        protected override void OnSucc()
        {
            switch (PayEntity.PublishPayType)
            {
                case Entity.YaPublishPayType.Gas:
                    {
                        GasOnSucc();
                        break;
                    }
                case Entity.YaPublishPayType.Water:
                    {
                        WaterOnSucc();
                        break;
                    }
                case Entity.YaPublishPayType.TV:
                    {
                        TVOnSucc();
                        break;
                    }
                case Entity.YaPublishPayType.Power:
                    {
                        PowerOnSucc();
                        break;
                    }
            }
        }

        private void GasOnSucc()
        {
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());

            PayEntity.QueryTraceNo = handler.GetStringValue("BF05");
            PayEntity.UserName = handler.GetStringValue("1F1B");
            PayEntity.UserAddress = handler.GetStringValue("1F2B");
            PayEntity.QueryAmount = double.Parse(handler.GetStringValue("1F3B")) / 100;
            PayEntity.QueryDateStart = handler.GetStringValue("1F4B");
            PayEntity.QueryDateEnd = handler.GetStringValue("1F5B");
        }

        private void WaterOnSucc()
        {
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());

            PayEntity.QueryTraceNo = handler.GetStringValue("BF05");
            PayEntity.UserName = handler.GetStringValue("1F1B");
            PayEntity.UserAddress = handler.GetStringValue("1F2B");
            PayEntity.QueryAmount = double.Parse(handler.GetStringValue("1F3B")) / 100;
            PayEntity.WaterFee = double.Parse(handler.GetStringValue("1F4B")) / 100;
            PayEntity.WaterTotalAmount = double.Parse(handler.GetStringValue("1F5B")) / 100;
        }

        private void TVOnSucc()
        {
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());

            PayEntity.QueryTraceNo = handler.GetStringValue("BF05");
            PayEntity.UserName = handler.GetStringValue("1F1B");
            PayEntity.QueryAmount = double.Parse(handler.GetStringValue("1F2B")) / 100;
            PayEntity.QueryDateEnd = handler.GetStringValue("1F3B");
            PayEntity.Price1 = double.Parse(handler.GetStringValue("1F4B")) / 100;
            PayEntity.Price2 = double.Parse(handler.GetStringValue("1F5B")) / 100;
            PayEntity.PriceInfo = handler.GetStringValue("1F6B");
        }

        private void PowerOnSucc()
        {
        }

    }
}
