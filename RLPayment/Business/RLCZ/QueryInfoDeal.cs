using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RLPayment.Business.RLCZ
{
    class QueryInfoDeal : FrameActivity
    {

        private RLCZEntity _entity;
        private string Amount;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;
                Amount = "0.01";
                GetElementById("ok").Click += new HtmlElementEventHandler(OKClick);
            }
            catch(Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }

        private void OKClick(object sender, HtmlElementEventArgs e)
        {
            if (!string.IsNullOrEmpty(Amount))
            {
                if (double.TryParse(Amount, out _entity.Amount))
                {
                    StartActivity("热力充值缴费方式选择");
                }
                else
                {
                    Log.Error("金额转换错误|金额中含有字符串");
                }
            }
        }

        protected override void FrameReturnClick()
        {
            StartActivity("热力充值输入热力号");
        }
    }
}
