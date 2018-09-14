using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks.ChinaUnion;
using Landi.FrameWorks;

namespace YAPayment.Package
{
    class CSignIn_YAPaymentPay : YAPaymentPay
    {
        protected override void OnSucc()
        {
            DoSignInSucc();
            if (Result == TransResult.E_SUCC)
            {
                HasSignIn = true;
                QMPay.HasSignIn = true;
                //EnqueueWork(new CReverse_YAPaymentPay());//签到不发冲正，因为在交易的过程中会有冲正报文产生
            }
            else
            {
                HasSignIn = false;
                QMPay.HasSignIn = false;
            }
        }

        protected override void Packet()
        {
            SendPackage.SetString(0, "0800");
            SendPackage.SetString(11, GetTraceNo());
            switch (DType)
            {
                case DesType.TripleDes:
                    SendPackage.SetString(60, "00" + GetBatchNo() + "003");
                    break;
                default:
                    SendPackage.SetString(60, "00" + GetBatchNo() + "001");
                    break;
            }
            SendPackage.SetArrayData(63, Encoding.Default.GetBytes("001"));
        }

        protected override void OnBeforeTrans()
        {
            if (!RealEnv)
            {
                HasSignIn = true;

                //不进行交易的时候，将手动的存入密钥，以用于后来mac计算
                byte[] key = new byte[KeyLength];
                for (int i = 0; i < KeyLength; i++)
                {
                    key[i] = 0x01;
                }
                KeyManager.SetEnPinKey(SectionName, key);
                KeyManager.SetEnMacKey(SectionName, key);
                KeyManager.SetDePinKey(SectionName, key);
                KeyManager.SetDeMacKey(SectionName, key);
            }
            else
                HasSignIn = false;
        }
    }
}
