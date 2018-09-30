using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalLib;

namespace RLPayment
{
    class Global
    {
        #region 通联
        public static TerminalPay gTerminalPay;
        public static string gBankCardLibName = "TTlBankCardLib";//"TYlBankCardLib"
        public delegate void TransDelegate(ResponseData m_Response);
        #endregion

        #region 威富通
        public static string gWFTBankCardLibName = "TWftPayLib";
        public delegate void WFTTransDelegate(ResponseData m_Response);
        #endregion
    }
}
