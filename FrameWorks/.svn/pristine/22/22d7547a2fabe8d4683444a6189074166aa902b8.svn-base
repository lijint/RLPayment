using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks.HardWare
{
    /// <summary>
    /// 历史遗留问题，目的是为了不想改动设备类源码中的日志记录代码，目前统一用Log类来记录日志
    /// </summary>
    public static class AppLog
    {
        public enum LogMessageType
        {
            Debug = 0,
            Info = 1,
            Warn = 2,
            Error = 3,
            Fatal = 4,
        }

        public static void Write(string message, AppLog.LogMessageType messageType)
        {
            switch (messageType)
            {
                case LogMessageType.Debug:
                    Log.Debug(message);
                    break;
                case LogMessageType.Info:
                    Log.Info(message);
                    break;
                case LogMessageType.Warn:
                    Log.Warn(message);
                    break;
                case LogMessageType.Error:
                    Log.Error(message);
                    break;
                case LogMessageType.Fatal:
                    Log.Fatal(message);
                    break;
            }
        }
        public static void Write(string message, AppLog.LogMessageType messageType, Exception ex)
        {
            switch (messageType)
            {
                case LogMessageType.Debug:
                    Log.Debug(message, ex);
                    break;
                case LogMessageType.Info:
                    Log.Info(message, ex);
                    break;
                case LogMessageType.Warn:
                    Log.Warn(message, ex);
                    break;
                case LogMessageType.Error:
                    Log.Error(message, ex);
                    break;
                case LogMessageType.Fatal:
                    Log.Fatal(message, ex);
                    break;
            }
        }

        public static void Write(string message, AppLog.LogMessageType messageType, Type type)
        {
            switch (messageType)
            {
                case LogMessageType.Debug:
                    Log.Debug(type.FullName + ":" + message);
                    break;
                case LogMessageType.Info:
                    Log.Info(type.FullName + ":" + message);
                    break;
                case LogMessageType.Warn:
                    Log.Warn(type.FullName + ":" + message);
                    break;
                case LogMessageType.Error:
                    Log.Error(type.FullName + ":" + message);
                    break;
                case LogMessageType.Fatal:
                    Log.Fatal(type.FullName + ":" + message);
                    break;
            }
        }

        public static void Write(string message, AppLog.LogMessageType messageType, Exception ex, Type type)
        {
            switch (messageType)
            {
                case LogMessageType.Debug:
                    Log.Debug(type.FullName + ":" + message,ex);
                    break;
                case LogMessageType.Info:
                    Log.Info(type.FullName + ":" + message,ex);
                    break;
                case LogMessageType.Warn:
                    Log.Warn(type.FullName + ":" + message,ex);
                    break;
                case LogMessageType.Error:
                    Log.Error(type.FullName + ":" + message,ex);
                    break;
                case LogMessageType.Fatal:
                    Log.Fatal(type.FullName + ":" + message,ex);
                    break;
            }
        }
    }
}
