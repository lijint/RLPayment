﻿using Landi.FrameWorks;
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
                _entity.CardNO = RecvPackage.Substring(2, 8);
                _entity.HOTUSERID = RecvPackage.Substring(10, 10);
                _entity.Addr = RecvPackage.Substring(20, 64);
                _entity.UserName = RecvPackage.Substring(84, 64);
                _entity.CompanyCode = RecvPackage.Substring(148, 2);
                _entity.PastBalance = double.Parse(RecvPackage.Substring(150, 11).Trim());
                _entity.TotalArrears = double.Parse(RecvPackage.Substring(161, 11).Trim());
                int times = int.Parse(RecvPackage.Substring(172, 2).Trim());
                if (times > 0)
                {
                    int pos = 174;
                    for (int i = 0; i < times; i++)
                    {
                        UserInfo u = new UserInfo();
                        u.FeeType = RecvPackage.Substring(pos, 16);
                        pos += 16;
                        u.HeatingPeriod = RecvPackage.Substring(pos, 9);
                        pos += 9;
                        u.Area = double.Parse(RecvPackage.Substring(pos, 11));
                        pos += 11;
                        u.Price = double.Parse(RecvPackage.Substring(pos, 7));
                        pos += 7;
                        u.ReceivableAmount = double.Parse(RecvPackage.Substring(pos, 11));
                        pos += 11;
                        u.amountOwed = double.Parse(RecvPackage.Substring(pos, 11));
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
