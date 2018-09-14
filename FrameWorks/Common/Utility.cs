using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace Landi.FrameWorks
{
    [StructLayout(LayoutKind.Sequential)]
    public class SystemTime
    {
        public ushort year;
        public ushort month;
        public ushort dayofweek;
        public ushort day;
        public ushort hour;
        public ushort minute;
        public ushort second;
        public ushort milliseconds;
    }

    public class Utility
    {
        /// <summary>
        /// 拨号
        /// </summary>
        public static void CreateConnection()
        {
            RasManager myRas = new RasManager();
            myRas.PhoneNumber = "*99**PPP*1#";
            myRas.UserName = "username";
            myRas.Password = "password";
            myRas.Connect();
        }

        /// <summary>
        /// 拨号多次
        /// </summary>
        /// <param name="retryTimes"></param>
        /// <returns></returns>
        public static bool CreateConnection(string dialPhoneNum, int retryTimes)
        {

            bool ret = false;
            if (retryTimes == 0)
            {
                retryTimes = 2; // 默认连接两次
            }

            try
            {
                Log.Info("开始拨号...");
                RasManager rsm = new RasManager();
                Application.DoEvents();

                if (rsm.isConnect()) //已经在线就不再拨号
                {
                    ret = true;
                    Log.Info("已经在线");
                }
                else
                {
                    rsm.UserName = "";
                    rsm.Password = "";
                    rsm.PhoneNumber = dialPhoneNum;

                    for (int i = 0; i < retryTimes; i++)
                    {
                        int rsmRet = rsm.Connect();
                        Log.Info("rsm Connect Return:" + rsmRet.ToString());
                        if (rsmRet == 0)
                        {
                            ret = true;
                            break;
                        }
                        if (rsm.isConnect())
                        {
                            ret = true;
                            break;
                        }
                    }

                    Log.Info("多次拨号完成 ret=" + ret.ToString());
                }
            }
            catch (Exception err)
            {
                Log.Error("CreateConnection Failed ****err.Message" + err.Message);
            }

            return ret;
        }

        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns>本机IP</returns>
        public static IPAddress[] GetHostIP()
        {
            IPHostEntry ipe = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> targets = new List<IPAddress>();
            for (int i = 0; i < ipe.AddressList.Length; i++)
            {
                IPAddress ipa = ipe.AddressList[i];
                if (ipa.ToString().Split('.').Length == 4)
                    targets.Add(ipa);
            }
            if (targets.Count == 0)
                return null;
            else
                return targets.ToArray();
        }

        /// <summary>
        /// 挂断拨号连接
        /// </summary>
        public static void HangUpAllConnection()
        {
            Process cmd = new Process();
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.FileName = "rasdial.exe";
            cmd.StartInfo.Arguments = @"/disconnect";
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();
            cmd.WaitForExit();
        }

        /// <summary>
        /// 中英文右对齐
        /// </summary>
        /// <param name="str"></param>
        /// <param name="totalByteCount"></param>
        /// <param name="chs"></param>
        /// <returns></returns>
        public static string padRightEx(string str, int totalByteCount, char chs)
        {
            Encoding coding = Encoding.GetEncoding("gb2312");
            int dcount = 0;
            foreach (char ch in str.ToCharArray())
            {
                if (coding.GetByteCount(ch.ToString()) == 2)
                    dcount++;
            }
            string w = str.PadLeft(totalByteCount - dcount, chs);
            return w;
        }

        /// <summary>
        /// 中英文左对齐
        /// </summary>
        /// <param name="str"></param>
        /// <param name="totalByteCount"></param>
        /// <param name="chs"></param>
        /// <returns></returns>
        public static string padLeftEx(string str, int totalByteCount, char chs)
        {
            Encoding coding = Encoding.GetEncoding("gb2312");
            int dcount = 0;
            foreach (char ch in str.ToCharArray())
            {
                if (coding.GetByteCount(ch.ToString()) == 2)
                    dcount++;
            }
            string w = str.PadRight(totalByteCount - dcount, chs);
            return w;
        }

        /// <summary>
        /// 中英居中对齐
        /// </summary>
        /// <param name="str"></param>
        /// <param name="totalByteCount"></param>
        /// <param name="chs"></param>
        /// <returns></returns>
        public static string padCenterEx(string str, int totalByteCount, char chs)
        {
            Encoding coding = Encoding.GetEncoding("gb2312");
            int dcount = 0;
            foreach (char ch in str.ToCharArray())
            {
                if (coding.GetByteCount(ch.ToString()) == 2)
                    dcount++;
            }

            string w = str.PadRight((totalByteCount - dcount), chs);
            // w = w.PadRight(totalByteCount, chs);
            return w;
        }

        /// <summary>
        /// 按字节截取字节串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string bSubstring(string s, int start, int length)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(s);
            return System.Text.Encoding.Default.GetString(bytes, start, length);
        }


        /// <summary>
        /// 读取卡号信息
        /// </summary>
        /// <param name="strTrack2"></param>
        /// <param name="strTrack3"></param>
        /// <returns></returns>
        public static string GetCardNumber(string strTrack2, string strTrack3)
        {
            string ret = "";
            if ((strTrack2.Length == 0) && (strTrack3.Length == 0))
            {
                return ret;
            }
            //从第二位起取，直至等号字符位置
            if (strTrack2.Length != 0)
            {
                string[] tmp = strTrack2.Split('=');
                // 如果大于19位只取19位
                if (tmp[0].Length > 21)
                {
                    ret = tmp[0].Substring(0, 21);
                }
                else
                {
                    ret = tmp[0];
                }
            }
            else if (strTrack3.Length != 0)
            {
                string[] tmp = strTrack3.Split('=');
                // 如果大于19位只取19位
                if (tmp[0].Length > 23)
                {
                    ret = tmp[0].Substring(2, 21);
                }
                else
                {
                    ret = tmp[0].Substring(2);
                }
            }
            return ret;
        }

        /// <summary>
        /// 从二磁道获取有效期
        /// </summary>
        /// <param name="strTrack2"></param>
        /// <param name="strTrack3"></param>
        /// <returns></returns>
        public static string GetExpDate(string strTrack2, string strTrack3)
        {
            string ret = "";

            int iIndexFlag = 0;
            if (!String.IsNullOrEmpty(strTrack2))
            {
                if (strTrack2.Contains("="))
                {
                    if (strTrack2.StartsWith("59"))
                    {
                        //万事达卡
                        iIndexFlag = strTrack2.IndexOf("=") + 1 + 3;
                    }
                    else
                    {
                        iIndexFlag = strTrack2.IndexOf("=") + 1;
                    }
                    ret = strTrack2.Substring(iIndexFlag, 4);
                }
                else if (strTrack2.Contains("D"))
                {
                    if (strTrack2.StartsWith("59"))
                    {
                        //万事达卡
                        iIndexFlag = strTrack2.IndexOf("D") + 1 + 3;
                    }
                    else
                    {
                        iIndexFlag = strTrack2.IndexOf("D") + 1;
                    }
                    ret = strTrack2.Substring(iIndexFlag, 4);
                }
            }

            return ret;
        }

        /// <summary>
        /// 根据二磁道，检测是否IC卡
        /// </summary>
        /// <param name="strTrack2"></param>
        /// <returns></returns>
        public static bool CheckIcCardFlag(string strTrack2)
        {
            try
            {
                bool bRet = false;
                string strCardNumber = GetCardNumber(strTrack2, "");
                if (!String.IsNullOrEmpty(strTrack2))
                {
                    string strIcFlag = "";
                    int iIndexFlag = 0;
                    if (!String.IsNullOrEmpty(strCardNumber) && !String.IsNullOrEmpty(strTrack2))
                    {
                        if (strTrack2.Contains("="))
                        {
                            if (strCardNumber.StartsWith("59"))
                            {
                                //万事达卡
                                iIndexFlag = strTrack2.IndexOf("=") + 1 + 4 + 3;
                            }
                            else
                            {
                                iIndexFlag = strTrack2.IndexOf("=") + 1 + 4;
                            }
                            strIcFlag = strTrack2.Substring(iIndexFlag, 1);
                        }
                        else if (strTrack2.Contains("D"))
                        {
                            if (strCardNumber.StartsWith("59"))
                            {
                                //万事达卡
                                iIndexFlag = strTrack2.IndexOf("D") + 1 + 4 + 3;
                            }
                            else
                            {
                                iIndexFlag = strTrack2.IndexOf("D") + 1 + 4;
                            }
                            strIcFlag = strTrack2.Substring(iIndexFlag, 1);
                        }
                        if (strIcFlag == "2" || strIcFlag == "6")
                        {
                            bRet = true;
                        }
                    }
                }

                return bRet;
            }
            catch (Exception ex)
            {
                Log.Error("根据二磁道，检测是否IC卡", ex);
                throw;
            }
        }

        /// <summary>
        /// 获取要打印的银行卡卡号
        /// </summary>
        /// <param name="cardNo">原始银行卡号</param>
        /// <returns></returns>
        public static string GetPrintCardNo(string cardNo)
        {
            if (cardNo.Length <= 10)
            {
                return cardNo;
            }
            else
            {
                string leftString = cardNo.Substring(0, 6);
                string middleString = ("").PadLeft(cardNo.Length - 10, '*');
                string rightString = cardNo.Substring(6 + middleString.Length, 4);
                return leftString + middleString + rightString;
            }
        }

        /// <summary>
        /// 新的获取打印银行卡卡号
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public static string GetPrintCardNoEx(string cardNo)
        {
            string leftString = cardNo.Substring(0, cardNo.Length - 5);
            string middleString = ("").PadLeft(4, '*');
            string rightString = cardNo.Substring(cardNo.Length - 1, 1);
            return leftString + middleString + rightString;
        }

        /// <summary>
        /// 将金额转换为定长字符串，并加上前导零 注意:不可传入负金额
        /// </summary>
        /// <param name="strAmount"></param>
        /// <returns></returns>
        public static string AmountToString(string strAmount)
        {
            string result = "000000000000";
            try
            {
                double lAmount;

                if (strAmount.Length > 0)
                {
                    lAmount = double.Parse(strAmount);
                    lAmount = lAmount * 100;
                    //result = (new String('0', 12 - lAmount.ToString().Length)) + lAmount.ToString();
                    // 先将X100后的小数点去掉，如果不够左补0
                    string tmp = lAmount.ToString("0").PadLeft(12, '0');
                    // 固定为定长12
                    result = tmp.Substring(tmp.Length - 12, 12);
                }
            }
            catch (Exception)
            {
                result = "0";
            }
            return result;
        }

        /// <summary>
        /// 将金额转换为定长字符串，并加上前导零 注意:不可传入负金额
        /// </summary>
        /// <param name="strAmount"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string AmountToStringEx(string strAmount, int length) 
        {
            string result = "000000000000";
            try
            {
                double lAmount;

                if (strAmount.Length > 0)
                {
                    lAmount = double.Parse(strAmount);
                    lAmount = lAmount * 100;
                    // 先将X100后的小数点去掉，如果不够左补0
                    string tmp = lAmount.ToString("0").PadLeft(length, '0');
                    result = tmp.Substring(tmp.Length - length, length);
                }
            }
            catch (Exception)
            {
                result = "0";
            }
            return result;
        }

        /// <summary>
        /// 将定长字符串转换为金额，去掉前导零，加入小数点
        /// </summary>
        /// <param name="strString"></param>
        /// <returns></returns>
        public static string StringToAmount(string strString)
        {
            string result = "0.00";
            try
            {
                double lAmount;

                if (strString.Length > 0)
                {
                    lAmount = double.Parse(strString);
                    lAmount = lAmount / 100;

                    result = lAmount.ToString("##,#0.00");
                }
            }
            catch (Exception)
            {
                result = "0.00";
            }
            return result;
        }

        

        /// <summary>
        /// 10进制串转为BCD码   
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static byte[] str2Bcd(String asc)
        {
            int len = asc.Length;
            int mod = len % 2;

            if (mod != 0)
            {
                asc = "0" + asc;
                len = asc.Length;
            }

            byte[] abt = new byte[len];
            if (len >= 2)
            {
                len = len / 2;
            }

            byte[] bbt = new byte[len];
            abt = System.Text.Encoding.Default.GetBytes(asc);
            int j, k;

            for (int p = 0; p < asc.Length / 2; p++)
            {
                if ((abt[2 * p] >= '0') && (abt[2 * p] <= '9'))
                {
                    j = abt[2 * p] - '0';
                }
                else if ((abt[2 * p] >= 'a') && (abt[2 * p] <= 'z'))
                {
                    j = abt[2 * p] - 'a' + 0x0a;
                }
                else
                {
                    j = abt[2 * p] - 'A' + 0x0a;
                }

                if ((abt[2 * p + 1] >= '0') && (abt[2 * p + 1] <= '9'))
                {
                    k = abt[2 * p + 1] - '0';
                }
                else if ((abt[2 * p + 1] >= 'a') && (abt[2 * p + 1] <= 'z'))
                {
                    k = abt[2 * p + 1] - 'a' + 0x0a;
                }
                else
                {
                    k = abt[2 * p + 1] - 'A' + 0x0a;
                }

                int a = (j << 4) + k;
                byte b = (byte)a;
                bbt[p] = b;
            }
            return bbt;
        }

        public static bool ValidateBCD(string str)
        {
            bool ret = false;
            if (string.IsNullOrEmpty(str))
                return ret;
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsLetterOrDigit(str, i))
                    return ret;
            }
            return true;
        }

        /// <summary>
        /// 将bcd码转换成字符串
        /// </summary>
        /// <param name="bcd_buf"></param>
        /// <param name="conv_len"></param>
        /// <returns></returns>
        public static string bcd2str(byte[] bcd_buf, int conv_len)
        {
            int i = 0, j = 0;
            byte tmp = 0x00;
            byte[] ret = new byte[conv_len * 2];
            for (i = 0, j = 0; i < conv_len; i++)
            {
                tmp = (byte)(bcd_buf[i] >> 4);
                tmp += (byte)((tmp > 9) ? ('A' - 10) : '0');
                ret[j++] = tmp;
                tmp = (byte)(bcd_buf[i] & 0x0f);
                tmp += (byte)((tmp > 9) ? ('A' - 10) : '0');
                ret[j++] = tmp;
            }

            return Encoding.Default.GetString(ret).TrimEnd('\0');
        }

        /// <summary>
        /// 新的针对binary类型的 字符串 转换 字节数组
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static byte[] str2Bcd2(string asc)
        {
            int m = 0;
            ulong l = Convert.ToUInt64(asc);
            byte[] t1 = new byte[8];
            for (m = 0; m < 8; m++)
            {
                if (l < 256)
                {
                    t1[m] = (byte)l;
                    break;
                }
                else
                {
                    t1[m] = (byte)(l % 256);
                    l = (l - t1[m]) / 256;
                }
            }
            int lg = 0;
            if (asc.Length % 2 != 0)
            {
                lg = asc.Length / 2 + 1;
            }
            else
            {
                lg = asc.Length / 2;
            }
            byte[] t2 = new byte[lg];
            Array.Copy(t1, 0, t2, 0, lg);
            Array.Reverse(t2);
            return t2;
        }

        //新的针对binary类型的 字节数组 转换 字符串
        public static string bcd2str2(byte[] bcd_buf, int conv_len)
        {
            ulong iRet = bcd_buf[0];
            if (bcd_buf.Length > 1)
            {
                for (int k = 1; k < bcd_buf.Length; k++)
                {
                    iRet = iRet * 256 + bcd_buf[k];
                }
            }
            string result = iRet.ToString().PadLeft(conv_len * 2, '0');
            return result;
        }


        /// <summary>
        /// Ping某个IP地址
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>成功Success失败TimeOut</returns>
        public static string PingResult(string ip)
        {
            try
            {
                System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
                //System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
                //options.DontFragment = true;
                //string data = "landi";
                //byte[] buffer = Encoding.ASCII.GetBytes(data);
                //int timeout = 5000;
                System.Net.NetworkInformation.PingReply reply = p.Send(ip);//, timeout, buffer, options);
                return reply.Status.ToString();
            }
            catch (System.Exception e)
            {
                Log.Error("[PubFunc.cs][PingResult]Error!\n" + e.ToString());
                return "fail";
            }
        }

        /// <summary>
        /// 设置系统时间
        /// </summary>
        /// <param name="newdatetime">新时间</param>
        /// <returns></returns>
        public static bool SetSysTime(DateTime newdatetime)
        {
            try
            {
                SystemTime st = new SystemTime();
                st.year = Convert.ToUInt16(newdatetime.Year);
                st.month = Convert.ToUInt16(newdatetime.Month);
                st.day = Convert.ToUInt16(newdatetime.Day);
                //st.dayofweek = Convert.ToUInt16(newdatetime.DayOfWeek);
                st.hour = Convert.ToUInt16(newdatetime.Hour);
                st.minute = Convert.ToUInt16(newdatetime.Minute);
                st.second = Convert.ToUInt16(newdatetime.Second);
                return DllImport.SetLocalTime(st);
            }
            catch (System.Exception e)
            {
                Log.Error("[PubFunc.cs][SetSysTime]Error!\n" + e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 字符串转日期
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime String2Datetime(string datetime)
        {
            try
            {
                int year = Convert.ToInt32(datetime.Substring(0, 4));
                int month = Convert.ToInt32(datetime.Substring(4, 2));
                int day = Convert.ToInt32(datetime.Substring(6, 2));
                int hour = Convert.ToInt32(datetime.Substring(8, 2));
                int mi = Convert.ToInt32(datetime.Substring(10, 2));
                int ss = Convert.ToInt32(datetime.Substring(12, 2));
                DateTime newDt = new DateTime(year, month, day, hour, mi, ss);
                return newDt;
            }
            catch (System.Exception)
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// 去除byte数组null值，返回字符串
        /// </summary>
        /// <param name="sByte"></param>
        /// <returns></returns>
        public static string GetByteArrayString(byte[] sByte)
        {
            int i;
            for (i = 0; i < sByte.Length; i++ )
            {
                if (sByte[i] == 0)
                {
                    break;
                }
            }
            byte[] bResult = new byte[i];
            Array.Copy(sByte, bResult, i);

            return Encoding.Default.GetString(bResult);
        }

        /// <summary>
        /// 去除byte数组null值，返回数组
        /// </summary>
        /// <param name="sByte"></param>
        /// <returns></returns>
        public static byte[] TrimNullArrayOfByte(byte[] sByte)
        {
            int i;
            for (i = 0; i < sByte.Length; i++)
            {
                if (sByte[i] == 0)
                {
                    break;
                }
            }
            byte[] bResult = new byte[i];
            Array.Copy(sByte, bResult, i);

            return bResult;
        }

        /// <summary>
        /// 比较两个字节数组是否相等
        /// </summary>
        /// <param name="b1">byte数组1</param>
        /// <param name="b2">byte数组2</param>
        /// <returns>是否相等</returns> 
        public static bool ByteEquals(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length) return false;
            if (b1 == null || b2 == null) return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }

        private const int SND_FILENAME = 0x00020000;
        private const int SND_ASYNC = 0x0001;
        /// <summary>
        /// 声音文件播放
        /// </summary>
        /// <param name="PathFileName">路径文件</param>
        public static void SoundPlay(String PathFileName)
        {
            if (!File.Exists(PathFileName)) return;
            DllImport.PlaySound(PathFileName, 0, SND_ASYNC | SND_FILENAME);
        }

        /// <summary>
        /// 一次性生成多层目录
        /// </summary>
        /// <param name="fileName">含文件名的绝对路径(例C:\a\b\c\d\aa.html)</param>
        /// <returns></returns>
        public static bool MakeFullDir(string fullFile)
        {
            if (File.Exists(fullFile)) return true;

            if (fullFile.Length < 1) return false;
            if (fullFile.IndexOf(":") == -1) return false;
            string[] fileArray = fullFile.Split('\\');
            string tPath = fileArray[0];
            for (int i = 1; i < fileArray.Length - 1; i++)
            {
                tPath += "\\" + fileArray[i];
                if (!Directory.Exists(tPath))
                {
                    Directory.CreateDirectory(tPath);
                }
            }
            return true;
        }

        public static bool CheckIDNo(string idNo)
        {
            bool ret = false;
            int[] iWeight = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
            char[] cCheck = { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
            int Sum = 0;
            char check;

            try
            {
                if (idNo.Length != 18)
                {
                    return ret;
                }

                /* 身份证18位校验位是否有效 */
                for (int i = 0; i < 17; i++)
                {
                    Sum += iWeight[i] * int.Parse(idNo.Substring(i, 1));
                }
                Sum %= 11;

                char[] tmp = idNo.ToCharArray();
                if ('x' == tmp[17])
                {
                    check = 'X';
                }
                else
                {
                    check = tmp[17];
                }

                if (cCheck[Sum] == check)
                {
                    ret = true;
                }
            }
            catch (Exception e)
            {
                Log.Error("CheckIDNo Error\n" + e.ToString());
            }

            return ret;
        }

        /// <summary>
        /// 获取父级目录
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string GetParentDir(string dir, int n) //n为几级父目录
        {
            string st = System.Environment.CurrentDirectory;
            try
            {
                System.Environment.CurrentDirectory = dir;
                for (int i = 1; i <= n; i++)
                {
                    System.Environment.CurrentDirectory = "..";
                }
                return System.Environment.CurrentDirectory;
            }
            finally
            {
                System.Environment.CurrentDirectory = st;//恢复最初
            }
        }

        /// <summary>
        /// 以Socket测试的方式确认网络连接是否正常
        /// </summary>
        /// <param name="hostIP"></param>
        /// <param name="hostPort"></param>
        /// <returns></returns>
        public static bool CheckConnection(string hostIP, int hostPort)
        {
            bool bStatus = false;
            try
            {
                if (TimeOutSocket.Connect(hostIP, hostPort, 5000))
                {
                    bStatus = true;
                }
            }
            catch (System.Exception e)
            {
                Log.Error("[PubFunc][CheckNetStatus]Error!\n" + e.ToString());
                return false;
            }
            return bStatus;
        }

        /// <summary>
        /// 文件夹复制
        /// </summary>
        /// <param name="varFromDirectory"></param>
        /// <param name="varToDirectory"></param>
        public static void CopyFiles(string varFromDirectory, string varToDirectory)
        {
            Directory.CreateDirectory(varToDirectory);
            if (!Directory.Exists(varFromDirectory)) return;
            string[] directories = Directory.GetDirectories(varFromDirectory);
            if (directories.Length > 0)
            {
                foreach (string d in directories)
                {
                    CopyFiles(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
                }
            }
            string[] files = Directory.GetFiles(varFromDirectory);
            if (files.Length > 0)
            {
                foreach (string s in files)
                {
                    File.Copy(s, varToDirectory + s.Substring(s.LastIndexOf("\\")), true);
                }
            }
        }

        /// <summary>
        /// 备份日志文件
        /// </summary>
        public static bool CopyFile(string sourceFile, string destFile)
        {
            try
            {
                if (!File.Exists(sourceFile))
                {
                    return false;
                }
                if (File.Exists(destFile))
                {
                    return false;
                }
                if (!Directory.Exists(destFile))
                {
                    Directory.CreateDirectory(destFile.Substring(0, destFile.LastIndexOf('\\')));
                }
                File.Copy(sourceFile, destFile);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("[Utility.cs][CopyFile]Error!" + "\n",ex);
                return false;
            }
        }

        /// <summary>    
        /// 转换人民币大小金额    
        /// </summary>    
        /// <param name="num">金额</param>    
        /// <returns>返回大写形式</returns>    
        public static string Amount2Cn(decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字    
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字    
            string str3 = "";    //从原num值中取出的值    
            string str4 = "";    //数字的字符串形式    
            string str5 = "";  //人民币大写金额形式    
            int i;    //循环变量    
            int j;    //num的值乘以100的字符串长度    
            string ch1 = "";    //数字的汉语读法    
            string ch2 = "";    //数字位的汉字读法    
            int nzero = 0;  //用来计算连续的零值是几个    
            int temp;            //从原num值中取出的值    

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数    
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式    
            j = str4.Length;      //找出最高位    
            if (j > 15) { return "溢出"; }
            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分    

            //循环取出每一位需要转换的值    
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值    
                temp = Convert.ToInt32(str3);      //转换为数字    
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时    
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位    
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3))
                {
                    //如果该位是亿位或元位，则必须写上    
                    ch2 = str2.Substring(i, 1);
                }
                str5 = str5 + ch1 + ch2;

                if (i == j - 1 && str3 == "0")
                {
                    //最后一位（分）为0时，加上“整”    
                    str5 = str5 + "整";
                }
            }
            if (num == 0)
            {
                str5 = "零元整";
            }
            return str5;
        }


        /// <summary>    
        /// 转换人民币大小金额  (一个重载，将字符串先转换成数字在调用CmycurD)   
        /// </summary>    
        /// <param name="num">用户输入的金额，字符串形式未转成decimal</param>    
        /// <returns></returns>    
        public static string Amount2Cn(string numstr)
        {
            try
            {
                decimal num = Convert.ToDecimal(numstr);
                return Amount2Cn(num);
            }
            catch
            {
                return "非数字形式！";
            }
        }

        /// <summary>
        /// 文件二进制序列化
        /// </summary>
        /// <typeparam name="T">序列化对象</typeparam>
        /// <param name="filePath">序列化文件</param>
        /// <param name="SerObj"></param>
        public static void SaveToFile<T>(string filePath, List<T> SerObj)
                where T : class
        {
            try
            {
                if (SerObj == null || SerObj.Count == 0)
                    return;

                using (FileStream fileStream = new FileStream(filePath, FileMode.Append))
                {
                    BinaryFormatter b = new BinaryFormatter();
                    foreach (T item in SerObj)
                        b.Serialize(fileStream, item);
                    fileStream.Close();
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// 二进制文件反序列化
        /// </summary>
        /// <typeparam name="T">反序列化对象</typeparam>
        /// <param name="filePath">反序列化文件</param>
        /// <param name="DerObj"></param>
        public static void RestoreFromFile<T>(string filePath, List<T> DerObj)
            where T : class
        {
            if (File.Exists(filePath) && DerObj != null)
            {
                try
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        BinaryFormatter b = new BinaryFormatter();
                        while (fileStream.Position < fileStream.Length)
                            DerObj.Add(b.Deserialize(fileStream) as T);
                        fileStream.Close();
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("[RestoreFromFile]ERROR", ex);
                }
            }
            return;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

    }


    /// <summary>
    /// 测试超时套接字处理类
    /// </summary>
    class TimeOutSocket
    {
        private static bool IsConnectionSuccessful = false;
        private static Exception socketException;
        private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);

        public static bool Connect(string serverIp, int serverPort, int timeoutMSec)
        {
            TimeoutObject.Reset();
            socketException = null;
            TcpClient tcpClient = new TcpClient();
            tcpClient.BeginConnect(serverIp, serverPort, new AsyncCallback(CallBackMethod), tcpClient);
            //tcpclient.LingerState.LingerTime = 0;
            if (TimeoutObject.WaitOne(timeoutMSec, false))
            {
                tcpClient.Close();
                if (IsConnectionSuccessful)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                tcpClient.Close();
                return false;
            }
        }

        private static void CallBackMethod(IAsyncResult asyncresult)
        {
            try
            {
                IsConnectionSuccessful = false;
                TcpClient tcpclient = asyncresult.AsyncState as TcpClient;

                if (tcpclient.Client != null)
                {
                    tcpclient.EndConnect(asyncresult);
                    IsConnectionSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                IsConnectionSuccessful = false;
                socketException = ex;
            }
            finally
            {
                TimeoutObject.Set();
            }
        }
    }
}
