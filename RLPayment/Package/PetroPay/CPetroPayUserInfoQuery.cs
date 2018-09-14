using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using YAPayment.Package;
using Landi.FrameWorks;

namespace YAPayment.Package.PetroPay
{
    class CPetroPayUserInfoQuery : YAPaymentPay
    {
        private byte[] PacketField48()
        {
            string field48 = "C2v22901510000" + "".PadLeft(40, ' ') + "000000" + "2";
            byte[] tmp = Encoding.Default.GetBytes(field48);
            
            //Test
            string loginName = LoginName;
            string loginPsd = LoginPsd;

#if DEBUG
            //loginName = "FW050106Z002010" + loginName;
            //loginName = "FW0100020";
            //loginName = "FW010101Z00501010" + loginName;
            loginName = "FW060101B00901100" + loginName;
            loginPsd = "ODg4OA==";
#endif
            TLVHandler handler = new TLVHandler();
            handler.AddTag("FF44", loginName);
            handler.AddTag("FF45", loginPsd);
            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);
            return result;
        }

        protected override void Packet()
        {
            SendPackage.SetString(0, "0100");
            SendPackage.SetString(3, "380000");
            SendPackage.SetString(11, GetTraceNo());
            SendPackage.SetString(25, "00"); //服务点条件代码
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
            //48域
            UserID = Encoding.Default.GetString(RecvPackage.GetArrayData(48), 14, 40);
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());

            UserName = handler.GetStringValue("2F1A");
            UserNo = handler.GetStringValue("BF06");
            UserTel = handler.GetStringValue("BF07");
            IDCard = handler.GetStringValue("BF08");
            Sex = handler.GetStringValue("BF09") == "0" ? "男" : "女";
            RecordCount = int.Parse(handler.GetStringValue("BF10"));
            RecordInfo = handler.GetStringValue("BF11");
        }
    }
}
