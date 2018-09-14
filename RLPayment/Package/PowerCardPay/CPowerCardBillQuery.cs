using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Package.PowerCardPay
{
    class CPowerCardBillQuery : PowerPay
    {
        private int _currentIndex;

        private StringBuilder _sbData;

        public TransResult BillQuery()
        {
            TransResult result = TransResult.E_RECV_FAIL;
            try
            {
                _sbData = new StringBuilder();
                _currentIndex = 1; //包上送次数，从1开始
                result = Communicate();
                if (result != TransResult.E_SUCC)
                    return result;
                _currentIndex = 2;
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
            SendPackage.SetString(0, "0100");
            SendPackage.SetString(3, "310000");
            SendPackage.SetString(11, GetTraceNo());
            SendPackage.SetString(25, "92"); //服务点条件代码
            SendPackage.SetString(41, GetTerminalNo());
            SendPackage.SetString(42, GetMerchantNo());
            SendPackage.SetArrayData(48, PacketField48());
            SendPackage.SetString(49, "156");
            SendPackage.SetArrayData(57, PacketField57());
            SendPackage.SetString(60, "00" + GetBatchNo() + "000");
        }

        private byte[] PacketField48()
        {
            string temp = "C0V2560265200000" + "123456".PadRight(50, ' ') + "000000";
            byte[] tmp = Encoding.Default.GetBytes(temp);
            TLVHandler handler = new TLVHandler();
            handler.AddTag("1F50", "0004");
            handler.AddTag("FF29", GetTerminalNo());
            handler.AddTag("FF28", GetMerchantNo());
            handler.AddTag("FF01", _currentIndex.ToString());
            handler.AddTag("1F2A", PayEntity.CityPowerNo);
            handler.AddTag("1F1A", PayEntity.PowerBusiness.ToString());      //1---智能电卡 预付费； 2---后付费；

            if (_currentIndex == 1)
            {
                handler.AddTag("1F3A", PayEntity.PowerCardData.EF1);
                handler.AddTag("1F4A", PayEntity.PowerCardData.EF2);
                handler.AddTag("1F5A", PayEntity.PowerCardData.EF5);

            }
            else if (_currentIndex == 2)
            {
                handler.AddTag("BF05", PayEntity.QueryTraceNo);
            }

            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length + 1];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);

            Array.Copy(Encoding.Default.GetBytes("#"), 0, result, tmp.Length + content.Length, 1);

            Log.Debug("End PacketField48");
            return result;
        }

        private byte[] PacketField57()
        {
            string tmp = "";
            //TLVHandler handler = new TLVHandler();
            try
            {
                if (_currentIndex == 1)
                {
                    //for (int i = 0; i < 516; i++)
                    //{

                    //    tmp += "1";
                    //} 
                    //handler.AddTag("5FF2", PayEntity.PowerCardData.EF2);
                    //handler.AddTag("5FF3", PayEntity.PowerCardData.EF3);
                    //handler.AddTag("5FF3", PayEntity.PowerCardData.EF4);
                    //handler.AddTag("5FF1", "123");
                    //handler.AddTag("5FF2", "456");
                    //handler.AddTag("5FF3", "789");
                    tmp = PayEntity.PowerCardData.EF3;
                    //tmp += PayEntity.PowerCardData.EF3 + "|";
                    //tmp += PayEntity.PowerCardData.EF4;
                }
                else if (_currentIndex == 2)
                {
                    tmp = PayEntity.PowerCardData.EF4;
                }
            }
            catch (Exception ex)
            {
                Log.Error("packet 57 file err : ", ex);
            }
            //byte[] result = handler.GetTLVWithLength(3);

            byte[] result = Encoding.Default.GetBytes(tmp);

            Log.Debug("End PacketField57 temp:" + tmp);
            return result;

        }



        protected override void OnSucc()
        {
            byte[] tmp = RecvPackage.GetArrayData(48);
            PayEntity.UserID = Encoding.Default.GetString(tmp, 16, 50);


            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());
            if (_currentIndex == 1)
            {
                PayEntity.QueryTraceNo = handler.GetStringValue("BF05");
            }
            else if (_currentIndex == 2)
            {
                PayEntity.QueryTraceNo = handler.GetStringValue("BF05");
                PayEntity.PowerReturnMsg = handler.GetStringValue("1F1B");
                PayEntity.UserName = handler.GetStringValue("1F2B");
                PayEntity.UserAddress = handler.GetStringValue("1F3B");
                PayEntity.PowerAdvName = handler.GetStringValue("1F4B");
                PayEntity.PowerCardNo = handler.GetStringValue("1F5B");
                PayEntity.EleFeeAccountName = handler.GetStringValue("1F6B");
                PayEntity.PowerAreaNum = handler.GetStringValue("1F7B");
                PayEntity.EleFeeNum = handler.GetStringValue("1F8B");
                PayEntity.PowerIdentity = handler.GetStringValue("1F9B");
                if (handler.GetStringValue("1F0B") != "")
                {
                    PayEntity.PowerPayCount = int.Parse(handler.GetStringValue("1F0B"));
                }
                if (handler.GetStringValue("1F1C") != "")
                {
                    PayEntity.PayAmount = double.Parse(handler.GetStringValue("1F1C")) / 100;
                }
            }
        }

        protected override void OnHostFail(string returnCode, string returnMessage)
        {
            base.OnHostFail(returnCode,returnMessage);
            try
            {
                TLVHandler handler = new TLVHandler();
                handler.ParseTLV(Get48TLVBytes());
                ReturnMessage = handler.GetStringValue("FF52");
                if (ReturnCode == "TH")
                {
                    ReturnMessage = handler.GetStringValue("1F1B");
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Query HostFail" + ex.Message);
            }
        }
    }
}
