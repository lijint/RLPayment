using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Xml;
using Landi.FrameWorks.ChinaUnion;
using System.Collections;
using System.IO;
using YAPayment.Entity;
using YAPayment.Package.EMV.YAPaymentEMV;

namespace YAPayment.Package
{
    class QMPay: ChinaUnionPay
    {
        private static bool sHasSignIn;
        public static bool HasSignIn
        {
            get { return sHasSignIn; }
            set { sHasSignIn = value; }
        }

        protected QMEntity PayEntity
        {
            get { return BaseBusinessEntity as QMEntity; }
        }

        public ArrayList GetCreditCardReceipt()
        {
            string sTitle = "***银联商务\"全民付\"自助交易凭条***";
            int splitStringLen = Encoding.Default.GetByteCount("---------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);

            Lprint.Add("  ");
            Lprint.Add("   交易类型 :  信用卡还款");
            Lprint.Add("   商户编号 :  " + GetMerchantNo());
            Lprint.Add("   终端编号 :  " + GetTerminalNo());
            Lprint.Add("   借记卡号 :  " + Utility.GetPrintCardNo(CommonData.BankCardNum));
            Lprint.Add("   信用卡号 :  " + Utility.GetPrintCardNo(PayEntity.CreditcardNum));

            Lprint.Add("   日期/时间:  " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Lprint.Add("   参 考 号 :  " + PayEntity.PayReferenceNo);
            Lprint.Add("   凭 证 号 :  " + PayEntity.PayTraceNo);
            Lprint.Add("   批 次 号 :  " + GetBatchNo());
            Lprint.Add("   ---------------------------------------------");
            Lprint.Add("   还款金额 :  " + CommonData.Amount.ToString("########0.00") + "元");
            Lprint.Add("   手 续 费 :  " + PayEntity.Fee.ToString("########0.00") + "元");
            Lprint.Add("   交易金额 :  " + PayEntity.TotalAmount.ToString("########0.00") + "元");
            Lprint.Add("   ");
            Lprint.Add("           *** 全民付,全民生活便利付 ***");
            Lprint.Add("             客服电话: 95534");
            return Lprint;
        }

        public ArrayList GetMobileReceipt()
        {
            string sTitle = "***银联商务\"全民付\"自助交易凭条***";
            int splitStringLen = Encoding.Default.GetByteCount("---------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);

            Lprint.Add("  ");
            Lprint.Add("   交易类型 :  手机话费直充");
            Lprint.Add("   商户编号 :  " + GetMerchantNo());
            Lprint.Add("   终端编号 :  " + GetTerminalNo());
            Lprint.Add("   银行卡号 :  " + Utility.GetPrintCardNo(CommonData.BankCardNum));

            Lprint.Add("   日期/时间:  " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Lprint.Add("   参 考 号 :  " + PayEntity.PayReferenceNo);
            Lprint.Add("   凭 证 号 :  " + PayEntity.PayTraceNo);
            Lprint.Add("   批 次 号 :  " + GetBatchNo());
            Lprint.Add("   ---------------------------------------------");
            Lprint.Add("   交易金额 :  " + CommonData.Amount.ToString("########0.00") + "元");
            Lprint.Add("   手机号码 :  " + PayEntity.PhoneNo);
            Lprint.Add("   (直充交易可能稍有延迟,如长时间未能到帐,请咨询客服");
            Lprint.Add("   中心)");
            Lprint.Add("   ");
            Lprint.Add("           *** 全民付,全民生活便利付 ***");
            Lprint.Add("             客服电话: 95534");
            return Lprint;
        }

        public QMPay() { }

        public QMPay(PackageBase pb)
            : base(pb) { }

        protected override void HandleFrontBytes(byte[] headBytes)
        {
            byte tmp = headBytes[9];
            switch (tmp & 0x0F)
            {
                case 0x03:
                    EnqueueWork(new CSignIn_YAPaymentPay());
                    break;
                case 0x04://更新公钥
                    CYADownCA ca = new CYADownCA();
                    ca.DownPublishCA();
                    break;
                case 0x05://下载IC卡参数
                    CYADownAID aid = new CYADownAID();
                    aid.DownAID();
                    break;
            }
        }

        protected override string SectionName
        {
            get { return QMEntity.SECTION_NAME; }
        }

        protected override void OnHostFail(string returnCode, string returnMessage)
        {
            if (returnCode == "99" || returnCode == "A0")
            {
                HasSignIn = false;
                EnqueueWork(new CSignIn_YAPaymentPay());
            }
        }

        public static bool CreateFile(string filname, byte[] szBuffer, int len)
        {
            bool bRet = true;
            try
            {
                if (File.Exists(filname))
                {
                    File.Delete(filname);
                }
                FileStream fs = new FileStream(filname, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(szBuffer, 0, len);
                bw.Close();
            }
            catch(Exception ex)
            {
                Log.Warn("[QMPay][CreateFile]生成文件失败", ex);
                bRet = false;
            }
            return bRet;
        }
    }
}
