using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace Landi.FrameWorks
{
    internal class ActivityHandler : Handler
    {
        public ActivityManager Proxy;
        public const int TRANSACTION = 9999;

        public ActivityHandler(Looper looper)
            : base(looper)
        {

        }

        public ActivityHandler()
            : base()
        {

        }

        protected override void HandleMessage(Message msg)
        {
            switch (msg.what)
            {
                case TRANSACTION:
                    PackageBase pac = msg.obj as PackageBase;
                    PackageBase.ResultHandler handler = pac.OnResult;
                    bool first = true;
                    TransResult nRet = TransResult.E_SEND_FAIL;
                    TransResult ret = TransResult.E_SEND_FAIL;
                repeat:
                    ret = TransResult.E_SEND_FAIL; 
                    for (int count = 0; count < pac.RepeatTimes; count++)
                    {
                        ret = pac.Communicate();
                        if (ret == TransResult.E_SUCC)
                            break;
                        System.Threading.Thread.Sleep(200);
                    }
                    if (first)
                    {
                        first = false;
                        nRet = ret;
                    }
                    //将在交易之中，产生的交易进入队列上送
                    while ((pac = pac.DequeueWork()) != null)
                        goto repeat;
                    Proxy.TransactionCompleted(handler, nRet);
                    break;
                default:
                    //如果不是交易则activity必须实现IHandleMessage，自行处理消息
                    Proxy.HandleCustomMessage(msg);
                    break;
            }
        }
    }
}
