using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks
{
    public interface IHandleMessage
    {
        void HandleMessage(Message message);
    }
}
