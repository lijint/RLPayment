using Landi.FrameWorks;
using RLPayment.Entity;
using RLPayment.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalLib;
using static Landi.FrameWorks.Package.Other.SocketCommunicate;

namespace RLPayment.Business.RLCZ
{
    class BeingCspospTransDeal : FrameActivity
    {
        private RLCZEntity _entity;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;
                if (BeingProcess() == 0)
                {
                    RequestData _request = new RequestData();
                    Random r = new Random();

                    switch (_entity.PayType)
                    {
                        case 0:
                            //银行卡
                            _request.Amount = _entity.Amount;
                            Global.gTerminalPay.BusinessLib = String.Format("{0}.PayService", Global.gBankCardLibName);
                            Global.gTerminalPay.RequestEntity = _request;
                            Global.gTerminalPay.Pay(_request);
                            break;
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
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }

        }
        private int BeingProcess()
        {
            int ret = -1;
#if !DEBUG
            CSPospTrans cSPospTrans = new CSPospTrans(_entity);
            cSPospTrans.setIPAndPort(_entity.CspospServerIP, _entity.CspospServerPort);
            TransResult result = cSPospTrans.transact();
            if (result == TransResult.E_SUCC)
            {
                ret = 0;
            }
            else
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err " + "respcode : " + cSPospTrans.respcode + " respmsg : " + cSPospTrans.respmsg + "RETURNCODE:" + cSPospTrans.RETURNCODE + " MESSAGE" + cSPospTrans.MESSAGE);
                ShowMessageAndGotoMain("预通知失败|" + cSPospTrans.respcode + " == " + cSPospTrans.RETURNCODE);
            }
#else
            ret = 0;
#endif
            return ret;
        }

        protected override void InsertCardStart()
        {
            StartActivity("热力充值插入银行卡");
        }

        protected override void PreCreateSucc()
        {
            StartActivity("热力充值二维码显示");
        }

    }
}
