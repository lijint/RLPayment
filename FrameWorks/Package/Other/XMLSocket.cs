using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Landi.FrameWorks.Iso8583;

namespace Landi.FrameWorks.Package.Other
{
    class XMLSocket : PackageBase
    {
        protected XMLConfig SendXMLPackage = null;
        protected XMLConfig RecvXMLPackage = null;
        
        protected XMLSocket()
        {
            //读取XML文件
            readXML();
        }

        private void readXML()
        {
            string xmlFileName = "";
            SendXMLPackage = new XMLConfig(xmlFileName);
        }

        internal override TransResult transact()
        {
            TransResult ret = TransResult.E_SEND_FAIL;
            Socket socket = null;
            if (RealEnv)
            {
                if (!GPRSConnect()) return ret;
                IPAddress ip = IPAddress.Parse(serverIP);
                IPEndPoint ipe = new IPEndPoint(ip, serverPort); //把ip和端口转化为IPEndPoint实例
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.SendTimeout = sendTimeOut * 1000;
                    socket.ReceiveTimeout = recvTimeOut * 1000;
                    socket.Connect(ipe);
                }
                catch (Exception err)
                {
                    Log.Error(this.GetType().Name, err);
                    return ret;
                }
            }

            try
            {
                ret = TransResult.E_PACKET_FAIL;
                byte[] SendBytes = new byte[0];
                PackFix();
                Packet();
                if (SendXMLPackage == null)
                    return TransResult.E_INVALID;

                string xmlStr = SendXMLPackage.GetXML();
                SendBytes = Encoding.Default.GetBytes(xmlStr);
                byte[] head = PackBytesAtFront(xmlStr.Length);
                int headLen = head.Length;

                int sendLen_all = head.Length + xmlStr.Length; 
                byte[] sendstr_all = new byte[sendLen_all];
                Array.Copy(head, sendstr_all, head.Length);
                Array.Copy(SendBytes, 0, sendstr_all, head.Length, SendBytes.Length);

                ////记录原始发送日志
                CLog.Info(CLog.GetXMLLog(Encoding.Default.GetString(sendstr_all), this, CLog.LogType.Send));

                ret = TransResult.E_SEND_FAIL;
                if (RealEnv)
                {
                    int sendLen = 0;
                    sendLen = socket.Send(sendstr_all, sendLen_all, 0);
                    if (sendLen <= 0)
                    {
                        socket.Close();
                        return ret;
                    }
                }

                ////从服务器端接受返回信息
                ret = TransResult.E_RECV_FAIL;
                int recvLen = 0;
                if (RealEnv)
                {
                    sRecvBuffer.Initialize();
                    recvLen = socket.Receive(sRecvBuffer, sRecvBuffer.Length, 0);

                    if (recvLen <= 0)
                    {
                        socket.Close();
                        return ret;
                    }
                    byte[] RecvBytes = new byte[recvLen - headLen];
                    Array.Copy(sRecvBuffer, headLen, RecvBytes, 0, recvLen - headLen);
                    byte[] headBytes = new byte[headLen];
                    Array.Copy(sRecvBuffer, headBytes, headLen);
                    //解包
                    ret = TransResult.E_UNPACKET_FAIL;
                    //FrontBytes = headBytes;
                    //HandleFrontBytes(headBytes);//根据报文头来判断是否要下载密钥
                    //RecvPackage.ParseBuffer(RecvBytes, SendPackage.ExistValue(0));
                    RecvXMLPackage = new XMLConfig();
                    RecvXMLPackage.loadXML(Encoding.Default.GetString(RecvBytes));

                    //记录原始接收日志
                    byte[] logRecv = new byte[recvLen];
                    Array.Copy(sRecvBuffer, logRecv, recvLen);
                    //CLog.LogPackage(logRecv, RecvPackage, CLog.LogType.Recv);
                    CLog.Info(CLog.GetXMLLog(Encoding.Default.GetString(logRecv), this, CLog.LogType.Recv));
                    bool nRet = UnPackFix();
                   if (!mInvokeSetResult)
                       throw new Exception("should invoke SetRespInfo() in UnPackFix()");
                    mInvokeSetResult = false;
                    if (nRet)
                    {
                        ret = TransResult.E_SUCC;
                    }
                    else
                    {
                        ret = TransResult.E_HOST_FAIL;
                    }
                }
                else
                {
                    ret = TransResult.E_SUCC;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(this.GetType().Name, ex);
            }
            finally
            {
                if (socket != null && socket.Connected)
                    socket.Close();
            }
            return ret;
        }

        /// <summary>
        /// 重要
        /// </summary>
        protected override string SectionName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// 打固定包
        /// </summary>
        protected override void PackFix()
        {
            
        }

        protected override bool UnPackFix()
        {
            return true;
        }

        /// <summary>
        /// 打包报文头
        /// </summary>
        /// <param name="dataLen"></param>
        /// <returns></returns>
        protected override byte[] PackBytesAtFront(int dataLen)
        {
            return new byte[0];
        }

        /// <summary>
        /// 不需要
        /// </summary>
        /// <returns></returns>
        protected override bool NeedCalcMac()
        {
            return false;
        }

        /// <summary>
        /// 解包报文头
        /// </summary>
        /// <param name="headBytes"></param>
        protected override void HandleFrontBytes(byte[] headBytes)
        {
            
        }
    }
}
