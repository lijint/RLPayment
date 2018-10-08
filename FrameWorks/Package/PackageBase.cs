using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks.Iso8583;
using Landi.FrameWorks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using log4net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Landi.FrameWorks.HardWare;
using System.Xml;

namespace Landi.FrameWorks
{
    public enum TransResult : int
    {
        E_SUCC = 0,               // 成功返回
        E_SEND_FAIL = 1,          // 不需要冲正
        E_RECV_FAIL = 2,          // 需要冲正
        E_HOST_FAIL = 3,          // 主机返回错误代码
        E_MAC_ERROR = 4,          // 服务端报文mac校验错
        E_CHECK_FAIL = 5,	      // 冲正
        E_UNPACKET_FAIL = 6,     // 解包失败
        E_KEYVERIFY_FAIL = 7,    // 签到校验失败
        E_PACKET_FAIL = 8,      //打包失败
        E_INVALID = 9,            //无效交易
    }

    public abstract class PackageBase
    {
        public delegate void ResultHandler(TransResult result);
        protected enum AlgorithmType : int
        {
            X99 = 1,      // X9.9算法
            ECB = 2,      // ECB算法
            CBC = 3,      // CBC算法
        }

        protected enum DesType : int
        {
            Des = 1, //单des
            TripleDes = 3, //3des
        }

        protected enum EncryptType
        {
            Soft, //软加密
            Hardware, //硬加密
        }

        protected EncryptType EnType = EncryptType.Soft; //加密类型
        protected DesType DType = DesType.TripleDes; //Des类型
        protected AlgorithmType AlType = AlgorithmType.X99; //算法类型
        protected int KeyLength = 16;//密钥长度

        protected BaseEntity BaseBusinessEntity
        {
            get { return (BaseEntity)businessBundle.Get(Activity.ENTITYKEY); }
        }

        private string mSchemaFile;
        protected Iso8583Package SendPackage;
        protected Iso8583Package RecvPackage;

        private int headLen;
        internal string serverIP;
        internal int serverPort = -1;
        internal int sendTimeOut = -1;
        internal int recvTimeOut = -1;
        internal static byte[] sRecvBuffer = new byte[1024 * 1024];

        public string ReturnCode;
        public string ReturnMean;
        public string ReturnMessage;

        internal bool mInvokeSetResult = false;
        protected string mSectionName;
        internal int RepeatTimes = 1;
        private bool mRealEnv = false;
        /// <summary>
        /// 0表示不进行真实通讯，否则表示进行真实通讯
        /// </summary>
        protected bool RealEnv
        {
            get { return mRealEnv; }
            private set { mRealEnv = value; }
        }
        private static readonly object sLock = new object();
        internal ResultHandler OnResult;
        protected TransResult Result = TransResult.E_SUCC;

        protected static readonly string StartupPath = Application.StartupPath;

        #region 错误配置信息缓存
        private static readonly object sErrorLock = new object();
        private struct MeanAndShow
        {
            public string Mean;
            public string Show;
        }

        private static Dictionary<string, Dictionary<string, MeanAndShow>> sParsedInfo = new Dictionary<string, Dictionary<string, MeanAndShow>>();
        private string mErrConfigFile;
        protected virtual void ParseRespMessage(string code, ref string mean, ref string show)
        {
            ParseRespMessage(mErrConfigFile, code, ref mean, ref show);
            //增加业务错误码解析
            if (BaseBusinessEntity != null)
                BaseBusinessEntity.ParseRespMessage(code, ref mean, ref show);
        }

        /// <summary>
        /// 应答码解析
        /// </summary>
        /// <param name="code">应答码</param>
        /// <param name="mean">意义</param>
        /// <param name="show">显示的内容</param>
        /// <returns></returns>
        private static void ParseRespMessage(string configFile, string code, ref string mean, ref string show)
        {
            if (string.IsNullOrEmpty(configFile) || Path.GetExtension(configFile) != ".xml")
                return;
            lock (sErrorLock)
            {
                if (!sParsedInfo.ContainsKey(configFile))
                {
                    XMLConfig config = new XMLConfig(configFile);
                    XmlNode node;
                    int elementCount = config.GetNodeElementCount("/Config");
                    if (elementCount > 0)
                    {
                        Dictionary<string, MeanAndShow> infos = new Dictionary<string, MeanAndShow>();
                        for (int iPer = 0; iPer <= elementCount - 1; iPer++)
                        {
                            node = config.GetNodeElementById("/Config", iPer);
                            MeanAndShow ins = new MeanAndShow();
                            ins.Mean = config.GetAttributeValue(node, "Mean");
                            ins.Show = config.GetAttributeValue(node, "Show");

                            infos.Add(config.GetAttributeValue(node, "Code"), ins);
                        }
                        sParsedInfo.Add(configFile, infos);
                    }
                }
                Dictionary<string, MeanAndShow> ret = sParsedInfo[configFile];
                if (ret.ContainsKey(code))
                {
                    mean = ret[code].Mean;
                    show = ret[code].Show;
                }
            }
        }
        #endregion

        #region EnqueueWork and DequeueWork 在一次交易中，如果有其他交易要上送，可以进入此队列
        private static Dictionary<string, List<PackageBase>> mDeferWorks;
        protected void EnqueueWork(PackageBase instance)
        {
            EnqueueWork(mSectionName, instance);
        }

        private static void EnqueueWork(string Name, PackageBase instance)
        {
            bool add = true;
            lock (sLock)
            {
                if (mDeferWorks == null)
                    mDeferWorks = new Dictionary<string, List<PackageBase>>();
                List<PackageBase> list = null;
                if (mDeferWorks.ContainsKey(Name))
                    list = mDeferWorks[Name];
                else
                    list = new List<PackageBase>();
                for (int i = 0; i < list.Count; i++)
                    if (list[i].GetType().FullName == instance.GetType().FullName)
                    {
                        add = false;
                        break;
                    }
                if (add)
                    list.Add(instance);
                if (!mDeferWorks.ContainsKey(Name))
                    mDeferWorks.Add(Name, list);
            }
        }

        protected bool RemoveWork(PackageBase instance)
        {
            return RemoveWork(mSectionName, instance);
        }

        private static bool RemoveWork(string Name, PackageBase instance)
        {
            bool remove = false;
            lock (sLock)
            {
                if (mDeferWorks != null && mDeferWorks.ContainsKey(Name) && mDeferWorks[Name].Count > 0)
                {
                    for (int i = 0; i < mDeferWorks[Name].Count; i++)
                        if (mDeferWorks[Name][i].GetType().FullName == instance.GetType().FullName)
                        {
                            mDeferWorks[Name].RemoveAt(i);
                            remove = true;
                        }
                }
            }
            return remove;
        }

        internal PackageBase DequeueWork()
        {
            return DequeueWork(mSectionName);
        }

        private static PackageBase DequeueWork(string Name)
        {
            PackageBase pb = null;
            lock (sLock)
            {
                if (mDeferWorks != null && mDeferWorks.ContainsKey(Name) && mDeferWorks[Name].Count > 0)
                {
                    pb = mDeferWorks[Name][0];
                    mDeferWorks[Name].RemoveAt(0);
                }
            }
            return pb;
        }
        #endregion

        protected void SetResult(TransResult ret)
        {
            Result = ret;
        }

        protected void WriteIniFile(string key, string value)
        {
            ConfigFile.WriteConfig(mSectionName, key, value);
        }

        protected string ReadIniFile(string key)
        {
            return ConfigFile.ReadConfigAndCreate(mSectionName, key);
        }

        protected PackageBase()
        {
            readConfig();
            SendPackage = new Iso8583Package(mSchemaFile);
            RecvPackage = new Iso8583Package(mSchemaFile);
        }

        protected PackageBase(PackageBase pb)
        {
            readConfig();
            SendPackage = new Iso8583Package(pb.SendPackage);
            RecvPackage = new Iso8583Package(mSchemaFile);
        }

        protected void SetRepeatTimes(int times)
        {
            if (times >= 1)
                RepeatTimes = times;
        }

        private static List<string> mNameList = new List<string>();
        private void readConfig()
        {
            mSectionName = SectionName;
            if (!string.IsNullOrEmpty(mSectionName))
            {
                if (!mNameList.Contains(mSectionName))
                {
                    defaultPrepareConfig();
                    mNameList.Add(mSectionName);
                }
            }
            else
                throw new Exception(this + "必须配置区间名");

            mErrConfigFile = ReadIniFile("ErrConfFile");
            if (!string.IsNullOrEmpty(mErrConfigFile))
                mErrConfigFile = Path.Combine(Application.StartupPath, mErrConfigFile);
            else
                mErrConfigFile = null;
            mSchemaFile = ReadIniFile("SchemaFile");
            if (string.IsNullOrEmpty(mSchemaFile))
                throw new Exception("报文域配置文件不能为空");
            else
                mSchemaFile = Path.Combine(Application.StartupPath, mSchemaFile);

            if (ReadIniFile("Use") == "0")
                RealEnv = false;
            else
                RealEnv = true;
            if (Esam.IsUse)
                EnType = EncryptType.Hardware;
            string content = ReadIniFile("Des");
            if (content != "1" && content != "3")
                throw new Exception("DES类型配置错误");
            if (content == "1")
            {
                DType = DesType.Des;
                KeyLength = 8;
            }
            content = ReadIniFile("MacAlgorithm");
            if (content != "1" && content != "2" && content != "3")
                throw new Exception("MAC算法配置错误");
            if (content == "2")
                AlType = AlgorithmType.ECB;
            else if (content == "3")
                AlType = AlgorithmType.CBC;

            if (RealEnv)
            {
                IPAddress.Parse(ReadIniFile("ServerIP"));
                serverIP = ReadIniFile("ServerIP");
                serverPort = int.Parse(ReadIniFile("ServerPort"));
                sendTimeOut = int.Parse(ReadIniFile("SendTimeOut"));
                recvTimeOut = int.Parse(ReadIniFile("RecvTimeOut"));
            }
        }

        protected abstract string SectionName
        {
            get;
        }

        protected virtual string GetTraceNo()
        {
            string TraceNo = ReadIniFile("TraceNo");
            if (TraceNo == "")
                TraceNo = "0";
            int tmp;
            if (!int.TryParse(TraceNo, out tmp))
                return null;
            if (tmp == 999999)
                tmp = 0;
            tmp++;
            WriteIniFile("TraceNo", tmp.ToString());
            return tmp.ToString().PadLeft(6, '0');
        }

        protected abstract void PackFix();

        protected abstract bool UnPackFix();

        protected virtual void Packet()
        {

        }

        protected abstract byte[] PackBytesAtFront(int dataLen);

        /// <summary>
        /// 计算MAC值
        /// </summary>
        /// <param name="macBytes">参与MAC计算的数据</param>
        /// <param name="DataOut">计算出的MAC值</param>
        /// <returns>true表示计算成功，否则表示失败</returns>
        protected virtual bool CalcMacByMackey(byte[] macBytes, byte[] MAC)
        {
            bool ret = false;
            switch (AlType)
            {
                case AlgorithmType.ECB:
                    ret = CalcMac_ECB(this, macBytes, KeyManager.GetEnMacKey(mSectionName),KeyManager.GetDeMacKey(mSectionName), MAC);
                    break;
                case AlgorithmType.CBC:
                case AlgorithmType.X99:
                    ret = CalcMac_CBC_X99(this, macBytes, KeyManager.GetEnMacKey(mSectionName), KeyManager.GetDeMacKey(mSectionName), AlType, MAC);
                    break;
            }
            return ret;
        }

        protected virtual bool CalcMacByPinkey(byte[] macBytes, byte[] MAC)
        {
            bool ret = false;
            switch (AlType)
            {
                case AlgorithmType.ECB:
                    ret = CalcMac_ECB(this, macBytes, KeyManager.GetEnPinKey(mSectionName), KeyManager.GetDePinKey(mSectionName), MAC);
                    break;
                case AlgorithmType.CBC:
                case AlgorithmType.X99:
                    ret = CalcMac_CBC_X99(this, macBytes, KeyManager.GetEnPinKey(mSectionName), KeyManager.GetDePinKey(mSectionName), AlType, MAC);
                    break;
            }
            return ret;
        }

        /// <summary>
        /// 对数据进行每8个字节循环异或
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] CalcMacXor(byte[] data)
        {
            int len = (data.Length % 8 == 0 ? data.Length : data.Length + 8 - data.Length % 8);
            byte[] cData = new byte[len];
            Array.Copy(data, cData, data.Length);
            byte[] ret = new byte[8];
            for (int i = 0; i < len; i += 8)
            {
                for (int j = 0; j < 8; j++)
                {
                    ret[j] ^= cData[j + i];
                }
            }
            return ret;
        }


        private void defaultPrepareConfig()
        {
            string[] keys = new string[] { "Use", "ServerIP", "ServerPort", "MerchantNo", "TerminalNo", "TPDU", "Head", "SendTimeOut", "RecvTimeOut", "BatchNo", "TraceNo", "SchemaFile", "ErrConfFile", "SoftMasterKey", "KeyIndex", "Des", "MacAlgorithm" };
            for (int i = 0; i < keys.Length; i++)
            {
                if (ReadIniFile(keys[i]) == "")
                {
                    if (keys[i] == "Use")
                        WriteIniFile(keys[i], "0");
                    else if (keys[i] == "SendTimeOut")
                        WriteIniFile(keys[i], "30");
                    else if (keys[i] == "RecvTimeOut")
                        WriteIniFile(keys[i], "30");
                }
            }
        }

        protected string GetMerchantNo()
        {
            string MerchantNo = ReadIniFile("MerchantNo");
            if (MerchantNo == "")
            {
                throw new Exception("尚未配置商户号");
            }
            return MerchantNo;
        }

        protected string GetTerminalNo()
        {
            string TerminalNo = ReadIniFile("TerminalNo");
            if (TerminalNo == "")
            {
                throw new Exception("尚未配置终端号");
            }
            return TerminalNo;
        }

        protected string GetBatchNo()
        {
            string BatchNo = ReadIniFile("BatchNo");
            if (BatchNo == "")
            {
                throw new Exception("尚未配置批次号");
            }
            return BatchNo;
        }

        protected void SetBatchNo(string BatchNo)
        {
            WriteIniFile("BatchNo", BatchNo);
        }

        protected string GetTPDU()
        {
            string TPDU = ReadIniFile("TPDU");
            if (TPDU == "")
            {
                throw new Exception("尚未配置TPDU");
            }
            return TPDU;
        }

        protected string GetHead()
        {
            string Head = ReadIniFile("Head");
            if (Head == "")
            {
                throw new Exception("尚未配置包头");
            }
            return Head;
        }

        protected byte[] GetSoftMasterKey()
        {
            string SoftMasterKey = ReadIniFile("SoftMasterKey");
            if (SoftMasterKey == "")
            {
                throw new Exception("尚未配置主密钥明文");
            }
            if (((DType == DesType.Des && SoftMasterKey.Length != 16) || (DType == DesType.TripleDes && SoftMasterKey.Length != 32)) && EnType == EncryptType.Soft)
                throw new Exception("主密钥明文长度不合法");
            return Utility.str2Bcd(SoftMasterKey);
        }

        protected int GetKeyIndex()
        {
            string KeyIndex = ReadIniFile("KeyIndex");
            if (KeyIndex == "")
            {
                throw new Exception("尚未配置密钥索引");
            }
            int index = 0;
            if (!int.TryParse(KeyIndex, out index) || index < 0)
                throw new Exception("非法的密钥索引");
            return index;
        }

        /// <summary>
        /// 判断报文打包时是否需要计算MAC
        /// </summary>
        /// <returns>true表示需要计算MAC，将调用CalcMac函数，false表示相反</returns>
        protected abstract bool NeedCalcMac();

        protected void SavePackageToFile()
        {
            FileStream fileStream = new FileStream(Path.Combine(Application.StartupPath, this.GetType().FullName + ".dat"), FileMode.Create);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(fileStream, SendPackage);
            fileStream.Close();
            //Test 冲正
            //File.Copy(Path.Combine(Application.StartupPath, this.GetType().FullName + ".dat"),
            //    Path.Combine(@"d:\", this.GetType().FullName + ".dat"), true);
        }

        protected void RestorePackageFromFile()
        {
            string reverseFile = Path.Combine(Application.StartupPath, this.GetType().FullName + ".dat");
            if (!File.Exists(reverseFile))
            {
                Result = TransResult.E_INVALID;
                return;
            }
            try
            {
                FileStream fileStream = new FileStream(reverseFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fileStream != null)
                {
                    BinaryFormatter b = new BinaryFormatter();
                    SendPackage = b.Deserialize(fileStream) as Iso8583Package;
                    fileStream.Close();
                }
            }
            catch (System.Exception)
            {
                Result = TransResult.E_INVALID;
            }
        }

        protected void DeletePackageFile()
        {
            File.Delete(Path.Combine(Application.StartupPath, this.GetType().FullName + ".dat"));
        }

        protected bool GPRSConnect()
        {
            if (!GPRS.IsUse) return true;
            bool ret = GPRS.CreateConnection(3, serverIP, serverPort);
            if (ret)
            {
                Log.Info("GPRS连接成功！");
            }
            else
            {
                Log.Info("GPRS连接失败！");
            }
            return ret;
        }

        /// <summary>
        /// 将private改为internal virtual，为了项目不止使用iso8583报文通讯
        /// </summary>
        /// <returns></returns>
        internal virtual TransResult transact()
        {
            TransResult ret = TransResult.E_SEND_FAIL;
            Socket socket = null;
            if (RealEnv)
            {
                if (!GPRSConnect()) return ret;
                IPAddress ip = IPAddress.Parse(serverIP);
                IPEndPoint ipe = new IPEndPoint(ip, serverPort); //把ip和端口转化为IPEndPoint实例
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.SendTimeout = sendTimeOut * 1000;
                    socket.ReceiveTimeout = recvTimeOut * 1000;
                    socket.Connect(ipe);
                }
                catch (Exception err)
                {
                    Log.Error(this.GetType().Name, err);
                    return ret;
                }
            }

            try
            {
                ret = TransResult.E_PACKET_FAIL;
                byte[] SendBytes = new byte[0];
                PackFix();
                Packet();
                if (SendPackage.IsNull())
                    return TransResult.E_INVALID;
                byte[] MAC = new byte[8];

                //计算mac并将mac打包进去
                if (NeedCalcMac())
                {
                    byte[] macKey = null;
                    if (EnType == EncryptType.Hardware)
                        macKey = KeyManager.GetEnMacKey(mSectionName);
                    else
                        macKey = KeyManager.GetDeMacKey(mSectionName);
                    if (macKey == null)
                        throw new Exception("尚未设置MAC密钥");
                    if ((DType == DesType.Des && macKey.Length == 16) || (DType == DesType.TripleDes && macKey.Length == 8))
                    {
                        throw new Exception("MAC密钥长度不符合DES算法");
                    }
                    int macField = 64;
                    if (SendPackage.ExistBit(1))
                        macField = 128;
                    SendPackage.SetArrayData(macField, new byte[8]);
                    byte[] tmp = SendPackage.GetSendBuffer();
                    byte[] macBytes = new byte[tmp.Length - 8];
                    Array.Copy(tmp, macBytes, macBytes.Length);
                    if (CalcMacByMackey(macBytes, MAC))
                    {
                        SendPackage.SetArrayData(macField, MAC);
                    }
                    else
                    {
                        SendPackage.ClearBitAndValue(macField);
                        throw new Exception("计算MAC失败");
                    }
                }
                SendBytes = SendPackage.GetSendBuffer();
                if (SendBytes.Length <= 0)
                {
                    return ret;
                }
                byte[] head = PackBytesAtFront(SendBytes.Length);
                headLen = head.Length;
                int sendLen_all = SendBytes.Length + head.Length;

                byte[] sendstr_all = new byte[sendLen_all];
                Array.Copy(head, sendstr_all, head.Length);
                Array.Copy(SendBytes, 0, sendstr_all, head.Length, SendBytes.Length);

                //记录原始发送日志
                //CLog.LogPackage(sendstr_all, SendPackage, CLog.LogType.Send);
                CLog.Info(CLog.GetLog(sendstr_all, SendPackage, this, CLog.LogType.Send));

                ret = TransResult.E_SEND_FAIL;
                if (RealEnv)
                {
                    int sendLen = 0;
                    sendLen = socket.Send(sendstr_all, sendLen_all, 0);
                    if (sendLen <= 0)
                    {
                        socket.Close();
                        return ret;
                    }
                }

                //从服务器端接受返回信息
                ret = TransResult.E_RECV_FAIL;
                int recvLen = 0;
                if (RealEnv)
                {
                    sRecvBuffer.Initialize();
                    recvLen = socket.Receive(sRecvBuffer, sRecvBuffer.Length, 0);

                    if (recvLen <= 0)
                    {
                        socket.Close();
                        return ret;
                    }
                    byte[] RecvBytes = new byte[recvLen - headLen];
                    Array.Copy(sRecvBuffer, headLen, RecvBytes, 0, recvLen - headLen);
                    byte[] headBytes = new byte[headLen];
                    Array.Copy(sRecvBuffer, headBytes, headLen);
                    //解包
                    ret = TransResult.E_UNPACKET_FAIL;
                    FrontBytes = headBytes;
                    HandleFrontBytes(headBytes);//根据报文头来判断是否要下载密钥
                    RecvPackage.ParseBuffer(RecvBytes, SendPackage.ExistValue(0));

                    //记录原始接收日志
                    byte[] logRecv = new byte[recvLen];
                    Array.Copy(sRecvBuffer, logRecv, recvLen);
                    //CLog.LogPackage(logRecv, RecvPackage, CLog.LogType.Recv);
                    CLog.Info(CLog.GetLog(logRecv, RecvPackage, this, CLog.LogType.Recv));
                    bool nRet = UnPackFix();
                    if (!mInvokeSetResult)
                        throw new Exception("should invoke SetRespInfo() in UnPackFix()");
                    mInvokeSetResult = false;
                    if (nRet)
                    {
                        ret = TransResult.E_SUCC;
                    }
                    else
                    {
                        ret = TransResult.E_HOST_FAIL;
                    }
                }
                else
                {
                    ret = TransResult.E_SUCC;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(this.GetType().Name, ex);
            }
            finally
            {
                if (socket != null && socket.Connected)
                    socket.Close();
            }
            return ret;
        }

        public byte[] FrontBytes;
        protected abstract void HandleFrontBytes(byte[] headBytes);

        protected void SetRespInfo(string returnCode, string returnMean, string returnMessage)
        {
            ReturnCode = returnCode;
            ReturnMean = returnMean;
            ReturnMessage = returnMessage;
            mInvokeSetResult = true;
        }

        protected virtual void OnBeforeTrans() { }
        //修改该方法的获取范围从internal 改为 public
        public TransResult Communicate()
        {
            lock (sLock)
            {
                ActivityManager.SystemStatus |= AppStatus.OnCommunicating;
                OnBeforeTrans();
                if (Result == TransResult.E_INVALID)
                {
                    ActivityManager.SystemStatus &= ~AppStatus.OnCommunicating;
                    return Result;
                }
                Result = transact();
                ActivityManager.SystemStatus &= ~AppStatus.OnCommunicating;
            }

            handleResult();
            return Result;
        }

        private void handleResult()
        {
            try
            {
                if (RealEnv)
                {
                    switch (Result)
                    {
                        case TransResult.E_SUCC:
                            try
                            {
                                OnSucc();
                            }
                            catch (System.Exception ex)
                            {
                                Log.Error(this.GetType().Name, ex);
                                SetResult(TransResult.E_UNPACKET_FAIL);
                            }
                            break;
                        case TransResult.E_HOST_FAIL:
                            OnHostFail(ReturnCode, ReturnMessage);
                            break;
                        case TransResult.E_RECV_FAIL:
                            OnRecvFail();
                            break;
                        default:
                            OnOtherResult();
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(this.GetType().Name, ex);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(this.GetType().Name + ":" + Result.ToString());
            if (!RealEnv)
            {
                Log.Info(sb.ToString());
                return;
            }
            if (Result == TransResult.E_INVALID)
            {

            }
            else if (Result == TransResult.E_SUCC)
            {
                Log.Info(sb.ToString());
                sb.AppendLine();
                CLog.Info(sb.ToString());
            }
            else if (Result == TransResult.E_HOST_FAIL)
            {
                sb.Append(",ReturnCode:" + ReturnCode + ",ReturnMessage:" + ReturnMean);
                Log.Warn(sb.ToString());
                sb.AppendLine();
                CLog.Warn(sb.ToString());
            }
            else
            {
                Log.Warn(sb.ToString());
                sb.AppendLine();
                CLog.Warn(sb.ToString());
            }
        }

        protected virtual void OnRecvFail()
        {

        }

        protected virtual void OnHostFail(string returnCode, string returnMessage)
        {

        }

        protected virtual void OnSucc()
        {

        }

        protected virtual void OnOtherResult()
        {

        }

        #region save or restore business data
        private static Bundle businessBundle = Activity.businessBundle;

        protected static void SaveString(string key, string value)
        {
            businessBundle.PutString(key, value);
        }

        protected static void SaveInt(string key, int value)
        {
            businessBundle.PutInt(key, value);
        }

        protected static void SaveDouble(string key, double value)
        {
            businessBundle.PutDouble(key, value);
        }

        protected static void SaveBoolean(string key, Boolean value)
        {
            businessBundle.PutBoolean(key, value);
        }

        protected static void SaveStringArray(string key, string[] value)
        {
            businessBundle.PutStringArray(key, value);
        }

        protected static void SaveIntArray(string key, int[] value)
        {
            businessBundle.PutIntArray(key, value);
        }

        protected static void Save(string key, object value)
        {
            businessBundle.Put(key, value);
        }

        protected static string RestoreString(string key)
        {
            return businessBundle.GetString(key);
        }

        protected static int RestoreInt(string key)
        {
            return businessBundle.GetInt(key);
        }

        protected static double RestoreDouble(string key)
        {
            return businessBundle.GetDouble(key);
        }


        protected static Boolean RestoreBoolean(string key)
        {
            return businessBundle.GetBoolean(key);
        }

        protected static string[] RestoreStringArray(string key)
        {
            return businessBundle.GetStringArray(key);
        }

        protected static int[] RestoreIntArray(string key)
        {
            return businessBundle.GetIntArray(key);
        }

        protected static object Restore(string key)
        {
            return businessBundle.Get(key);
        }
        #endregion

        #region save or restore global data
        internal static Bundle globalBundle = Activity.globalBundle;

        protected static void SaveStringGlobal(string key, string value)
        {
            globalBundle.PutString(key, value);
        }

        protected static void SaveIntGlobal(string key, int value)
        {
            globalBundle.PutInt(key, value);
        }

        protected static void SaveDoubleGlobal(string key, double value)
        {
            globalBundle.PutDouble(key, value);
        }

        protected static void SaveBooleanGlobal(string key, Boolean value)
        {
            globalBundle.PutBoolean(key, value);
        }

        protected static void SaveStringArrayGlobal(string key, string[] value)
        {
            globalBundle.PutStringArray(key, value);
        }

        protected static void SaveIntArrayGlobal(string key, int[] value)
        {
            globalBundle.PutIntArray(key, value);
        }

        protected static void SaveGlobal(string key, object value)
        {
            globalBundle.Put(key, value);
        }

        protected static string RestoreStringGlobal(string key)
        {
            return globalBundle.GetString(key);
        }

        protected static int RestoreIntGlobal(string key)
        {
            return globalBundle.GetInt(key);
        }

        protected static double RestoreDoubleGlobal(string key)
        {
            return globalBundle.GetDouble(key);
        }


        protected static Boolean RestoreBooleanGlobal(string key)
        {
            return globalBundle.GetBoolean(key);
        }

        protected static string[] RestoreStringArrayGlobal(string key)
        {
            return globalBundle.GetStringArray(key);
        }

        protected static int[] RestoreIntArrayGlobal(string key)
        {
            return globalBundle.GetIntArray(key);
        }

        protected static object RestoreGlobal(string key)
        {
            return globalBundle.Get(key);
        }
        #endregion
    }
}
