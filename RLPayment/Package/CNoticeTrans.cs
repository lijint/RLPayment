using Landi.FrameWorks;
using Landi.FrameWorks.Package.Other;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Package
{
    public class CNoticeTrans: SocketCommunicate
    {
        public RLCZEntity _entity;
        private string TransCode;

        public CNoticeTrans(RLCZEntity entity)
        {
            if (entity != null)
                _entity = entity;
            HeadLength = 4;
            TransCode = "02";
        }

        protected override byte[] Packet()
        {
            _entity.HOTBILLTYPE = "电子凭证";
            _entity.HOTBILLNO = _entity.OrderNumber;
            if(_entity.PayType==0)
            {
                _entity.HOTPAYTYPE = "网上银行";
            }
            else if(_entity.PayType==1)
            {
                _entity.HOTPAYTYPE = "微信";
            }
            else if(_entity.PayType==2)
            {
                _entity.HOTPAYTYPE = "支付宝";
            }

            SendPackage = "";
            SendPackage += TransCode;
            SendPackage += _entity.BANKCODE.PadLeft(2, ' ');
            SendPackage += _entity.BUSSINESSCODE.PadLeft(8, ' ');
            SendPackage += _entity.GUICODE.PadLeft(16, ' ');
            SendPackage += _entity.CardNO.PadLeft(10, ' ');
            SendPackage += "".PadLeft(16 - GetLength(_entity.HOTBILLTYPE), ' ') + _entity.HOTBILLTYPE;
            SendPackage += _entity.HOTBILLNO.PadLeft(32, ' ');
            SendPackage += _entity.OrderNumber.PadLeft(32, ' ');
            SendPackage += "".PadLeft(16 - GetLength(_entity.HOTPAYTYPE), ' ') + _entity.HOTPAYTYPE;
            SendPackage += string.Format("{0:F2}", _entity.Amount).PadLeft(11, '0');
            SendPackage += _entity.bBankBackTransDateTime;
            if (SendPackage == null)
                return null;
            return Encoding.GetEncoding("GBK").GetBytes(SendPackage);

        }
        protected override bool UnPacket(byte[] recv_all)
        {
            RecvPackage = Encoding.GetEncoding("GBK").GetString(recv_all);
            Log.Info("recv packet : " + RecvPackage);

            bool ret = false;
            if (string.IsNullOrEmpty(RecvPackage))
                return ret;
            try
            {
                _entity.ReturnCode = RecvPackage.Substring(0, 2);
                if (_entity.ReturnCode != "00")
                {
                    _entity.ReturnMsg = _entity.GetReturnMsg(_entity.ReturnCode);
                    return ret;
                }
                ret = true;
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
            return ret;
        }
        protected override byte[] PacketHead(byte[] SendBytes)
        {
            string sendStr = Encoding.GetEncoding("GBK").GetString(SendBytes);
            string sendlen = Convert.ToString(sendStr.Length, 16).PadLeft(4, '0');
            string sendallstr = sendlen + sendStr;
            return Encoding.GetEncoding("GBK").GetBytes(sendallstr);

            //int headLength = HeadLength;
            //int totalLength = SendBytes.Length;
            //byte[] head = new byte[headLength];
            //byte[] send_all = new byte[headLength + totalLength];

            //for (int i = headLength; i > 0; i--)
            //{
            //    head[i - 1] = (byte)(totalLength % 256);
            //    totalLength = totalLength / 256;
            //}
            //Array.Copy(head, send_all, headLength);
            //Array.Copy(SendBytes, 0, send_all, headLength, SendBytes.Length);
            //return send_all;
        }
        protected override int UnPacketHead(byte[] recvHead)
        {
            string len16str = Encoding.GetEncoding("GBK").GetString(recvHead);
            int len10 = Convert.ToInt32(len16str, 16);
            return len10;
        }

        public static int GetLength(string str)
        {
            return Encoding.GetEncoding("GBK").GetBytes(str).Length;
        }

    }
}
