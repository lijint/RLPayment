using System;
using System.Collections.Generic;
using System.Text;

namespace YAPayment.Package.PowerCardPay
{
    class UserQueryInfo
    {
        private string[] values;
        public UserQueryInfo(string info)
        {
            values = info.Split(new char[]{'&'},StringSplitOptions.None);
        }

        public enum QueryValueType
        {
            Index,
            Date,
            Pay,
            Dedit,
            Message
        }

        public string GetCorrectValue(QueryValueType type)
        {
            int index = (int) type;
            if (values.Length <= index)
            {
                return "";
            }
            else
            {
                string result = "";
                switch (type)
                {
                    case QueryValueType.Index:
                    case QueryValueType.Message:
                        result= values[index];
                        break;
                    case QueryValueType.Date:
                        result = values[index];
                        break;
                    case QueryValueType.Pay:
                    case QueryValueType.Dedit:
                        result= GetCorrectFormat(values[index]);
                        break;
                }
                return result;
            }
        }

        private string GetCorrectFormat(string value)
        {
            double result = double.Parse(value) / 100;
            return result.ToString("#####0.00");
        }

        public string GetCommitValue()
        {
            long total = long.Parse(values[2]) + long.Parse(values[3]);
            values[4] = total.ToString();
            string result="";
            for (int i = 0; i < values.Length; i++)
            {
                result += values[i] + "&";
            }
            return result + ";";
        }
    }
}
