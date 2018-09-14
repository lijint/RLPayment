using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Package.PowerCardPay
{
    class CPowerCardUserBillQuery: PowerPay
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
            string temp = "C6V2560265000000" + PayEntity.DBNo.PadRight(50,' ') + "000000";
            byte[] tmp = Encoding.Default.GetBytes(temp);
            TLVHandler handler = new TLVHandler();
            handler.AddTag("1F50", "0004");
            handler.AddTag("FF28", GetMerchantNo());
            handler.AddTag("FF29", GetTerminalNo());
            handler.AddTag("1F1A", GetBranchNo());// PayEntity.CityPowerNo);
            handler.AddTag("1F2A", GetOperatorNo());// PayEntity.CityPowerNo);    

            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length + 1];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);

            Array.Copy(Encoding.Default.GetBytes("#"), 0, result, tmp.Length + content.Length, 1);

            return result;
        }

        protected override void OnHostFail(string returnCode, string returnMessage)
        {
            base.OnHostFail(returnCode, returnMessage);
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());
            ReturnMessage = handler.GetStringValue("1FAB");
        }

        protected override void OnSucc()
        {
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());

            PayEntity.QueryTraceNo = handler.GetStringValue("BF05");
            PayEntity.EleFeeAccountNum = handler.GetStringValue("1F2B");
            PayEntity.EleFeeNum = handler.GetStringValue("1F1B");
            PayEntity.UserID = handler.GetStringValue("1F3B");
            PayEntity.UserName = handler.GetStringValue("1F4B");
            PayEntity.UserAddress = handler.GetStringValue("1F5B");       
            PayEntity.UserPayAmount = double.Parse(handler.GetStringValue("1F6B")) / 100;
            PayEntity.PayAmount = double.Parse(handler.GetStringValue("1F7B")) / 100;

            string info = handler.GetStringValue("1F8B");
            PayEntity.UserQueryInfo = new List<UserQueryInfo>();
            string[] data = info.Split(new string[]{"&;"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in data)
            {
                PayEntity.UserQueryInfo.Add(new UserQueryInfo(item));
            }
           
        }
    }
}
