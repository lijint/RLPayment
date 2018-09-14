using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace Landi.FrameWorks
{
    public static class AdManager
    {
        private static AdFormBase mCurrentAd;
        private static void WriteIniFile(string key, string value)
        {
            ConfigFile.WriteConfig("AdConfig", key, value);
        }

        private static string ReadIniFile(string key)
        {
            return ConfigFile.ReadConfigAndCreate("AdConfig", key);
        }

        private static bool mIsUse = false;
        private static string mAdType = typeof(AdManager).Namespace + ".";
        private static string mPath = Path.Combine(Application.StartupPath, "Ad");
        private static int mLeft = 0;
        private static int mTop = 0;
        private static int mWidth = 1024;
        private static int mHeight = 768;
        private static bool mTopMost = true;

        internal static event MethodInvoker EnterAd;
        internal static event MethodInvoker LeaveAd;

        internal static void NotifyEnterAd()
        {
            if (EnterAd != null)
                EnterAd();
        }

        internal static void NotifyLeaveAd()
        {
            if (LeaveAd != null)
                LeaveAd();
        }

        static AdManager()
        {
            string[] items ={ "Use", "Type", "Path", "Left", "Top", "Width", "Height", "SwitchInterval" };
            for (int i = 0; i < items.Length; i++)
            {
                if (ReadIniFile(items[i]) == "")
                {
                    switch (items[i])
                    {
                        case "Type":
                            WriteIniFile(items[i], "Picture");
                            break;
                        case "Path":
                            WriteIniFile(items[i], "Ad");
                            break;
                        case "Left":
                            WriteIniFile(items[i], "0");
                            break;
                        case "Top":
                            WriteIniFile(items[i], "0");
                            break;
                        case "Width":
                            WriteIniFile(items[i], "1024");
                            break;
                        case "Height":
                            WriteIniFile(items[i], "768");
                            break;
                        case "TopMost":
                            WriteIniFile(items[i], "1");
                            break;
                        case "SwitchInterval":
                            WriteIniFile(items[i], "5");
                            break;
                        case "Use":
                            WriteIniFile(items[i], "0");
                            break;
                    }
                }
            }
            try
            {
                if (ReadIniFile("Use") == "0")
                    mIsUse = false;
                else
                    mIsUse = true;
                mAdType += ReadIniFile("Type") + "AdForm";
                mPath = Path.Combine(Application.StartupPath, ReadIniFile("Path"));
                mLeft = int.Parse(ReadIniFile("Left"));
                mTop = int.Parse(ReadIniFile("Top"));
                mWidth = int.Parse(ReadIniFile("Width"));
                mHeight = int.Parse(ReadIniFile("Height"));
                if (ReadIniFile("TopMost") == "0")
                    mTopMost = false;
                else
                    mTopMost = true;
            }
            catch (System.Exception ex)
            {
                Log.Error(null, ex);
            }
        }

        private static bool mInitialized;
        internal static void ShowAd()
        {
            if (!mIsUse)
                return;
            if (mCurrentAd == null)
            {
                mCurrentAd = (AdFormBase)Assembly.GetAssembly(typeof(AdManager)).CreateInstance(mAdType, false);
            }
            if (mCurrentAd != null)
            {
                if (!mInitialized)
                {
                    if (!mCurrentAd.Initialize(mPath, mLeft, mTop, mWidth, mHeight, mTopMost))
                    {
                        mCurrentAd = null;
                        throw new Exception("广告初始化失败");
                    }
                    else
                        mInitialized = true;
                }
                if (mInitialized)
                    mCurrentAd.ShowAd();
            }
            else
                throw new Exception("暂不支持的广告类型");
        }

        internal static void HideAd()
        {
            if (mIsUse && mCurrentAd != null)
            {
                mCurrentAd.HideAd();
            }
        }
    }
}
