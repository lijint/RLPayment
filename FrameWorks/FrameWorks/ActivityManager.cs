using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Landi.Tools;

namespace Landi.FrameWorks
{
    public class ActivityManager
    {
        private static readonly object mLock = new object();
        private static ActivityManager Instance;

        private WebBrowser mWebBrowser;
        private string mConfigFileName;
        private ActivityManagerHandler mHandler;
        private string mStartupPath;

        private HandlerThread mManagerThread;
        private HandlerThread mWorkThread;

        internal static AppStatus SystemStatus = AppStatus.Initialzing;

        /// <summary>
        /// 界面秒针
        /// </summary>
        private TimerWrapper mPageTimer;
        /// <summary>
        /// 监控秒针
        /// </summary>
        private TimerWrapper mMonitorTimer;
        /// <summary>
        /// 当前界面
        /// </summary>
        private Activity currentActivity;
        /// <summary>
        /// 上一个界面
        /// </summary>
        private Activity previousActivity;
        
        private Dictionary<string, Activity> mActivityNameMap;
        private Dictionary<string, Activity> mActivityClassMap;

        internal bool ResetTimerOnMouseDown = false;

        private ApplicationInfo mApplicationInfo;
        private string mTimerConfigString;

        public static event MethodInvoker BeforeStart;

        internal void SetTimerConfig(TimerConfig config)
        {
            if (config != null)
                mTimerConfigString = config.ToString();
        }

        private static void onBeforeStart()
        {
            if (BeforeStart != null)
            {
                try
                {
                    BeforeStart();
                }
                catch (System.Exception ex)
                {
                    Log.Error(Instance, ex);
                }
                finally
                {
                    Application.DoEvents();
                }
            }
        }

        /// <summary>
        /// 系统状态
        /// </summary>
        public static AppStatus AppSystemStatus
        {
            get { return SystemStatus; }
        }

        public static event MethodInvoker BeforeQuit;

        private static void onBeforeQuit()
        {
            if (BeforeQuit != null)
            {
                try
                {
                    BeforeQuit();
                }
                catch (System.Exception ex)
                {
                    Log.Error(Instance, ex);
                }
                finally
                {
                    Application.DoEvents();
                }
            }
        }

        private ActivityManager(WebBrowser wb, string configFileName)
        {
            this.mWebBrowser = wb;
            this.mStartupPath = Application.StartupPath;
            this.mConfigFileName = Path.Combine(this.mStartupPath, configFileName);
            mActivityNameMap = new Dictionary<string, Activity>();
            mActivityClassMap = new Dictionary<string, Activity>();
            mPageTimer = new TimerWrapper(0, 1, pageTimer_TimeTick, pageTimer_TimeOut);
            mMonitorTimer = new TimerWrapper(1, monitorTimer_TimeTick);
            mTimerConfigString = (TimerConfig.Default()).ToString();
        }

        private ActivityManager(WebBrowser wb)
            : this(wb, "Manifest.xml")
        {
        }

        private void Start()
        {
            Log.Info("=======================System Start=========================");
            //GlobalAppData.ReadGlobalAppData();//读取应用数据
            if (!parseApplicationInfo())
            {
                Quit();
                return;
            }
            if (!mApplicationInfo.ShowCursor)
                Cursor.Hide();
            (mWebBrowser.Parent as Form).FormBorderStyle = FormBorderStyle.None;
            (mWebBrowser.Parent as Form).StartPosition = FormStartPosition.Manual;
            (mWebBrowser.Parent as Form).Location = new System.Drawing.Point(mApplicationInfo.Left, mApplicationInfo.Top);
            (mWebBrowser.Parent as Form).Size = new System.Drawing.Size(mApplicationInfo.Width, mApplicationInfo.Height);
            (mWebBrowser.Parent as Form).TopMost = mApplicationInfo.TopMost;
            (mWebBrowser.Parent as Form).BringToFront();

            mWebBrowser.AllowWebBrowserDrop = false;
            mWebBrowser.WebBrowserShortcutsEnabled = false;
            mWebBrowser.IsWebBrowserContextMenuEnabled = false;
            mWebBrowser.ScriptErrorsSuppressed = true;
            mWebBrowser.AllowNavigation = true;
            mWebBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(mWebBrowser_DocumentCompleted);

            Thread.CurrentThread.Name = "UIThread";

            mManagerThread = new HandlerThread("ManagerThread");
            mManagerThread.Run();
            mHandler = new ActivityManagerHandler(mManagerThread.GetLooper());
            mHandler.Proxy = this;

            mWorkThread = new HandlerThread("WorkThread");
            mWorkThread.Run();
            Activity.aH = new ActivityHandler(mWorkThread.GetLooper());
            Activity.aH.Proxy = this;
            Activity.mQueue = mWorkThread.GetLooper().Queue;
            Activity.Context = this;

            AdManager.EnterAd += new MethodInvoker(AdManager_EnterAd);
            AdManager.LeaveAd += new MethodInvoker(AdManager_LeaveAd);
            onBeforeStart();
            mMonitorTimer.Start();
            SendMessage(ActivityManagerHandler.LAUNCH_ACTIVITY, new Intent(mApplicationInfo.FirstActivity));
        }

        private void AdManager_LeaveAd()
        {
            SystemStatus = AppStatus.Idle;
            stopPageTimer();
            startPageTimer();
            mWebBrowser.Invoke((MethodInvoker)delegate()
            {
                Activity.RecoverFormAd(currentActivity);
            });
        }

        private void AdManager_EnterAd()
        {
            SystemStatus = AppStatus.OnAd;
            Log.Info("Enter { 广告 }");
            stopPageTimer();
            mWebBrowser.Invoke((MethodInvoker)delegate()
            {
                Activity.SwitchToAd(currentActivity);
            });
        }

        private bool mExiting;
        internal void Quit()
        {
            if (mWebBrowser.InvokeRequired)
            {
                mWebBrowser.Invoke(new MethodInvoker(Quit));
            }
            else
            {
                if ((currentActivity == null || currentActivity.CanQuit()))
                {
                    mExiting = true;
                    stopPageTimer();
                    onBeforeQuit();
                    Log.Info("=======================System Quit==========================");
                    Application.Exit();
                }
            }
        }

        private void mWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            onSetContentViewComplete();
        }

        /// <summary>
        /// 待处理界面数据
        /// </summary>
        private Intent mPendingIntent;

        private Activity getActivity(string activityName, string activityClass)
        {
            if (activityName == null)
                activityName = "";
            if (activityClass == null)
                activityClass = "";
            if (activityName == "" && activityClass == "")
                return null;
            if (!mActivityNameMap.ContainsKey(activityName))
            {
                if (!mActivityClassMap.ContainsKey(activityClass))
                    return null;
                else
                    return mActivityClassMap[activityClass];
            }
            else
                return mActivityNameMap[activityName];
        }

        /// <summary>
        /// 修改，进入主界面之前判断是否有卡存在，如果存在则直接跳到退卡界面
        /// </summary>
        /// <param name="intent"></param>
        /// <returns></returns>
        internal bool LeaveCurrentActivity(Intent intent)
        {
            if (string.IsNullOrEmpty(intent.ActivityName) && string.IsNullOrEmpty(intent.ActivityClassName))
            {
                startPageTimer();
                return false;
            }
            if (currentActivity != null)
            {
                if (intent.ActivityName == currentActivity.MyIntent.ActivityName || intent.ActivityClassName == currentActivity.MyIntent.ActivityClassName)
                {
                    startPageTimer();
                    return false;
                }
                stopPageTimer();

                previousActivity = currentActivity;
            }

            Activity instance = null;
            if ((instance = getActivity(intent.ActivityName, intent.ActivityClassName)) != null)
            {
                mPendingIntent = new Intent(instance.MyIntent);
                mPendingIntent.CopyExtraFrom(intent);
            }
            else
            {
                //初始化界面
                if (!mApplicationInfo.ContainsActivityInfo(intent.ActivityName, intent.ActivityClassName))
                {
                    Log.Error("配置项<" + intent.ActivityName + ">不存在或对应处理类未实现");
                    startPageTimer();
                    return false;
                }
                mPendingIntent = new Intent();
                mPendingIntent.CopyExtraFrom(intent);
                mPendingIntent.ActivityName = intent.ActivityName;
                mPendingIntent.ActivityClassName = intent.ActivityClassName;
                ActivityInfo pageInfo = mApplicationInfo.GetActivityInfo(mPendingIntent.ActivityName, mPendingIntent.ActivityClassName);
                mPendingIntent.ActivityName = pageInfo.NameCn;
                mPendingIntent.ActivityClassName = pageInfo.ClassName;
                mPendingIntent.PageFileName = pageInfo.FilePath;
                switch (Path.GetExtension(mPendingIntent.PageFileName))
                {
                    case ".html":
                    case ".htm":
                        mPendingIntent.AlonePage = true;
                        break;
                    case ".txt":
                        mPendingIntent.AlonePage = false;
                        break;
                    default:
                        Log.Error(mPendingIntent.PageFileName + ":非法的网页文件格式");
                        startPageTimer();
                        return false;
                }
                mPendingIntent.TimeOut = Math.Abs(pageInfo.TimeOut);
                if (pageInfo.TimeOut > 0)
                    mPendingIntent.ShowTimerCount = true;

                if (!string.IsNullOrEmpty(pageInfo.Sound))
                {
                    mPendingIntent.SoundPath = Path.Combine(mApplicationInfo.SoundPath, pageInfo.Sound);
                    if (!File.Exists(mPendingIntent.SoundPath))
                        mPendingIntent.SoundPath = null;
                }
                mPendingIntent.NextActivity = pageInfo.Next;

                string fullName = Path.Combine(Intent.PageFilePath, mPendingIntent.PageFileName);
                if (!File.Exists(fullName))
                {
                    Log.Error("页面文件" + fullName + "不存在");
                    startPageTimer();
                    return false;
                }
            }
            Leave();

            //添加判断是否进入主界面和是否有卡
            if ((intent.ActivityName == mApplicationInfo.MainActivity ||
                intent.ActivityClassName ==
                mApplicationInfo.GetActivityInfo(mApplicationInfo.MainActivity, "").ClassName) &&
                CommonData.BIsCardIn)
            {
                CommonData.BIsCardIn = false;
                GotoEjectCard();
                return false;
            }

            return true;
        }

        private void stopPageTimer()
        {
            mPageTimer.Stop();
        }

        private void startPageTimer()
        {
            if (currentActivity != null)
            {
                mPageTimer.OutTime = currentActivity.MyIntent.TimeOut;
                mPageTimer.Start();
            }
        }

        internal void SetContentView()
        {
            string realFileName = Path.Combine(Intent.PageFilePath, mPendingIntent.PageFileName);
            if (mPendingIntent.AlonePage)
            {
                mWebBrowser.Invoke((MethodInvoker)delegate()
                {
                    mWebBrowser.Navigate(realFileName);
                    Application.DoEvents();
                });
            }
            else
            {

                mWebBrowser.Invoke((MethodInvoker)delegate()
                {
                    if (mWebBrowser.Document.GetElementById("Content") == null)//从整页加载模式换回模版模式时，调用模版。
                    {
                        string frameFile = Path.Combine(Intent.PageFilePath, "Frame.html");
                        mWebBrowser.Navigate(frameFile);
                        Application.DoEvents();
                    }
                    else
                    {
                        //mWebBrowser.Document.GetElementById("Content").InnerHtml = contentHtml; //这里id名指定为"Content"
                        Application.DoEvents();
                        onSetContentViewComplete();
                    }
                });
            }
        }

        private ITimeTick TimeTickHandler;

        private void TimeTick(int count)
        {
            if (TimeTickHandler != null)
            {
                try
                {
                    TimeTickHandler.OnTimeTick(count);
                }
                catch (System.Exception ex)
                {
                    Log.Error(currentActivity, ex);
                }
            }
        }

        private void onSetContentViewComplete()
        {
            if (mPendingIntent.AlonePage)//如果是单独画面
            {
                mWebBrowser.Document.Body.KeyDown += new HtmlElementEventHandler(Body_KeyDown);
                mWebBrowser.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown);
            }
            else
            {
                mWebBrowser.Document.Body.KeyDown -= new HtmlElementEventHandler(Body_KeyDown);
                mWebBrowser.Document.Body.MouseDown -= new HtmlElementEventHandler(Body_MouseDown);
                mWebBrowser.Document.Body.KeyDown += new HtmlElementEventHandler(Body_KeyDown);
                mWebBrowser.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown);
                string realFileName = Path.Combine(Intent.PageFilePath, mPendingIntent.PageFileName);
                using (StreamReader fileReader = new StreamReader(realFileName, Encoding.GetEncoding("utf-8")))
                {
                    string contentHtml = fileReader.ReadToEnd();
                    mWebBrowser.Document.GetElementById("Content").InnerHtml = contentHtml;
                }
            }
            if (mPendingIntent.ShowTimerCount && mPendingIntent.TimeOut > 0 && mWebBrowser.Document.GetElementById(TimerConfig.Id) == null)
            {
                if (mWebBrowser.Document.GetElementById("Content") == null)
                {
                    AppLog.Write("Id Content 不存在，倒计时忽略显示！", AppLog.LogMessageType.Warn);
                }
                else
                {
                    mWebBrowser.Document.GetElementById("Content").InnerHtml = mTimerConfigString + mWebBrowser.Document.GetElementById("Content").InnerHtml;
                }
                Application.DoEvents();
            }
            SendMessage(ActivityManagerHandler.SET_VIEW_COMPLETE);
        }

        private void switchTimeTick()
        {
            if (currentActivity != null && currentActivity is ITimeTick)
                TimeTickHandler = currentActivity as ITimeTick;
            else
                TimeTickHandler = null;
        }

        /// <summary>
        /// 初始化交易数据和公共数据
        /// </summary>
        private void updateBusinessRelative()
        {
            if (currentActivity.MyIntent.ActivityName == mApplicationInfo.MainActivity || 
                currentActivity.MyIntent.ActivityName == mApplicationInfo.FirstActivity || 
                currentActivity.MyIntent.ActivityName == mApplicationInfo.ManagementActivity)
            {
                Activity.Stratagy = null;//lanpp
                Activity.businessBundle.Clear();
                CommonData.Clear();
            }
        }

        private void updateSystemStatus()
        {
            switch (SystemStatus)
            {
                case AppStatus.Initialzing:
                case AppStatus.OnStopServer:
                    if (currentActivity.MyIntent.ActivityName == mApplicationInfo.MainActivity)
                        SystemStatus = AppStatus.Idle;
                    else if (currentActivity.MyIntent.ActivityName == mApplicationInfo.ManagementActivity)
                        SystemStatus = AppStatus.Managed;
                    break;
                case AppStatus.Idle:
                    if (currentActivity.MyIntent.ActivityName == mApplicationInfo.MainActivity)
                        SystemStatus = AppStatus.Idle;
                    else if (currentActivity.MyIntent.ActivityName == mApplicationInfo.FirstActivity)
                        SystemStatus = AppStatus.Initialzing;
                    else if (currentActivity.MyIntent.ActivityName == mApplicationInfo.ManagementActivity)
                        SystemStatus = AppStatus.Managed;
                    else
                        SystemStatus = AppStatus.OnBusiness;
                    break;
                case AppStatus.OnBusiness:
                    if (currentActivity.MyIntent.ActivityName == mApplicationInfo.MainActivity)
                        SystemStatus = AppStatus.Idle;
                    break;
                case AppStatus.Managed:
                    if (currentActivity.MyIntent.ActivityName == mApplicationInfo.MainActivity)
                        SystemStatus = AppStatus.Idle;
                    else if (currentActivity.MyIntent.ActivityName == mApplicationInfo.FirstActivity)
                        SystemStatus = AppStatus.Initialzing;
                    break;
                case AppStatus.OnAd:
                    break;
                default:
                    if (currentActivity.MyIntent.ActivityName == mApplicationInfo.StopServerActivity)
                        SystemStatus = AppStatus.OnStopServer;
                    break;
            }
        }

        private bool parseApplicationInfo()
        {
            if (mApplicationInfo != null)
                return true;
            mApplicationInfo = ApplicationInfo.DoParse(mConfigFileName);
            if (mApplicationInfo == null)
                Log.Fatal("Parse ApplicationInfo Failed!");
            return mApplicationInfo == null ? false : true;
        }

        public static void Start(WebBrowser wb, string configFileName)
        {
            if (Instance == null)
            {
                lock (mLock)
                {
                    if (Instance == null)
                    {
                        ActivityManager at = new ActivityManager(wb, configFileName);
                        Instance = at;
                        at.Start();
                    }
                }
            }
            else
            {
                Log.Error("ActivityManager has been started");
            }
        }

        public static void Start(WebBrowser wb)
        {
            if (Instance == null)
            {
                lock (mLock)
                {
                    if (Instance == null)
                    {
                        ActivityManager at = new ActivityManager(wb);
                        Instance = at;
                        at.Start();
                    }
                }
            }
            else
            {
                Log.Error("ActivityManager has been started");
            }
        }

        private void pageTimer_TimeOut(object sender, TimerWrapperEventArgs e)
        {
            mWebBrowser.BeginInvoke((MethodInvoker)delegate()
            {
                HtmlElement tmp = mWebBrowser.Document.GetElementById(TimerConfig.Id);
                if (tmp != null && currentActivity.MyIntent.ShowTimerCount)
                    tmp.InnerText = "";
                Application.DoEvents();
                TimeOut();
            });
        }

        private void pageTimer_TimeTick(object sender, TimerWrapperEventArgs e)
        {
            mWebBrowser.BeginInvoke((MethodInvoker)delegate()
            {
                HtmlElement tmp = mWebBrowser.Document.GetElementById(TimerConfig.Id);
                int remain = e.OutCount - e.CurrentCount;
                if (tmp != null && currentActivity.MyIntent.ShowTimerCount && (SystemStatus & AppStatus.OnAd) != AppStatus.OnAd)
                {
                    tmp.InnerText = remain.ToString();
                }
                Application.DoEvents();
                TimeTick(e.CurrentCount);
            });
        }

        private void monitorTimer_TimeTick(object sender, TimerWrapperEventArgs e)
        {
            int nCurrentCount = mMonitorTimer.CurrentCount;
            mMonitorTimer.Stop();
            try
            {
                #region 清空管理界面计数器
                if (nCurrentCount % mClearClickNumInterval == 0)
                    clearClickNum();
                #endregion

                long calcInterval = GlobalAppData.GetInstance().MonitorTimeInterval;
                if (nCurrentCount % calcInterval == 0)
                {
                    //SystemStatus == AppStatus.Idle || 
                    if (SystemStatus == AppStatus.OnAd ||
                     SystemStatus == AppStatus.OnStopServer)
                    {
                        #region 自动开关机时间检测
                        if (GlobalAppData.GetInstance().CloseAcmSwitch)
                        {
                            Log.Info("自动开关机时间检测...");
                            DateTime shutTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " +
                                GlobalAppData.GetInstance().CloseAcmTime);
                            DateTime shutTimeEnd = shutTime.AddMinutes(15); //15分钟内关机
                            if (DateTime.Now >= shutTime && DateTime.Now <= shutTimeEnd)
                            {
                                switch (GlobalAppData.GetInstance().CloseAcmType)
                                {
                                    case GlobalAppData.ECloseAcmType.Close:
                                        WindowsController.ExitWindows(RestartOptions.PowerOff, true);
                                        System.Threading.Thread.Sleep(5000);
                                        break;
                                    case GlobalAppData.ECloseAcmType.Restart:
                                        WindowsController.ExitWindows(RestartOptions.Reboot, true);
                                        System.Threading.Thread.Sleep(5000);
                                        break;
                                }
                                return;
                            }
                        }
                        #endregion

                        #region 发送tms状态监控报文
                        //if (GlobalAppData.GetInstance().TMSSwitch)
                        //{
                        //    #region 设备自检，判断当前设备是否正常，暂停服务或者恢复服务
                        //    Log.Info("设备自检开始...");

                        //    bool isHdError = HardwareManager.CheckAll();
                        //    if (!isHdError && SystemStatus != AppStatus.OnStopServer)
                        //    {
                        //        bool isNetError = !HardwareManager.CheckOne("GPRS");
                        //        HideAd();
                        //        string errMsg = isNetError ? "网络故障" : "设备故障";
                        //        Intent stop = new Intent(mApplicationInfo.StopServerActivity);
                        //        stop.PutExtra("Message", errMsg);
                        //        SendMessage(ActivityManagerHandler.LAUNCH_ACTIVITY, stop);
                        //    }
                        //    #endregion

                        //    Log.Info("发送TMS终端监控报文...");
                        //}
                        #endregion
                    }

                    nCurrentCount = 0;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(this, ex);
            }
            finally
            {
                mMonitorTimer.CurrentCount = nCurrentCount;
                mMonitorTimer.Start();
            }
        }

        //网页鼠标点击事件
        private void Body_MouseDown(object sender, HtmlElementEventArgs e)
        {
            if (ResetTimerOnMouseDown)
            {
                mPageTimer.CurrentCount = 0;
                //mPageTimer.Stop();
                //mPageTimer.Start();
            }
        }

        //网页按键按下事件
        private void Body_KeyDown(object sender, HtmlElementEventArgs e)
        {
            if (ResetTimerOnMouseDown)
            {
                mPageTimer.CurrentCount = 0;
                //mPageTimer.Stop();
                //mPageTimer.Start();
            }

            Keys KeyAscii = (Keys)e.KeyPressedCode;
            if (KeyAscii == Keys.Q) //Q键
                Quit();
            if (!mExiting)
                Activity.KeyDown(currentActivity, KeyAscii); 
        }

        #region 发送消息
        internal void SendMessage(int what)
        {
            SendMessage(what, null, 0, 0);
        }

        internal void SendMessage(int what, object obj)
        {
            SendMessage(what, obj, 0, 0);
        }

        internal void SendMessage(int what, object obj, int arg1)
        {
            SendMessage(what, obj, arg1, 0);
        }

        internal void SendMessage(int what, object obj, int arg1, int arg2)
        {
            lock (this)
            {
                Message msg = Message.Obtain(what, arg1, arg2, obj);
                mHandler.SendMessage(msg);
            }
        }
        #endregion

        #region 进入管理界面
        private static int mClickNum = 0;
        /// <summary>
        /// 5S内清空
        /// </summary>
        private const int mClearClickNumInterval = 8;
        private static object mObjClock = new object();
        /// <summary>
        /// 进入管理界面
        /// </summary>
        internal void ManagerEntry()
        {
            lock (mObjClock)
            {
                addClickNum();
                if ((SystemStatus == AppStatus.Idle ||
                    SystemStatus == AppStatus.OnStopServer) &&
                    mClickNum >= 3)
                {
                    HideAd();
                    GotoManage();
                    mClickNum = 0;
                }
            }
        }

        private void clearClickNum()
        {
            mClickNum = 0;
        }

        private void addClickNum()
        {
            mClickNum++;
        }

        internal void GotoManage()
        {
            if (!string.IsNullOrEmpty(mApplicationInfo.ManagementActivity))
                SendMessage(ActivityManagerHandler.LAUNCH_ACTIVITY, new Intent(mApplicationInfo.ManagementActivity));
        }
        #endregion

        internal void Enter()
        {
            if (!mayCreateActivity())
                return;
            Log.Info("Enter " + currentActivity);
            updateSystemStatus();
            updateBusinessRelative();
            switchTimeTick();
            clearClickNum();
            startPageTimer();
            mWebBrowser.Invoke((MethodInvoker)delegate()
            {
                mWebBrowser.Focus();
                mWebBrowser.Focus();
                mWebBrowser.Document.Focus();
                Activity.Enter(currentActivity);
            });
        }

        private bool mayCreateActivity()
        {
            Activity instance = null;
            bool needCreate = false;
            lock (this)
            {
                if ((instance = getActivity(mPendingIntent.ActivityName, mPendingIntent.ActivityClassName)) != null)
                {
                    instance.MyIntent = mPendingIntent;
                }
                else
                {
                    try
                    {
                        mWebBrowser.Invoke((MethodInvoker)delegate()
                        {
                            instance = (Activity)Assembly.GetEntryAssembly().CreateInstance(mPendingIntent.ActivityClassName, false);
                        });
                        instance.MyIntent = mPendingIntent;
                        needCreate = true;
                        mActivityNameMap.Add(mPendingIntent.ActivityName, instance);
                        mActivityClassMap.Add(mPendingIntent.ActivityClassName, instance);
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(this, ex);
                        stopPageTimer();
                        startPageTimer();
                        return false;
                    }
                }
            }

            if (needCreate)
            {
                mWebBrowser.Invoke((MethodInvoker)delegate()
                {
                    Activity.Creat(instance);
                });
            }
            currentActivity = instance;
            mPendingIntent = null;
            return true;
        }

        internal void Leave()
        {
            mWebBrowser.Invoke((MethodInvoker)delegate()
            {
                if (previousActivity != null)
                {
                    Activity.Leave(previousActivity);
                    if (!previousActivity.mKeepAlive)
                    {
                        Activity.Destroy(previousActivity);
                        mActivityNameMap.Remove(previousActivity.MyIntent.ActivityName);
                        mActivityClassMap.Remove(previousActivity.MyIntent.ActivityClassName);
                    }
                }
            });
            SendMessage(ActivityManagerHandler.LEAVE_ACTIVITY_COMPLETE);
        }

        internal void TimeOut()
        {
            Activity.TimeOut(currentActivity);
        }

        internal void GoBack()
        {
            SendMessage(ActivityManagerHandler.LAUNCH_ACTIVITY, new Intent(previousActivity.MyIntent.ActivityName));
        }

        internal void GotoMain()
        {
            SendMessage(ActivityManagerHandler.LAUNCH_ACTIVITY, new Intent(mApplicationInfo.MainActivity));
        }

        internal void GotoNext()
        {
            SendMessage(ActivityManagerHandler.LAUNCH_ACTIVITY, new Intent(currentActivity.MyIntent.NextActivity));
        }

        internal void GotoEjectCard()
        {
            if (!string.IsNullOrEmpty(mApplicationInfo.EjectCardActivity))
                SendMessage(ActivityManagerHandler.LAUNCH_ACTIVITY, new Intent(mApplicationInfo.EjectCardActivity));
        }

        internal void HandleCustomMessage(Message msg)
        {
            if (currentActivity is IHandleMessage)
                (currentActivity as IHandleMessage).HandleMessage(msg);
        }

        #region show or hide ad
        internal void ShowAd()
        {
            if (mWebBrowser.InvokeRequired)
            {
                mWebBrowser.Invoke(new MethodInvoker(ShowAd));
            }
            else
            {
                if (SystemStatus == AppStatus.Idle)
                {
                    AdManager.ShowAd();
                }
            }
        }

        internal void HideAd()
        {
            if (mWebBrowser.InvokeRequired)
            {
                mWebBrowser.Invoke(new MethodInvoker(HideAd));
            }
            else
            {
                if (SystemStatus == AppStatus.OnAd)
                {
                    AdManager.HideAd();
                }
            }
        }
        #endregion

        internal string Dump()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("StartupPath:" + mStartupPath);
            sb.AppendLine("SystemStatus:" + SystemStatus.ToString());
            sb.AppendLine("ResetTimerOnMouseDown:" + ResetTimerOnMouseDown);
            sb.AppendLine("TimerConfigString:" + mTimerConfigString);
            sb.AppendLine(mApplicationInfo.ToString());
            sb.AppendLine("CurrentActivity:" + currentActivity);
            sb.Append("PreviousActivity:" + previousActivity);
            Log.Debug(sb.ToString());
            return sb.ToString();
        }

        internal void SetPosition(int left, int top, int width, int height)
        {
            (mWebBrowser.Parent as Form).Left = left;
            (mWebBrowser.Parent as Form).Top = top;
            (mWebBrowser.Parent as Form).Width = width;
            (mWebBrowser.Parent as Form).Height = height;
            Application.DoEvents();
        }

        internal void SetTopMost(bool topmost)
        {
            (mWebBrowser.Parent as Form).TopMost = topmost;
            Application.DoEvents();
        }

        internal HtmlElement GetElementById(string id)
        {
            return DelegateHelper.WBGetElementById(mWebBrowser, id);
            //return mWebBrowser.Document.GetElementById(id);
        }

        private delegate void Invoker(Activity target, object reportedData);
        internal void Report(object reportedData)
        {
            mWebBrowser.Invoke(new Invoker(Activity.InvokeReport), new object[] { currentActivity, reportedData });
        }

        internal void ReportAsync(object reportedData)
        {
            mWebBrowser.BeginInvoke(new Invoker(Activity.InvokeReport), new object[] { currentActivity, reportedData });
        }

        internal object InvokeScript(string scriptName)
        {
            return mWebBrowser.Document.InvokeScript(scriptName);
        }

        internal object InvokeScript(string scriptName, object[] args)
        {
            return mWebBrowser.Document.InvokeScript(scriptName, args);
        }

        internal void DestroyActivity(string activityName, string activityClass)
        {
            lock (this)
            {
                Activity instance = null;
                if ((instance = getActivity(activityName, activityClass)) != null)
                {
                    if (currentActivity == instance)
                        currentActivity.mKeepAlive = false;
                    else
                    {
                        mWebBrowser.Invoke((MethodInvoker)delegate()
                        {
                            Activity.Destroy(instance);
                        });

                        mActivityNameMap.Remove(instance.MyIntent.ActivityName);
                        mActivityClassMap.Remove(instance.MyIntent.ActivityClassName);
                    }
                }
            }
        }

        internal void RestartTimer()
        {
            stopPageTimer();
            startPageTimer();
        }

        /// <summary>
        /// 获取页面的数据类
        /// </summary>
        /// <returns></returns>
        internal BaseEntity GetBusinessEntity()
        {
            return (BaseEntity)Activity.businessBundle.Get(Activity.ENTITYKEY);
        }
    }

    /// <summary>
    /// 系统状态
    /// </summary>
    public enum AppStatus
    {
        Initialzing = 1,//初始化中
        Idle = 2,//空闲中
        OnBusiness = 4,//进入业务
        OnCommunicating = 8,//正在进行报文通讯
        Managed = 16,//进入管理模式
        OnAd = 32,//正在显示广告
        OnStopServer = 64,//暂停服务
    }

    /// <summary>
    /// XML配置文件单项信息
    /// </summary>
    public class ActivityInfo
    {
        [XmlAttribute("NameCn")]
        public string NameCn;
        [XmlAttribute("FilePath")]
        public string FilePath;
        [XmlAttribute("ClassName")]
        public string ClassName;
        [XmlAttribute("TimeOut")]
        public int TimeOut;
        [XmlAttribute("Next")]
        public string Next;
        [XmlAttribute("Sound")]
        public string Sound;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            b.Append("ActivityInfo");

            b.Append("{ FilePath=");
            b.Append(FilePath);

            b.Append(" ClassName=");
            b.Append(ClassName);

            b.Append(" TimeOut=");
            b.Append(TimeOut);
            return b.ToString();

        }
    }

    public class ApplicationInfo
    {
        public bool TopMost;
        public int Left;
        public int Top;
        public int Width;
        public int Height;
        public bool ShowCursor;
        public string WebPath;
        public string FirstActivity;
        public string MainActivity;
        public string EjectCardActivity;
        public string StopServerActivity;
        public string ManagementActivity;
        public string SoundPath;
        public List<ActivityInfo> ActivityConfig;
        private Dictionary<string, ActivityInfo> mNameMap;
        private Dictionary<string, ActivityInfo> mClassMap;

        public ApplicationInfo()
        {
            ActivityConfig = new List<ActivityInfo>();
            mNameMap = new Dictionary<string, ActivityInfo>();
            mClassMap = new Dictionary<string, ActivityInfo>();
        }

        public bool ContainsActivityInfo(string activityNameCn, string activityClass)
        {
            if (activityNameCn == null)
                activityNameCn = "";
            if (activityClass == null)
                activityClass = "";
            return mNameMap.ContainsKey(activityNameCn) || mClassMap.ContainsKey(activityClass);
        }

        public ActivityInfo GetActivityInfo(string activityNameCn, string activityClass)
        {
            if (activityNameCn == null)
                activityNameCn = "";
            if (activityClass == null)
                activityClass = "";
            if (mNameMap.ContainsKey(activityNameCn))
                return mNameMap[activityNameCn];
            else if (mClassMap.ContainsKey(activityClass))
                return mClassMap[activityClass];
            else
                return null;
        }

        public static ApplicationInfo DoParse(string mConfigFileName)
        {
            if (!File.Exists(mConfigFileName))
            {
                Log.Error(mConfigFileName + "文件不存在");
                return null;
            }

            try
            {
                ApplicationInfo info = null;
                XmlSerializer serial = new XmlSerializer(typeof(ApplicationInfo));
                StreamReader reader = new StreamReader(mConfigFileName);

                info = serial.Deserialize(reader) as ApplicationInfo;
                if (info == null)
                    return null;
                for (int i = 0; i < info.ActivityConfig.Count; i++)
                {
                    if (info.mNameMap.ContainsKey(info.ActivityConfig[i].ClassName))
                    {
                        info.ActivityConfig.RemoveAt(i);
                        i--;
                        continue;
                    }
                    info.mNameMap.Add(info.ActivityConfig[i].ClassName, info.ActivityConfig[i]);
                }

                info.ActivityConfig.Clear();

                Type[] types = Assembly.GetEntryAssembly().GetTypes();

                for (int index = 0; index < types.Length; index++)
                {
                    if (info.mNameMap.ContainsKey(types[index].Name))
                    {
                        info.mNameMap[types[index].Name].ClassName = types[index].FullName;
                        info.ActivityConfig.Add(info.mNameMap[types[index].Name]);
                    }
                }

                info.mNameMap.Clear();
                for (int i = 0; i < info.ActivityConfig.Count; i++)
                {
                    info.mNameMap.Add(info.ActivityConfig[i].NameCn, info.ActivityConfig[i]);
                    info.mClassMap.Add(info.ActivityConfig[i].ClassName, info.ActivityConfig[i]);
                }
                if (string.IsNullOrEmpty(info.WebPath))
                    info.WebPath = "Web";
                info.WebPath = Path.Combine(Application.StartupPath, info.WebPath);
                Intent.PageFilePath = info.WebPath;

                if (string.IsNullOrEmpty(info.SoundPath))
                    info.SoundPath = "Sound";
                info.SoundPath = Path.Combine(Application.StartupPath, info.SoundPath);

                if (info.MainActivity == null)
                    info.MainActivity = "主界面";
                if (info.FirstActivity == null || !info.ContainsActivityInfo(info.FirstActivity, null))
                    return null;

                return info;
            }
            catch (System.Exception ex)
            {
                Log.Error(null, ex);
                return null;
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            b.AppendLine("WebPath:" + WebPath);
            b.AppendLine("FirstActivity:" + FirstActivity);
            b.AppendLine("MainActivity:" + MainActivity);
            b.AppendLine("ManagementActivity:" + ManagementActivity);
            b.Append("SoundPath:" + SoundPath);
            return b.ToString();
        }
    }

    public class TimerConfig
    {
        internal static readonly string Id = "System.TimeDiv";
        public int Top;
        public int Left;
        public int Width;
        public int Height;
        public string Color;
        public string Font_Family;
        public int Font_Size;

        public static TimerConfig Default()
        {
            TimerConfig config = new TimerConfig();
            config.Width = 50;
            config.Height = 30;
            config.Top = 80;
            config.Left = 930;
            config.Color = "white";
            config.Font_Family = "黑体";
            config.Font_Size = 25;
            return config;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<span id=");
            sb.Append(Id);
            sb.Append(" style=\"position: absolute; z-index:9999; width: ");
            sb.Append(Width);
            sb.Append("px; height: ");
            sb.Append(Height);
            sb.Append("px; top: ");
            sb.Append(Top);
            sb.Append("px; left: ");
            sb.Append(Left);
            sb.Append("px; font-family:");
            sb.Append(Font_Family);
            sb.Append(";color:");
            sb.Append(Color);
            sb.Append(";font-size:");
            sb.Append(Font_Size);
            sb.Append("px;text-align:right\"></span>");
            return sb.ToString();
        }
    }
}
