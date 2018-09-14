using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Landi.FrameWorks;
using System.IO;

namespace YAPayment.Package
{
    class CDownAID : YAPaymentPay
    {
        public byte[] BField62 = new byte[0];
        public byte[] BAID = new byte[0];

        public static bool ICDownAID(List<byte[]> aidItemList)
        {
            bool result = false;
            byte[] bAllAID = new byte[1024 * 4];
            int nAllAID = 0;
            TransResult eRet = TransResult.E_SUCC;
            foreach (byte[] item in aidItemList)
            {
                CDownAID dAID = new CDownAID();
                dAID.BField62 = new byte[item.Length];
                Array.Copy(item, dAID.BField62, item.Length);
                eRet = dAID.Communicate();
                if (eRet != TransResult.E_SUCC)
                {
                    break;
                }

                Array.Copy(dAID.BAID, 0, bAllAID, nAllAID, dAID.BAID.Length);
                nAllAID += dAID.BAID.Length;
            }

            Log.Warn("IC卡下载IC卡参数CDownAID：Ret=" + eRet.ToString());
            if (eRet == TransResult.E_SUCC)
            {
                Log.Info("IC卡AID下载参数成功");
                string caPath = Path.Combine(StartupPath, "pbocaid.txt");
                result = CreateFile(caPath, bAllAID, nAllAID);
            }

            return result;
        }

        protected override void Packet()
        {
            SendPackage.SetString(0, "0800");
            SendPackage.SetString(60, "00" + GetBatchNo() + "380");
            SendPackage.SetArrayData(63, BField62);
        }

        protected override void OnSucc()
        {
            SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //记录批次号
            byte[] bField63 = RecvPackage.GetArrayData(63);
            if (bField63[0] == 0x31)
            {
                BAID = new byte[bField63.Length - 1];
                Array.Copy(bField63, 1, BAID, 0, bField63.Length - 1);
            }
        }
        protected override bool NeedCalcMac()
        {
            return false;
        }
    }
}
