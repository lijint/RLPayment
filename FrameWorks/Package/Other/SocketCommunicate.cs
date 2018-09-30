using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Landi.FrameWorks.Package.Other
{
    public abstract class SocketCommunicate
    {

        private string serverIP;
        private int serverPort;

        public bool IsRecvLen;
        public int HeadLength;

        public string SendPackage;
        public string RecvPackage;


        public enum TransResult : int
        {
            E_SUCC = 0,               // 成功返回
            E_SEND_FAIL = 1,          // 发送失败
            E_RECV_FAIL = 2,          // 接收失败
            E_HOST_FAIL = 3,          // 主机返回错误代码
            E_MAC_ERROR = 4,          // 服务端报文mac校验错
            E_CHECK_FAIL = 5,         // 冲正
            E_UNPACKET_FAIL = 6,      // 解包失败
            E_KEYVERIFY_FAIL = 7,     // 签到校验失败
            E_PACKET_FAIL = 8,        //打包失败
            E_INVALID = 9,            //无效交易
        }


        public SocketCommunicate()
        {
        }

        public int setIPAndPort(string ip,int port)
        {
            if (string.IsNullOrEmpty(ip) || port == 0)
                return -1;
            serverIP = ip;
            serverPort = port;
            return 0;
        }

        public TransResult transact()
        {
            TransResult ret = TransResult.E_SEND_FAIL;
            Socket socket = null;
            IPAddress ip = IPAddress.Parse(serverIP);
            IPEndPoint ipe = new IPEndPoint(ip, serverPort); //把ip和端口转化为IPEndPoint实例
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.SendTimeout = 30 * 1000;
                socket.ReceiveTimeout = 30 * 1000;
                socket.Connect(ipe);
            }
            catch (Exception err)
            {
                Log.Error(this.GetType().Name, err);
                return ret;
            }

            try
            {
                ret = TransResult.E_PACKET_FAIL;
                byte[] SendBytes = new byte[0];
                Packet();
                if (SendPackage == null)
                    return TransResult.E_INVALID;

                SendBytes = Encoding.UTF8.GetBytes(SendPackage);
                byte[] send_all = PacketHead(SendBytes);
                ret = TransResult.E_SEND_FAIL;
                int sendLen = 0;
                sendLen = socket.Send(send_all, send_all.Length, 0);
                if (sendLen <= 0)
                {
                    socket.Close();
                    return ret;
                }
                ////从服务器端接受返回信息
                ret = TransResult.E_RECV_FAIL;
                int recvLen = 0;
                byte[] recHead = new byte[HeadLength];
                int recvPacketLength = 0;

                //接收包头
                recvLen = socket.Receive(recHead, HeadLength, 0);
                if (recvLen <= 0)
                {
                    socket.Close();
                    return ret;
                }
                for (int i = 0; i < HeadLength; i++)
                {
                    recvPacketLength += recHead[i] * (256 ^ i);
                }

                //接收包体
                recvLen = 0;
                byte[] recv_all = new byte[recvPacketLength];
                recv_all.Initialize();
                recvLen = socket.Receive(recv_all, recvPacketLength, 0);
                
                if (recvLen <= 0)
                {
                    socket.Close();
                    return ret;
                }
                RecvPackage = Encoding.UTF8.GetString(recv_all);
                UnPacketHead();
                
                bool nRet = UnPacket();
                if (nRet)
                {
                    ret = TransResult.E_SUCC;
                }
                else
                {
                    ret = TransResult.E_HOST_FAIL;
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
        

        protected virtual byte[] PacketHead(byte[] SendBytes)
        { return null; }
        protected virtual byte[] UnPacketHead()
        { return null; }


        protected virtual void Packet()
        { }
        protected virtual bool UnPacket()
        { return true; }
    }
}
