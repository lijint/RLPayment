using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;
using InputChaIphoneLib;
using System.IO;
using EudemonLink;
using TerminalLib;
using Microsoft.Win32;

namespace RLPayment
{
    public partial class Form1 : Form
    {
        System.Timers.Timer m_timer;
        private bool m_check;
        public Form1()
        {
            WriteWebBrowserRegKey("FEATURE_BROWSER_EMULATION", 10001, true);//此处暂时只能设置到IE10,如果设置成IE11,加载的页面内容将允许被选中
            InitializeComponent();
            m_check = false;
            m_timer = new System.Timers.Timer();
            m_timer.Interval = 1000;
            Cursor.Hide();
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(m_timer_Elapsed);
            Global.gTerminalPay = new TerminalPay();
            if (GlobalAppData.GetInstance().EudemonSwitch)
            {
                EudemonHandler.Instance.SetEudemonTicket(60);
            }
        }

        void m_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                m_timer.Stop();
                if (ActivityManager.AppSystemStatus == AppStatus.Idle ||
                    ActivityManager.AppSystemStatus == AppStatus.OnAd ||
                    ActivityManager.AppSystemStatus == AppStatus.OnStopServer)
                {
                    if (no_service())
                    {
                        if (m_check)
                            return;
                        FileBackUp();
                        m_check = true;
                    }
                    else
                        m_check = false;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error("[m_timer_Elapsed]Error", ex);
               
            }
            finally
            {
                m_timer.Start();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ActivityManager.BeforeStart += new MethodInvoker(ActivityManager_BeforeStart);
            ActivityManager.BeforeQuit += new MethodInvoker(ActivityManager_BeforeQuit);
            ActivityManager.Start(webBrowser1);
        }

        void ActivityManager_BeforeQuit()
        {
            frmInputCha.Instanse.Dispose();
            numberInput.Instanse.Dispose();
        }

        void ActivityManager_BeforeStart()
        {
            frmInputCha.Instanse.ShowConvertLanguage = false;
            frmInputCha.Instanse.SetDesktopLocation(150, 435);
            numberInput.Instanse.SetDesktopLocation(150, 435);
            //m_timer.Start();
        }

        #region 操作文件

        /// <summary>
        /// 文件备份
        /// </summary>
        private void FileBackUp()
        {
            //备份文件
            //string sourceFile = Path.Combine(Environment.CurrentDirectory, GlobalAppData.GetInstance().AccessFile);
            //string destFile = Path.Combine(Environment.CurrentDirectory, String.Format("Backup\\Log{0}.bak", DateTime.Now.ToString("yyyyMMddHHmmss")));
            //if (Utility.CopyFile(sourceFile, destFile))
            //{
            //    //复制成功，则清理一次数据
            //    //TransAccessFactory accessCom = new TransAccessFactory();
            //    //accessCom.DeleteTransLog();
            //    Log.Info("备份数据库成功");
            //}
            //else
            //{
            //    Log.Warn("备份数据库失败");
            //}
            //删除有效期外的文件
            DeleteFiles(Environment.CurrentDirectory + @"\Log");
            DeleteFiles(Environment.CurrentDirectory + @"\Backup");
        }

        private void DeleteFolder(string deleteDirectory)
        {
            if (Directory.Exists(deleteDirectory))
            {
                foreach (string deleteFile in Directory.GetFileSystemEntries(deleteDirectory))
                {
                    if (File.Exists(deleteFile))

                        File.Delete(deleteFile);
                    else
                        DeleteFolder(deleteFile);
                }
                Directory.Delete(deleteDirectory);
            }
        }

        private void DeleteFiles(string dir)
        {
            try
            {
                if (Directory.Exists(dir)) //如果存在这个文件夹删除之
                {
                    foreach (string file in Directory.GetFileSystemEntries(dir))
                    {
                        if (!file.Contains("."))
                        {

                            int nFileNameStart = file.LastIndexOf("\\") + 1;
                            string fileName = file.Substring(nFileNameStart);
                            string year = fileName.Substring(0, 4);
                            string month = fileName.Substring(4, 2);
                            string day = fileName.Substring(6, 2);
                            DateTime logTime = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));

                            DateTime currentTime = DateTime.Parse(DateTime.Now.ToString());//当前日期
                            System.TimeSpan ts = currentTime - logTime;//日期相减
                            int days = ts.Days;//得到相减后的天数
                            if (days > 30 && Directory.Exists(file))//删除30天之外的日志文件
                            {
                                Log.Info("删除" + fileName + "文件夹");
                                DeleteFolder(file);
                            }
                        }
                        else
                        {
                            int start = file.LastIndexOf("\\") + 1;//得到文件名（日期）开始
                            string strFileName = file.Substring(start);
                            if (start != -1)
                            {
                                string fileName;
                                if (file.Substring(file.LastIndexOf(".") + 1).ToLower() == "bak")
                                    fileName = file.Substring(start + 3, 8);
                                else if (file.Substring(file.LastIndexOf(".") + 1).ToLower() == "log")
                                    fileName = file.Substring(start, 8);//end - start);//截取文件名,end - start为截取的长度
                                else
                                {
                                    Log.Info("删除" + strFileName + "文件");
                                    File.Delete(file); //直接删除其中的文件
                                    continue;
                                }

                                string year = fileName.Substring(0, 4);
                                string month = fileName.Substring(4, 2);
                                string day = fileName.Substring(6, 2);
                                DateTime logTime = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));

                                DateTime currentTime = DateTime.Parse(DateTime.Now.ToString());//当前日期
                                System.TimeSpan ts = currentTime - logTime;//日期相减
                                int days = ts.Days;//得到相减后的天数

                                if (days > 30 && File.Exists(file))//删除30天之外的日志文件
                                {
                                    Log.Info("删除" + strFileName + "文件");
                                    File.Delete(file); //直接删除其中的文件
                                }
                            }
                        }

                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Info("[DeleteFiles]Error", ex);
            }

        }
        #endregion

        bool no_service()
        {
            //Test
            //return false;

            bool bRet = false;
            TimeSpan t = DateTime.Now.TimeOfDay;
            if ((t.Hours == 22 && t.Minutes >= 30) ||
                t.Hours == 23 ||
                t.Hours == 0)
            {
                 bRet = true;
            }

            return bRet;
        }
        public static void WriteWebBrowserRegKey(string regkeyname, int regkeyvalue, bool bIsUse)
        {
            RegistryKey reg = null;
            try
            {
                reg = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\" + regkeyname, true);
                if (reg == null)
                    reg = Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\" + regkeyname);
                string applicationname = Application.ProductName + @".exe";
                if (bIsUse)
                {
                    reg.SetValue(applicationname, regkeyvalue, RegistryValueKind.DWord);
                }
                else
                {
                    reg.DeleteValue(applicationname, false);
                }

            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                if (reg != null)
                    reg.Close();
            }
        }

    }
}