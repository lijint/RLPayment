using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Collections;

namespace Landi.FrameWorks
{
    public static class ConfigFile
    {
        private static readonly INIClass instance = new INIClass(Path.Combine(Application.StartupPath, "FrameWorks.ini"));

        public static void WriteConfig(string sectionName,string key,string value)
        {
            instance.IniWriteValue(sectionName, key, value);
        }

        private static string ReadConfigInner(string sectionName, string key, bool createWhenNoExist)
        {
           return  ReadConfigInner(sectionName, key, createWhenNoExist, "");
        }

        private static string ReadConfigInner(string sectionName, string key, bool createWhenNoExist,string defaultValue)
        {
            string content = instance.IniReadValue(sectionName, key);
            if (content == "" && createWhenNoExist)
            {
                instance.IniWriteValue(sectionName, key, defaultValue);
                content = defaultValue;
            }
            return content;
        }

        public static string ReadConfig(string sectionName, string key)
        {
            return ReadConfigInner(sectionName, key, false);
        }

        public static Hashtable ReadConfig(string sectionName)
        {
            return instance.IniReadValue(sectionName);
        }

        public static string ReadConfigAndCreate(string sectionName, string key)
        {
            return ReadConfigInner(sectionName, key, true);
        }

        public static string ReadConfigAndCreate(string sectionName, string key,string defaultValue)
        {
            return ReadConfigInner(sectionName, key, true, defaultValue);
        }
    }
}
