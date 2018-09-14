using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace YAPayment.Package.PowerCardPay
{
    class CPowerCardBillCheck : PowerPay
    {
        private int _currentIndex;

        private int _count = 1;//返回包数


        public TransResult BillCheck()
        {
            TransResult result = TransResult.E_RECV_FAIL;
            try
            {
                _currentIndex = 1;//包上送次数，从1开始
                while (_currentIndex <= _count)  //当包数量大于1时，需再次发送请求报文。
                {
                    result = Communicate();
                    if (result != TransResult.E_SUCC)
                        return result;
                    _currentIndex++;
                }
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
            //SendPackage.SetString(2, CommonData.BankCardNum); //c
            //if (!string.IsNullOrEmpty(CommonData.BankCardNum) && CommonData.BankCardNum.Length != 0)
            //{
            //    SendPackage.SetString(2, CommonData.BankCardNum);
            //}
            SendPackage.SetString(3, "310000");
            //SendPackage.SetString(4, Utility.AmountToString(CommonData.Amount.ToString()));
            SendPackage.SetString(11, GetTraceNo());
            SendPackage.SetString(12, DateTime.Now.ToString("HHmmss"));
            SendPackage.SetString(13, DateTime.Now.ToString("MMdd"));
            SendPackage.SetString(25, "92");

            //SendPackage.SetString(37, PayEntity.PayReferenceNo);
            SendPackage.SetString(41, GetTerminalNo());
            SendPackage.SetString(42, GetMerchantNo());

            SendPackage.SetArrayData(48, PacketField48());
            SendPackage.SetString(49, "195");
            //SendPackage.SetArrayData(57, PacketField57());
            //SendPackage.SetString(57, (PayEntity.PowerCardData.EF31 + PayEntity.PowerCardData.EF32).PadRight(999, ' '));
            SendPackage.SetString(60, "00" + GetBatchNo() + "000");
        }

        //private byte[] PacketField57()
        //{
            //string temp = "";
            //if (_currentIndex == 1)
            //{
            //    temp = (PayEntity.PowerCardData.EF31 + PayEntity.PowerCardData.EF32).PadRight(999, ' ');
            //}
            //else if (_currentIndex == 2)
            //{
            //    temp = (PayEntity.PowerCardData.EF41 + PayEntity.PowerCardData.EF42).PadRight(999, ' ');
            //}
            //byte[] tmp = Encoding.Default.GetBytes(temp);
            //return tmp;
        //}


        private byte[] PacketField48()
        {
            string temp = "C1V2560265200000" + " ".PadRight(50, ' ') + "000000";
            byte[] tmp = Encoding.Default.GetBytes(temp);
            TLVHandler handler = new TLVHandler();
            handler.AddTag("1F50", "0004");
            handler.AddTag("FF28", GetMerchantNo());
            handler.AddTag("FF29", GetTerminalNo());

            //Log.Info("PayEntity.PayFlowNo:" + PayEntity.PayFlowNo);
            //handler.AddTag("2F2B", _currentIndex.ToString());//包上送次数
            if (_currentIndex == 1)
            {
                handler.AddTag("1F1A", PayEntity.PowerPayConfirmCode);
            }
            //else if (_currentIndex == 2)
            //{
            //    handler.AddTag("BF05", PayEntity.CheckTraceNo);
            //}
            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length + 1];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);

            Array.Copy(Encoding.Default.GetBytes("#"), 0, result, tmp.Length + content.Length, 1);

            return result;
        }


        protected override void OnRecvFail()
        {
            base.OnRecvFail();
            if (_currentIndex == 2)
            {
                PayEntity.ConfirmTraceNo = "";
            }
        }

        protected override void OnHostFail(string returnCode, string returnMessage)
        {
            base.OnHostFail(returnCode, returnMessage);
            if (_currentIndex == 2)
            {
                PayEntity.ConfirmTraceNo = "";
            }
        }

        protected override void OnSucc()
        {
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());
            if (_currentIndex == 1)
            {
                PayEntity.CheckTraceNo = handler.GetStringValue("BF05");
                PayEntity.CheckReturnMsg = handler.GetStringValue("1F1B");
                PayEntity.CheckRechargeAmount = handler.GetStringValue("1F2B");
                PayEntity.CheckRemuneration = handler.GetStringValue("1F3B");
                PayEntity.CheckOrderStatus = handler.GetStringValue("1F4B");
                //PayEntity.PowerCardData.EF1 = handler.GetStringValue("3F5B");
                //PayEntity.PowerCardData.EF2 = handler.GetStringValue("3F6B");
                //PayEntity.PowerCardData.EF5 = handler.GetStringValue("3F7B");
                //PayEntity.CheckBuyEleTimes = handler.GetStringValue("3F8B");
                //PayEntity.CheckWriteAmount = handler.GetStringValue("3F9B");
                //PayEntity.CheckUserAmount = handler.GetStringValue("3F10");
            }

            //byte[] tmp = RecvPackage.GetArrayData(57);
            //string temp57 = PubFunc.ByteArrayToHexString(tmp, tmp.Length).Trim();
            //Log.Debug(temp57);

            //if (_currentIndex == 1)
            //{
            //    PayEntity.CheckBF31 = temp57.Substring(0, 256);
            //}
            //else if (_currentIndex == 2)
            //{
            //    PayEntity.CheckBF32 = temp57.Substring(0, 256);
            //    PayEntity.CheckBF41 = temp57.Substring(256, 256);
            //    PayEntity.CheckBF42 = temp57.Substring(512, 256);
            //    PayEntity.Check57sKey1 = temp57.Substring(768, 16);
            //    PayEntity.Check57sKey2 = temp57.Substring(784, 16);
            //    PayEntity.Check57sKey3 = temp57.Substring(800, 16);
            //}

        }
    }
}
