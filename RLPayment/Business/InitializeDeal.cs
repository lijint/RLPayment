using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.IO;
using System.ComponentModel;
using Landi.FrameWorks;
using TerminalLib;
using Landi.Tools;

namespace RLPayment.Business
{
    class InitializeDeal : FrameActivity, ITimeTick
    {
        /// <summary>
        /// 当前流程
        /// </summary>
        private int step = 0;
        private int readyTime = 3;
        public static bool Initialized;
        Global.TransDelegate transDelegate;

        private void processing(int index)
        {
            GetElementById("img" + (index + 1)).SetAttribute("src", "images/ing.gif");
        }

        private void success(int index)
        {
            GetElementById("img" + (index + 1)).SetAttribute("src", "images/csh_success.png");
        }


        /// <summary>
        /// 初始化画面流程控制方法--正在处理
        /// </summary>
        /// <param name="index"></param>
        //private void processing(int index)
        //{
        //    GetElementById("Wait" + index.ToString()).Style = "visibility:hidden";
        //    GetElementById("Success" + index.ToString()).Style = "visibility:hidden";
        //    GetElementById("Flash" + index.ToString()).Style = "height: 32px; width: 36px; visibility:visible";
        //}

        /// <summary>
        /// 初始化画面流程控制方法2--完成
        /// </summary>
        /// <param name="index"></param>
        //private void success(int index)
        //{
        //    GetElementById("Flash" + index.ToString()).Style = "height: 32px; width: 36px; visibility:hidden";
        //    GetElementById("Wait" + index.ToString()).Style = "visibility:hidden";
        //    GetElementById("Success" + index.ToString()).Style = "visibility:visible";
        //}
        
        private void initdata()
        {
            step = 0;
            Initialized = false;
            readyTime = 3;
            SetManageEntryInfo("ManageEntry");
            setRetName("back");
            setComName("componnent");
            Global.gTerminalPay.MutliThread = true;
            transDelegate = Ini_ResponseEvent;
            Global.gTerminalPay.ResponseEvent -= new ResponseHandle(transDelegate);
            Global.gTerminalPay.ResponseEvent += new ResponseHandle(transDelegate);
        }

        protected override void OnLeave()
        {
            Global.gTerminalPay.ResponseEvent -= new ResponseHandle(transDelegate);
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            initdata();
            processing(step);
            Global.gTerminalPay.BusinessLib = String.Format("{0}.InitService", "TDefaultLib");
            Global.gTerminalPay.Init();
        }

        private void Ini_ResponseEvent(ResponseData ResponseEntity)
        {
            try
            {
                //打印步骤
                //SetValue(rtbxMessage, String.Format("{0}流程:{1}, 步骤:{2}, 返回:{3}, 描述:{4}", "", ResponseEntity.ProcedureCode, ResponseEntity.StepCode, ResponseEntity.returnCode, ResponseEntity.args));
                if (ResponseEntity.returnCode == "EE")
                {
                    //无法捕获的异常
                     StopServiceDeal.Message =  "系统故障|暂停服务。";
                    StartActivity("暂停服务");
                    return;
                }

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
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                AppLog.Write("[Ini_ResponseEvent Err]"+ex.Message, AppLog.LogMessageType.Error);
            }
        }

        /// <summary>
        /// 初始化业务
        /// </summary>
        /// <param name="ResponseEntity"></param>
        private void InitCallback(ResponseData ResponseEntity)
        {
            try
            {
                if (ResponseEntity.StepCode == "ProceduresEnd")
                {
                    if (ResponseEntity.returnCode == "00")
                    {
                        success(step);
                        Global.gTerminalPay.BusinessLib = String.Format("{0}.SignInService", Global.gBankCardLibName);
                        Global.gTerminalPay.SignIn();
                        step = 1;
                        processing(step);
                    }
                    else
                    {
                       StopServiceDeal.Message= "系统初始化失败|" + ResponseEntity.returnCode + "," + ResponseEntity.args;
                       StartActivity("暂停服务");
                    }
                }
            }
            catch (Exception ex)
            {
                  StopServiceDeal.Message=  "InitCallback" + ex.Message;
                  StartActivity("暂停服务");
            }
        }

        /// <summary>
        /// 签到业务
        /// </summary>
        /// <param name="ResponseEntity"></param>
        private void SignInCallback(ResponseData ResponseEntity)
        {
            if (ResponseEntity.StepCode == "ProceduresEnd")
            {
                if (ResponseEntity.returnCode == "00")
                {
                    //GetElementById("msg").Style = "height: 70%; visibility:block;";
                    readyTime = 3;
                    success(step);
                    step += 1;
                    processing(step);
                    success(step);
                    step = 3;
                }
                else
                {
                      StopServiceDeal.Message = "签到失败|" + ResponseEntity.returnCode + "," + ResponseEntity.args;
                      StartActivity("暂停服务");
                }
            }
        }
        protected override void OnCreate()
        {
            TimerConfig config = TimerConfig.Default();
            config.Top = 685;
            config.Left = 772;
            config.Color = "red";
            config.Font_Size = 20;
            SetTimerConfig(config);
            if (ConfigFile.ReadConfigAndCreate("AppData", "AutoRun","1").Trim() == "1")
            {
                if (SetAutoRunCtrlRegInfo(true))
                    Log.Info("设置开机自启动成功");
            }
            //初装机注册文件
            if (GlobalAppData.GetInstance().AppFirst && RegsvrStarTrans())
            {
                Log.Info("注册成功");
                GlobalAppData.GetInstance().AppFirst = false;
            }
        }

        public void OnTimeTick(int count)
        {
            if (step == 3)
            {
                if (readyTime == 0)
                {
                    Initialized = true;
                    StartActivity("主界面");
                }
                else
                    GetElementById("procNum").InnerText = (readyTime--).ToString();
            }
        }
    }
}
