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

        /// <summary>
        /// 初始化画面流程控制方法--正在处理
        /// </summary>
        /// <param name="index"></param>
        private void processing(int index)
        {
            GetElementById("img" + (index + 1)).SetAttribute("src", "images/ing.gif");
        }

        /// <summary>
        /// 初始化画面流程控制方法2--完成
        /// </summary>
        /// <param name="index"></param>
        private void success(int index)
        {
            GetElementById("img" + (index + 1)).SetAttribute("src", "images/csh_success.png");
        }

        
        private void initdata()
        {
            step = 0;
            Initialized = false;
            readyTime = 3;
            SetManageEntryInfo("ManageEntry");
            setRetName("back");
            setComName("componnent");
        }


        protected override void OnEnter()
        {
            base.OnEnter();
            initdata();
            processing(step);
            Global.gTerminalPay.BusinessLib = String.Format("{0}.InitService", "TDefaultLib");
            Global.gTerminalPay.Init();
        }

        /// <summary>
        /// 初始化业务
        /// </summary>
        /// <param name="ResponseEntity"></param>
        protected override void InitCallback(ResponseData ResponseEntity)
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
                        step += 1;
                        processing(step);
                    }
                    else
                    {
                        StopServiceDeal.Message = "系统初始化失败|" + ResponseEntity.returnCode + "," + ResponseEntity.args;
                        StartActivity("暂停服务");
                    }
                }
            }
            catch (Exception ex)
            {
                StopServiceDeal.Message = "InitCallback" + ex.Message;
                StartActivity("暂停服务");
            }
        }

        /// <summary>
        /// 签到业务
        /// </summary>
        /// <param name="ResponseEntity"></param>
        protected override void SignInCallback(ResponseData ResponseEntity)
        {
            //if (Global.gTerminalPay.BusinessLib == Global.gBankCardLibName)
            //{
            if (ResponseEntity.StepCode == "ProceduresEnd")
            {
                if (ResponseEntity.returnCode == "00")
                {
                    #region 根据银行返回设置本地时间
                    DateTimeHelper.SetLocalDateTime(Convert.ToDateTime(ResponseEntity.TransDateTime));
                    #endregion
                    success(step);
                    //Global.gTerminalPay.BusinessLib = String.Format("{0}.SignInService", Global.gWFTBankCardLibName);
                    //Global.gTerminalPay.SignIn();
                    step += 1;
                    processing(step);
                    readyTime = 3;
                    success(step);
                    step = 3;

                }
                else
                {
                    StopServiceDeal.Message = "签到失败|" + ResponseEntity.returnCode + "," + ResponseEntity.args;
                    StartActivity("暂停服务");
                }
            }
            //}
            //else if(Global.gTerminalPay.BusinessLib == Global.gWFTBankCardLibName)
            //{
            //    if (ResponseEntity.StepCode == "ProceduresEnd")
            //    {
            //        if (ResponseEntity.returnCode == "00")
            //        {
            //            readyTime = 3;
            //            success(step);
            //            step = 3;
            //        }
            //        else
            //        {
            //            StopServiceDeal.Message = "签到失败|" + ResponseEntity.returnCode + "," + ResponseEntity.args;
            //            StartActivity("暂停服务");
            //        }
            //    }
            //}
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

        protected override void EEError()
        {
            StopServiceDeal.Message = "系统故障|暂停服务。";
            StartActivity("暂停服务");
        }
    }
}
