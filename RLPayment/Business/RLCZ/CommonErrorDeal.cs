using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Business.RLCZ
{
    class CommonErrorDeal : MessageActivity
    {
        protected override void DoMessage(object message)
        {
            IsConDisplay(true);
            string msgstr = (string)message;
            int index = msgstr.IndexOf('|');
            if (index > -1)
            {
                string[] messagestr = msgstr.Split(new char[] { '|' });

                if (messagestr.Length == 2)
                {
                    GetElementById("Message1").InnerText = messagestr[0];
                    GetElementById("Message2").InnerText = messagestr[1];
                }
            }
            else
            {
                GetElementById("Message1").InnerText = msgstr;
                GetElementById("Message2").InnerText = " ";
            }
        }

        protected override void FrameReturnClick()
        {
            GotoMain();
        }

    }
}
