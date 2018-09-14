using System;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Package.PowerCardPay
{
    class CPowerCardWriteAgain : PowerPay
    {
        private int _mProcess = 0;
        //PowerCardInfo m_powerCardInfo = new PowerCardInfo();
        //string m_writeCardTraceNo = "";

        public TransResult WritePowerCardAgain()
        {
            TransResult result = TransResult.E_RECV_FAIL;
            try
            {
                CPowerCard card = new CPowerCard();
                if (!card.ReadPowerCard(PayEntity.PowerCardData))
                    return TransResult.E_SEND_FAIL;

                _mProcess = 1;
                while (_mProcess <= 2)
                {
                    result = Communicate();
                    if (result != TransResult.E_SUCC)
                        return result;
                    _mProcess++;
                }
                //_mProcess = 1;
                //result = Communicate();

                //if (result != TransResult.E_SUCC)
                //    return result;

                if (PayEntity.PowerCardData.EF2.Substring(8, 8) == PayEntity.PowerCardData.W_EF2.Substring(8, 8))
                {
                    Log.Debug("EF2:" + PayEntity.PowerCardData.EF2.Substring(8, 8) + ",W_EF2:" + PayEntity.PowerCardData.W_EF2.Substring(8, 8));
                    return TransResult.E_SUCC;
                }
                if (!card.WritePowerCard(PayEntity.PowerCardData))
                    return TransResult.E_HOST_FAIL;
            }
            catch (Exception ex)
            {
                Log.Error("[CPowerCardWriteAgain][GetWriteAgainInfo]Error!", ex);
            }

            return result;
        }

        protected override void Packet()
        {
            SendPackage.SetString(0, "0320");
            SendPackage.SetString(3, "290000");
            SendPackage.SetString(4, Utility.AmountToString(CommonData.Amount.ToString()));
            SendPackage.SetString(11, GetTraceNo());
            SendPackage.SetString(25, "00"); //服务点条件代码
            SendPackage.SetString(41, GetTerminalNo());
            SendPackage.SetString(42, GetMerchantNo());
            SendPackage.SetArrayData(48, PacketField48());
            SendPackage.SetString(49, "156");
            //byte[] field57 = PacketField57();
            //if (field57!=null && field57.Length > 0)
            SendPackage.SetArrayData(57, PacketField57());
            SendPackage.SetString(60, "00" + GetBatchNo() + "362");
        }

        private byte[] PacketField48()
        {
            string temp = "B1V2560265200000" + PayEntity.PowerCardNo.PadRight(50, ' ') + "000000";
            byte[] tmp = Encoding.Default.GetBytes(temp);
            TLVHandler handler = new TLVHandler();
            handler.AddTag("1F50", "0004");
            handler.AddTag("FF28", GetMerchantNo());
            handler.AddTag("FF29", GetTerminalNo());
            //handler.AddTag("3F2B", "51401");

            handler.AddTag("3F2B", PayEntity.PowerPayConfirmCode);
            if (_mProcess == 1)
            {
                handler.AddTag("1F3A", PayEntity.PowerCardData.EF1);
                handler.AddTag("1F4A", PayEntity.PowerCardData.EF2);
                handler.AddTag("1F5A", PayEntity.PowerCardData.EF5);
                //handler.AddTag("1F3A", "680100270280000000010000000000120000500000000000000001000001000000647890000445177640029316");
                //handler.AddTag("1F4A", "0000006400000007");
                //handler.AddTag("1F5A", "6811002B02000001000001000000647890000445177640000075F800000007000000000100040100000015010218016D16");
            }
            else if (_mProcess == 2)
            {
                handler.AddTag("BF05", PayEntity.ReWriteCardTraceNo1);

            }
            //else if (_mProcess == 3)
            //{
            //    handler.AddTag("BF05", PayEntity.ReWriteCardTraceNo2);
            //}
            handler.AddTag("FF01", _mProcess.ToString());
            handler.AddTag("1F1A", PayEntity.PowerBusiness.ToString());
            handler.AddTag("1F2A", PayEntity.CityPowerNo);
            //handler.AddTag("1F2A", "51401");

            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length + 1];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);

            Array.Copy(Encoding.Default.GetBytes("#"), 0, result, tmp.Length + content.Length, 1);

            return result;
        }

        private byte[] PacketField57()
        {
            string temp = "";

            if (_mProcess == 1)
            {
                temp = PayEntity.PowerCardData.EF3;
                //temp = "680100FC0000430000004300000043000000215000004300000043000000430000002150000043000000430000004300000021500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000009000";
                //temp += PayEntity.PowerCardData.EF3.PadRight(256, ' ');
                //temp += PayEntity.PowerCardData.EF4.PadRight(256, ' ');
            }
            else if (_mProcess == 2)
            {
                temp = PayEntity.PowerCardData.EF4.PadRight(256, ' ');
                //temp = "680100FC000043000000430000004300000021500000430000004300000043000000215000004300000043000000430000002150000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000AB16";
                temp += PayEntity.PowerCardData.CardInfo.PadRight(256, ' ');
                temp += PayEntity.PowerCardData.CardNo.PadRight(16, ' ');
                temp += PayEntity.PowerCardData.Random.PadRight(16, ' ');
            }
            byte[] result = Encoding.Default.GetBytes(temp);

            Log.Debug("End PacketField57 temp:" + temp);
            return result;
            //return Iso8583Package.str2Bcd(temp);// ascii转bcd传出
        }

        protected override void OnSucc()
        {
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());

            if (_mProcess == 1)
            {
                PayEntity.ReWriteCardTraceNo1 = handler.GetStringValue("BF05");
            }
            else if (_mProcess == 2)
            {
                PayEntity.ReWriteCardTraceNo2 = handler.GetStringValue("BF05");
                PayEntity.ReWriteCardReturnMsg = handler.GetStringValue("3F1B");
                PayEntity.PowerCardData.W_EF1 = handler.GetStringValue("3F2B");
                PayEntity.PowerCardData.W_EF2 = handler.GetStringValue("3F3B");
                PayEntity.PowerCardData.W_EF5 = handler.GetStringValue("3F4B");
                if (string.IsNullOrEmpty(handler.GetStringValue("3F5B")))
                {
                    PayEntity.PowerPayCount = int.Parse(handler.GetStringValue("3F5B"));
                }
                PayEntity.ReWriteCardAmount = handler.GetStringValue("3F6B");
                PayEntity.ReWriteUserAmount = handler.GetStringValue("3F7B");
            }
            //PayEntity.ConfirmTraceNo = handler.GetStringValue("BF05");
            //PayEntity.PowerCardData.W_EF2 = handler.GetStringValue("1F38");
            //PayEntity.PowerCardData.W_EF5 = handler.GetStringValue("1F39");

            if (_mProcess != 1)
            {
                byte[] tmp = RecvPackage.GetArrayData(57);
                string temp57 = Encoding.Default.GetString(tmp);// PubFunc.ByteArrayToHexString(tmp, tmp.Length).Trim();
                Log.Debug(temp57);

                if (_mProcess == 2)
                {
                    PayEntity.PowerCardData.W_EF31 = CalcuMsg(temp57.Substring(0, 130));
                    PayEntity.PowerCardData.W_EF32 = CalcuMsg(temp57.Substring(130, 30));
                    PayEntity.PowerCardData.W_EF41 = CalcuMsg(temp57.Substring(160, 130));
                    PayEntity.PowerCardData.W_EF42 = CalcuMsg(temp57.Substring(290, 30));
                    PayEntity.PowerCardData.CertDes = temp57.Substring(320, 16);
                    PayEntity.PowerCardData.LimitDes = temp57.Substring(336, 16);
                    PayEntity.PowerCardData.ExtDes = temp57.Substring(352, 16);

                }
            }
        }

        protected string CalcuMsg(string instr)
        {
            string outstr = "";
            
            if (string.IsNullOrEmpty(instr) && instr.Contains("{") && instr.Contains("}"))
            {
                string[] strs = instr.Split(new char[] { '{', '}' });
                int beginIndex = instr.IndexOf('{');
                int endIndex = instr.IndexOf('}');
                string lenstr = instr.Substring(beginIndex, endIndex - beginIndex);
                if (lenstr != "0" && lenstr != null)
                {
                    for (int i = 0; i < strs.Length; i++)
                    {
                        if (string.Compare(lenstr, strs[i]) == 0)
                        {
                            outstr += "".PadLeft(int.Parse(lenstr), '0');
                        }
                        else
                        {
                            outstr += strs[i];
                        }
                    }
                }
            }
            else
                outstr = instr;
            outstr = outstr.Trim();
            return outstr;
        }
    }
}
