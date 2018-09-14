using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;
using System.Collections;
using YAPayment.Package;

namespace YAPayment.Business.PetroPay
{
    public class PetroPayBillShowDeal : Activity,ITimeTick
    {
        PageManage pageManage = new PageManage();
        ArrayList aList = new ArrayList();

        void Up_Click(object sender, HtmlElementEventArgs e)
        {
            ArrayList downPage = pageManage.OnPrePage();
            GetElementById("Down").Style = "display:block;";
            if (pageManage.nCurPage == 1)//第一页
            {
                GetElementById("Up").Style = "display:none;";
            }
            GetElementById("PageNum").InnerText = pageManage.nCurPage.ToString() + "/" + pageManage.nTotalPage.ToString();
            SetPageContent(downPage);
        }

        void Down_Click(object sender, HtmlElementEventArgs e)
        {
            ArrayList downPage = pageManage.OnNextPage();
            GetElementById("Up").Style = "display:block;";
            if (pageManage.nCurPage == pageManage.nTotalPage)//最后一页
            {
                GetElementById("Down").Style = "display:none;";
            }
            GetElementById("PageNum").InnerText = pageManage.nCurPage.ToString() + "/" + pageManage.nTotalPage.ToString();
            SetPageContent(downPage);
        }

        void SetPageContent(ArrayList _list)
        {
            InvokeScript("tableTrRemove");
            for (int i = 0; i < _list.Count; i++)
            {
                string[] _str = _list[i].ToString().Split(',');
                _str[1] = double.Parse(_str[1]).ToString("######0.00");
                InvokeScript("tablePayTrAdd", _str);
            }
            //InvokeScript("tableFunction");
        }

        void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("中石油支付用户信息显示");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        void Confirm_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("中石油支付确认信息");
        }

        public void OnTimeTick(int count)
        {
            if (count == 1)
            {
                GetElementById("Proc").Style = "visibility:hidden";
               GetElementById("Back").Style = "display:block;";

                if (aList.Count == 0)
                {
                    InvokeScript("tableShowErr", new object[] { "没有数据", "1" });//1错误信息不消失
                    return;
                }
                if (pageManage.nTotalPage > 1)
                {
                    GetElementById("PageInfo").Style = "display:block;";
                    GetElementById("Up").Click += new HtmlElementEventHandler(Up_Click);
                    GetElementById("Down").Click += new HtmlElementEventHandler(Down_Click);
                    GetElementById("Up").Style = "display:none;";
                    GetElementById("Down").Style = "display:block;";
                    GetElementById("PageNum").InnerText = pageManage.nCurPage.ToString() + "/" + pageManage.nTotalPage.ToString();
                }

                GetElementById("PayAmount").InnerText = CommonData.Amount.ToString("######0.00");
                ArrayList firstPage = pageManage.GetPageConten(1);
                SetPageContent(firstPage);

                if (CommonData.Amount != 0)
                {
                    GetElementById("Ok").Style = "display:block;";
                    GetElementById("Ok").Click += new HtmlElementEventHandler(Confirm_Click);
                }

                GetElementById("Back").Click += new HtmlElementEventHandler(Back_Click);

            }
        }

        protected override void OnEnter()
        {
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
            aList.Clear();
            aList.AddRange(YAPaymentPay.ListQueryInfo);
            pageManage.SetContent(aList);
        }
    }
}
