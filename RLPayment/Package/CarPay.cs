using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Landi.FrameWorks;
using Landi.FrameWorks.ChinaUnion;
using YAPayment.Entity;
using YAPayment.Package.EMV.PowerEMV;
using YAPayment.Package.EMV.YAPaymentEMV;

namespace YAPayment.Package
{
    class CarPay : ChinaUnionPay
    { 
        private static bool sHasSignIn;
        public static bool HasSignIn
        {
            get { return sHasSignIn; }
            protected set { sHasSignIn = value; }
        }

        protected CarEntity PayEntity
        {
            get { return BaseBusinessEntity as CarEntity; }
        }

        protected byte[] Get48TLVBytes()
        {
            byte[] tmp = RecvPackage.GetArrayData(48);
            int len = int.Parse(Encoding.Default.GetString(tmp, 72, 3));
            byte[] tmp1 = new byte[len];
            Array.Copy(tmp, 75, tmp1, 0, len);
            return tmp1;
        }
        public ArrayList GetCarPayReceipt()
        {
            string sTitle = "***银联商务\"全民付\"汽车购票凭条***";
            int splitStringLen = Encoding.Default.GetByteCount("---------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);

            Lprint.Add("  ");
            Lprint.Add("   交易类型 :  汽车购票");
            Lprint.Add("   商户编号 :  " + GetMerchantNo());
            Lprint.Add("   终端编号 :  " + GetTerminalNo());
            Lprint.Add("   车票ID   :  " + PayEntity.TicketId);
            Lprint.Add("   发车时间 :  " + PayEntity.SelectLine.DrvDateTime);
            Lprint.Add("   目的地   :  " + PayEntity.SelectLine.StopName);
            Lprint.Add("   发车站名 :  " + PayEntity.SelectLine.CarryStaName);
            Lprint.Add("   消费金额 :  " + CommonData.Amount.ToString("########0.00") + "元");
            Lprint.Add("           *** 全民付,全民生活便利付 ***");
            Lprint.Add("             客服电话: 95534");
            return Lprint;
        }
        public CarPay() { }

        public CarPay(PackageBase pb)
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
                        CCarDownCA ca = new CCarDownCA();
                        ca.DownPublishCA();
                        break;
                    case 0x05://下载IC卡参数
                        CCarDownAID aid = new CCarDownAID();
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
            get { return CarEntity.SECTION_NAME; }
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
}
