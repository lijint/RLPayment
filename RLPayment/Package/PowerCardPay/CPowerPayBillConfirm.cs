using System;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Package.PowerCardPay
{
    class CPowerPayBillConfirm : PowerPay
    {
        int m_process;

        public TransResult BillConfirm()
        {
            TransResult result = TransResult.E_RECV_FAIL;
            try
            {
                m_process = 1;
                result = Communicate();
                if (result != TransResult.E_SUCC)
                    return result;
                m_process = 2;
                result = Communicate();
            }
            catch (Exception ex)
            {
                Log.Error("[CPowerPayBillConfirm][BillConfirm]Error!", ex);
            }

            return result;
        }

        protected override void Packet()
        {
            SendPackage.SetString(0, "0320");
            //SendPackage.SetString(2, CommonData.BankCardNum);
            SendPackage.SetString(3, "290000");
            SendPackage.SetString(4, Utility.AmountToString(CommonData.Amount.ToString()));
            SendPackage.SetString(11, GetTraceNo());
            SendPackage.SetString(25, "00"); //服务点条件代码
            SendPackage.SetString(42, GetMerchantNo());
            SendPackage.SetString(41, GetTerminalNo());

            SendPackage.SetArrayData(48, PacketField48());
            SendPackage.SetString(49, "156");
            SendPackage.SetArrayData(57, PacketField57());
            SendPackage.SetString(60, "00" + GetBatchNo() + "362");
        }

        private byte[] PacketField48()
        {
            string temp = "B0V2560265200000" + PayEntity.PowerCardNo.PadRight(50, ' ') + "000000";
            byte[] tmp = Encoding.Default.GetBytes(temp);
            TLVHandler handler = new TLVHandler();
            handler.AddTag("1F50", "0004");

            if (m_process == 1)
            {
                handler.AddTag("FF28", GetMerchantNo());
                handler.AddTag("FF29", GetTerminalNo());
                handler.AddTag("1F3A", PayEntity.PowerCardData.EF1);
                handler.AddTag("1F4A", PayEntity.PowerCardData.EF2);
                handler.AddTag("1F5A", PayEntity.PowerCardData.EF5);
            }
            else if (m_process == 2)
            {
                handler.AddTag("BF05", PayEntity.ConfirmTraceNo);
                //handler.AddTag("1F4A", PayEntity.PowerCardData.EF5);
            }
            handler.AddTag("BF15", PayEntity.PayFlowNo);
            handler.AddTag("FF01", m_process.ToString());
            handler.AddTag("1F1A", PayEntity.PowerBusiness.ToString());
            handler.AddTag("1F2A", PayEntity.CityPowerNo);


            //handler.AddTag("FF54", PayEntity.EleFeeNum);
            //handler.AddTag("FF55", PayEntity.EleFeeAccountNum);
            //handler.AddTag("1F20", Utility.AmountToString(CommonData.Amount.ToString()));
            //handler.AddTag("1F21", DateTime.Now.ToString("yyyyMMdd"));
            //handler.AddTag("1F22", PayEntity.PowerCardData.CardNo);
            //handler.AddTag("1F23", PayEntity.PowerCardData.Random);
            //handler.AddTag("1F24", PayEntity.PowerCardData.CardInfo);
            //handler.AddTag("1F25", PayEntity.PowerIdentity);
            //handler.AddTag("FF30", GetBranchNo());
            //handler.AddTag("FF31",GetOperatorNo());

            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length + 1];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);

            Array.Copy(Encoding.Default.GetBytes("#"), 0, result, tmp.Length + content.Length, 1);

            return result;
        }

        private byte[] PacketField57()
        {
            string tmp = "";
            if (m_process == 1)
            {
                tmp = PayEntity.PowerCardData.EF3.PadRight(256,' ');
                tmp += PayEntity.PowerCardData.CardNo.PadRight(16, ' ');
                tmp += PayEntity.PowerCardData.Random.PadRight(16, ' ');
                tmp += PayEntity.PowerIdentity.PadRight(16, ' ');
            }
            else if (m_process == 2)
            {
                tmp = PayEntity.PowerCardData.EF4.PadRight(256, ' ');
                tmp += PayEntity.PowerCardData.CardInfo.PadRight(256, ' ');//EF2

            }

            //tmp += PayEntity.PowerCardData.EF4.PadLeft(256, ' ');
            //tmp += PayEntity.PowerCardData.EF5.PadRight(98, ' ');//EF5


            byte[] result = Encoding.Default.GetBytes(tmp);

            Log.Debug("End PacketField57 temp:" + tmp);
            return result;
        }


        protected override void OnSucc()
        {
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());
            PayEntity.ConfirmTraceNo = handler.GetStringValue("BF05");
            if (m_process == 2)
            {
                PayEntity.PowerPayConfirmCode = handler.GetStringValue("3F2B");
                PayEntity.PowerRetMsg = handler.GetStringValue("3F1B");
            }
            //PayEntity.PowerCardData.W_EF1 = handler.GetStringValue("1F37");
            //PayEntity.PowerCardData.W_EF2 = handler.GetStringValue("1F38");
            //PayEntity.PowerCardData.W_EF5 = handler.GetStringValue("1F39");

            //byte[] tmp = RecvPackage.GetArrayData(57);
            //string temp57 = PubFunc.ByteArrayToHexString(tmp, tmp.Length).Trim();
            //Log.Debug(temp57);

            //PayEntity.PowerCardData.W_EF31 = temp57.Substring(0, 264);
            //PayEntity.PowerCardData.W_EF32 = temp57.Substring( 264, 268);
            //PayEntity.PowerCardData.W_EF41 = temp57.Substring( 532, 264);
            //PayEntity.PowerCardData.W_EF42 = temp57.Substring( 796, 268);
            //PayEntity.PowerCardData.CertDes = temp57.Substring( 1064, 16);
            //PayEntity.PowerCardData.LimitDes = temp57.Substring( 1080, 16);
            //PayEntity.PowerCardData.ExtDes = temp57.Substring( 1096, 16);
        }
    }
}
