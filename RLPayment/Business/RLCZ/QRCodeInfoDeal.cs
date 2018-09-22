using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLPayment.Business.RLCZ
{
    class QRCodeInfoDeal : FrameActivity
    {
        private RLCZEntity _entity;

        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;

                if (_entity.PayType == 1)
                {
                    GetElementById("tbText").InnerHtml = "微信";
                }
                else if (_entity.PayType == 2)
                {
                    GetElementById("tbText").InnerHtml = "支付宝";
                }
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }
        protected override void FrameReturnClick()
        {
            StartActivity("热力充值缴费方式选择");
        }
    }
}
