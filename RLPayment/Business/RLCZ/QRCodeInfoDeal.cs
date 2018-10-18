using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using TerminalLib;
using ThoughtWorks.QRCode.Codec;

namespace RLPayment.Business.RLCZ
{
    class QRCodeInfoDeal : FrameActivity
    {
        private RLCZEntity _entity;

        public int FlagCancel { get; private set; }

        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                FlagCancel = 0;
                _entity = GetBusinessEntity() as RLCZEntity;

                if (_entity.PayType == 1)
                {
                    GetElementById("tbText").InnerHtml = "微信";
                }
                else if (_entity.PayType == 2)
                {
                    GetElementById("tbText").InnerHtml = "支付宝";
                }
                string enCodeString = Global.gTerminalPay.ResponseEntity.AttachField["QRCodeEncoder"];
                if (string.IsNullOrEmpty(enCodeString))
                    return;
                GetElementById("QRCode").SetAttribute("src", GetQRCodeImg(enCodeString));

            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }
        protected override void FrameReturnClick()
        {
            Global.gTerminalPay.WaitInsertCardCancel();
            //Sleep(2000);
            StartActivity("热力充值正在返回");
        }

        private string GetQRCodeImg(string codeUrl)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
            qrCodeEncoder.QRCodeScale = 3;
            qrCodeEncoder.QRCodeVersion = 8;

            string filename = "qrcode.bmp";
            using (Bitmap bt = qrCodeEncoder.Encode(codeUrl, Encoding.UTF8))
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    bt.Save(ms, ImageFormat.Bmp);
                    //重新生成Image对象 
                    using (System.Drawing.Image img2 = System.Drawing.Image.FromStream(ms))
                    {
                        //返回新的Image对象 
                        img2.Save(Environment.CurrentDirectory + @"\QRCode\" + filename);
                    }
                }
            }
            return Environment.CurrentDirectory + @"\QRCode\" + filename;
        }

        protected override void PayCallback(ResponseData ResponseEntity)
        {
            if (ResponseEntity.StepCode == "ProceduresEnd")
            {
                if (ResponseEntity.returnCode == "00")
                {
                    _entity.bBankBackTransDateTime = DateTime.Now.Year + ResponseEntity.TransDate + ResponseEntity.TransTime;
                    _entity.bHostSerialNumber = ResponseEntity.HostSerialNumber;
                    //交易成功
                    StartActivity("热力充值正在前置通信");
                    //StartActivity("热力充值通用成功");
                }
                else
                {
                    ShowMessageAndGotoMain("交易失败|" + ResponseEntity.args);
                }
            }
        }


    }
}
