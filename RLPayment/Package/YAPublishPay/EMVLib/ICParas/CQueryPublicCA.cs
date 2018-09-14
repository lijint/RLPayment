using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Landi.FrameWorks;
using System.IO;

namespace YAPayment.Package
{
    public struct CaItem
    {
        /// <summary>
        /// RID
        /// </summary>
        public byte[] cRid;
        /// <summary>
        /// 索引
        /// </summary>
        public byte[] cIndex;
        /// <summary>
        /// 有效期
        /// </summary>
        public byte[] cDate;
    }

    /// <summary>
    /// field62[0]:0没有公钥信息，1已经存放了所有公钥信息，2分多次，3结束:RID,索引,有效期
    /// </summary>
    class CQueryPublicCA : YAPaymentPay
    {
        public byte BField63 = 0;
        public string StrField62 = "100";
        public List<CaItem> CaItemList = new List<CaItem>();
        public static List<CaItem> TotalCaItemList = new List<CaItem>();

        /// <summary>
        /// 同步下载IC卡公钥入口
        /// </summary>
        /// <returns></returns>
        public static bool ICPublicCAEntry()
        {
            //Log.Info("========开始下载IC卡公钥========");
            TotalCaItemList.Clear();
            bool bResult = false;
            byte bContinue = 0x30;
            CQueryPublicCA qca = new CQueryPublicCA();
            TransResult ret = qca.Communicate();
            if (ret == TransResult.E_SUCC)
            {
                bResult = true;
                bContinue = qca.BField63;
                while ((ret == TransResult.E_SUCC) && (bContinue == 0x32))
                {
                    System.Threading.Thread.Sleep(2000);//四川版需要停顿
                    CQueryPublicCA qca2 = new CQueryPublicCA();
                    qca2.StrField62 = "1" + TotalCaItemList.Count.ToString().PadLeft(2, '0');
                    ret = qca2.Communicate();
                    if (ret == TransResult.E_SUCC)
                    {
                        bContinue = qca2.BField63;
                    }
                    else
                    {
                        bResult = false;
                    }
                }
                if (bResult && TotalCaItemList.Count > 0)
                {
                    System.Threading.Thread.Sleep(2000);//四川版需要停顿
                    bResult = CDownPublicCA.ICDownPublicCA(TotalCaItemList);
                    if (bResult)
                    {
                        CEndPublicCA eca = new CEndPublicCA();
                        eca.Communicate();
                    }
                }
            }
            Log.Warn("IC卡公钥下载参数CQueryPublicCA：Ret=" + ret.ToString());
            //Log.Info("========结束下载IC卡公钥========");
            return bResult;
        }

        protected override void Packet()
        {
            SendPackage.SetString(0, "0820");
            SendPackage.SetString(60, "00" + GetBatchNo() + "372");
            SendPackage.SetArrayData(63, Encoding.Default.GetBytes(StrField62), 0, 3);
        }

        protected override void OnSucc()
        {
            SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //记录批次号
            byte[] bField63 = RecvPackage.GetArrayData(63);
            BField63 = bField63[0];
            GetCARID(bField63);
            TotalCaItemList.AddRange(CaItemList);
            //if (BField63 == 0x32)
            //{
            //    CQueryPublicCA qCA = new CQueryPublicCA();
            //    qCA.StrField62 = "1" + TotalCaItemList.Count.ToString().PadLeft(2, '0');
            //    EnqueueWork(qCA);
            //    mCaItemCount++;
            //}
            //else
            //{
            //    if (mCaItemCount == TotalCaItemList.Count)
            //    {
            //        IsQuerySucess = CDownPublicCA.ICDownPublicCA(TotalCaItemList);
            //    }
            //}
        }

        /// <summary>
        /// 0没有公钥信息，1已经存放了所有公钥信息，2分多次，3结束:RID,索引,有效期
        /// </summary>
        /// <param name="field62"></param>
        /// <returns></returns>
        private void GetCARID(byte[] field62)
        {
            if (field62[0] == 0x30)
            {
                return;
            }
            byte[] bCa = new byte[field62.Length - 1];
            Array.Copy(field62, 1, bCa, 0, bCa.Length);
            int nOffset = 0;
            if (field62[0] == 0x31 || field62[0] == 0x32 || field62[0] == 0x33)
            {
                for (; nOffset < bCa.Length; )
                {
                    CaItem caItem = new CaItem();
                    if (bCa[nOffset] == 0x9F && bCa[nOffset + 1] == 0x06)
                    {
                        nOffset += 2;//TAG 9F06
                        int ridLen = bCa[nOffset];//LEN
                        caItem.cRid = new byte[ridLen];
                        nOffset += 1;
                        Array.Copy(bCa, nOffset, caItem.cRid, 0, ridLen);//RID
                        nOffset = nOffset + ridLen;

                        nOffset += 2;//TAG 9F22
                        int indexLen = bCa[nOffset];//LEN
                        caItem.cIndex = new byte[indexLen];
                        nOffset += 1;
                        Array.Copy(bCa, nOffset, caItem.cIndex, 0, indexLen);//INDEX
                        nOffset = nOffset + indexLen;

                        nOffset += 2;//TAG DF05
                        int dateLen = bCa[nOffset];//LEN
                        caItem.cDate = new byte[dateLen];
                        nOffset += 1;
                        Array.Copy(bCa, nOffset, caItem.cDate, 0, dateLen);//DATE
                        nOffset = nOffset + dateLen;

                        CaItemList.Add(caItem);
                    }
                    else
                        nOffset++;
                }
            }
        }

        protected override bool NeedCalcMac()
        {
            return false;
        }
    }
}
