using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.YAPublishPay
{
    class YAPublishPayInputUserNoDeal :Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
            GetElementById("UserID").LostFocus += YAPublishPayInputUserNoDeal_LostFocus;
            GetElementById("UserID").Focus();
            GetElementById("UserID").Focus();
        }

        void YAPublishPayInputUserNoDeal_LostFocus(object sender, HtmlElementEventArgs e)
        {
            GetElementById("ErrMsg").Style = "display:none";
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string userName = GetElementById("UserID").GetAttribute("value").Trim();
            if (userName.Length == 0)
            {
                GetElementById("ErrMsg").Style = "display:block";
                return;
            }

#if DEBUG
            if ((GetBusinessEntity() as YAEntity).PublishPayType == YaPublishPayType.TV)
            {
                userName = "000214510687";
                //000246050181 000214510687 000272833377
            }

            if ((GetBusinessEntity() as YAEntity).PublishPayType == YaPublishPayType.Water)
            {
                userName = "00101986";
                //1	00101986 //2	00101986//3	00102043//4	00102043//5	00102043//6	00102043//7	00102065//8	00102065
                //9	00102065//10	00102065//11	00102065//12	00102065//13	00102065//14	00102065//15	00102065
                //16	00102065//17	00102065//18	00102065//19	00102559//20	00102559//21	00102559//22	00102559
                //23	00102559//24	00102559//25	00102794//00102730//2	00102799//3	00102623//4	00101536//5	00101096//6	00101127
                //7	00101130//8	00101133//9	00101136//10	00102735
            }
            //ú��
            // 00132  00140 02555 04889 04897
            // 04911  04942 04959 05497 06821
            // 12564 14208 14240 18391 21416
            // 22365 22428 24080
#endif

            (GetBusinessEntity() as YAEntity).UserID = userName;
            StartActivity("雅安支付账单查询");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            InputNumber("UserID", keyCode);
            base.OnKeyDown(keyCode);
        }
    }
}
