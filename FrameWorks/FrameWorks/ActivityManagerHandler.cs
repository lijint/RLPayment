using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks
{
    internal class ActivityManagerHandler : Handler
    {
        public ActivityManager Proxy;

        public const int LAUNCH_ACTIVITY = 1;
        public const int LEAVE_ACTIVITY_COMPLETE = 2;
        public const int SET_VIEW_COMPLETE = 3;
        public int NextCommand = LAUNCH_ACTIVITY;

        public ActivityManagerHandler(Looper looper)
            : base(looper)
        {

        }

        public ActivityManagerHandler()
            : base()
        {

        }

        protected override void HandleMessage(Message msg)
        {
            if (msg.what == NextCommand)
            {
                switch (msg.what)
                {
                    case LAUNCH_ACTIVITY:
                        if (Proxy.LeaveCurrentActivity((Intent)msg.obj))
                            NextCommand = LEAVE_ACTIVITY_COMPLETE;
                        break;
                    case LEAVE_ACTIVITY_COMPLETE:
                        Proxy.SetContentView();
                        NextCommand = SET_VIEW_COMPLETE;
                        break;
                    case SET_VIEW_COMPLETE:
                        Proxy.Enter();
                        NextCommand = LAUNCH_ACTIVITY;
                        break;
                    default:
                        return;
                }
            }
        }

        internal string codeToString(int code)
        {
            switch (code)
            {
                case LAUNCH_ACTIVITY: return "LAUNCH_ACTIVITY";
                case SET_VIEW_COMPLETE: return "SET_VIEW_COMPLETE";
                case LEAVE_ACTIVITY_COMPLETE: return "LEAVE_ACTIVITY_COMPLETE";
            }
            return "(unknown)";
        }

    }
}
