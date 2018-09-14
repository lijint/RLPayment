using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;

namespace Landi.FrameWorks.ChinaUnion
{
    public abstract class ChinaUnionPay : PackageBase
    {
        protected ChinaUnionPay()
        {

        }

        protected ChinaUnionPay(PackageBase pb)
            : base(pb)
        {

        }

        protected override bool NeedCalcMac()
        {
            if (SendPackage.GetString(0) == "0800")
                return false;
            else
                return true;
        }

        protected override byte[] PackBytesAtFront(int dataLen)
        {
            int sendLen_all = dataLen + 13;
            byte[] sendstr_all = new byte[sendLen_all];

            byte[] before = new byte[13];

            //长度位
            before[0] = (byte)((sendLen_all - 2) / 256);
            before[1] = (byte)((sendLen_all - 2) % 256);

            //TPDU
            byte[] TPDU = new byte[5];
            TPDU = Utility.str2Bcd(GetTPDU());
            Array.Copy(TPDU, 0, before, 2, 5);

            //包头
            byte[] head = new byte[12];
            head = Utility.str2Bcd(GetHead());
            Array.Copy(head, 0, before, 7, 6);

            return before;
        }

        protected override bool UnPackFix()
        {
            string returnCode = RecvPackage.GetString(39);
            string msgMean = "", msgShow = "";
            ParseRespMessage(returnCode, ref msgMean, ref msgShow);

            SetRespInfo(returnCode, msgMean, msgShow);
            if (returnCode == "00")
                return true;
            else
                return false;
        }

        protected override void PackFix()
        {
            SendPackage.SetString(41, GetTerminalNo());
            SendPackage.SetString(42, GetMerchantNo());
        }

        protected void PackReverse(string reason)
        {
            SendPackage.SetString(0, "0400");
            if (String.IsNullOrEmpty(reason))
            {
                reason = "06";
            }
            SendPackage.SetString(39, reason); //冲正原因
            SendPackage.ClearBitAndValue(26);
            SendPackage.ClearBitAndValue(52);
            SendPackage.ClearBitAndValue(53);
            SendPackage.ClearBitAndValue(64);
        }

        /// <summary>
        /// 冲正使用的55域
        /// </summary>
        protected byte[] GetICAutoField55(byte[] _field55, int fieldLen)
        {
            //95 9F1E 9F10 9F36 DF31
            byte[] field55 = new byte[fieldLen];
            Array.Copy(_field55, field55, fieldLen);
            TLVHandler tlv = new TLVHandler();
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(field55);
            byte[] value = new byte[0];

            #region 打包Field55

            if ((value = handler.GetBytesValue("95")) != null)
            {
                tlv.AddTag("95", value);
            }
            if ((value = handler.GetBytesValue("9F1E")) != null)
            {
                tlv.AddTag("9F1E", value);
            }
            if ((value = handler.GetBytesValue("9F10")) != null)
            {
                tlv.AddTag("9F10", value);
            }
            if ((value = handler.GetBytesValue("9F36")) != null)
            {
                tlv.AddTag("9F36", value);
            }
            if ((value = handler.GetBytesValue("DF31")) != null)
            {
                tlv.AddTag("DF31", value);
            }
            #endregion

            return tlv.GetTLV();
        }

        protected void DoSignInSucc()
        {
            string time = RecvPackage.GetString(12); //时间
            string date = RecvPackage.GetString(13); //日期
            SetBatchNo(RecvPackage.GetString(60).Substring(2, 6));//记录批次号

            byte[] bField62 = new byte[0];
            bField62 = Utility.str2Bcd(RecvPackage.GetString(62));

            byte[] EPinkey = new byte[KeyLength];
            byte[] EMackey = new byte[KeyLength];
            switch (DType)
            {
                case DesType.Des:
                    Array.Copy(bField62, 0, EPinkey, 0, KeyLength);
                    Array.Copy(bField62, 12, EMackey, 0, KeyLength);
                    break;
                case DesType.TripleDes:
                    Array.Copy(bField62, 0, EPinkey, 0, KeyLength);
                    Array.Copy(bField62, 20, EMackey, 0, 8);
                    Array.Copy(bField62, 20, EMackey, 8, 8);
                    break;
            }
            KeyManager.SetEnMacKey(SectionName, EMackey);
            KeyManager.SetEnPinKey(SectionName, EPinkey);
            //Log.Debug("MackeyEn:" + Utility.bcd2str(EMackey, EMackey.Length));
            //Log.Debug("PinkeyEn:" + Utility.bcd2str(EPinkey, EPinkey.Length));

            byte[] PinKey = null, WorkKey = null;
            if (EnType == EncryptType.Soft)
            {
                byte[] MasterKey = GetSoftMasterKey();
                if (DType == DesType.Des)
                {
                    PinKey = Encrypt.DESDecrypt(EPinkey, MasterKey);
                    WorkKey = Encrypt.DESDecrypt(EMackey, MasterKey);
                }
                else if (DType == DesType.TripleDes)
                {
                    PinKey = Encrypt.DES3Decrypt(EPinkey, MasterKey);
                    WorkKey = Encrypt.DES3Decrypt(EMackey, MasterKey);
                }
            }
            else
            {
                PinKey = new byte[KeyLength];
                WorkKey = new byte[KeyLength];
                Esam.SetWorkmode(Esam.WorkMode.Encrypt);

                Esam.UserDecrypt(GetKeyIndex(), EPinkey, KeyLength, PinKey);
                Esam.UserDecrypt(GetKeyIndex(), EMackey, KeyLength, WorkKey);
                Esam.SetWorkmode(Esam.WorkMode.Default);
            }
            KeyManager.SetDeMacKey(SectionName, WorkKey);
            KeyManager.SetDePinKey(SectionName, PinKey);

            if (!CheckKeyValue())
            {
                SetResult(TransResult.E_KEYVERIFY_FAIL);
            }
            else
            {
                //更新当前机器时间
                int year = System.DateTime.Now.Year;
                int month = Convert.ToInt32(date.Substring(0, 2));
                int day = Convert.ToInt32(date.Substring(2, 2));
                int hour = Convert.ToInt32(time.Substring(0, 2));
                int mi = Convert.ToInt32(time.Substring(2, 2));
                int ss = Convert.ToInt32(time.Substring(4, 2));
                DateTime newDt = new DateTime(year, month, day, hour, mi, ss);
#if !DEBUG

                Utility.SetSysTime(newDt);
#endif
            }
        }

        private bool CheckKeyValue()
        {
            try
            {
                byte[] bField62 = new byte[0];
                bField62 = Utility.str2Bcd(RecvPackage.GetString(62));

                byte[] pinKeyValue = new byte[8];
                byte[] macKeyValue = new byte[8];
                byte[] calcData = new byte[8];
                for (int iPer = 0; iPer < 8; iPer++)
                {
                    calcData[iPer] = 0x00;
                }

                bool pRet = CalcMacByPinkey(calcData, pinKeyValue);
                bool mRet = CalcMacByMackey(calcData, macKeyValue);
                if (pRet && mRet)
                {
                    byte[] pinCheckValue = new byte[4];
                    byte[] macCheckValue = new byte[4];
                    byte[] pinCheckValue_calc = new byte[4];
                    byte[] macCheckValue_calc = new byte[4];
                    switch (DType)
                    {
                        case DesType.Des:
                            Array.Copy(bField62, 8, pinCheckValue, 0, 4);
                            Array.Copy(bField62, 20, macCheckValue, 0, 4);
                            break;
                        case DesType.TripleDes:
                            Array.Copy(bField62, 16, pinCheckValue, 0, 4);
                            Array.Copy(bField62, 36, macCheckValue, 0, 4);//28
                            break;
                    }
                    Array.Copy(pinKeyValue, pinCheckValue_calc, 4);
                    Array.Copy(macKeyValue, macCheckValue_calc, 4);
                    bool checkPinRet = Utility.ByteEquals(pinCheckValue, pinCheckValue_calc);
                    bool checkMacRet = Utility.ByteEquals(macCheckValue, macCheckValue_calc);
                    if (!checkPinRet)
                        Log.Warn("[CheckKeyValue]PIN Key Check Failed!");
                    if (!checkMacRet)
                        Log.Warn("[CheckKeyValue]MAC Key Check Failed!");

                    if (checkPinRet && checkMacRet)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Log.Error("[CheckKeyValue]Error!", e);
                return false;
            }
        }
    }
}
