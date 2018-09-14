using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;

namespace Landi.FrameWorks
{
    public class INIClass
    {
        public string inipath;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, [In, Out] char[] retVal, int nSize, string filePath);  

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="INIPath">文件路径</param>
        public INIClass(string INIPath)
        {
            inipath = INIPath;
        }

        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="Section">项目名称(如 [TypeName] )</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.inipath);
        }

        /// <summary>
        /// 读出INI文件
        /// </summary>
        /// <param name="Section">项目名称(如 [TypeName] )</param>
        /// <param name="Key">键</param>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, this.inipath);
            return temp.ToString();
        }

        /// <summary>
        /// 读出节点下所有数据INI文件
        /// </summary>
        /// <param name="Section">项目名称(如 [TypeName]</param>
        /// <returns></returns>
        public Hashtable IniReadValue(string Section)
        {
            StringBuilder temp = new StringBuilder(1000);
            char[] keys = new char[1000];
            int i = GetPrivateProfileString(Section, null, null, keys, 1000, this.inipath);
            Hashtable hs = new Hashtable();
            if (i != 0)
            {
                string[] value = new string(keys).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in value)
                {
                    hs.Add(item, IniReadValue(Section, item));
                }
            }

            return hs;
        }

        /// <summary>
        /// 验证文件是否存在
        /// </summary>
        /// <returns>布尔值</returns>
        public bool ExistINIFile()
        {
            return File.Exists(inipath);
        }
    }

}
