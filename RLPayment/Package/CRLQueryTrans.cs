using Landi.FrameWorks;
using Landi.FrameWorks.Package.Other;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Package
{
    public class CRLQueryTrans : SocketCommunicate
    {
        public RLCZEntity _entity;
        private string TransCode;

        public CRLQueryTrans(RLCZEntity entity)
        {
            if (entity != null)
                _entity = entity;
            HeadLength = 4;
            TransCode = "01";
        }

        protected override byte[] Packet()
        {
            SendPackage = "";
            SendPackage += TransCode;
            SendPackage += _entity.BANKCODE.PadLeft(2, ' ');
            SendPackage += _entity.BUSSINESSCODE.PadLeft(8, ' ');
            SendPackage += _entity.GUICODE.PadLeft(16, ' ');
            SendPackage += _entity.CardNO.PadLeft(8, ' ');
            if (SendPackage == null)
                return null;
            return Encoding.GetEncoding("GBK").GetBytes(SendPackage);
            //return Encoding.UTF8.GetBytes(SendPackage);
        }
        protected override bool UnPacket(byte[] recv_all)
        {
            int bLen = recv_all.Length;

            //RecvPackage = Encoding.UTF8.GetString(recv_all);
            RecvPackage = Encoding.GetEncoding("GBK").GetString(recv_all);
            Log.Info("recv packet : " + RecvPackage);

            bool ret = false;
            if (string.IsNullOrEmpty(RecvPackage))
                return ret;
            try
            {
                int pos = 0;
                _entity.ReturnCode = getEveryField(pos, recv_all, 2);
                pos += 2;
                if (_entity.ReturnCode != "00")
                {
                    _entity.ReturnMsg = _entity.GetReturnMsg(_entity.ReturnCode);
                    return ret;
                }

                _entity.CardNO = getEveryField(pos, recv_all, 8);
                pos += 8;
                _entity.Addr = getEveryField(pos, recv_all, 64);
                pos += 64;
                _entity.UserName = getEveryField(pos, recv_all, 64);
                pos += 64;
                _entity.CompanyCode = getEveryField(pos, recv_all, 2);
                pos += 2;
                _entity.PastBalance = double.Parse(getEveryField(pos, recv_all, 11).Trim());
                pos += 11;
                _entity.TotalArrears = double.Parse(getEveryField(pos, recv_all, 11).Trim());
                pos += 11;
                int times = int.Parse(getEveryField(pos, recv_all, 2).Trim());
                pos += 2;
                if (times > 0)
                {
                    for (int i = 0; i < times; i++)
                    {
                        UserInfo u = new UserInfo();
                        u.FeeType = getEveryField(pos, recv_all, 16);
                        pos += 16;
                        u.HeatingPeriod = getEveryField(pos, recv_all, 9);
                        pos += 9;
                        u.Area = double.Parse(getEveryField(pos, recv_all, 11));
                        pos += 11;
                        u.Price = double.Parse(getEveryField(pos, recv_all, 7));
                        pos += 7;
                        u.ReceivableAmount = double.Parse(getEveryField(pos, recv_all, 11));
                        pos += 11;
                        u.amountOwed = double.Parse(getEveryField(pos, recv_all, 11));
                        pos += 11;

                        _entity.userInfoList.Add(u);
                    }
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
            string sendlen = Convert.ToString(sendStr.Length, 16).PadLeft(4,'0');
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
            Log.Info("recv packet len : " + len10);

            return len10;
        }

        private string getEveryField(int pos,byte[] recv,int len)
        {
            byte[] bcode = new byte[len];
            Array.Copy(recv, pos, bcode, 0, len);
            return Encoding.GetEncoding("GBK").GetString(bcode);
        }
    }
}
