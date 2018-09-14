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
using YAPayment.Entity;
using YAPayment.Package.EMV.PowerEMV;

namespace YAPayment.Package
{
    class PowerPay: ChinaUnionPay
    {
        private static bool sHasSignIn;
        public static bool HasSignIn
        {
            get { return sHasSignIn; }
            protected set { sHasSignIn = value; }
        }

        protected PowerEntity PayEntity
        {
            get { return BaseBusinessEntity as PowerEntity; }
        }


        public ArrayList GetUserReceipt()
        {
            string sTitle = "银联商务\"雅安公共事业缴费系统\"自助交易凭条";
            int splitStringLen = Encoding.Default.GetByteCount("------------------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = "";
            if(iLeftLength > 0)
                sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);
            Lprint.Add("  ");
            Lprint.Add("   交易类型 :  用户号缴费");
            Lprint.Add("   商户编号 :  " + GetMerchantNo());
            Lprint.Add("   终端编号 :  " + GetTerminalNo());
            Lprint.Add("   银行卡号 :  " + Utility.GetPrintCardNo(CommonData.BankCardNum));
            Lprint.Add("   日期/时间:  " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Lprint.Add("   参 考 号 :  " + PayEntity.PayReferenceNo);
            Lprint.Add("   凭 证 号 :  " + PayEntity.PayTraceNo);
            Lprint.Add("   批 次 号 :  " + GetBatchNo());
            Lprint.Add("   ---------------------------------------");
            Lprint.Add("   交易金额 :  " + CommonData.Amount.ToString("########0.00") + "元");
            Lprint.Add("   用 户 号 :  " + PayEntity.DBNo);
            return Lprint;
        }

        public ArrayList GetCardReceipt(bool containTail=false)
        {
            string sTitle = "银联商务\"雅安公共事业缴费系统\"自助交易凭条";
            int splitStringLen = Encoding.Default.GetByteCount("------------------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = "";
            if (iLeftLength > 0)
                sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);
            Lprint.Add("  ");
            Lprint.Add("   交易类型 :  购电卡缴费");
            Lprint.Add("   商户编号 :  " + GetMerchantNo());
            Lprint.Add("   终端编号 :  " + GetTerminalNo());
            Lprint.Add("   银行卡号 :  " + Utility.GetPrintCardNo(CommonData.BankCardNum));
            Lprint.Add("   日期/时间:  " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Lprint.Add("   参 考 号 :  " + PayEntity.PayReferenceNo);
            Lprint.Add("   凭 证 号 :  " + PayEntity.PayTraceNo);
            Lprint.Add("   批 次 号 :  " + GetBatchNo());
            Lprint.Add("   用 户 号 :  " + PayEntity.PowerCardNo);
            Lprint.Add("   银联商户订单号 :  " + PayEntity.PowerPayConfirmCode);
            Lprint.Add("   ---------------------------------------");
            Lprint.Add("   交易金额 :  " + CommonData.Amount.ToString("########0.00") + "元");
            Lprint.Add("   用户名称 :  " + PayEntity.UserName);
            Lprint.Add("   电卡卡号 :  " + PayEntity.PowerCardNo);
            if (containTail)
            {
                Lprint.Add("  ---------------------------------------");
                Lprint.Add(" 【注意】:确认超时，请联系95534，申请退款！谢谢您的配合！");
            }
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

        public PowerPay() { }

        public PowerPay(PackageBase pb)
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
                            //EnqueueWork(new CSignIn_PowerPay());
                        }
                        break;
                    case 0x04://更新公钥
                        CPowerDownCA ca = new CPowerDownCA();
                        ca.DownPublishCA();
                        break;
                    case 0x05://下载IC卡参数
                        CPowerDownAID aid = new CPowerDownAID();
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
            get { return PowerEntity.SECTION_NAME; }
        }

        protected override void OnHostFail(string returnCode, string returnMessage)
        {
            //如果签到，一直返回A0|99，可能存在死循环
            //不在做队列交易
            if (returnCode == "99" || returnCode == "A0")
            {
                HasSignIn = false;
                //EnqueueWork(new CSignIn_PowerPay());
            }
        }

        /// <summary>
        /// 获取网点编号
        /// </summary>
        /// <returns></returns>
        protected string GetBranchNo()
        {
            string BranchNo = ReadIniFile("BranchNo");
            if (BranchNo == "")
            {
                throw new Exception("尚未配置网点编号");
            }
            return BranchNo;
        }

        /// <summary>
        /// 获取操作员编号
        /// </summary>
        /// <returns></returns>
        protected string GetOperatorNo()
        {
            string OperatorNo = ReadIniFile("OperatorNo");
            if (OperatorNo == "")
            {
                throw new Exception("尚未配置操作员编号");
            }
            return OperatorNo;
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

        protected string GetFieldString(byte[] source, int startIndex, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(source, startIndex, result, 0, length);
            return Encoding.Default.GetString(result);
        }

        protected double GetFieldDouble(byte[] source, int startIndex, int length)
        {
            string value = GetFieldString(source, startIndex, length);
            double temp = Convert.ToDouble(value) / 100;
            return temp;
        }
    }
}
