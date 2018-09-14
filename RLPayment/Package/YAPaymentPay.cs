using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Landi.FrameWorks;
using Landi.FrameWorks.ChinaUnion;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using YAPayment.Package.EMV.YAPaymentEMV;
using YAPayment.Entity;

namespace YAPayment.Package
{
    class YAPaymentPay: ChinaUnionPay
    {
        private static bool sHasSignIn;
        public static bool HasSignIn
        {
            get { return sHasSignIn; }
            protected set { sHasSignIn = value; }
        }

        protected YAEntity PayEntity
        {
            get { return BaseBusinessEntity as YAEntity; }
        }

        public ArrayList GetReceipt()
        {
            //string sTitle = "***银联商务\"中石油社区\"自助交易凭条***";
            //string sTitle = "银联商务\"雅安公共事业缴费系统\"自助交易凭条";
            string sTitle = "***\"智慧雅安  民芯工程\"自助便民终端交易***";
            int splitStringLen = Encoding.Default.GetByteCount("------------------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = "";
            if(iLeftLength > 0)
                sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);
            Lprint.Add("  ");
            Lprint.Add("   交易类型 :  账单号支付");
            Lprint.Add("   商户编号 :  " + GetMerchantNo());
            Lprint.Add("   终端编号 :  " + GetTerminalNo());
            Lprint.Add("   银行卡号 :  " + Utility.GetPrintCardNo(CommonData.BankCardNum));
            Lprint.Add("   日期/时间:  " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Lprint.Add("   参 考 号 :  " + PayEntity.PayReferenceNo);
            Lprint.Add("   凭 证 号 :  " + PayEntity.PayTraceNo);
            Lprint.Add("   批 次 号 :  " + GetBatchNo());
            Lprint.Add("   ---------------------------------------");
            Lprint.Add("   交易金额 :  " + CommonData.Amount.ToString("########0.00") + "元");
            Lprint.Add("   用 户 号 :  " + PayEntity.UserID);
            return Lprint;
        }

        public ArrayList GetTPReceipt()
        {
            string sTitle = "***\"智慧雅安  民芯工程\"自助便民终端交易***";
            int splitStringLen = Encoding.Default.GetByteCount("------------------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = "";
            if (iLeftLength > 0)
                sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);
            Lprint.Add("  ");
            Lprint.Add("   交易类型 :  交警罚没");
            Lprint.Add("   商户编号 :  " + GetMerchantNo());
            Lprint.Add("   终端编号 :  " + GetTerminalNo());
            Lprint.Add("   银行卡号 :  " + Utility.GetPrintCardNo(CommonData.BankCardNum));
            Lprint.Add("   日期/时间:  " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Lprint.Add("   参 考 号 :  " + PayEntity.PayReferenceNo);
            Lprint.Add("   凭 证 号 :  " + PayEntity.PayTraceNo);
            Lprint.Add("   批 次 号 :  " + GetBatchNo());
            Lprint.Add("   ---------------------------------------");
            Lprint.Add("   缴费流水号 :  " + PayEntity.TPPayFlowNo);
            Lprint.Add("   交易金额 :  " + CommonData.Amount.ToString("########0.00") + "元");
            Lprint.Add("   决定书编号 :  " + PayEntity.TPDecisionNo);
            return Lprint;
        }

        protected byte[] Get48TLVBytes()
        {
            byte[] tmp = RecvPackage.GetArrayData(48);
            int len = int.Parse(Encoding.Default.GetString(tmp, 72, 3));
            byte[] tmp1 = new byte[len];
            Array.Copy(tmp, 75, tmp1, 0, len);
            return tmp1;
        }

        public YAPaymentPay() { }

        public YAPaymentPay(PackageBase pb)
            : base(pb) { }

        /// <summary>
        /// 根据报文头部，来更新或下载参数
        /// </summary>
        /// <param name="headBytes"></param>
        protected override void HandleFrontBytes(byte[] headBytes)
        {
            byte tmp = headBytes[9];
            try
            {
                switch (tmp & 0x0F)
                {
                    case 0x03:
                        {
                            HasSignIn = false;
                            //EnqueueWork(new CSignIn_YAPaymentPay());
                        }
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
            catch(Exception ex)
            {
                Log.Warn("HandleFrontBytesy异常", ex);
            }
        }

        protected override string SectionName
        {
            get { return YAEntity.SECTION_NAME; }
        }

        protected override void OnHostFail(string returnCode, string returnMessage)
        {
            //如果签到，一直返回A0|99，可能存在死循环
            //不在做队列交易
            if (returnCode == "99" || returnCode == "A0")
            {
                HasSignIn = false;
                //EnqueueWork(new CSignIn_YAPaymentPay());
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
            catch
            {
                Log.Warn("生成文件失败");
                bRet = false;
            }
            return bRet;
        }

        //public ResponseData GetResponseData()
        //{
        //    ResponseData rd = new ResponseData();
        //    rd.BankCardNo = CommonData.BankCardNum;
        //    rd.Amount = CommonData.Amount.ToString("########0.00");
        //    rd.BatchNo = GetBatchNo();
        //    rd.RefNo = PayEntity.PayReferenceNo;
        //    rd.TraceNo = PayEntity.PayTraceNo;
        //    rd.PayFlowNo = PayEntity.PayFlowNo;
        //    rd.UserID = PayEntity.UserID;

        //    return rd;
        //}
    }

    [SerializableAttribute]
    class ResponseData
    {
        public string BankCardNo = "";
        public string Amount = "";
        public string BatchNo = "";
        public string TraceNo = "";
        public string RefNo = "";
        public string PayFlowNo = "";
        public string UserID = "";
    }


    [SerializableAttribute]
    class ConfirmFailInfo
    {
        public string BankCardNo;
        public string PayTraceNo;
        public string PayReferenceNo;
        public DateTime TransDate = DateTime.Now;
        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.AppendLine("时间：" + TransDate.ToString("yyyy/MM/dd HH:mm:ss"));
            temp.AppendLine("卡号：" + BankCardNo);
            temp.AppendLine("流水号：" + PayTraceNo);
            temp.AppendLine("系统参考号：" + PayReferenceNo);
            temp.AppendLine("=====================================");

            return temp.ToString();
        }
    }
}
