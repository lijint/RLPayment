using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.CarTicket
{
    class TicketInfoDeal : Activity
    {
        private CarEntity _carEntity;
        private List<TicketLines> _ticketLineList;
        private int _pageCount;
        private int _currPage = 1;
        private bool _isClick = false;
        protected override void OnEnter()
        {
            GetElementById("Return").Click += Cancle_Click;
            GetElementById("nPage").Click += NextPage_Click;
            GetElementById("aPage").Click += PrePage_Click;
            _carEntity = GetBusinessEntity<CarEntity>();
            _ticketLineList = _carEntity._BstTicketByCityResponse.TicketLinesQuery;
            _pageCount = _ticketLineList.Count/8 + _ticketLineList.Count%8 != 0 ? 1 : 0;
            InvokeScript("hideElement", new object[] { "aPage" });
            if (_pageCount==1)
                InvokeScript("hideElement", new object[] { "nPage" });
            GetElementById("pagenums").InnerText = _pageCount.ToString();
            GetElementById("pernums").InnerText = _currPage.ToString();
            InitPageData();
        }

        void Cancle_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("车票预订主画面");
        }

        private void NextPage_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            try
            {
                if (_isClick) return;
                _isClick = true;
                _currPage++;
                InvokeScript("showElement", new object[] { "aPage" });
                if (_currPage >= _pageCount)
                    InvokeScript("hideElement", new object[] {"nPage"});
                InitPageData();
                GetElementById("pernums").InnerText = _currPage.ToString();
            }
            finally
            {
                _isClick = false;
            }
        }

        private void PrePage_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            try
            {
                if (_isClick) return;
                _isClick = true;
                _currPage--;
                InvokeScript("showElement", new object[] { "nPage" });
                if (_currPage <= 1)
                    InvokeScript("hideElement", new object[] {"aPage"});
                InitPageData();
                GetElementById("pernums").InnerText = _currPage.ToString();
            }
            finally
            {
                _isClick = false;
            }
        }

        private void InitPageData()
        {
            SetEleVioIndex(0);
            SetEleVioIndex(1);
            SetEleVioIndex(2);
            SetEleVioIndex(3);
            SetEleVioIndex(4);
            SetEleVioIndex(5);
            SetEleVioIndex(6);
            SetEleVioIndex(7);
            for (int i = 0; i < 8; i++)
            {
                GetElementById(i.ToString()).Click += TicketInfo_Click;
            }
        }

        void TicketInfo_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            HtmlElement ele = sender as HtmlElement;
            if (ele == null) return;
            int id =int.Parse(ele.Id);
            int row = (_currPage - 1) * 8 + id;

            _carEntity.SelectLine = _ticketLineList[row];
            StartActivity("乘客信息");
        }


        private void SetEleVioIndex(int index)
        {
            int row = (_currPage - 1)*8 + index;
            if (row >= _ticketLineList.Count)
            {
                string format = string.Format("msg{0}-", index);
                GetElementById(format + "0").InnerText = "";
                GetElementById(format + "1").InnerText = "";
                GetElementById(format + "2").InnerText = "";
                GetElementById(format + "3").InnerText = "";
                GetElementById(format + "4").InnerText = "";
                GetElementById(format + "5").InnerText = "";
            }
            else
            {
                TicketLines line = _ticketLineList[row];
                string format = string.Format("msg{0}-", index);
                GetElementById(format + "0").InnerText = line.CarryStaName;
                GetElementById(format + "1").InnerText = line.City;
                GetElementById(format + "2").InnerText = line.DrvDateTime;
                GetElementById(format + "3").InnerText = line.FullPrice;
                GetElementById(format + "4").InnerText = line.EndStaName;
                GetElementById(format + "5").InnerText = line.Amount;
            }
        }

        private void SetPageIndex()
        {
            GetElementById("pernums").InnerText = _currPage.ToString();
            GetElementById("pagenums").InnerText = _pageCount.ToString();
        }
    }
}
