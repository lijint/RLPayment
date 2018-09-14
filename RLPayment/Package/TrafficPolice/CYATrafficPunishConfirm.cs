using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Package.TrafficPolice
{
    class CYATrafficPunishConfirm : YAPaymentPay
    {
        private string _index;//单笔序号
        private string _cjjgid;//采集机关
        private string _jdsbh;
        /// <summary>
        /// 决定书编号
        /// </summary>
        public string Jdsbh
        {
            get
            {
                return _jdsbh;
            }
        }
        public CYATrafficPunishConfirm(string index,string cjjgid)
        {
            _index = index;
            _cjjgid = cjjgid;
        }

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
            string temp = "C3V2571165010000" + "".PadLeft(50, ' ') + "000000";
            byte[] tmp = Encoding.Default.GetBytes(temp);
            TLVHandler handler = new TLVHandler();
            handler.AddTag("1F50", "0004");
            handler.AddTag("FF28", GetMerchantNo());
            handler.AddTag("FF29", GetTerminalNo());
            handler.AddTag("2F1A", _index);
            handler.AddTag("2F2A", _cjjgid);
            handler.AddTag("2F3A", PayEntity.LicenseNo);

            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length + 1];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);

            Array.Copy(Encoding.Default.GetBytes("#"), 0, result, tmp.Length + content.Length, 1);

            return result;
        }

        protected override void OnSucc()
        {
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());

            _jdsbh = handler.GetStringValue("1F2B");

        }
    }
}
