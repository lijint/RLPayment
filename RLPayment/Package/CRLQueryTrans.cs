using Landi.FrameWorks.Package.Other;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Package
{
    class CRLQueryTrans : SocketCommunicate
    {
        public RLCZEntity _entity;
        private string TransCode;

        CRLQueryTrans(RLCZEntity entity)
        {
            if (entity != null)
                _entity = entity;
            HeadLength = 4;
            TransCode = "01";
        }

        protected override void Packet()
        {
            SendPackage = "";
            SendPackage += TransCode;
            SendPackage += _entity.BANKCODE.PadLeft(2, ' ');
            SendPackage += _entity.BUSSINESSCODE.PadLeft(8, ' ');
            SendPackage += _entity.GUICODE.PadLeft(16, ' ');
            SendPackage += _entity.CardNO.PadLeft(8, ' ');

        }
        protected override bool UnPacket()
        {
            return base.UnPacket();
        }
        protected override byte[] PacketHead(byte[] SendBytes)
        {
            int headLength = HeadLength;
            int totalLength = SendBytes.Length;
            byte[] head = new byte[headLength];
            byte[] send_all = new byte[headLength + totalLength];

            for (int i = headLength; i > 0; i--)
            {
                head[i - 1] = (byte)(totalLength % 256);
                totalLength = totalLength / 256;
            }
            Array.Copy(head, send_all, headLength);
            Array.Copy(SendBytes, 0, send_all, headLength, SendBytes.Length);
            return send_all;
        }
    }
}
