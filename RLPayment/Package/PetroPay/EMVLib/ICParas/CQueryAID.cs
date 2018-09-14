using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Landi.FrameWorks;
using System.IO;

namespace YAPayment.Package
{
    /// <summary>
    /// field62[0]:0没有信息，1已经存放了所有信息，2分多次，3结束:AID1,AID2
    /// </summary>
    class CQueryAID : YAPaymentPay
    {
        public byte BField62 = 0;
        public string StrField62 = "100";
        public List<byte[]> AIDItemList = new List<byte[]>();

        /// <summary>
        /// 同步下载IC卡AID入口
        /// </summary>
        /// <returns></returns>
        public static bool ICAIDEntry()
        {
            //Log.Info("========开始下载IC卡参数========");
            TransResult eRet = TransResult.E_SUCC;
            bool bResult = false;
            byte bContinue = 0x30;
            List<byte[]> aidItemList = new List<byte[]>();
            CQueryAID qaid = new CQueryAID();
            eRet = qaid.Communicate();
            if (eRet == TransResult.E_SUCC)
            {
                bResult = true;
                aidItemList.AddRange(qaid.AIDItemList);
                bContinue = qaid.BField62;
                if (bContinue == 0x32)
                {
                    while (eRet == TransResult.E_SUCC && bContinue == 0x32)
                    {
                        System.Threading.Thread.Sleep(2000);//四川版需要停顿
                        CQueryAID qaid2 = new CQueryAID();
                        qaid2.StrField62 = "1" + aidItemList.Count.ToString().PadLeft(2, '0');
                        eRet = qaid2.Communicate();
                        if (eRet == TransResult.E_SUCC)
                        {
                            aidItemList.AddRange(qaid2.AIDItemList);
                            bContinue = qaid2.BField62;
                        }
                        else
                        {
                            bResult = false;
                        }
                    }
                }
                if (bResult && aidItemList.Count > 0)
                {
                    System.Threading.Thread.Sleep(2000);//四川版需要停顿
                    bResult = CDownAID.ICDownAID(aidItemList);
                    if (bResult)
                    {
                        CEndAID eaid = new CEndAID();
                        eaid.Communicate();
                    }

                    //string caPath = Path.Combine(StartupPath, "pbocaid.txt");
                    //byte[] bAllAID = new byte[1024 * 4];
                    //int nAllAID = 0;
                    //foreach (byte[] item in aidItemList)
                    //{
                    //    Array.Copy(item, 0, bAllAID, nAllAID, item.Length);
                    //    nAllAID += item.Length;
                    //}
                    //bResult = CreateFile(caPath, bAllAID, nAllAID);
                }
            }
            Log.Warn("IC卡下载IC卡参数CQueryAID：Ret=" + eRet.ToString());
            //Log.Info("========结束下载IC卡参数========");
            return bResult;
        }



        protected override void Packet()
        {
            SendPackage.SetString(0, "0820");
            SendPackage.SetString(60, "00" + GetBatchNo() + "382");
            SendPackage.SetArrayData(63, Encoding.Default.GetBytes(StrField62), 0, 3);
        }

        protected override bool NeedCalcMac()
        {
            return false;
        }

        protected override void OnSucc()
        {
            SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //记录批次号
            byte[] bField63 = RecvPackage.GetArrayData(63);
            BField62 = bField63[0];
            GetAID(bField63);
        }

        /// <summary>
        /// field62[0]:0没有信息，1已经存放了所有信息，2分多次，3结束:AID1,AID2
        /// </summary>
        /// <param name="field62"></param>
        /// <returns></returns>
        private void GetAID(byte[] field62)
        {
            if (field62[0] == 0x30)
            {
                return;
            }
            byte[] bAID = new byte[field62.Length - 1];
            Array.Copy(field62, 1, bAID, 0, field62.Length - 1);
            int nOffset = 0;
            int nCurr = 0;
            if (field62[0] == 0x31 || field62[0] == 0x32 || field62[0] == 0x33)
            {
                for (; nOffset < bAID.Length; )
                {
                    if (bAID[nOffset] == 0x9F && bAID[nOffset + 1] == 0x06)
                    {
                        nOffset += 2;//TAG 9F06
                        int aidLen = bAID[nOffset];//LEN
                        nOffset += 1;
                        nOffset = nOffset + aidLen;//AID
                        byte[] bAIDItem = new byte[aidLen + 3];
                        Array.Copy(bAID, nCurr, bAIDItem, 0, aidLen + 3);
                        AIDItemList.Add(bAIDItem);
                        nCurr = nOffset;
                    }
                    else
                        nOffset++;
                }
            }
        }
    }
}
