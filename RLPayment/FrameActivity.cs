using Landi.FrameWorks;
using RLPayment.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TerminalLib;

namespace RLPayment
{
    public class FrameActivity : FrameworkActivity
    {
        //private static string returnName;
        //private static string comName;

        Global.TransDelegate transDelegate;

        protected override void OnEnter()
        {
            try
            { 
                Global.gTerminalPay.MutliThread = true;
                transDelegate = Ini_ResponseEvent;
                Global.gTerminalPay.ResponseEvent -= new ResponseHandle(transDelegate);
                Global.gTerminalPay.ResponseEvent += new ResponseHandle(transDelegate);
                base.OnEnter();
                //if (GetElementById(returnName) != null)
                //{
                //    GetElementById(returnName).Click += new HtmlElementEventHandler(ReturnClick);
                //}

            }
            catch (Exception ex)
            {

            }
        }

        private void Ini_ResponseEvent(ResponseData ResponseEntity)
        {
            try
            {
                //打印步骤
                Log.Info(String.Format("{0}流程:{1}, 步骤:{2}, 返回:{3}, 描述:{4}", "", ResponseEntity.ProcedureCode, ResponseEntity.StepCode, ResponseEntity.returnCode, ResponseEntity.args));
                if (ResponseEntity.returnCode == "EE")
                {
                    EEError();
                    return;
                }

                DefaultStepCodeDealUi(ResponseEntity);
                switch (ResponseEntity.ProcedureCode)
                {
                    case "Initialization":
                        //1,初始化
                        InitCallback(ResponseEntity);
                        break;
                    case "SignIn":
                        //签到
                        SignInCallback(ResponseEntity);
                        break;
                    case "BankCardPay":
                        //消费
                        PayCallback(ResponseEntity);
                        break;
                    case "BalanceQuery":
                        //余额
                        BalanceQueryCallback(ResponseEntity);
                        break;
                    case "CardReaderInsert":
                        //插卡
                        CardReaderInsertCallBack(ResponseEntity);
                        break;
                    case "CardReaderEject":
                        //退卡
                        CardReaderEjectCallBack(ResponseEntity);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                AppLog.Write("[Ini_ResponseEvent Err]" + ex.Message, AppLog.LogMessageType.Error);
            }
        }

        private void DefaultStepCodeDealUi(ResponseData ResponseEntity)
        {
            if (ResponseEntity.ProcedureCode == "BankCardPay" || ResponseEntity.ProcedureCode == "CardReaderInsert")
            {
            }
            else
            {
                return;
            }
            if (ResponseEntity.StepCode == "InsertCardStart" || ResponseEntity.StepCode == "PassCardStart" || ResponseEntity.StepCode == "ScanCoderStart")
            {
                InsertCardStart();
                //ShowPanel(PanelName.InsertCard);
            }
            else if (ResponseEntity.StepCode == "ScanCodeWftImageStart")
            {
                PreCreateSucc();
            }
            else if (ResponseEntity.StepCode == "InsertCardCancel")
            {
                InsertCardCancel();
            }
            else if (ResponseEntity.StepCode == "HasIcCardInside")
            {
                HasCardInside();
                //ShowProcess("已插卡，正在读取IC卡信息");/// 由于IC卡的读取需要4秒左右时间，所以新增该信息提示。
            }
            else if (ResponseEntity.StepCode == "InsertCardEnd")
            {
                //ShowAppMessage("卡号" + ResponseEntity.CardNumber + ",有效期" + ResponseEntity.ExpireDate);
                InsertCardEnd();
            }
            else if (ResponseEntity.StepCode == "InputPasswordStart")
            {
                //ShowPanel(PanelName.InputPw);

                GotoNextActivity();
            }
            else if (ResponseEntity.StepCode == "InputPassword")
            {
                InputPassword();
                //SetElementHtml(tbxPassword, ResponseEntity.args);
            }
            else if (ResponseEntity.StepCode == "InputPasswordError")
            {
                //SetElementHtml(label13, "密码：" + ResponseEntity.args);
                InputPasswordErr();
            }
            else if (ResponseEntity.StepCode == "InputPasswordEnd")
            {
                GotoProcess();
            }
        }

        protected virtual void PreCreateSucc()
        { }
        protected virtual void InsertCardCancel()
        { }

        protected virtual void InsertCardStart()
        { }

        protected virtual void GotoProcess()
        { }

        protected virtual void InsertCardEnd()
        { }
        
        protected virtual void InputPasswordErr()
        { }

        protected virtual void InputPassword()
        { }

        protected virtual void HasCardInside()
        { }

        protected virtual void EEError()
        {
            ShowMessageAndGotoMain("程序出错|请返回！");
        }
    

    protected virtual void GotoNextActivity()
        { }


        #region Callback
        protected virtual void SignInCallback(ResponseData ResponseEntity)
        {
        }

        protected virtual void InitCallback(ResponseData ResponseEntity)
        {
        }

        protected virtual void PayCallback(ResponseData ResponseEntity)
        {
        }

        protected virtual void BalanceQueryCallback(ResponseData ResponseEntity)
        {
        }
        protected virtual void CardReaderInsertCallBack(ResponseData ResponseEntity)
        {
        }

        protected virtual void CardReaderEjectCallBack(ResponseData ResponseEntity)
        {
        }
        #endregion


        protected override void OnLeave()
        {
            Global.gTerminalPay.ResponseEvent -= new ResponseHandle(transDelegate);
        }
    }
}
