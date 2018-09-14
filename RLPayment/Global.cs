using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalLib;

namespace RLPayment
{
     class Global
    {
         public static TerminalPay gTerminalPay;
         public static string gBankCardLibName = "TGhBankCardLib";//"TYlBankCardLib"
         public delegate void TransDelegate(ResponseData m_Response);
    }
}
