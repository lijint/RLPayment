using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Collections;
using Landi.FrameWorks;
using YAPayment.Package;

namespace YAPayment.Business.PetroPay
{
    class PetroPayUserInfoShowDeal : Activity,ITimeTick
    {
        PageManage pageManage = new PageManage();
        ArrayList aList = new ArrayList();
        private int nRow = -1;

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
            for (int i = 0; i < _list.Count;i++ )
            {
                string[] _str = _list[i].ToString().Split(',');
                InvokeScript("tableTrAdd", _str);
            }
            InvokeScript("tableFunction");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        void Confirm_Click(object sender, HtmlElementEventArgs e)
        {
            nRow = int.Parse(InvokeScript("tableGetClickTr").ToString());
            if (nRow < 0)
            {
                InvokeScript("tableShowErr", new object[] { "请选择其中一条数据进行交易!", "0" });
                return;
            }

            ArrayList _list = pageManage.GetPageConten(pageManage.nCurPage);
            YAPaymentPay.SelectRecordInfo = _list[nRow - 1].ToString().Split(',');
            StartActivity("中石油支付账单费用查询");
        }

        protected override void OnEnter()
        {
            GetElementById("UserName").InnerText = YAPaymentPay.UserName;
            GetElementById("UserSex").InnerText = YAPaymentPay.Sex;
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
            aList.Clear();
            aList.AddRange(YAPaymentPay.RecordInfo.Split(':'));
            pageManage.SetContent(aList);
        }

        public void OnTimeTick(int count)
        {
            if (count == 1)
            {
                GetElementById("Proc").Style = "visibility:hidden";
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
                GetElementById("Ok").Style = "display:block;";
                GetElementById("Ok").Click += new HtmlElementEventHandler(Confirm_Click);

                ArrayList firstPage = pageManage.GetPageConten(1);
                SetPageContent(firstPage);
            }
        }
    }
}
