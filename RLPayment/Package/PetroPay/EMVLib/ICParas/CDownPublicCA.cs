using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Landi.FrameWorks;
using System.IO;

namespace YAPayment.Package
{
    class CDownPublicCA : YAPaymentPay
    {
        public byte[] BField62 = new byte[0];
        public byte[] BCA = new byte[0];

        public static bool ICDownPublicCA(List<CaItem> caItemList)
        {
            bool result = false;
            if (caItemList.Count > 0)
            {
                byte[] bAllCA = new byte[1024 * 4];
                int nAllCA = 0;

                TransResult eRet = TransResult.E_SUCC;
                foreach (CaItem item in caItemList)
                {
                    byte[] rid = new byte[item.cRid.Length + 3];
                    rid[0] = 0x9F;
                    rid[1] = 0x06;
                    rid[2] = (byte)item.cRid.Length;
                    Array.Copy(item.cRid, 0, rid, 3, item.cRid.Length);

                    byte[] index = new byte[item.cIndex.Length + 3];
                    index[0] = 0x9F;
                    index[1] = 0x22;
                    index[2] = (byte)item.cIndex.Length;
                    Array.Copy(item.cIndex, 0, index, 3, item.cIndex.Length);

                    CDownPublicCA dCA = new CDownPublicCA();
                    dCA.BField62 = new byte[rid.Length + index.Length];
                    Array.Copy(rid, dCA.BField62, rid.Length);
                    Array.Copy(index, 0, dCA.BField62, rid.Length, index.Length);
                    eRet = dCA.Communicate();
                    //Test
                    //eRet = TransResult.E_SUCC;
                    //string s = "9F0605A0000000039F220172DF05083230313030313031DF060101DF070101DF028190BD9F074D8F60501D2E87B3AB03DCA80C83AF9CE81372AD34B7FA639767E5E6B2491ADCAF943FA165D09AB25B4B8FF541E6D2D3B0B70705B105266751D27E8E56FD9D0974F67B3B2E84322DA7E56152A4E42CC63727EB160B2E5310DF125E74F55618FE8727B167B6456431CFDE80C025D0CB1DE7DDC3186B7314085C7CCA301C691F5577690FD2DE5FC62665CB163F0DDF0403010001DF03141ECF2EFE0B01FA7C94F3960056E748C8FF4F1D09";
                    //dCA.BCA = Utility.str2Bcd(s);
                    if (eRet != TransResult.E_SUCC)
                    {
                        break;
                    }

                    Array.Copy(dCA.BCA, 0, bAllCA, nAllCA, dCA.BCA.Length);
                    nAllCA += dCA.BCA.Length;
                    System.Threading.Thread.Sleep(2000);//四川版需要停顿
                }

                Log.Warn("IC卡公钥下载参数CDownPublicCA：Ret=" + eRet.ToString());
                if (eRet == TransResult.E_SUCC)
                {
                    Log.Info("IC卡公钥下载参数成功");
                    string caPath = Path.Combine(StartupPath, "pbocCA.txt");
                    result = CreateFile(caPath, bAllCA, nAllCA);
                }
            }
            return result;
       }

        protected override void Packet()
        {
            SendPackage.SetString(0, "0800");
            SendPackage.SetString(60, "00" + GetBatchNo() + "370");
            SendPackage.SetArrayData(63, BField62);
        }

        protected override void OnSucc()
        {
            SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //记录批次号
            byte[] bField63 = RecvPackage.GetArrayData(63);
            if (bField63[0] == 0x31)
            {
                BCA = new byte[bField63.Length - 1];
                Array.Copy(bField63, 1, BCA, 0, bField63.Length - 1);
                Dictionary<string, byte[]> ht = new Dictionary<string, byte[]>();
                TLVHandler.ParseTLV(BCA, ht);
                if (!ValidatorCA(ht))
                    SetResult(TransResult.E_UNPACKET_FAIL);
            }
        }

        private bool ValidatorCA(Dictionary<string,byte[]> ht)
        {
            bool ret = true;
            try
            {
                string[] TagName = new string[] { "9F06", "9F22", "DF05", "DF06", "DF07", "DF02", "DF04", "DF03" };
                for (int i = 0; i < TagName.Length; i++)
                {
                    if (!ht.ContainsKey(TagName[i]))
                    {
                        return false;
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error("CDownPublicCA ValidatorCA Failed!", e);
                ret = false;
            }

            return ret;
        }

        protected override bool NeedCalcMac()
        {
            return false;
        }
    }
}
