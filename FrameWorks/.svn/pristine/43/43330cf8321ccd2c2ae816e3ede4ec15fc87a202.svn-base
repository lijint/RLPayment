using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Collections;

namespace Landi.FrameWorks.HardWare
{
    public class GPRS : HardwareBase<GPRS, bool>, IManagedHardware, IManageNet
    {
        public string ApnDialNum; //apn拨号号码
        public string ApnDialParam; //apn拨号参数
        public bool UseAppDialParam = false; //是否使用应用参数
        public string HostIP = "";
        public int HostPort = 0;

        public GPRS()
        {
            ApnDialNum = ReadIniFile("ApnDialNum").Trim();
            ApnDialParam = ReadIniFile("ApnDialParam").Trim();
            UseAppDialParam = ReadIniFile("UseAppDialParam").Trim() == "1";
            if (ApnDialNum == "")
                throw new Exception("未配置apn拨号号码");
            else if (UseAppDialParam && ApnDialParam == "")
                throw new Exception("apn拨号参数配置错误");
        }

        public void UpdateHostInfo(string hostIP, int hostPort)
        {
            HostIP = hostIP;
            HostPort = hostPort;
        }

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

        public static void CreateConnection()
        {
            //RasManager myRas = new RasManager();
            //myRas.PhoneNumber = "*99**PPP*1#";
            //myRas.UserName = "username";
            //myRas.Password = "password";
            //myRas.Connect();
        }

        //private static bool hadSetDialParam = false; //是否已设置过apn参数，避免重复设置
        public static bool CreateConnection(int retryTimes, string hostIP, int hospPort)
        {
            bool ret = false;
            if (retryTimes == 0) retryTimes = 2; //默认连接两次
            try
            {
                if (!IsUse)
                {
                    Log.Warn("已关闭GPRS模块，无需拨号");
                    return true;
                }
                else
                {
                    Log.Info("开始拨号...");
                    RasManager rsm = new RasManager();
                    if (rsm.isConnect() && (CheckConnection(hostIP, hospPort)))
                    {
                       Log.Warn("已经在线");
                        return true;
                    }
                    else
                    {
                        if (GetInstance().UseAppDialParam)
                        {
                            if (!SetDialParam()) return false;
                        }
                        rsm.UserName = "";
                        rsm.Password = "";
                        rsm.PhoneNumber = GetInstance().ApnDialNum;
                        Log.Info("拨号号码:" + rsm.PhoneNumber);
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
            }
            catch (Exception err)
            {
                Log.Error("[GPRS][CreateConnection]Error!\n", err);
                ret = false;
            }
            return ret;
        }


        private static SerialPort serialPort;
        private static bool hadSetParam = false;
        /// <summary>
        /// 设置拨号接入点参数
        /// </summary>
        /// <returns></returns>
        public static bool SetDialParam()
        {
            try
            {
                HangUpAllConnection();
                if (hadSetParam) return true;

                if (serialPort == null)
                {
                    serialPort = new SerialPort(Port, Bps);
                }
                serialPort.NewLine = "\r\n";
                serialPort.RtsEnable = true;
                serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                serialPort.Open();
                Log.Info("设置拨号参数apn=" + GetInstance().ApnDialParam);
                serialPort.WriteLine(GetInstance().ApnDialParam);
                System.Threading.Thread.Sleep(300);
                serialPort.Close();
            }
            catch (System.Exception e)
            {
                Log.Error("[GPRS][SetDialParam]Error!\n", e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 串口接收回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int n = serialPort.BytesToRead;
                byte[] buf = new byte[n];
                serialPort.Read(buf, 0, n);
                Log.Info("串口接收数据：" + Encoding.ASCII.GetString(buf));
                if (n > 0) hadSetParam = true;
            }
            catch (System.Exception ex)
            {
                Log.Error("[GPRS][serialPort_DataReceived]Error!", ex);
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
                Log.Info("[CheckConnection] ip=" + hostIP + " port=" + hostPort + " status=" + bStatus);
            }
            catch (System.Exception ex)
            {
                Log.Error("[GPRS][CheckNetStatus]Error!\n", ex);
                return false;
            }
            return bStatus;
        }


        #region IManagedHardware 成员

        public object Open()
        {
            try
            {
                if (!IsUse)
                {
                    Log.Info("已关闭GPRS模块，无需拨号");
                    return true;
                }

                if (GlobalAppData.GetInstance().BusiServerIpAndPort.Count == 0)
                {
                    Log.Info("业务IP或端口没有分配，请先分配");
                    return false;
                }

                HangUpAllConnection();
                foreach (KeyValuePair<string, string> item in GlobalAppData.GetInstance().BusiServerIpAndPort)
                {
                    string[] temp = item.Value.Split(':');
                    if (!CreateConnection(3, temp[0], int.Parse(temp[1])))
                        return false;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error("[GPRS][Open]Error", ex);
                return false;
            }
            return true;
        }

        public object Close()
        {
            try
            {
                HangUpAllConnection();
            }
            catch (System.Exception ex)
            {
                Log.Error("[GPRS][Open]Close", ex);
                return false;
            }
            
            return true;
        }

        public object CheckStatus()
        {
            try
            {
                if (!IsUse) return true;
                foreach (KeyValuePair<string, string> item in GlobalAppData.GetInstance().BusiServerIpAndPort)
                {
                    string[] temp = item.Value.Split(':');
                    if (!CheckConnection(temp[0], int.Parse(temp[1])))
                    {
                        return false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error("[GPRS][Open]CheckStatus", ex);
                return false;
            }
            
            return true;
        }

        public bool MeansError(object status)
        {
            return !(bool)status;
        }

        #endregion
    }
}
