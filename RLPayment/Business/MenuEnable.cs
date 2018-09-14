using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Business
{
    internal class MenuEnable
    {
        private static string _water;
        private static string _tv;
        private static string _power;
        private static string _mobile;
        private static string _creditCard;
        private static string _trafficPolice;
        private static string _carTicket;
        private static string _gas;


        public static void MenuEnableClear()
        {
            _water = _tv = _power = _mobile = _creditCard = _trafficPolice = _carTicket = _gas = "";
        }

        public static bool Gas
        {
            get
            {
                if (string.IsNullOrEmpty(_gas))
                    _gas = ConfigFile.ReadConfigAndCreate("AppData", "Gas").Trim();
                return _gas == "1";
            }
        }
        public static bool Water
        {
            get
            {
                if (string.IsNullOrEmpty(_water))
                    _water = ConfigFile.ReadConfigAndCreate("AppData", "Water").Trim();
                return _water == "1";
            }
        }

        public static bool TV
        {
            get
            {
                if (string.IsNullOrEmpty(_tv))
                    _tv = ConfigFile.ReadConfigAndCreate("AppData", "TV").Trim();
                return _tv == "1";
            }
        }

        public static bool Power
        {
            get
            {
                if (string.IsNullOrEmpty(_power))
                    _power = ConfigFile.ReadConfigAndCreate("AppData", "Power").Trim();
                return _power == "1";
            }
        }

        public static bool Mobile
        {
            get
            {
                if (string.IsNullOrEmpty(_mobile))
                    _mobile = ConfigFile.ReadConfigAndCreate("AppData", "Mobile").Trim();
                return _mobile == "1";
            }
        }

        public static bool CreditCard
        {
            get
            {
                if (string.IsNullOrEmpty(_creditCard))
                    _creditCard = ConfigFile.ReadConfigAndCreate("AppData", "CreditCard").Trim();
                return _creditCard == "1";
            }
        }

        public static bool TrafficPolice
        {
            get
            {
                if (string.IsNullOrEmpty(_trafficPolice))
                    _trafficPolice = ConfigFile.ReadConfigAndCreate("AppData", "TrafficPolice").Trim();
                return _trafficPolice == "1";
            }
        }

        public static bool CarTicket
        {
            get
            {
                if (string.IsNullOrEmpty(_carTicket))
                    _carTicket = ConfigFile.ReadConfigAndCreate("AppData", "CarTicket").Trim();
                return _carTicket == "1";
            }
        }
    }
}
