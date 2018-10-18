using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalLib;

namespace RLPayment.Business.RLCZ
{
    class BeingQueryQRCodeDeal : FrameActivity
    {
        private RLCZEntity _entity;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;
                RequestData _request = new RequestData();
                Random r = new Random();

                switch (_entity.PayType)
                {
                    case 1:
                        //微信

                        /// 消费金额
                        _request.Amount = _entity.Amount;
                        _request.OrderSubject = "微信扫码支付";

                        /// 商户流水
                        int iAppLogId = r.Next(10000, 99999);
                        _request.AppTransLogId = iAppLogId.ToString().PadRight(16, '0');

                        _request.AttachField.Add("PayChannel", "WEIXIN");
                        _request.AttachField.Add("PayWay", "SCAN_PAY");
                        //_request.AttachField.Add("PayWay", "QRCODE_PAY");

                        Global.gTerminalPay.BusinessLib = String.Format("{0}.PayService", Global.gWFTBankCardLibName);
                        Global.gTerminalPay.RequestEntity = _request;
                        Global.gTerminalPay.Pay(_request);

                        break;
                    case 2:
                        //支付宝

                        /// 消费金额
                        _request.Amount = _entity.Amount;
                        _request.OrderSubject = "支付宝扫码支付";

                        /// 商户流水
                        iAppLogId = r.Next(10000, 99999);
                        _request.AppTransLogId = iAppLogId.ToString().PadRight(16, '0');

                        _request.AttachField.Add("PayChannel", "ALIPAY");
                        _request.AttachField.Add("PayWay", "SCAN_PAY");
                        //_request.AttachField.Add("PayWay", "QRCODE_PAY");

                        Global.gTerminalPay.BusinessLib = String.Format("{0}.PayService", Global.gWFTBankCardLibName);
                        Global.gTerminalPay.RequestEntity = _request;
                        Global.gTerminalPay.Pay(_request);

                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }

        protected override void PreCreateSucc()
        {
            _entity.OrderNumber = Global.gTerminalPay.ResponseEntity.PosTraceNumber;
            StartActivity("热力充值二维码显示");
        }

        protected override void PayCallback(ResponseData ResponseEntity)
        {
            if (ResponseEntity.StepCode == "ProceduresEnd")
            {
                if (ResponseEntity.returnCode != "00")
                {
                    ShowMessageAndGotoMain("获取二维码失败|"+ ResponseEntity.args);
                }
            }
        }
    }
}
