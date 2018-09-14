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
                default:
                    //如果不是交易则activity必须实现IHandleMessage，自行处理消息
                    Proxy.HandleCustomMessage(msg);
                    break;
            }
        }
    }
}
