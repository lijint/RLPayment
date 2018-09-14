using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Landi.FrameWorks
{
    public abstract class ManagePageActivity : Activity, IHandleMessage
    {
        protected enum Result
        {
            Again,  //需重试
            Success,    //读卡成功
            HardwareError,  //硬件故障
            Fail,   //读卡失败
            Cancel, //用户取消
            TimeOut,    //输入超时
        }

        private string mManageEntryId;
        private const int MANAGE_ENTRY = 1;

        protected ManagePageActivity()
        {
            mManageEntryId = ManageEntryId;
            if (mManageEntryId == null || GetElementById(mManageEntryId) == null)
                throw new Exception("错误的管理界面进入ID");
        }

        protected override void OnEnter()
        {
            GetElementById(mManagePageId).Click += new System.Windows.Forms.HtmlElementEventHandler(Back_Click);
            SendMessage(MANAGE_ENTRY);
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            UserToQuit = true;
        }

        private static void DoRead(LoopReadActivity ac)
        {
            Result ret = Result.Again;
            while (ret == Result.Again)
            {
                if (UserToQuit)
                {
                    ret = Result.Cancel;
                    break;
                }
                else if (TimeIsOut)
                {
                    ret = Result.TimeOut;
                    break;
                }
                ret = ac.ReadOnce();
            }
            if (ret == Result.Success || ret == Result.Cancel || ret == Result.TimeOut)
                Log.Debug("ReadCard : " + ret.ToString());
            else if (ret == Result.Fail)
                Log.Warn("ReadCard : " + ret.ToString());
            else
                Log.Error("ReadCard : " + ret.ToString());
            ac.HandleResult(ret);
        }

        /// <summary>
        /// 返回按钮的Id
        /// </summary>
        protected abstract string ManageEntryId
        {
            get;
        }

        /// <summary>
        /// 根据参数实现界面跳转
        /// </summary>
        /// <param name="result"></param>
        protected abstract void HandleResult(Result result);

        protected abstract Result ReadOnce();

        protected Result DefaultRead()
        {
            string trk1 = "", trk2 = "", trk3 = "";
            CardReader.Status ret = CardReader.InsertCard(ref trk1, ref trk2, ref trk3);
            if (ret == CardReader.Status.CARD_SUCC)
            {
                CommonData.BIsCardIn = true;//有卡进入
                if (trk1.Trim() == "")
                    Log.Debug("Track1 : NULL");
                else
                    Log.Debug("Track1 : " + trk1);
                if (trk2.Trim() == "")
                    Log.Debug("Track2 : NULL");
                else
                    Log.Debug("Track2 : " + trk2);
                if (trk3.Trim() == "")
                    Log.Debug("Track3 : NULL");
                else
                    Log.Debug("Track3 : " + trk3);

                Track1 = trk1;
                Track2 = trk2;
                Track3 = trk3;
                string CardNumber = Utility.GetCardNumber(trk2, trk3);
                BankCardNum = CardNumber;
                if (CardNumber.Trim().Length > 0)
                {
                    return Result.Success;
                }
                else
                {
                    return Result.Fail;
                }
            }
            else if (ret == CardReader.Status.CARD_WAIT)
            {
                return Result.Again;
            }
            else
            {
                Log.Error("ReaderOnce:" + ret.ToString());
                return Result.HardwareError;
            }
        }

        /// <summary>
        /// 磁条卡优先，兼容IC卡
        /// </summary>
        /// <returns></returns>
        protected Result DefaultRead3()
        {
            Result ret = DefaultRead();
            if ((ret == Result.Fail) && GlobalAppData.GetInstance().UseICCard)
            {
                //读卡失败，如果有启动IC，则进入IC卡读卡模式
                ReportSync("none");
                CardReader.CardType(0, 0);
                EMVTransProcess emv = new EMVTransProcess();
                int state = emv.EMVTransInit(0, EMVTransProcess.PbocTransType.PURCHASE);
                CardReader.CardPowerDown();
                if (state == 0)
                {
                    SaveBoolean("UseICCard", true);//交易变量
                    BankCardNum = emv.EMVInfo.CardNum;
                    Track2 = emv.EMVInfo.Track2;
                    Log.Debug("IC Card In");
                    return Result.Success;
                }
            }

            return ret;
        }


        /// <summary>
        /// IC卡优先，兼容磁条卡，不降级处理
        /// </summary>
        /// <returns></returns>
        protected Result DefaultRead4()
        {
            bool isIcCard = true;
            int nCardType = 0;
            string trk1 = "", trk2 = "", trk3 = "";
            CardReader.Status ret = CardReader.InsertCard(ref trk1, ref trk2, ref trk3);
            if (ret == CardReader.Status.CARD_SUCC)
            {
                CommonData.BIsCardIn = true;//有卡进入
                //读卡模式隐藏按钮
                ReportSync("none");
                Log.Debug("Track1:" + trk1);
                Log.Debug("Track2 : " + trk2);
                Log.Debug("Track3 : " + trk3);
                string CardNumber = Utility.GetCardNumber(trk2, trk3);
                if (CardNumber.Trim().Length > 0)
                {
                    Log.Info("MS Card Deal");
                    Track1 = trk1;
                    Track2 = trk2;
                    Track3 = trk3;
                    BankCardNum = CardNumber;
                    ExpDate = Utility.GetExpDate(trk2, trk3);
                    isIcCard = Utility.CheckIcCardFlag(trk2);
                    nCardType = 1;
                }

                if (isIcCard && GlobalAppData.GetInstance().UseICCard)
                {
                    Log.Info("IC Card Deal");
                    CardReader.CardType(0, 0);//防止读卡器问题类型问题
                    EMVTransProcess emv = new EMVTransProcess();
                    int state = emv.EMVTransInit(0, EMVTransProcess.PbocTransType.PURCHASE);
                    CardReader.CardPowerDown();
                    if (state == 0)
                    {
                        BankCardNum = emv.EMVInfo.CardNum;
                        Track2 = emv.EMVInfo.Track2;
                        ExpDate = emv.EMVInfo.CardExpDate;
                        CardSeqNum = emv.EMVInfo.CardSeqNum;
                        nCardType += 2;
                    }
                }

                BankCardType = (UserBankCardType)nCardType;

                if (isIcCard && nCardType < 2)//不降级处理
                    return Result.Fail;

                if (BankCardType == UserBankCardType.None)
                    return Result.Fail;
            }
            else if (ret == CardReader.Status.CARD_WAIT)
            {
                return Result.Again;
            }
            else
            {
                Log.Error("ReaderOnce:" + ret.ToString());
                return Result.HardwareError;
            }

            return Result.Success;
        }

        protected Result DefaultRead2()
        {
            string trk1 = "", trk2 = "", trk3 = "";
            CardReader2.Status ret = CardReader2.InsertCard(ref trk1, ref trk2, ref trk3);
            if (ret == CardReader2.Status.CARD_SUCC)
            {
                if (trk1.Trim() == "")
                    Log.Debug("Track1 : NULL");
                else
                    Log.Debug("Track1 : " + trk1);
                if (trk2.Trim() == "")
                    Log.Debug("Track2 : NULL");
                else
                    Log.Debug("Track2 : " + trk2);
                if (trk3.Trim() == "")
                    Log.Debug("Track3 : NULL");
                else
                    Log.Debug("Track3 : " + trk3);

                Track1 = trk1;
                Track2 = trk2;
                Track3 = trk3;
                string CardNumber = Utility.GetCardNumber(trk2, trk3);
                BankCardNum = CardNumber;
                if (CardNumber.Trim().Length > 0)
                {
                    return Result.Success;
                }
                else
                {
                    return Result.Fail;
                }
            }
            else if (ret == CardReader2.Status.CARD_WAIT)
            {
                return Result.Again;
            }
            else
            {
                Log.Error("Reader2Once:" + ret.ToString());
                return Result.HardwareError;
            }
        }

        protected sealed override void OnTimeOut()
        {

        }

        protected override void OnReport(object progress)
        {
            string msg = (string)progress;
            switch (msg)
            {
                case "none":
                    {
                        GetElementById(mManagePageId).Style = "display:none";
                        break;
                    }
            }
        }

        public void HandleMessage(Message message)
        {
            if (message.what == MANAGE_ENTRY)
            {
                DoRead(this);
            }
        }
    }
}
