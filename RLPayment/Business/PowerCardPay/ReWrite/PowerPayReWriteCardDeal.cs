using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;
using YAPayment.Package;
using YAPayment.Package.PowerCardPay;

namespace YAPayment.Business.PowerCardPay
{
    internal class PowerPayReWriteCardDeal : Activity
    {
        private PowerEntity m_entity = null;
        private TransResult _transResult;
        protected override void OnEnter()
        {
            DestroySelf(); //设置成自动销毁，每次重新生成

            m_entity = GetBusinessEntity() as PowerEntity;

            if (SyncTransaction(new CReverse_PowerPay()) == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("交易超时，请重试");
                return;
            }
            //ReportSync("PowerReadCard");
            _transResult= TransResult.E_RECV_FAIL;
            PostSync(ReWrite);
            if (_transResult != TransResult.E_SUCC)
            {
                ShowMessageAndGotoMain("补写卡交易失败，请重试！");
                return;
            }
            StartActivity("电力支付交易完成");
        }

        private void ReWrite()
        {
            CPowerCardWriteAgain writeAgain = new CPowerCardWriteAgain();
            _transResult = writeAgain.WritePowerCardAgain();
            Log.Debug("补写卡交易返回：" + writeAgain.ReturnCode);
        }
    }
}
