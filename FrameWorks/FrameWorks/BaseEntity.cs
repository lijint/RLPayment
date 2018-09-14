using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks
{
    [SerializableAttribute]
    public abstract class BaseEntity
    {
        #region IC数据

        /// <summary>
        /// IC卡发送55域
        /// </summary>
        public byte[] SendField55 = new byte[0];
        /// <summary>
        /// IC卡接收55域
        /// </summary>
        public byte[] RecvField55 = new byte[0];
        /// <summary>
        /// 38域
        /// </summary>
        public string RecvField38 = "";
        /// <summary>
        /// 是否是IC卡
        /// </summary>
        public bool IsICCard = false;
        #endregion

        /// <summary>
        /// 业务名称
        /// </summary>
        public string BusinessName = "";
        /// <summary>
        /// 支付流水号
        /// </summary>
        public string PayTraceNo = "";
        /// <summary>
        /// 系统参考号
        /// </summary>
        public string PayReferenceNo = "";

        /// <summary>
        /// 配置节点名称
        /// </summary>
        public abstract string SectionName
        {
            get;
        }

        /// <summary>
        /// 业务返回码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mean"></param>
        /// <param name="show"></param>
        public virtual void ParseRespMessage(string code, ref string mean, ref string show)
        {
        }

        /// <summary>
        /// 数据库开关
        /// </summary>
        public bool AccessSwitch
        {
            get { return ReadIniFile("AccessSwitch") == "1"; }
            set { WriteIniFile("AccessSwitch", value ? "1" : "0"); }
        }
        /// <summary>
        /// 数据库驱动
        /// </summary>
        public string AccessProviderName
        {
            get { return ReadIniFile("AccessProviderName"); }
            set { WriteIniFile("AccessProviderName", value); }
        }
        /// <summary>
        /// 数据库文件
        /// </summary>
        public string AccessFile
        {
            get { return ReadIniFile("AccessFile"); }
            set { WriteIniFile("AccessFile", value); }
        }
        /// <summary>
        /// 数据库密码
        /// </summary>
        public string AccessPin
        {
            get { return ReadIniFile("AccessPin"); }
            private set { WriteIniFile("AccessPin", value); }
        }

        /// <summary>
        /// 初始化下载AID和CA
        /// </summary>
        public bool DownLoadAidAndCA
        {
            get { return ReadIniFile("DownLoadAidAndCA") == "1"; }
            set { WriteIniFile("DownLoadAidAndCA", value ? "1" : "0"); }
        }

        /// <summary>
        /// 是否启用IC卡
        /// </summary>
        public bool UseICCard
        {
            get { return ReadIniFile("UseICCard") == "1"; }
            set { WriteIniFile("UseICCard", value ? "1" : "0"); }
        }

        /// <summary>
        /// 交易实例是否启用
        /// </summary>
        public bool IsUseEntity
        {
            get { return ReadIniFile("Use") == "1"; }
            private set { WriteIniFile("Use", value ? "1" : "0"); }
        }

        protected void WriteIniFile(string key, string value)
        {
            ConfigFile.WriteConfig(SectionName, key, value);
        }

        protected string ReadIniFile(string key)
        {
            return ConfigFile.ReadConfigAndCreate(SectionName, key);
        }
    }
}
