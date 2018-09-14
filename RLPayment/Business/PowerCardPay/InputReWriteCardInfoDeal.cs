using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using YAPayment.Entity;

namespace YAPayment.Business.PowerCardPay
{
    class InputReWriteCardInfoDeal : Activity
    {
        private PowerEntity _entity;
        protected override void OnEnter()
        {
            _entity = (PowerEntity)GetBusinessEntity();
            //GetElementById("Amount").InnerText = _entity.PayAmount.ToString();
            //GetElementById("Back").Click += new HtmlElementEventHandler(Back_Click);
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            try
            {
                string ConfirmCode = GetElementById("TxtConfirmCode").GetAttribute("value").Trim();
                string UserNo = GetElementById("TxtUserNo").GetAttribute("value").Trim();

                if (ConfirmCode.Length == 0)
                {
                    GetElementById("ErrMsg").Style = "display:block";
                    GetElementById("TxtConfirmCode").SetAttribute("value", "");
                    GetElementById("TxtUserNo").SetAttribute("value", "");

                    return;
                }
                _entity.PowerPayConfirmCode = ConfirmCode;
                _entity.PowerCardNo = UserNo;


                //_entity.PowerPayConfirmCode = "YLSW120200000925";
                //_entity.PowerCardNo = "";
                StartActivity("电力支付补写卡插入电卡");
            }
            catch (Exception ex)
            {
                Log.Error("input rewrite info err", ex);
            }

        }

        //private void Back_Click(object sender, HtmlElementEventArgs e)
        //{
        //    StartActivity("电力支付账单信息");
        //}

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("ErrMsg").Style = "display:none";
            //InputAmount("TxtConfirmCode", keyCode);
            //InputAmount("TxtUserNo", keyCode);

            base.OnKeyDown(keyCode);
        }
    }
}
