using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Net;
using Landi.FrameWorks.Common;


namespace Landi.FrameWorks
{
    public class Activity
    {
        internal static ActivityManager Context;

        internal static ActivityHandler aH;
        internal static MessageQueue mQueue;
        private static bool mUserToQuit = false;
        private static bool mTimeIsOut = false;
        private static object mLock = new object();
        private static bool mTransCompleted = false;
        internal bool mKeepAlive = true;
        protected static readonly string StartupPath = Application.StartupPath;

        protected Activity() { }

        protected static bool UserToQuit
        {
            get { lock (mLock) { return mUserToQuit; } }
            set { lock (mLock) { mUserToQuit = value; } }
        }

        protected static bool TimeIsOut
        {
            get { lock (mLock) { return mTimeIsOut; } }
            set { lock (mLock) { mTimeIsOut = value; } }
        }

        #region BusinessStratagy
        internal static BusinessStratagy Stratagy;
        internal const string ENTITYKEY = "lanpp_BaseEntity";
        protected void EnterBusiness(BusinessStratagy stratagy)
        {
            Stratagy = stratagy;
            Save(ENTITYKEY, stratagy.BusinessEntity);
            (Restore(ENTITYKEY) as BaseEntity).BusinessName = stratagy.BusinessName;
        }

        protected static string GetBusinessName()
        {
            return Stratagy.BusinessName;
        }

        protected static BaseEntity GetBusinessEntity()
        {
            return Restore(ENTITYKEY) as BaseEntity;
        }

        protected static T GetBusinessEntity<T>() where T : BaseEntity
        {
            return Restore(ENTITYKEY) as T;
        }

        protected static void ShowMessageAndGotoMain(object message)
        {
            ShowMessage(message, null, true);
        }

        protected static void ShowMessageAndGotoMain(object message, MethodInvoker method)
        {
            ShowMessage(message, method, true);
        }

        protected static void ShowMessageAndGoBack(object message)
        {
            ShowMessage(message, null, false);
        }

        protected static void ShowMessageAndGoBack(object message, MethodInvoker method)
        {
            ShowMessage(message, method, false);
        }

        private static Intent ShowMessageIntent = new Intent();
        private static void ShowMessage(object message, MethodInvoker method, bool mainOrBack)
        {
            if (!string.IsNullOrEmpty(Stratagy.MessageActivity))
            {
                ShowMessageIntent.ActivityName = Stratagy.MessageActivity;
                ShowMessageIntent.ClearExtra();
                ShowMessageIntent.PutExtra(MessageActivity.MESSAGE, message);
                ShowMessageIntent.PutExtra(MessageActivity.Main_OR_BACK, mainOrBack);
                ShowMessageIntent.PutExtra(MessageActivity.METHOD, method);
                StartActivity(ShowMessageIntent);
            }
        }
        #endregion

        protected static void Flush()
        {
            Application.DoEvents();
        }

        protected static void Sleep(int millSecond)
        {
            Thread.Sleep(millSecond);
        }

        protected static void DestroyActivity(string activityName)
        {
            Context.DestroyActivity(activityName, null);
        }

        protected static void DestroyActivity(Type activityClass)
        {
            Context.DestroyActivity(null, activityClass.FullName);
        }

        protected void DestroySelf()
        {
            mKeepAlive = false;
        }

        #region SendKey
        protected static void SendKeyDown(Keys key)
        {
            DllImport.keybd_event((byte)key, 0, 0, 0);
        }

        protected static void SendKeyUp(Keys key)
        {
            DllImport.keybd_event((byte)key, 0, 2, 0);
        }
        #endregion

        /// <summary>
        /// 设置定时器在界面上的显示属性
        /// </summary>
        /// <param name="config"></param>
        protected static void SetTimerConfig(TimerConfig config)
        {
            Context.SetTimerConfig(config);
        }

        #region Report
        /// <summary>
        /// 调用后主线程同步执行OnReport函数,OnReport函数由子类重写
        /// </summary>
        /// <param name="reportedData">传给OnReport的参数</param>
        protected static void ReportSync(object reportedData)
        {
            Context.Report(reportedData);
        }

        /// <summary>
        /// 调用后主线程异步执行OnReport函数,若在主线程调用则为同步执行，OnReport函数由子类重写
        /// </summary>
        /// <param name="reportedData">传给OnReport的参数</param>
        protected static void ReportAsync(object reportedData)
        {
            Context.ReportAsync(reportedData);
        }

        internal static void InvokeReport(Activity target,object reportedData)
        {
            target.OnReport(reportedData);
        }

        protected virtual void OnReport(object progress)
        {

        }

        #endregion

        #region PlaySound
        protected void PlaySound()
        {
            PlaySound(MyIntent.SoundPath);
        }

        protected static void PlaySound(string soundPath)
        {
            if (!string.IsNullOrEmpty(soundPath))
                DllImport.PlaySound(soundPath, 0, DllImport.SoundFlag.SND_ASYNC | DllImport.SoundFlag.SND_FILENAME);
        }
        #endregion

        #region add or remove IdleHandler
        protected static void AddIdleHandler(MessageQueue.IdleHandler handler)
        {
            mQueue.AddIdleHandler(handler);
        }

        protected static void RemoveIdleHandler(MessageQueue.IdleHandler handler)
        {
            mQueue.RemoveIdleHandler(handler);
        }
        #endregion

        #region SendMessage
        protected static void SendMessage(int what)
        {
            SendMessage(what, null, 0, 0);
        }

        protected static void SendMessage(int what, object obj)
        {
            SendMessage(what, obj, 0, 0);
        }

        protected static void SendMessage(int what, object obj, int arg1)
        {
            SendMessage(what, obj, arg1, 0);
        }

        protected static void SendMessage(int what, object obj, int arg1, int arg2)
        {
            lock (mLock)
            {
                Message msg = Message.Obtain(what, arg1, arg2, obj);
                if (what != ActivityHandler.TRANSACTION || !aH.HasMessages(ActivityHandler.TRANSACTION))
                    aH.SendMessage(msg);
            }
        }

        protected static bool PostAsync(MethodInvoker r)
        {
            return aH.Post(r);
        }

        protected static bool PostSync(MethodInvoker r)
        {
            Message mess = Message.Obtain(r);
            mess.waitEvent = new AutoResetEvent(false);
            bool status = aH.SendMessage(mess);
            if (status)
                while (!mess.handled)
                    Application.DoEvents();
            Application.DoEvents();
            mess.waitEvent.Set();
            return status;
        }

        protected static bool PostDelayed(MethodInvoker r, long delayMillis)
        {
            return aH.PostDelayed(r, delayMillis);
        }

        protected static bool PostAtDateTime(MethodInvoker r, DateTime when)
        {
            if (when < DateTime.Now)
                return false;
            TimeSpan span = when - DateTime.Now;
            return aH.PostDelayed(r, (int)span.TotalMilliseconds);
        }

        protected static bool PostAtTime(MethodInvoker r, string whenStr)
        {
            DateTime when = DateTime.Now;
            if (!DateTime.TryParse(whenStr, out when))
                return false;
            return PostAtDateTime(r, when);
        }

        #endregion

        public Intent MyIntent;

        #region show or hide ad
        protected virtual void OnSwitchToAd()
        {

        }

        protected virtual void OnRecoverFromAd()
        {

        }

        public static event MethodInvoker EnterAd
        {
            add { AdManager.EnterAd += value; }
            remove { AdManager.EnterAd -= value; }
        }

        public static event MethodInvoker LeaveAd
        {
            add { AdManager.LeaveAd += value; }
            remove { AdManager.LeaveAd -= value; }
        }

        protected static void ShowAd()
        {
            Context.ShowAd();
        }

        protected static void HideAd()
        {
            Context.HideAd();
        }
        #endregion

        /// <summary>
        /// print current system runtime information for debugging
        /// </summary>
        /// <returns>runtime infomation</returns>
        protected static string Dump()
        {
            return Context.Dump();
        }

        internal static void Creat(Activity target)
        {
            try
            {
                if (target != null)
                    target.OnCreate();
            }
            catch (Exception ex)
            {
                Log.Error(target, ex);
            }
            finally
            {
                Application.DoEvents();
            }
        }

        internal static void Destroy(Activity target)
        {
            try
            {
                if (target != null)
                    target.OnDestroy();
            }
            catch (Exception ex)
            {
                Log.Error(target, ex);
            }
            finally
            {
                Application.DoEvents();
            }
        }

        internal static void SwitchToAd(Activity target)
        {
            try
            {
                target.OnSwitchToAd();
            }
            catch (Exception ex)
            {
                Log.Error(target, ex);
            }
            finally
            {
                Application.DoEvents();
            }
        }

        internal static void RecoverFormAd(Activity target)
        {
            try
            {
                target.OnRecoverFromAd();
            }
            catch (Exception ex)
            {
                Log.Error(target, ex);
            }
            finally
            {
                Application.DoEvents();
            }
        }

        internal static void Enter(Activity target)
        {
            try
            {
                UserToQuit = false;
                TimeIsOut = false;
                if (target != null)
                {
                    PlaySound(target.MyIntent.SoundPath);
                    target.OnEnter();
                }
            }
            catch (Exception ex)
            {
                Log.Error(target, ex);
            }
            finally
            {
                Application.DoEvents();
            }
        }

        internal static void Leave(Activity target)
        {
            try
            {
                UserToQuit = true;
                if (target != null) 
                    target.OnLeave();
            }
            catch (Exception ex)
            {
                Log.Error(target, ex);
            }
            finally
            {
                if (!string.IsNullOrEmpty(target.MyIntent.SoundPath))
                    DllImport.PlaySound(null, 0, 0);
                Application.DoEvents();
            }
        }

        internal static void TimeOut(Activity target)
        {
            try
            {
                TimeIsOut = true;
                if (target != null) 
                    target.OnTimeOut();
            }
            catch (Exception ex)
            {
                Log.Error(target, ex);
            }
            finally
            {
                Application.DoEvents();
            }
        }

        internal static void KeyDown(Activity target,Keys keyCode)
        {
            try
            {
                if (target != null)
                    target.OnKeyDown(keyCode);
            }
            catch (System.Exception ex)
            {
                Log.Error(target, ex);
            }
            finally
            {
                //Application.DoEvents();//加了之后组合键不行
            }
        }

        protected virtual void OnCreate()
        {

        }

        protected virtual void OnDestroy()
        {

        }

        protected virtual void OnEnter()
        {

        }

        protected virtual void OnLeave()
        {

        }

        /// <summary>
        /// 创建注册表自运行批处理文件
        /// </summary>
        /// <returns></returns>
        protected static bool SetAutoRunCtrlRegInfo(bool isApp)
        {
            RegistryKey reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
            if (reg != null)
            {
                if (isApp)
                    reg.SetValue("Userinit", Application.ExecutablePath);
                else
                    reg.SetValue("Userinit", "C:/Windows/system32/userinit.exe");
                reg.Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 设置进入管理界面按钮
        /// </summary>
        /// <param name="manageEntryID"></param>
        protected static void SetManageEntryInfo(string manageEntryID)
        {
            if (string.IsNullOrEmpty(manageEntryID) ||
                GetElementById(manageEntryID) == null)
                throw new Exception("错误的管理界面进入ID");

            HtmlElement btnManageEntryID = GetElementById(manageEntryID);
            btnManageEntryID.Click -= new HtmlElementEventHandler(btnManageEntryID_Click);
            btnManageEntryID.Click += new HtmlElementEventHandler(btnManageEntryID_Click);
        }

        private static void btnManageEntryID_Click(object sender, HtmlElementEventArgs e)
        {
            Context.ManagerEntry();
        }

        /// <summary>
        /// 若默认超时返回主界面则不需重载，否则重载并调用StartActivit或者StartFullActivity跳转到自定义界面
        /// </summary>
        protected virtual void OnTimeOut()
        {
            Context.GotoMain();
        }

        protected static void SetIfResetTimerOnMouseDown(bool reset)
        {
            Context.ResetTimerOnMouseDown = reset;
        }

        protected virtual void OnKeyDown(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.PageDown://下一页
                    {
                        HtmlElement btnOK = GetElementById("OK");
                        if (btnOK != null)
                            btnOK.InvokeMember("Click");
                        break;
                    }
                case Keys.PageUp://上一页
                    {
                        HtmlElement btnBack = GetElementById("Back");
                        if (btnBack != null)
                            btnBack.InvokeMember("Click");
                        break;
                    }
                case Keys.Enter:
                    {
                        HtmlElement btnOK = GetElementById("OK");
                        if (btnOK != null)
                            btnOK.InvokeMember("Click");
                        else
                        {
                            HtmlElement btnRetry = GetElementById("Retry");
                            if (btnRetry != null)
                                btnRetry.InvokeMember("Click");
                        }
                        break;
                    }
                case Keys.Escape:
                    {
                        HtmlElement btnReturn = GetElementById("Return");
                        if (btnReturn != null)
                            btnReturn.InvokeMember("Click");
                        break;
                    }
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            //b.Append("{ name=");
            //b.Append(MyIntent.ActivityName);
            //if (Log.IsDebugEnabled)
            //{
            //    b.Append(" web=");
            //    b.Append(MyIntent.PageFileName);

            //    b.Append(" deal=");
            //    b.Append(this.GetType().Name);
            //}

            //b.Append(" }");
            b.Append("{ ");
            b.Append(MyIntent.ActivityName);
            b.Append(" }");

            return b.ToString();
        }

        protected static HtmlElement GetElementById(string id)
        {
            return Context.GetElementById(id);
        }

        protected static void StartActivity(string activityName)
        {
            Context.SendMessage(ActivityManagerHandler.LAUNCH_ACTIVITY, new Intent(activityName));
        }

        protected static void StartActivity(Intent intent)
        {
            Context.SendMessage(ActivityManagerHandler.LAUNCH_ACTIVITY, intent);
        }

        protected static void StartActivity(Type activityType)
        {
            Intent intent = new Intent();
            intent.ActivityClassName = activityType.FullName;
            Context.SendMessage(ActivityManagerHandler.LAUNCH_ACTIVITY, intent);
        }

        public virtual bool CanQuit()
        {
            return false;
        }

        protected static void Quit()
        {
            Context.Quit();
        }

        protected static void GotoMain()
        {
            Context.GotoMain();
        }

        protected static void GotoNext()
        {
            Context.GotoNext();
        }

        protected static void GoBack()
        {
            Context.GoBack();
        }

        /// <summary>
        /// 设置窗口位置及大小
        /// </summary>
        /// <param name="left">左边位置</param>
        /// <param name="top">上边位置</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        protected static void SetPosition(int left, int top, int width, int height)
        {
            Context.SetPosition(left, top, width, height);
        }

        protected static void SetTopMost(bool topmost)
        {
            Context.SetTopMost(topmost);
        }

        protected void InputAmount(string id, Keys keyCode)
        {
            HtmlElement element = null;
            if (string.IsNullOrEmpty(id) || (element = GetElementById(id)) == null)
                return;
            string currentValue = element.GetAttribute("value");
            switch (keyCode)
            {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    int dotIndex = -1;
                    if ((dotIndex = currentValue.IndexOf('.')) < 0 || currentValue.Length - dotIndex < 3)
                    {
                        if (dotIndex >= 0 || (dotIndex < 0 && currentValue.Length < 10))
                            element.SetAttribute("value", currentValue + (char)keyCode);
                    }
                    break;
                case Keys.OemPeriod:
                    if (currentValue.IndexOf('.') < 0 && currentValue.Length > 0)
                        element.SetAttribute("value", currentValue + ".");
                    break;
                case Keys.Back:
                    if (!string.IsNullOrEmpty(currentValue) && currentValue.Length > 0)
                    {
                        currentValue = currentValue.Substring(0, currentValue.Length - 1);
                        element.SetAttribute("value", currentValue);
                    }
                    break;
            }
        }

        protected void InputNumber(string id, Keys keyCode)
        {
            HtmlElement element = null;
            if (string.IsNullOrEmpty(id) || (element = GetElementById(id)) == null)
                return;
            string currentValue = element.GetAttribute("value");
            int maxlLength = int.Parse(element.GetAttribute("maxlength"));
            
            switch (keyCode)
            {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    if(currentValue.Length != maxlLength)
                        element.SetAttribute("value", currentValue + (char)keyCode);
                    break;
                case Keys.Back:
                    if (!string.IsNullOrEmpty(currentValue) && currentValue.Length > 0)
                    {
                        currentValue = currentValue.Substring(0, currentValue.Length - 1);
                        element.SetAttribute("value", currentValue);
                    }
                    break;
            }
        }

        protected static object InvokeScript(string scriptName)
        {
            return Context.InvokeScript(scriptName);
        }

        protected static object InvokeScript(string scriptName, object[] args)
        {
            return Context.InvokeScript(scriptName, args);
        }

        protected static bool Ping(string targetIP)
        {
            string strRet = Utility.PingResult(targetIP);
            if (strRet.ToLower() != "success")
            {
                return false;
            }
            return true;
        }

        protected static void RestartTimer()
        {
            Context.RestartTimer();
        }

        protected static AppStatus SystemStatus
        {
            get { return ActivityManager.SystemStatus; }
        }

        #region 动态库注册
        /// <summary>
        /// 执行cmd指令
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        protected static string ExeCommand(string commandText)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            string strOutput = null;
            try
            {
                p.Start();
                p.StandardInput.WriteLine(commandText);
                p.StandardInput.WriteLine("exit");
                strOutput = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
            }
            catch (Exception e)
            {
                strOutput = e.Message;
            }
            return strOutput;
        }

        /// <summary>
        /// 判断系统位数
        /// </summary>
        /// <returns></returns>
        protected static string Distinguish64or32System()
        {
            try
            {
                string addressWidth = String.Empty;
                ConnectionOptions mConnOption = new ConnectionOptions();
                ManagementScope mMs = new ManagementScope("//localhost", mConnOption);
                ObjectQuery mQuery = new ObjectQuery("select AddressWidth from Win32_Processor");
                ManagementObjectSearcher mSearcher = new ManagementObjectSearcher(mMs, mQuery);
                ManagementObjectCollection mObjectCollection = mSearcher.Get();
                foreach (ManagementObject mObject in mObjectCollection)
                {
                    addressWidth = mObject["AddressWidth"].ToString();
                }
                return addressWidth;
            }
            catch (Exception ex)
            {
                Log.Error("[Distinguish64or32System]判断系统异常Error", ex);
                return String.Empty;
            }
        }

        protected static bool RegsvrStarTrans()
        {
            bool bRet = false;
            try
            {
                //是否是初装机

                if (Distinguish64or32System().Contains("64"))
                {
                    Log.Info("64位操作系统");
                    Register64();
                }
                else
                {
                    Log.Info("32位操作系统");
                    Register32();
                }
                bRet = true;
            }
            catch (Exception ex)
            {
                Log.Error("[RegsvrStarTrans]注册异常Error", ex);
            }
            return bRet;
        }
        /// <summary>
        /// 32位注册dll库
        /// </summary>
        private static void Register32()
        {
            try
            {
                //ExeCommand("del %Windir%\\system32\\msxml4.dll");
                //ExeCommand("del %Windir%\\system32\\msxml4r.dll");
                ExeCommand("copy msxml4.dll %Windir%\\system32\\");
                ExeCommand("copy msxml4r.dll %Windir%\\system32\\");
                ExeCommand("regsvr32 %Windir%\\system32\\msxml4.dll /s");
                //ExeCommand("regsvr32 BaseSTARTrans.dll /s");
                //ExeCommand("regsvr32 JhSTARTrans.dll /s");
            }
            catch
            {
                throw;
            }

        }

        /// <summary>
        /// 64位注册dll库
        /// </summary>
        private static void Register64()
        {
            try
            {
                ExeCommand("cd /d %~dp0");
                //ExeCommand("del %Windir%\\SysWOW64\\msxml4.dll");
                //ExeCommand("del %Windir%\\SysWOW64\\msxml4r.dll");
                ExeCommand("copy msxml4.dll %Windir%\\SysWOW64\\ /y");
                ExeCommand("copy msxml4r.dll %Windir%\\SysWOW64\\ /y");
                ExeCommand("%Windir%\\SysWOW64\\regsvr32.exe %windir%\\SysWOW64\\msxml4.dll /s");
                //ExeCommand("%Windir%\\SysWOW64\\regsvr32.exe  BaseSTARTrans.dll /s");
                //ExeCommand("%Windir%\\SysWOW64\\regsvr32.exe  JhSTARTrans.dll /s");
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region save or restore business data
        internal static Bundle businessBundle = new Bundle();

        protected static void ClearBusinessData()
        {
            businessBundle.Clear();
        }

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
        internal static Bundle globalBundle = new Bundle();
        protected static void ClearGlobalData()
        {
            globalBundle.Clear();
        }

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
