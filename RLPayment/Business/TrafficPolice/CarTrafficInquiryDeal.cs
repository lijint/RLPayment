using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;
using YAPayment.Package.TrafficPolice;

namespace YAPayment.Business.TrafficPolice
{
    class CarTrafficInquiryDeal : Activity
    {
        //private YAEntity _entity = null;

        protected override void OnEnter()
        {
            DestroySelf(); //设置成自动销毁，每次重新生成
            //_entity = GetBusinessEntity() as YAEntity;

            TransResult res = TransResult.E_RECV_FAIL;
            CYATrafficInquiry inquiry = new CYATrafficInquiry();
            res = SyncTransaction(inquiry);
            if (res == TransResult.E_SUCC)
            {
                StartActivity("雅安交警认罚信息");
            }
            else if (res == TransResult.E_HOST_FAIL)
            {
                ShowMessageAndGotoMain(inquiry.ReturnCode + "-" + inquiry.ReturnMessage);
            }
            else if (res == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("交易超时，请重试");
            }
            else
            {
                ShowMessageAndGotoMain("交易失败，请重试");
            }
        }
    }
}
