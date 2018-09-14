using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace Landi.FrameWorks
{
    public abstract class EsamActivity : Activity, IHandleMessage
    {
        private string mId;
        private int mCount;
        private int mPinLength;
        private bool mAllowBlank;
        private string mSectionName;
        private int mKeyIndex;
        private int mKeyLength;
        protected string Password;

        private const int INPUT_PASS = 1;
        private byte[] mPinKey;

        protected enum Result
        {
            Success,
            HardwareError,
            Cancel,
            TimeOut,
        }

        protected EsamActivity()
        {
            mId = InputId;
            if (string.IsNullOrEmpty(mId) || GetElementById(mId) == null)
                throw new Exception("错误的输入框ID");
            mPinLength = PinLength;
            if (mPinLength <= 0)
                mPinLength = 6;
            mAllowBlank = AllowBlank;
            mSectionName = SectionName;
            if (!int.TryParse(ConfigFile.ReadConfig(mSectionName, "KeyIndex"), out mKeyIndex) || mKeyIndex < 0)
                throw new Exception("错误的密钥索引");
            string content = ConfigFile.ReadConfig(mSectionName, "Des");
            if (content != "1" && content != "3")
                throw new Exception("错误的DES算法");
            if (content == "1")
                mKeyLength = 8;
            else
                mKeyLength = 16;
        }

        protected virtual int PinLength
        {
            get { return 6; }
        }

        protected virtual bool AllowBlank
        {
            get { return false; }
        }

        /// <summary>
        /// 密码输入框的Id
        /// </summary>
        protected abstract string InputId
        {
            get;
        }

        protected abstract string SectionName
        {
            get;
        }

        protected override void OnEnter()
        {
            mCount = 0;
            Password = "";
            if (Esam.IsUse)
                mPinKey = KeyManager.GetEnPinKey(mSectionName);
            else
                mPinKey = KeyManager.GetDePinKey(mSectionName);

            if (Esam.IsUse)
            {
                Esam.SetWorkmode(Esam.WorkMode.Encrypt);
                Esam.SetKeyLen(mKeyLength);
                Esam.SetMasterkeyNo(mKeyIndex);
                SendMessage(INPUT_PASS);
            }
        }

        private static void DoInput(EsamActivity instance)
        {
            Result ret = instance.defaultInputPassword();
            HandleResultInner(instance, ret);
        }

        /// <summary>
        /// 根据参数实现界面跳转
        /// </summary>
        /// <param name="result"></param>
        protected abstract void HandleResult(Result result);

        private static void HandleResultInner(EsamActivity instance, Result result)
        {
            if (result == Result.HardwareError)
                Log.Error("InputPassword : " + result.ToString());
            else
                Log.Debug("InputPassword : " + result.ToString());
            instance.HandleResult(result);
        }

        private Result defaultInputPassword()
        {
            string strPin = "";
            byte ubKey = 0;

            Esam.Status nRet = Esam.Status.ESAM_CANCEL;
            while (true)
            {
                if (UserToQuit)
                    return Result.Cancel;
                else if (TimeIsOut)
                    return Result.TimeOut;
                nRet = Esam.GetX98Pin(mPinKey, CommonData.BankCardNum, mPinLength, ref ubKey, ref strPin);

                if ((nRet == Esam.Status.ESAM_KEY_PRESSED) && (ubKey == (byte)Char.Parse("*")))
                {
                    ReportSync("LengthEnough");
                    KeyPressed('*');
                }

                if ((nRet == Esam.Status.ESAM_CLEAR))
                {
                    BackSpace();
                }

                //按回车键
                if ((nRet == Esam.Status.ESAM_SUCC))
                {
                    if ((mCount > 0) && (mCount < mPinLength))
                    {
                        strPin = "";
                        Password = "";
                        ubKey = 0;
                        mCount = 0;
                        ReportSync("LengthNotEnough");
                    }
                    else if (mCount == 0)
                    {
                        if (mAllowBlank)
                        {
                            Password = "";
                            return Result.Success;
                        }
                        else
                        {
                            strPin = "";
                            Password = "";
                            ubKey = 0;
                            mCount = 0;
                            ReportSync("LengthNotEnough");
                        }
                    }
                    else
                    {
                        Password = strPin;
                        return Result.Success;
                    }
                }

                //取消键
                if (nRet == Esam.Status.ESAM_CANCEL)
                {
                    return Result.Cancel;
                }

                //输入超时
                if ((ubKey != 0) && (nRet == Esam.Status.ESAM_TIME_OUT))
                {
                    return Result.TimeOut;
                }

                //错误
                if (nRet == Esam.Status.ESAM_FAIL)
                {
                    return Result.HardwareError;
                }
            }
        }

        /// <summary>
        /// 按确认时若长度错误则被调用，用来显示错误信息
        /// </summary>
        protected abstract void OnErrorLength();
        /// <summary>
        /// 普通按键按下时被调用，用来清除错误提示信息
        /// </summary>
        protected abstract void OnClearNotice();

        private void KeyPressed(char key)
        {
            if (mCount < mPinLength)
            {
                mCount++;
                ReportSync("" + key);
            }
        }

        private void BackSpace()
        {
            if (mCount > 0)
            {
                mCount--;
                ReportSync(null);
            }
        }

        protected sealed override void OnReport(object progress)
        {
            if ((string)progress == "LengthNotEnough")
            {
                GetElementById(mId).SetAttribute("value", "");
                OnErrorLength();
            }
            else if ((string)progress == "LengthEnough")
                OnClearNotice();
            else if (progress == null)
                GetElementById(mId).SetAttribute("value", GetElementById(mId).GetAttribute("value").Substring(0, mCount));
            else
                GetElementById(mId).SetAttribute("value", GetElementById(mId).GetAttribute("value") + (string)progress);
        }

        protected override void OnLeave()
        {
            Esam.SetWorkmode(Esam.WorkMode.Default);
        }

        protected sealed override void OnTimeOut()
        {
            if (!Esam.IsUse)
                HandleResultInner(this, Result.TimeOut);
        }

        public void HandleMessage(Message message)
        {
            if (message.what == INPUT_PASS)
            {
                DoInput(this);
            }
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            if (!Esam.IsUse)
            {
                switch (keyCode)
                {
                    case Keys.D0:
                    case Keys.D1:
                    case Keys.D2:
                    case Keys.D3:
                    case Keys.D4:
                    case Keys.D5:
                    case Keys.D6:
                    case Keys.D7:
                    case Keys.D8:
                    case Keys.D9:
                        ReportSync("LengthEnough");
                        KeyPressed((char)keyCode);
                        break;
                    case Keys.Enter:
                        if ((mCount > 0) && (mCount < mPinLength))
                        {
                            Password = "";
                            mCount = 0;
                            ReportSync("LengthNotEnough");
                        }
                        else if (mCount == 0)
                        {
                            if (mAllowBlank)
                            {
                                Finish();
                            }
                            else
                            {
                                Password = "";
                                ReportSync("LengthNotEnough");
                            }
                        }
                        else
                        {
                            Finish();
                        }
                        break;
                    case Keys.Back:
                        BackSpace();
                        break;
                    case Keys.Escape:
                        HandleResultInner(this, Result.Cancel);
                        break;
                }
            }
        }

        private void Finish()
        {
            byte[] tmp1 = Utility.str2Bcd("0000" + CommonData.BankCardNum.Substring(CommonData.BankCardNum.Length - 13, 12));
            Password = GetElementById(mId).GetAttribute("value");
            byte[] tmp2 = Utility.str2Bcd(Password.Length.ToString().PadLeft(2, '0') + Password + "F".PadRight(14 - Password.Length,'F'));
            for (int i = 0; i < tmp2.Length; i++)
                tmp2[i] ^= tmp1[i];
            byte[] pinb = null;
            if (mPinKey.Length == 16)
                pinb = Landi.FrameWorks.Encrypt.DES3Encrypt(tmp2, mPinKey);
            else
                pinb = Landi.FrameWorks.Encrypt.DESEncrypt(tmp2, mPinKey);
            Password = Utility.bcd2str(pinb, 8);
            HandleResultInner(this, Result.Success);
        }
    }
}
