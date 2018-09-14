using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections;
using Landi.FrameWorks.Iso8583;

namespace Landi.FrameWorks
{
    /// <summary>
    /// ISO 8583 数据包类
    /// </summary>
    [Serializable]
    public class Iso8583Package
    {
        private string messageType;
        private Iso8583Schema schema;
        private Bitmap bitmap;
        private SortedList<int, object> values;
        private bool smartBitmap = true;

        #region 构造函数
        /// <summary>
        /// 使用指定的 Schema 构造数据包类
        /// </summary>
        /// <param name="schema"></param>
        public Iso8583Package(Iso8583Schema schema)
        {
            this.bitmap = new Bitmap();
            this.values = new SortedList<int, object>(Bitmap.FieldCount);
            this.schema = schema;
        }
        /// <summary>
        /// 使用指定的 Schema 文件构造数据包类
        /// </summary>
        /// <param name="schemaFile"></param>
        public Iso8583Package(string schemaFile)
            : this(new Iso8583Schema(schemaFile))
        {
        }

        public Iso8583Package(Iso8583Package package)
        {
            this.bitmap = package.bitmap;
            this.values = package.values;
            this.schema = package.schema;
        }
        #endregion

        #region 公共属性
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType
        {
            get { return this.messageType; }
            set
            {
                if (value.Length != 4)
                    throw new Exception("长度不正确。");
                this.messageType = value;
            }
        }
        /// <summary>
        /// 指示是否使用智能位图模式进行组包和解包。
        /// 设置 true 时需要 Schema 为全128字段的定义。
        /// </summary>
        public bool SmartBitmap
        {
            get { return this.smartBitmap; }
            set
            {
                if (value)
                {
                    if (!this.schema.IsFullBitmap)
                        throw new Exception("架构定义不是全128字段的，不能开启智能位图模式进行组包和解包");
                }
                this.smartBitmap = value;
            }
        }
        #endregion

        #region 为数据域设置值
        /// <summary>
        /// 清除所有数据。
        /// </summary>
        public void Clear()
        {
            this.bitmap = new Bitmap();
            this.values = new SortedList<int, object>(Bitmap.FieldCount);
        }
        /// <summary>
        /// 为指定数据域设置一个字符串值
        /// </summary>
        /// <param name="bitNum">数据域</param>
        /// <param name="value">字符串值</param>
        public void SetString(int bitNum, string value)
        {
            if (!this.schema.fields.ContainsKey(bitNum))
                throw new Exception(String.Format("数据包定义不包含此域：{0}", bitNum));
            Iso8583Field field = this.schema.fields[bitNum];
            if (field.DataType != Iso8583DataType.B && field.DataType != Iso8583DataType.A)
                throw new Exception("格式不符");
            if (!string.IsNullOrEmpty(value) && Encoding.Default.GetByteCount(value) > field.Length)
                throw new Exception("长度过长");
            if (field.DataType == Iso8583DataType.B && !Utility.ValidateBCD(value))
                throw new Exception("数据内容不合法");
            values[bitNum] = value;
            this.bitmap.Set(bitNum, true);
        }

        public void ClearBitAndValue(int bitNum)
        {
            if (!this.schema.fields.ContainsKey(bitNum))
                throw new Exception(String.Format("数据包定义不包含此域：{0}", bitNum));
            if (values.ContainsKey(bitNum))
                values.Remove(bitNum);
            this.bitmap.Set(bitNum, false);
        }

        public void ClearBit(int bitNum)
        {
            if (!this.schema.fields.ContainsKey(bitNum))
                throw new Exception(String.Format("数据包定义不包含此域：{0}", bitNum));
            this.bitmap.Set(bitNum, false);
        }

        public bool ExistBit(int bitNum)
        {
            if (!this.schema.fields.ContainsKey(bitNum))
                return false;
            return bitmap.Get(bitNum);
        }

        public bool IsNull()
        {
            return values.Count == 0;
        }
        /// <summary>
        /// 为指定数据域设置一个数字值
        /// </summary>
        /// <param name="bitNum">数据域</param>
        /// <param name="value">数字值</param>
        public void SetNumber(int bitNum, int value)
        {
            if (!this.schema.fields.ContainsKey(bitNum))
                throw new Exception(String.Format("数据包定义不包含此域：{0}", bitNum));
            Iso8583Field field = this.schema.fields[bitNum];
            string strValue = value.ToString();
            if (strValue.Length > field.Length)
                throw new ArgumentException("数值过大", "value");
            switch (field.DataType)
            {
                case Iso8583DataType.R:
                    throw new Exception("格式不符。");
                default:
                    values[bitNum] = new string('0', field.Length - strValue.Length) + strValue;
                    this.bitmap.Set(bitNum, true);
                    break;
            }

        }
        /// <summary>
        /// 为指定数据域设置一个金额值
        /// </summary>
        /// <param name="bitNum">数据域</param>
        /// <param name="money">金额值</param>
        public void SetMoney(int bitNum, decimal money)
        {
            int value = Convert.ToInt32(money * 100);
            this.SetNumber(bitNum, value);
        }

        /// <summary>
        /// 为指定数据域设置一个日期值
        /// </summary>
        /// <param name="bitNum">数据域</param>
        /// <param name="time">日期值</param>
        //public void SetDateTime(int bitNum, DateTime time)
        //{
        //    if (!this.schema.fields.ContainsKey(bitNum))
        //        throw new Exception(String.Format("数据包定义不包含此域：{0}", bitNum));
        //    Iso8583Field field = this.schema.fields[bitNum];
        //    switch (field.DataType)
        //    {
        //        case Iso8583DataType.B:
        //            throw new Exception("格式不符。");
        //        default:
        //            switch (field.Format)
        //            {
        //                case Iso8583Format.YYMMDD:
        //                    values[bitNum] = time.ToString("yyMMdd");
        //                    break;
        //                case Iso8583Format.YYMM:
        //                    values[bitNum] = time.ToString("yyMM");
        //                    break;
        //                case Iso8583Format.MMDD:
        //                    values[bitNum] = time.ToString("MMdd");
        //                    break;
        //                case Iso8583Format.hhmmss:
        //                    values[bitNum] = time.ToString("HHmmss");
        //                    break;
        //                case Iso8583Format.MMDDhhmmss:
        //                    values[bitNum] = time.ToString("MMddHHmmss");
        //                    break;
        //                default:
        //                    throw new Exception("格式不符。");
        //            }
        //            break;
        //    }
        //    this.bitmap.Set(bitNum, true);
        //}

        public void SetArrayData(int bitNum, byte[] data)
        {
            SetArrayData(bitNum, data, 0, data.Length);
        }

        public void SetArrayData(int bitNum, byte[] data, int start)
        {
            SetArrayData(bitNum, data, start, data.Length - start);
        }

        /// <summary>
        /// 为指定数据域设置一个二进制值
        /// </summary>
        /// <param name="bitNum">数据域</param>
        /// <param name="data">二进制值</param>
        public void SetArrayData(int bitNum, byte[] data, int start, int count)
        {
            if (!this.schema.fields.ContainsKey(bitNum))
                throw new Exception(String.Format("数据包定义不包含此域：{0}", bitNum));
            Iso8583Field field = this.schema.fields[bitNum];

            if (field.DataType != Iso8583DataType.R)
                throw new Exception("格式不符。");
            if (data == null)
            {
                throw new Exception("数据不能为null。");
            }
            else
            {
                if (start < 0 || count <= 0)
                    throw new Exception("参数错误，数据长度不足。");
                if (start >= data.Length || start + count > data.Length)
                    throw new Exception("参数错误，数据长度不足。");
                if (count > 0 && count > field.Length)
                    throw new Exception("长度过长。");
                byte[] tmp = new byte[count];
                Array.Copy(data, start, tmp, 0, count);
                values[bitNum] = tmp;
                this.bitmap.Set(bitNum, true);
            }
        }

        #endregion

        #region 从数据域获取值
        /// <summary>
        /// 获取某个域上是否存在有效值。
        /// </summary>
        /// <param name="bitNum">数据域</param>
        /// <returns></returns>
        public bool ExistValue(int bitNum)
        {
            return this.values.ContainsKey(bitNum) && (this.values[bitNum] != null);
        }

        /// <summary>
        /// 从指定数据域获取字符串值
        /// </summary>
        /// <param name="bitNum">数据域</param>
        /// <returns></returns>
        public string GetString(int bitNum)
        {
            if (!this.schema.fields.ContainsKey(bitNum))
                throw new Exception(String.Format("数据包定义不包含此域：{0}", bitNum));
            Iso8583Field field = this.schema.fields[bitNum];
            if (!this.values.ContainsKey(bitNum) || (this.values[bitNum] == null))
                throw new Exception(String.Format("数据域 {0} 不包含任何有效值。", bitNum));
            switch (field.DataType)
            {
                case Iso8583DataType.A:
                case Iso8583DataType.B:
                    return this.values[bitNum].ToString(); 
                default:
                    throw new Exception("格式不符。");
            }

            //改为允许B格式
            //return this.values[bitNum].ToString();
        }
        /// <summary>
        /// 从指定数据域获取数字值
        /// </summary>
        /// <param name="bitNum">数据域</param>
        /// <returns></returns>
        public int GetNumber(int bitNum)
        {
            if (!this.schema.fields.ContainsKey(bitNum))
                throw new Exception(String.Format("数据包定义不包含此域：{0}", bitNum));
            Iso8583Field field = this.schema.fields[bitNum];
            if (!this.values.ContainsKey(bitNum) || (this.values[bitNum] == null))
                throw new Exception(String.Format("数据域 {0} 不包含任何有效值。", bitNum));
            switch (field.DataType)
            {
                case Iso8583DataType.R:
                    throw new Exception("格式不符。");
                default:
                    return Convert.ToInt32(this.values[bitNum]);
            }
        }
        /// <summary>
        /// 从指定数据域获取金额值
        /// </summary>
        /// <param name="bitNum">数据域</param>
        /// <returns></returns>
        public decimal GetMoney(int bitNum)
        {
            decimal money = this.GetNumber(bitNum);
            return money / 100;
        }
        /// <summary>
        /// 从指定数据域获取日期值
        /// </summary>
        /// <param name="bitNum">数据域</param>
        /// <returns></returns>
        //public DateTime GetDateTime(int bitNum)
        //{
        //    if (!this.schema.fields.ContainsKey(bitNum))
        //        throw new Exception(String.Format("数据包定义不包含此域：{0}", bitNum));
        //    Iso8583Field field = this.schema.fields[bitNum];
        //    if (!this.values.ContainsKey(bitNum) || (this.values[bitNum] == null))
        //        throw new Exception(String.Format("数据域 {0} 不包含任何有效值。", bitNum));
        //    switch (field.DataType)
        //    {
        //        case Iso8583DataType.B:
        //            throw new Exception("格式不符。");
        //        default:
        //            string value = (string)this.values[bitNum];
        //            switch (field.Format)
        //            {
        //                case Iso8583Format.YYMMDD:
        //                    return DateTime.ParseExact(value, "yyMMdd", null);
        //                case Iso8583Format.YYMM:
        //                    return DateTime.ParseExact(value, "yyMM", null);
        //                case Iso8583Format.MMDD:
        //                    return DateTime.ParseExact(value, "MMdd", null);
        //                case Iso8583Format.hhmmss:
        //                    return DateTime.ParseExact(value, "HHmmss", null);
        //                case Iso8583Format.MMDDhhmmss:
        //                    return DateTime.ParseExact(value, "MMddHHmmss", null);
        //                default:
        //                    throw new Exception("格式不符。");
        //            }
        //    }
        //}

        /// <summary>
        /// 从指定数据域获取二进制值
        /// </summary>
        /// <param name="bitNum">数据域</param>
        /// <returns></returns>
        public byte[] GetArrayData(int bitNum)
        {
            if (!this.schema.fields.ContainsKey(bitNum))
                throw new Exception(String.Format("数据包定义不包含此域：{0}", bitNum));
            Iso8583Field field = this.schema.fields[bitNum];
            if (!this.values.ContainsKey(bitNum) || (this.values[bitNum] == null))
                throw new Exception(String.Format("数据域 {0} 不包含任何有效值。", bitNum));
            switch (field.DataType)
            {
                case Iso8583DataType.R:
                    return (byte[])this.values[bitNum];
                default:
                    throw new Exception(String.Format("数据域 {0} 格式不是二进制。", bitNum));
            }

            //return this.str2Bcd((string)this.values[bitNum]);
        }
        #endregion

        #region 组包
        private int GetLength(int bitNum)
        {
            Debug.Assert(this.schema.fields.ContainsKey(bitNum));
            Iso8583Field field = this.schema.fields[bitNum];
            switch (field.Format)
            {
                case Iso8583Format.LVAR:
                case Iso8583Format.LLVAR:
                case Iso8583Format.LLLVAR:
                    string value = "";
                    byte[] valueBytes = null;
                    int len = 0;
                    if (this.values.ContainsKey(bitNum) && (this.values[bitNum] != null))
                    {
                        if (field.DataType == Iso8583DataType.R)
                        {
                            valueBytes = (byte[])values[bitNum];
                            len = valueBytes.Length;
                        }
                        else
                        {
                            value = (string)values[bitNum];
                            len = Encoding.Default.GetByteCount(value);

                            if (field.DataType == Iso8583DataType.B)
                            {
                                len = (len + 1) / 2;
                            }
                        }
                    }
                    //return len + field.Format - Iso8583Format.LVAR + 1;
                    int fix = 1; //LVAR、LLVAR长度一字节，LLLVAR长度2字节
                    if (field.Format == Iso8583Format.LLLVAR)
                    {
                        fix = 2;
                    }
                    return len + fix;
                default:
                    if (field.DataType == Iso8583DataType.B)
                    {
                        return (field.Length + 1) / 2;
                    }
                    else
                    {
                        return field.Length;
                    }
            }
        }

        private void AppendData(string str, Array dst, ref int pos)
        {
            if (String.IsNullOrEmpty(str)) return;
            byte[] field = Encoding.Default.GetBytes(str);
            System.Buffer.BlockCopy(field, 0, dst, pos, field.Length);
            pos += field.Length;
        }

        private void AppendData(byte[] field, Array dst, ref int pos)
        {
            if (field.Length < 1) return;
            System.Buffer.BlockCopy(field, 0, dst, pos, field.Length);
            pos += field.Length;
        }

        /// <summary>
        /// 组包一个 ISO 8583 数据包
        /// </summary>
        /// <returns></returns>
        public byte[] GetSendBuffer()
        {
            //若messageType有设置，优先使用
            if (!String.IsNullOrEmpty(this.messageType))
            {
                this.values[0] = this.messageType;
            }

            int len = 8 + 2; //8字节位图+2字节messageType

            Bitmap map = this.schema.bitmap;
            if (this.smartBitmap)
            {
                map = this.bitmap;
            }

            for (int bitNum = 2; bitNum <= Bitmap.FieldCount; bitNum++)
            {
                if (map.Get(bitNum))
                {
                    len += this.GetLength(bitNum);
                    if (bitNum > 64)
                    {
                        map.Set(1, true);
                    }
                }
            }

            if (map.Get(1))
            {
                len += 8; //128扩展，位图长度加8
            }

            byte[] bcd_tmp = new byte[0];
            byte[] result = new byte[len];
            int pos = 0;

            //第0域
            bcd_tmp = str2Bcd((string)this.values[0]);
            this.AppendData(bcd_tmp, result, ref pos);
            
            //位图域
            map.CopyTo(result, pos);
            if (map.Get(1))
            {
                pos += 16;
            }
            else
            {
                pos += 8;
            }

            //数据域
            for (int bitNum = 2; bitNum <= Bitmap.FieldCount; bitNum++)
            {
                if (!map.Get(bitNum)) continue;
                Iso8583Field field = this.schema.fields[bitNum];

                string value = "";
                byte[] valueBytes = null;
                len = 0;
                if (this.ExistValue(bitNum))
                {
                    switch (field.DataType)
                    {
                        case Iso8583DataType.A:
                        case Iso8583DataType.B:
                            value = (string)this.values[bitNum];
                            len = Encoding.Default.GetByteCount(value);
                            break;
                        case Iso8583DataType.R:
                            valueBytes = (byte[])this.values[bitNum];
                            len = valueBytes.Length;
                            break;
                    }
                }

                //长度位打包
                switch (field.Format)
                {
                    case Iso8583Format.LVAR:
                        bcd_tmp = str2Bcd(len.ToString("0"));
                        break;
                    case Iso8583Format.LLVAR:
                        bcd_tmp = str2Bcd(len.ToString("00"));
                        break;
                    case Iso8583Format.LLLVAR:
                        bcd_tmp = str2Bcd(len.ToString("000"));
                        break;
                    default:
                        //定长的进行补齐
                        bcd_tmp = new byte[0];

                        char padchar = ' ';
                        switch (field.Padchar)
                        {
                            case Iso8583PadType.Null:
                                padchar = '\0';
                                break;
                            case Iso8583PadType.Space:
                                padchar = ' ';
                                break;
                            case Iso8583PadType.Zero:
                                padchar = '0';
                                break;
                            default:
                                padchar = ' ';
                                break;
                        }
                        if (field.Align == Iso8583AlignType.Left)
                        {
                            if (field.DataType == Iso8583DataType.R)
                            {
                                byte[] tmp = new byte[field.Length];
                                Array.Copy(valueBytes, tmp, valueBytes.Length);
                                for (int i = valueBytes.Length; i < field.Length; i++)
                                    tmp[i] = (byte)padchar;
                                valueBytes = tmp;
                            }
                            else
                                value = value.PadRight(field.Length, padchar);
                        }
                        else
                        {
                            if (field.DataType == Iso8583DataType.R)
                            {
                                byte[] tmp = new byte[field.Length];
                                for (int i = 0; i < field.Length - valueBytes.Length; i++)
                                    tmp[i] = (byte)padchar;
                                Array.Copy(valueBytes, 0, tmp, field.Length - valueBytes.Length, valueBytes.Length);
                                valueBytes = tmp;
                            }
                            else
                                value = value.PadLeft(field.Length, padchar);
                        }
                        break;
                }
                this.AppendData(bcd_tmp, result, ref pos);                

                //域内容打包
                switch (field.DataType)
                {
                    case Iso8583DataType.A:
                        if (this.ExistValue(bitNum))
                        {
                            this.AppendData(value, result, ref pos);
                        }
                        break;
                    case Iso8583DataType.B:
                        if (this.ExistValue(bitNum))
                        {
                            if (field.Align == Iso8583AlignType.Left && (len % 2 != 0))
                            {
                                value += "0";
                            }

                            byte[] data = str2Bcd(value);
                            data.CopyTo(result, pos);
                            pos += data.Length;
                        }
                        break;
                    case Iso8583DataType.R:
                        if (this.ExistValue(bitNum))
                        {
                            valueBytes.CopyTo(result, pos);
                            pos += valueBytes.Length;
                        }
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        #endregion

        #region 解包
        /// <summary>
        /// 解包一个 ISO 8583 数据包
        /// </summary>
        /// <param name="buf">数据包</param>
        /// <param name="haveMT">数据包是否包含4字节的MessageType</param>
        public void ParseBuffer(byte[] buf, bool haveMT)
        {
            int pos = 0;
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (buf.Length < 10)
                throw new ArgumentException("数据包长度不符合定义", "buf");

            //获取messageType
            if (haveMT)
            {
                this.messageType = bcd2str(buf, 2);//Encoding.Default.GetString(buf, pos, 4);
                pos += 2;
                this.values[0] = this.messageType;
            }

            //获取位图
            byte[] data = new byte[16];
            if ((buf[pos] & 0x80) != 0)
            {
                Array.Copy(buf, pos, data, 0, data.Length);
                pos += data.Length;
            }
            else
            {
                Array.Copy(buf, pos, data, 0, 8); //只有64域
                pos += 8;
            }

            this.bitmap = new Bitmap(data);

            if (!this.smartBitmap && !this.schema.bitmap.IsEqual(data))
                throw new Exception("数据包的位图表和定义的不一致");
            for (int bitNum = 2; bitNum <= 128; bitNum++)
            {
                if (!bitmap.Get(bitNum)) continue;
                Iso8583Field field = this.schema.fields[bitNum];

                int len = 0;
                switch (field.Format)
                {
                    case Iso8583Format.LVAR:
                    case Iso8583Format.LLVAR:
                    case Iso8583Format.LLLVAR:
                        int varLen = 1;
                        if (field.Format == Iso8583Format.LLLVAR)
                        {
                            varLen = 2;
                        }
                        data = new byte[varLen];
                        Array.Copy(buf, pos, data, 0, varLen);
                        len = int.Parse(bcd2str(data, varLen));
                        pos += varLen;
                        break;
                    default:
                        len = field.Length;
                        break;
                }
                //if (buf.Length < pos + len)
                //throw new ArgumentException("数据包长度不符合定义", "buf");

                switch (field.DataType)
                {
                    case Iso8583DataType.B:
                        if (len > 0)
                        {
                            int reallen = len;
                            len = (len + 1) / 2;//换算压缩后长度

                            data = new byte[len];
                            Array.Copy(buf, pos, data, 0, data.Length);
                            string ascstr = bcd2str(data, data.Length);
                            if (reallen != ascstr.Length)
                            {
                                if (field.Align == Iso8583AlignType.Left)
                                {
                                    ascstr = ascstr.Substring(0, reallen);
                                }
                                else
                                {
                                    ascstr = ascstr.Substring(ascstr.Length - reallen, reallen);
                                }
                            }

                            this.values[bitNum] = ascstr;
                        }
                        break;
                    case Iso8583DataType.R:
                        if (len > 0)
                        {
                            data = new byte[len];
                            Array.Copy(buf, pos, data, 0, data.Length);
                            this.values[bitNum] = data;
                        }
                        break;
                    default:
                        this.values[bitNum] = Encoding.Default.GetString(buf, pos, len);
                        break;
                }
                pos += len;
            }
        }
        #endregion

        #region Util
        /// <summary>
        /// 获取一个适合在日志中输入的字符串
        /// </summary>
        /// <returns></returns>
        public string GetLogText()
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("Package(MessageType:{0}):", this.messageType);
            //sb.AppendLine();
            //sb.AppendLine("{");
            foreach (KeyValuePair<int, object> kvp in this.values)
            {
                Iso8583Field field = this.schema.fields[kvp.Key];
                string value = "content protected";
                if (!field.HideLog && kvp.Value != null)
                {
                    //value = (string)kvp.Value;
                    try
                    {
                        switch (field.DataType)
                        {
                            case Iso8583DataType.B:
                                value = "(BCD)";
                                value += (string) kvp.Value;
                                break;
                            case Iso8583DataType.R:
                                byte[] tmpHex = (byte[]) kvp.Value;
                                value = "(BCD)";
                                value += Encoding.Default.GetString(tmpHex);
                                value += "(HEX)";
                                value += bcd2str(tmpHex, tmpHex.Length);
                                break;
                            case Iso8583DataType.A:
                                value = (string) kvp.Value;
                                break;
                            default:
                                value = (string) kvp.Value;
                                break;
                        }
                    }
                    catch (System.Exception)
                    {
                        value += (string)kvp.Value;
                    }
                }

                sb.AppendFormat("[{0}]:[{1}]", "FLD" + kvp.Key.ToString("000"), value);
                sb.AppendLine();
            }
            //sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// ASCII转为BCD码   
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static byte[] str2Bcd(String asc)
        {
            int len = asc.Length;
            int mod = len % 2;

            if (mod != 0)
            {
                asc = "0" + asc;
                len = asc.Length;
            }

            byte[] abt = new byte[len];
            if (len >= 2)
            {
                len = len / 2;
            }

            byte[] bbt = new byte[len];
            abt = System.Text.Encoding.Default.GetBytes(asc);
            int j, k;

            for (int p = 0; p < asc.Length / 2; p++)
            {
                if ((abt[2 * p] >= '0') && (abt[2 * p] <= '9'))
                {
                    j = abt[2 * p] - '0';
                }
                else if ((abt[2 * p] >= 'a') && (abt[2 * p] <= 'z'))
                {
                    j = abt[2 * p] - 'a' + 0x0a;
                }
                else
                {
                    j = abt[2 * p] - 'A' + 0x0a;
                }

                if ((abt[2 * p + 1] >= '0') && (abt[2 * p + 1] <= '9'))
                {
                    k = abt[2 * p + 1] - '0';
                }
                else if ((abt[2 * p + 1] >= 'a') && (abt[2 * p + 1] <= 'z'))
                {
                    k = abt[2 * p + 1] - 'a' + 0x0a;
                }
                else
                {
                    k = abt[2 * p + 1] - 'A' + 0x0a;
                }

                int a = (j << 4) + k;
                byte b = (byte)a;
                bbt[p] = b;
            }
            return bbt;
        }

        /// <summary>
        /// 将bcd码转换成字符串
        /// </summary>
        /// <param name="bcd_buf"></param>
        /// <param name="conv_len"></param>
        /// <returns></returns>
        public static string bcd2str(byte[] bcd_buf, int conv_len)
        {
            int i = 0, j = 0;
            byte tmp = 0x00;
            byte[] ret = new byte[conv_len * 2];
            for (i = 0, j = 0; i < conv_len; i++)
            {
                tmp = (byte)(bcd_buf[i] >> 4);
                tmp += (byte)((tmp > 9) ? ('A' - 10) : '0');
                ret[j++] = tmp;
                tmp = (byte)(bcd_buf[i] & 0x0f);
                tmp += (byte)((tmp > 9) ? ('A' - 10) : '0');
                ret[j++] = tmp;
            }

            return Encoding.Default.GetString(ret).TrimEnd('\0');
        }

        #endregion
    }
}
