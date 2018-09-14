using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Landi.FrameWorks;
using log4net;

namespace Landi.FrameWorks.Iso8583
{
    public static class CLog
    {
        public enum LogType
        {
            Send,
            Recv,
        }

        #region log4net
        private static readonly ILog packageLog = LogManager.GetLogger("PackageLog");

        public static void Debug(object message)
        {
            packageLog.Debug(message);
        }

        public static void Debug(object message, Exception exception)
        {
            packageLog.Debug(message, exception);
        }

        public static void Info(object message)
        {
            packageLog.Info(message);
        }

        public static void Info(object message, Exception exception)
        {
            packageLog.Info(message, exception);
        }

        public static void Warn(object message)
        {
            packageLog.Warn(message);
        }

        public static void Warn(object message, Exception exception)
        {
            packageLog.Warn(message, exception);
        }

        public static void Error(object message)
        {
            packageLog.Error(message);
        }

        public static void Error(object message, Exception exception)
        {
            packageLog.Error(message, exception);
        }

        public static void Fatal(object message)
        {
            packageLog.Fatal(message);
        }

        public static void Fatal(object message, Exception exception)
        {
            packageLog.Fatal(message, exception);
        }
        #endregion

        public static void Write(string msg)
        {
            try
            {
                string logfile = Application.StartupPath + "\\log\\" + DateTime.Now.ToString("yyyyMMdd");
                
                if (!Directory.Exists(logfile))
                {
                    Directory.CreateDirectory(logfile);
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                //sb.AppendLine();
                sb.Append(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + " ");
                //sb.AppendLine();
                sb.Append(msg);

                logfile = Path.Combine(logfile,  "package.log");
                
                StreamWriter sw = new StreamWriter(logfile, true);
                sw.WriteLine(sb.ToString());
                sw.Close();
            }
            catch(Exception e)
            {
                Log.Error("[CLog]Error!" + e.ToString());
            }
        }

        public static string GetLog(byte[] data, Iso8583Package pac, PackageBase pBase, LogType type)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{Message:" + pBase.GetType().Name + ",Type:" + type.ToString() + ",Length:" + data.Length.ToString() + "}");
            if (packageLog.IsDebugEnabled)
                sb.AppendLine(GetFormattedString(data, 16));
            sb.Append(pac.GetLogText());
            return sb.ToString();
        }

        public static string GetXMLLog(string data, PackageBase pBase, LogType type)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{Message:" + pBase.GetType().Name + ",Type:" + type.ToString() + ",Length:" + data.Length.ToString() + "}");
            sb.Append(data);
            return sb.ToString();
        }

        public static void LogPackage(byte[] data, Iso8583Package pac, LogType type)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{Length:" + data.Length.ToString() + ",Type:" + type.ToString() + "}\n");
            sb.AppendLine(GetFormattedString(data, 16));
            sb.AppendLine();
            Write(sb.ToString());
        }

        public static void Write(byte[] msg)
        {
            string tmp = Utility.bcd2str(msg, msg.Length);
            Write(tmp);
        }

        public static void Write(byte[] msg, int bytesPerLine)
        {
            int lineLength = 3 * bytesPerLine;
            string tmp = bcd2strAddSpace(msg, 0,msg.Length);
            string newstr = "";
            for (; ; )
            {
                if (tmp.Length <= lineLength)
                {
                    newstr += tmp;
                    break;
                }
                newstr += tmp.Substring(0, lineLength) + "\r\n";
                tmp = tmp.Substring(lineLength);
            }
            Write(newstr);
        }

        public static string GetFormattedString(byte[] msg, int bytesPerLine)
        {
            StringBuilder newstr = new StringBuilder();
            int pos = 0, perLine = 0, padLength = 3 * bytesPerLine;
            for (; pos < msg.Length; newstr.Append("\r\n"))
            {
                perLine = bytesPerLine;
                if (pos + bytesPerLine > msg.Length)
                    perLine = msg.Length - pos;
                newstr.Append(bcd2strAddSpace(msg, pos, perLine).PadRight(padLength, ' ') + " : ");
                newstr.Append(Encoding.GetEncoding("gb2312").GetString(msg, pos, perLine));
                //for (int i = 0; i < perLine; i++)
                //{
                //    byte tmp = msg[pos + i];
                //    if (char.IsLetterOrDigit((char)tmp))
                //        newstr.Append((char)tmp);
                //    else
                //        newstr.Append('.');
                //}
                pos += perLine;
            }
            return newstr.ToString();
        }

        /// <summary>
        /// ½«bcdÂë×ª»»³É×Ö·û´®
        /// </summary>
        /// <param name="bcd_buf"></param>
        /// <param name="conv_len"></param>
        /// <returns></returns>
        public static string bcd2strAddSpace(byte[] bcd_buf, int pos, int conv_len)
        {
            int i = 0, j = 0;
            byte tmp = 0x00;
            byte[] ret = new byte[conv_len * 3];
            for (i = pos, j = 0; i < pos + conv_len; i++)
            {
                tmp = (byte)(bcd_buf[i] >> 4);
                tmp += (byte)((tmp > 9) ? ('A' - 10) : '0');
                ret[j++] = tmp;
                tmp = (byte)(bcd_buf[i] & 0x0f);
                tmp += (byte)((tmp > 9) ? ('A' - 10) : '0');
                ret[j++] = tmp;

                ret[j++] = 0x20;
            }
            return Encoding.Default.GetString(ret).TrimEnd('\0');
        }
    }
}
