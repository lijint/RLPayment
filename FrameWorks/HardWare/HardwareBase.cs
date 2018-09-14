using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Landi.FrameWorks.HardWare
{
    public class HardwareBase<T,S> : Singleton<T>
        where T : HardwareBase<T,S> ,new()
    {
        private bool mIsUse;
        public static bool IsUse
        {
            get { return GetInstance().mIsUse; }
        }

        private string mPort;
        public static string Port
        {
            get { return GetInstance().mPort; }
        }

        private int mBps;
        public static int Bps
        {
            get { return GetInstance().mBps; }
        }

        private string mSectionName;
        public static string Name
        {
            get { return GetInstance().mSectionName; }
        }

        private bool mHasConfig = true;

        protected HardwareBase()
        {
            //配置节点
            mSectionName = this.GetType().Name;

            string[] keys = new string[] { "Use", "Port", "Bps" };
            for (int i = 0; i < keys.Length; i++)
            {
                if (ReadIniFile(keys[i]) == "")
                {
                    if (i != 0)
                    {
                        if (i == 1)
                            mHasConfig = false;
                    }
                    else
                    {
                        WriteIniFile(keys[i], "0");
                    }
                }
            }
            if (!mHasConfig)
                throw new Exception(mSectionName + "尚未配置");
            else
            {
                mPort = ReadIniFile("Port").ToUpper();
                if (!mPort.StartsWith("COM"))
                    mPort = "COM" + mPort;
                mBps = int.Parse(ReadIniFile("Bps"));
                if (ReadIniFile("Use") == "0")
                    mIsUse = false;
                else
                    mIsUse = true;
            }
        }

        public static void AddedToManager()
        {
            if (GetInstance() is IManagedHardware)//要保证<T>实现IManagedHardware
                HardwareManager.AddHardWare(Name, GetInstance() as IManagedHardware);
            else
                throw new Exception(Name + "未实现IManagedHardware接口");
        }

        public static void RemovedFromManager()
        {
            if (GetInstance() is IManagedHardware)
                HardwareManager.RemoveHardWare(Name, GetInstance() as IManagedHardware);
        }

        protected void WriteIniFile(string key, string value)
        {
            ConfigFile.WriteConfig(mSectionName, key, value);
        }

        protected string ReadIniFile(string key)
        {
            return ConfigFile.ReadConfigAndCreate(mSectionName, key);
        }

        protected static S AsStatus(object status)
        {
            return (S)status;
        }

        public static S GetStatusFromManager()
        {
            return (S)HardwareManager.GetState(Name);
        }

        public static bool ExistError()
        {
            return HardwareManager.ExistError(Name);
        }

        public static bool CheckedByManager()
        {
            return HardwareManager.CheckOne(Name);
        }

        public static bool OpenByManager()
        {
            return HardwareManager.OpenOne(Name);
        }

        public static bool CloseByManager()
        {
            return HardwareManager.CloseOne(Name);
        }
    }
}
