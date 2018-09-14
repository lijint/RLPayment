using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks.HardWare;
using Landi.FrameWorks;

namespace Landi.FrameWorks
{
    public abstract class LoopReadActivity : Activity, IHandleMessage
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

        /// <summary>
        /// 是否启用IC卡
        /// </summary>
        private bool mIsUseICCard
        {
            get { return GetBusinessEntity().UseICCard; }
        }

        private string mBackId;
        protected string Track1;
        protected string Track2;
        protected string Track3;
        protected string BankCardNum;
        protected string ExpDate;
        protected string CardSeqNum;
        protected UserBankCardType BankCardType;
        private const int READ_CARD = 1;
        

        protected LoopReadActivity()
        {
            mBackId = ReturnId;
            if (mBackId == null || GetElementById(mBackId) == null)
                throw new Exception("错误的返回键ID");
        }

        protected override void OnEnter()
        {
            Track1 = "";
            Track2 = "";
            Track3 = "";
            BankCardNum = "";
            ExpDate = "";
            CardSeqNum = "";
            BankCardType = UserBankCardType.None;
            CommonData.BIsCardIn = false;
            //设置AID和CA文件前缀
            EMVTransProcess.EMVSetAidAndCAFileName(GetBusinessEntity().SectionName);
            GetElementById(mBackId).Click += new System.Windows.Forms.HtmlElementEventHandler(Back_Click);
            SendMessage(READ_CARD);
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
        protected abstract string ReturnId
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
                if(CardReader.IsUse)
                    CommonData.BIsCardIn = true;//有卡进入
                if (trk1.Trim() == "")
                    Log.Debug("Track1 : NULL");
                else
                    Log.Debug("Track1 : IN");
                if (trk2.Trim() == "")
                    Log.Debug("Track2 : NULL");
                else
                    Log.Debug("Track2 : IN");
                if (trk3.Trim() == "")
                    Log.Debug("Track3 : NULL");
                else
                    Log.Debug("Track3 : IN");

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
            if ((ret == Result.Fail) && mIsUseICCard)
            {
                //读卡失败，如果有启动IC，则进入IC卡读卡模式
                ReportSync("none");
                EMVTransProcess emv = new EMVTransProcess();
                int state = emv.EMVTransInit(0, EMVTransProcess.PbocTransType.PURCHASE);
                CardReader.CardPowerDown();
                if (state == 0)
                {
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
                if (CardReader.IsUse)
                    CommonData.BIsCardIn = true;//有卡进入
                //读卡模式隐藏按钮
                ReportSync("none");
                //Log.Debug("Track1:" + trk1);
                //Log.Debug("Track2 : " + trk2);
                //Log.Debug("Track3 : " + trk3);
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
                    Log.Info("isIcCard=" + isIcCard.ToString());
                    //isIcCard = true;//可能存在二磁道 ic卡判断错误的情况 所以IC卡都需要验证以防 45错误
                    nCardType = 1;
                }

                if (!CardReader.IsUse)
                {
                    isIcCard = false;
                }

                if (isIcCard && mIsUseICCard)
                {
                    //CardReader.CardType(0, 0);//防止读卡器问题类型问题
                    EMVTransProcess emv = new EMVTransProcess();
                    int state = emv.EMVTransInit(0, EMVTransProcess.PbocTransType.PURCHASE);
                    CardReader.CardPowerDown();
                    if (state == 0)
                    {
                        Log.Info("IC Card Deal");
                        BankCardNum = emv.EMVInfo.CardNum;
                        Track2 = emv.EMVInfo.Track2;
                        ExpDate = emv.EMVInfo.CardExpDate;
                        CardSeqNum = emv.EMVInfo.CardSeqNum;
                        nCardType += 2;
                    }
                    //else
                    //    isIcCard = false;//防止纯磁条卡无法交易
                }

                BankCardType = (UserBankCardType)nCardType;

                if (mIsUseICCard && isIcCard && nCardType < 2)//开启IC卡，不降级处理
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

        /// <summary>
        /// 插入带芯片卡
        /// </summary>
        /// <returns></returns>
        protected Result InsertICCard()
        {
            CardReader.Status ret = CardReader.Insert_ICcard();
            if (ret != CardReader.Status.CARD_WAIT)
                CommonData.BIsCardIn = true;

            switch (ret)
            {
                case CardReader.Status.CARD_WAIT:
                    {
                        return Result.Again;
                    }
                case CardReader.Status.CARD_SUCC:
                    {
                        ReportSync("none");
                        return Result.Success;
                    }
                default:
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
                        GetElementById(mBackId).Style = "display:none";
                        break;
                    }
            }
        }

        public void HandleMessage(Message message)
        {
            if (message.what == READ_CARD)
            {
                DoRead(this);
            }
        }
    }
}
