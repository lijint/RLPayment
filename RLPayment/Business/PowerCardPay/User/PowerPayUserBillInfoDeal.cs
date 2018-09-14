using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;
using System.Windows.Forms;
using YAPayment.Package.PowerCardPay;

namespace YAPayment.Business.PowerCardPay.User
{
    class PowerPayUserBillInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            PowerEntity entity = GetBusinessEntity() as PowerEntity;
            GetElementById("UserName").InnerText = entity.UserName;
            GetElementById("Address").InnerText = entity.UserAddress;
            GetElementById("UserID").InnerText = entity.UserID;
            GetElementById("PayAmount").InnerText = entity.UserPayAmount.ToString("######0.00") + "元";
            GetElementById("Balance").InnerText = entity.PayAmount.ToString("######0.00") + "元";
            RefreshTable(entity);

            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void RefreshTable(PowerEntity entity)
        {
            if (entity.UserQueryInfo==null) return;
            for (int i = 0; i < entity.UserQueryInfo.Count; i++)
            {
                UserQueryInfo info = entity.UserQueryInfo[i];
                GetElementById("index" + i).InnerText = info.GetCorrectValue(UserQueryInfo.QueryValueType.Index);
                GetElementById("month" + i).InnerText = info.GetCorrectValue(UserQueryInfo.QueryValueType.Date);
                GetElementById("pay" + i).InnerText = info.GetCorrectValue(UserQueryInfo.QueryValueType.Pay);
                GetElementById("dedit" + i).InnerText = info.GetCorrectValue(UserQueryInfo.QueryValueType.Dedit);
                GetElementById("message" + i).InnerText = info.GetCorrectValue(UserQueryInfo.QueryValueType.Message) +" ";

                GetElementById("r" + i).Style = "display:block;";
            }
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            CommonData.Amount = (GetBusinessEntity() as PowerEntity).UserPayAmount;
            StartActivity("电力支付用户金额确认");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
