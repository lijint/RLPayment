using Landi.FrameWorks;

namespace RLPayment.Business
{
    class StopServiceDeal : Activity,ITimeTick
    {
        private const int timeout = 30;
        public static string Message;

        protected override void OnEnter()
        {
            string msg = (string)MyIntent.GetExtra("Message");
            if (string.IsNullOrEmpty(msg))
                msg = Message;
            GetElementById("Message1").InnerText = msg;
            GetElementById("Message2").InnerText = "请联系工作人员！";

            //GetElementById("MessageSub").InnerText = msg;
            //GetElementById("MessageSub").Style = "font-size:25px; visibility:visible";
        }

        public override bool CanQuit()
        {
            return true;
        }

        public void OnTimeTick(int count)
        {
            if (count % timeout == 0)
            {
                //if (!InitializeDeal.Initialized)
                    StartActivity("初始化");
                //else
                //{
                //    HardwareManager.CheckAll();
                //    if (GPRS.ExistError())
                //        return;
                //    if (!CardReader.ExistError() && !Esam.ExistError())
                //            GotoMain();
                //}
            }
        }
    }
}
