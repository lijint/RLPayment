using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RLPayment.Business.RLCZ
{
    class InputRLCardNoDeal : FrameActivity
    {
        private RLCZEntity _entity;
        private string cardNum;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;

                _entity.LoadConfig();
                IsConDisplay(true);
                
                GetElementById("ok").Click += new HtmlElementEventHandler(OKClick);
                GetElementById("nums").Focus();
            }
            catch(Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }

        private void OKClick(object sender, HtmlElementEventArgs e)
        {
            cardNum = GetElementById("nums").GetAttribute("value");
            if (string.IsNullOrEmpty(cardNum))
                return;
            _entity.CardNO = cardNum;
            StartActivity("热力充值正在查询");
        }

        protected override void FrameReturnClick()
        {
            GotoMain();
        }
        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("nums").Focus();
            base.OnKeyDown(keyCode);
        }
    }
}
