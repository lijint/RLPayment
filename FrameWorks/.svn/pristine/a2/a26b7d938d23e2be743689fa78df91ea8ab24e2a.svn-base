using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using log4net;
using System.Windows.Forms;

namespace Landi.FrameWorks
{
    public static class Log
    {
        public enum LogOption
        {
            All,
            Application,
            Package,
        }

        private static readonly ILog frameLog = LogManager.GetLogger("FrameLog");

        public static void Debug(object message)
        {
            frameLog.Debug(message);
        }

        public static void Debug(object message, Exception exception)
        {
            frameLog.Debug(message, exception);
        }

        public static void Info(object message)
        {
            frameLog.Info(message);
        }

        public static void Info(object message, Exception exception)
        {
            frameLog.Info(message, exception);
        }

        public static void Warn(object message)
        {
            frameLog.Warn(message);
        }

        public static void Warn(object message, Exception exception)
        {
            frameLog.Warn(message, exception);
        }

        public static void Error(object message)
        {
            frameLog.Error(message);
        }

        public static void Error(object message, Exception exception)
        {
            frameLog.Error(message, exception);
        }

        public static void Fatal(object message)
        {
            frameLog.Fatal(message);
        }

        public static void Fatal(object message, Exception exception)
        {
            frameLog.Fatal(message, exception);
        }

        public static bool IsDebugEnabled
        {
            get { return frameLog.IsDebugEnabled; }
        }

        public static bool IsInfoEnabled
        {
            get { return frameLog.IsInfoEnabled; }
        }

        public static bool IsWarnEnabled
        {
            get { return frameLog.IsWarnEnabled; }
        }

        public static bool IsErrorEnabled
        {
            get { return frameLog.IsErrorEnabled; }
        }

        public static bool IsFatalEnabled
        {
            get { return frameLog.IsFatalEnabled; }
        }

        public static void DelLogByMonth(int monthsBefore,LogOption option)
        {
            if (monthsBefore <= 0)
                return;
            string from = DateTime.Now.AddMonths(-monthsBefore).ToString("yyyyMMdd");
            string to = DateTime.Now.ToString("yyyyMMdd");
            string dir = Application.StartupPath + "\\Log\\";
            string dic = null;
            foreach (string d in Directory.GetDirectories(dir))
            {
                dic = Path.GetFileName(d);
                if (int.Parse(dic) >= int.Parse(from) && int.Parse(dic) < int.Parse(to))
                {
                    if (option == LogOption.All)
                        Directory.Delete(d, true);
                    else if (option == LogOption.Application)
                        File.Delete(Path.Combine(d, "App.log"));
                    else
                        File.Delete(Path.Combine(d, "Package.log"));
                }
            }
        }

        public static void DelLogByDay(int daysBefore, LogOption option)
        {
            if (daysBefore <= 0)
                return;
            string from = DateTime.Now.AddDays(-daysBefore).ToString("yyyyMMdd");
            string to = DateTime.Now.ToString("yyyyMMdd");
            string dir = Application.StartupPath + "\\Log\\";
            string dic = null;
            foreach (string d in Directory.GetDirectories(dir))
            {
                dic = Path.GetFileName(d);
                if (int.Parse(dic) >= int.Parse(from) && int.Parse(dic) < int.Parse(to))
                {
                    if (option == LogOption.All)
                        Directory.Delete(d, true);
                    else if (option == LogOption.Application)
                        File.Delete(Path.Combine(d, "App.log"));
                    else
                        File.Delete(Path.Combine(d, "Package.log"));
                }
            }
        }
    }
}
