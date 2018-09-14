using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;
using YAPayment.Package.TrafficPolice;

namespace YAPayment.Business.TrafficPolice
{
    class TrafficInfoMainDeal : PrinterActivity
    {
        private YAEntity _entity = null;
        private bool _goToMain = true;
        private Dictionary<string, string> _dicMain;
        private Dictionary<string, string> _dicDetail;

        private string _index;//当前显示的明细序号

        protected override void OnEnter()
        {
            _entity = (YAEntity)GetBusinessEntity();
            _dicDetail = new Dictionary<string, string>();
            _dicMain = new Dictionary<string, string>();
            _dicMain.Add(_entity.CurrentIndex, _entity.InquiryInfo);

            GetElementById("page").InnerText = _entity.CurrentIndex;
            GetElementById("licensePlant").InnerText =  _entity.LicensePlant;
            ShowMainData();

            GetElementById("left").Click += new HtmlElementEventHandler(AboveDataClick);
            GetElementById("right").Click += new HtmlElementEventHandler(BehindDataClick);

            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("errMsgClose").Click += new HtmlElementEventHandler(Close_Click);
            GetElementById("MsgClose").Click += new HtmlElementEventHandler(Close_Click);
        }

        void Close_Click(object sender, HtmlElementEventArgs e)
        {
            HtmlElement ele = (HtmlElement) sender;
            if (ele.Id == "errMsgClose")
            {
                ShowErrMsg(false,"");
            }
            else if (ele.Id == "MsgClose")
            {
                ShowMsg(false, "");
                ShowOrHideDetail(true);
                ReLoadDetailData();
            }
        }
        private bool messageFlag=true;
        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            if (messageFlag&&!ReceiptPrinter.CheckedByManager())
            {
                ShowMsg(true, "打印机故障，不能打印决定书编号，如继续，请牢记编号！");
                messageFlag = false;
                return;
            }
            messageFlag = true;
            ShowOrHideDetail(false);
            ShowProcess(true);

            TransResult res = TransResult.E_RECV_FAIL;
            CYATrafficPunishConfirm punish = new CYATrafficPunishConfirm(_index,_entity.LicenseNo);
            res = SyncTransaction(punish);
            if (res == TransResult.E_SUCC)
            {
                ShowProcess(false);

                PrintReceipt(GetReceipt(punish.Jdsbh));
                ShowMsg(true, "认罚成功,决定书编号："+punish.Jdsbh);
            }
            else if (res == TransResult.E_HOST_FAIL)
            {
                ShowProcess(false);
                ShowErrMsg(true, punish.ReturnCode + "-" + punish.ReturnMessage);
            }
            else if (res == TransResult.E_RECV_FAIL)
            {
                ShowProcess(false);
                ShowErrMsg(true, "认罚超时，请重试");
            }
            else
            {
                ShowProcess(false);
                ShowErrMsg(true, "认罚失败，请重试");
            }
        }

        public ArrayList GetReceipt(string jdsbh)
        {
            string sTitle = "银联商务\"交警认罚凭条\"";
            int splitStringLen = Encoding.Default.GetByteCount("------------------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = "";
            if (iLeftLength > 0)
                sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);
            Lprint.Add("  ");
            Lprint.Add("   交易类型  :  交警认罚");
            Lprint.Add("   ---------------------------------------");
            Lprint.Add("   决定书编号:  " + jdsbh);
            return Lprint;
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            if (_goToMain)
               GotoMain();
            else
            {
                ShowOrHideDetail(false);
                ShowMsg(false,"");
                ShowErrMsg(false,"");
            }
        }

        void BehindDataClick(object sender, HtmlElementEventArgs e)
        {
            int index = int.Parse(_entity.CurrentIndex);
            index++;
            _entity.CurrentIndex = index.ToString();
            RefreshMainData();
            GetElementById("page").InnerText = _entity.CurrentIndex;
        }

        void AboveDataClick(object sender, HtmlElementEventArgs e)
        {
            int index = int.Parse(_entity.CurrentIndex);
            if (index == 1)
                return;
            else
            {
                index--;
                _entity.CurrentIndex = index.ToString();
            }
            RefreshMainData();
            GetElementById("page").InnerText = _entity.CurrentIndex;
        }

        private void RefreshMainData()
        {
            if (_dicMain.ContainsKey(_entity.CurrentIndex))
            {
                _entity.InquiryInfo = _dicMain[_entity.CurrentIndex];
                ShowMainData();
            }
            else
            {
                ShowProcess(true);
                TransResult res = TransResult.E_RECV_FAIL;
                CYATrafficInquiry inquiry = new CYATrafficInquiry();
                res = SyncTransaction(inquiry);
                if (res == TransResult.E_SUCC)
                {
                    _dicMain.Add(_entity.CurrentIndex, _entity.InquiryInfo);
                    ShowProcess(false);
                    ShowMainData();
                }
                else if (res == TransResult.E_HOST_FAIL)
                {
                    ShowProcess(false);
                    ShowErrMsg(true, inquiry.ReturnCode + "-" + inquiry.ReturnMessage);
                }
                else if (res == TransResult.E_RECV_FAIL)
                {
                    ShowProcess(false);
                    ShowErrMsg(true,"查询超时，请重试");
                }
                else
                {
                    ShowProcess(false);
                    ShowErrMsg(true,"查询失败，请重试");
                }
          
            }
        }
        /// <summary>
        /// 数据点击，显示明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainDataClick(object sender, HtmlElementEventArgs e)
        {
            ShowProcess(true);
            HtmlElement ele = (HtmlElement)sender;
            _index = ele.Name;//改变当前明细序号
            RefreshDetailData();
        }

        private void RefreshDetailData()
        {
            if (_dicDetail.ContainsKey(_index))
            {
                ShowProcess(false);
                ShowOrHideDetail(true);
                SetDetailData(_dicDetail[_index]);
            }
            else
            {
                CYATrafficDetailInquiry detail = new CYATrafficDetailInquiry(_index);
                TransResult res = SyncTransaction(detail);

                if (res == TransResult.E_SUCC)
                {
                    _dicDetail.Add(_index, detail.DetailInfo);
                    ShowProcess(false);
                    ShowOrHideDetail(true);
                    SetDetailData(detail.DetailInfo);
                }
                else if (res == TransResult.E_HOST_FAIL)
                {
                    ShowProcess(false);
                    ShowErrMsg(true, detail.ReturnCode + "-" + detail.ReturnMessage);
                }
                else if (res == TransResult.E_RECV_FAIL)
                {
                    ShowProcess(false);
                    ShowErrMsg(true, "查询超时，请重试");
                }
                else
                {
                    ShowProcess(false);
                    ShowErrMsg(true, "查询失败，请重试");
                }
            }
        }

        private void SetDetailData(string info)
        {
            string[] temp = info.Split(new char[] {'|'}, StringSplitOptions.None);
            GetElementById("tAddress").InnerText = temp[0] + " ";
            GetElementById("tTime").InnerText = temp[1] + " ";
            GetElementById("tTraID").InnerText = temp[2] + " ";
            GetElementById("tID").InnerText = temp[3] + " ";
            GetElementById("tTraNM").InnerText = temp[4] + " ";
            GetElementById("tContent").InnerText = temp[5] + " ";
            GetElementById("tFen").InnerText = temp[6] + " ";
            GetElementById("tAmt").InnerText = temp[7] + " ";
            switch (temp[8])
            {
                case "1":
                    GetElementById("tIndex").InnerText = "可处理";
                    BaseShowHide(true, "Ok");
                    break;
                case "0":
                    GetElementById("tIndex").InnerText = "不可处理";
                    BaseShowHide(false, "Ok");
                    break;
                default:
                    GetElementById("tIndex").InnerText = temp[8] + " ";
                    break;
            }
        }

        private void ReLoadDetailData()
        {
            _dicDetail.Remove(_index);
            RefreshDetailData();
        }

        /// <summary>
        /// 刷新主画面数据
        /// </summary>
        private void ShowMainData()
        {

            string table =
                "<table border=\"1\" cellpadding=\"2\" cellspacing=\"0\" class=\"userPay\" style=\"width: 877px;\"><tr><th style=\"width: 10%;text-align:center;\">序号</th><th style=\"width: 18%;text-align:center;\">违法时间</th><th style=\"width: 18%;text-align:center;\">罚款金额</th><th style=\"width: 10%;text-align:center;\">扣分</th><th style=\"text-align:center;\">违法地址</th></tr>";

            StringBuilder sb = new StringBuilder();

            string info = "序号1|违法时间|扣分|罚款金额|违法地址1违法地址1违法地址1违法地址1违法地址1违法地址1违法地址1&序号2|违法时间|扣分|罚款金额|违法地址2&序号3|违法时间|扣分|罚款金额|违法地址3&序号4|违法时间|扣分|罚款金额|违法地址1&序号5|违法时间|扣分|罚款金额|违法地址2&"; //_entity.InquiryInfo;
            if (_entity.CurrentIndex == "1")
                ShowLeftArrow(false);
            else
                ShowLeftArrow(true);

            string[] values = info.Split(new char[] {'&'}, StringSplitOptions.RemoveEmptyEntries);
                //序号|违法时间|扣分|罚款金额|违法地址&
            int count = 0;
            if (values.Length >= 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    sb.Append(SetMainData(i, values[i]));
                }
                ShowRightArrow(true);
                count = 5;
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                {
                    sb.Append(SetMainData(i, values[i]));
                }
                ShowRightArrow(false);
                count = values.Length;
            }

            table += sb.ToString() + "</table>";
            GetElementById("mainDataTable").InnerHtml = table;

            for (int i = 0; i < count; i++)
                GetElementById("r" + i).Click += new HtmlElementEventHandler(MainDataClick);
        }

        /// <summary>
        /// 设置查询数据
        /// </summary>
        /// <param name="i">游标</param>
        /// <param name="value">序号|违法时间|扣分|罚款金额|违法地址</param>
        private string SetMainData(int i,string value)
        {
            string[] temp = value.Split(new char[] { '|' }, StringSplitOptions.None);
            string result = string.Format("<tr id=\"r{0}\" name=\"{1}\" style=\"cursor:pointer;\" onmouseover=\"this.className='RowOver'\" onmouseout=\"this.className='RowOut'\"><td id=\"index{0}\">{1}</td><td id=\"month{0}\">{2}</td><td id=\"pay{0}\">{4}</td><td id=\"fen{0}\">{3}</td><td id=\"address{0}\">{5}</td></tr>",i.ToString(),temp[0],temp[1],temp[2],temp[3],temp[4].Length>20?temp[4].Substring(0,17)+"...":temp[4]);
           
            return result;
        }


        private void ShowOrHideDetail(bool isShow)
        {
            BaseShowHide(isShow, "detail");
            BaseShowHide(isShow, "shade");
            _goToMain = !isShow;
            if (!isShow)
                BaseShowHide(false, "Ok");
        }

        private void ShowProcess(bool isShow)
        {
            BaseShowHide(isShow, "shade");
            BaseShowHide(isShow, "process");
            BaseShowHide(!isShow, "Return");
        }

        private void ShowErrMsg(bool isShow, string message)
        {
            GetElementById("errMsg").InnerText = message;
            BaseShowHide(isShow, "errMsgShow");
            BaseShowHide(isShow, "shade");
            if (isShow)
            {
                BaseShowHide(true, "Return");
            }
            _goToMain = !isShow;
        }

        private void ShowMsg(bool isShow, string message)
        {
            GetElementById("Msg").InnerText = message;
            BaseShowHide(isShow, "MsgShow");
            BaseShowHide(isShow, "shade");
            BaseShowHide(!isShow, "Return");//提示信息
        }

        private void ShowLeftArrow(bool isShow)
        {
            BaseShowHide(isShow,"left");
        }
        private void ShowRightArrow(bool isShow)
        {
            BaseShowHide(isShow, "right");
        }
        private void BaseShowHide(bool isShow, string id)
        {
            if (isShow)
            {
                GetElementById(id).Style = GetElementById(id).Style.Replace("none", "block");
            }
            else
            {
                GetElementById(id).Style = GetElementById(id).Style.Replace("block", "none");
            }
        }

        protected override void HandleResult(PrinterActivity.Result result)
        {
            
        }
    }
}
