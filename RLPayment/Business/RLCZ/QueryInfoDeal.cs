using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RLPayment.Business.RLCZ
{
    class QueryInfoDeal : FrameActivity
    {

        private RLCZEntity _entity;
        private int CurrentPage;
        private int TotalPage;
        private int MaxRow = 5;
        private int MaxColumns = 5;

        private string Amount;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;

                GetElementById("ok").Click += new HtmlElementEventHandler(OKClick);

                GetElementById("cardNo").InnerHtml = _entity.CardNO;            //卡号
                GetElementById("addr").InnerHtml = _entity.Addr;
                GetElementById("pastBalance").InnerHtml = _entity.PastBalance.ToString();
                GetElementById("userName").InnerHtml = _entity.UserName;
                if (_entity.CompanyCode == "01")
                {
                    GetElementById("companyName").InnerHtml = "济宁新东供热有限责任公司";
                }
                else if (_entity.CompanyCode == "02")
                {
                    GetElementById("companyName").InnerHtml = "济宁高新公用事业发展股份有限公司";
                }
                GetElementById("TotalArrears").InnerHtml = _entity.TotalArrears.ToString();
                Amount = _entity.TotalArrears.ToString();
                //Amount = "0.01";
                CurrentPage = 1;
                TotalPage = _entity.userInfoList.Count % MaxRow != 0 ? _entity.userInfoList.Count / MaxRow + 1 : _entity.userInfoList.Count / MaxRow;
                if (TotalPage > 1)
                {
                    GetElementById("nextpage").Style = "visibility: block; width: 100px; height: 50px;";
                    GetElementById("previouspage").Style = "visibility: block; width: 100px; height: 50px;";
                    GetElementById("nextpage").Click += new HtmlElementEventHandler(NextPageClick);
                    GetElementById("previouspage").Click += new HtmlElementEventHandler(PreviousPageClick);
                }
                else
                {
                    GetElementById("nextpage").Style = "visibility: hidden; width: 100px; height: 50px;";
                    GetElementById("previouspage").Style = "visibility: hidden; width: 100px; height: 50px;";
                }

                DisplayInfo();
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }

        private void PreviousPageClick(object sender, HtmlElementEventArgs e)
        {
            if (CurrentPage <= 1)
                return;
            CurrentPage--;
            DisplayInfo();
        }

        private void NextPageClick(object sender, HtmlElementEventArgs e)
        {
            if (CurrentPage >= TotalPage)
                return;
            CurrentPage++;
            DisplayInfo();
        }

        private void OKClick(object sender, HtmlElementEventArgs e)
        {
            if (!string.IsNullOrEmpty(Amount))
            {
                if (double.TryParse(Amount, out _entity.Amount))
                {
                    StartActivity("热力充值缴费方式选择");
                }
                else
                {
                    Log.Error("金额转换错误|金额中含有字符串");
                }
            }
        }

        protected override void FrameReturnClick()
        {
            StartActivity("热力充值输入热力号");
        }

        private void DisplayInfo()
        {
            ClearTable();

            GetElementById("pernums").InnerHtml = CurrentPage.ToString();
            GetElementById("pagenums").InnerHtml = TotalPage.ToString();
            if (_entity.userInfoList.Count <= 0)
                return;
            for (int i = 0, j = (CurrentPage - 1) * MaxRow; i < MaxRow && j < _entity.userInfoList.Count; i++, j++)
            {
                GetElementById("feetype" + i).InnerHtml = _entity.userInfoList[j].FeeType;
                GetElementById("HeatingPeriod" + i).InnerHtml = _entity.userInfoList[j].HeatingPeriod;
                GetElementById("area" + i).InnerHtml = _entity.userInfoList[j].Area.ToString();
                GetElementById("price" + i).InnerHtml = _entity.userInfoList[j].Price.ToString();
                GetElementById("ReceivableAmount" + i).InnerHtml = _entity.userInfoList[j].ReceivableAmount.ToString();
                GetElementById("AmountOwed" + i).InnerHtml = _entity.userInfoList[j].amountOwed.ToString();
            }
        }

        private void ClearTable()
        {
            for (int i = 0; i < MaxRow; i++)
            {
                GetElementById("feetype" + i).InnerHtml = "";
                GetElementById("HeatingPeriod" + i).InnerHtml = "";
                GetElementById("area" + i).InnerHtml = "";
                GetElementById("price" + i).InnerHtml = "";
                GetElementById("ReceivableAmount" + i).InnerHtml = "";
                GetElementById("AmountOwed" + i).InnerHtml = "";
            }
        }
    }
}
