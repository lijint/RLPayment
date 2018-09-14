using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Landi.FrameWorks
{
    public class TLVHandler
    {
        [Serializable]
        public class TLVDef
        {
            public enum PadType
            {
                /// <summary>
                /// 填充空格
                /// </summary>
                Space,
                /// <summary>
                /// 填充'/0'
                /// </summary>
                Null,
                /// <summary>
                /// 填充'0'
                /// </summary>
                Zero,
            }

            public enum AlignType
            {
                /// <summary>
                /// 左对齐
                /// </summary>
                Left,
                /// <summary>
                /// 右对齐
                /// </summary>
                Right
            }

            public enum LengthFormat
            {
                /// <summary>
                /// 可变长域
                /// </summary>
                Var,
                /// <summary>
                /// 定长域
                /// </summary>
                Fix,
            }

            public enum DataType
            {
                /// <summary>
                /// 字母字符，A至Z或a至z，左靠，右部多余部分填空格
                /// </summary>
                A,
                /// <summary>
                /// 二进制位，左靠，右部多余部分填零。
                /// </summary>
                B,
                /// <summary>
                /// 16进制数据，但是不压缩
                /// </summary>
                R,
            }

            public string Tag;
            public DataType ValueType;
            public LengthFormat LengthType;
            public int Length;
            public AlignType Align;
            public PadType Padchar;
        }

        [Serializable]
        private class TLVDefs
        {
            public Dictionary<string, TLVDef> fields;
            private static Dictionary<string, TLVDefs> map = new Dictionary<string, TLVDefs>();

            public TLVDefs() 
            {
                fields = new Dictionary<string, TLVDef>();
            }

            public TLVDefs(string schemaFile)
                : this()
            {
                if (Path.GetExtension(schemaFile) != ".xml")
                    throw new ArgumentException("无效的xml文件");
                if (map.ContainsKey(schemaFile))
                {
                    this.fields = map[schemaFile].fields;
                }
                else
                {
                    this.LoadFromXml(schemaFile);
                    map.Add(schemaFile, this);
                }
            }

            private void LoadFromXml(string schemaFile)
            {
                XmlSerializer serial = new XmlSerializer(typeof(TLVDef[]));
                StreamReader reader = new StreamReader(schemaFile);
                TLVDef[] array = serial.Deserialize(reader) as TLVDef[];
                foreach (TLVDef field in array)
                {
                    this.AddField(field);
                }
            }

            public TLVDef GetTLVDefByTag(string tag)
            {
                if (fields != null)
                    return fields[tag];
                return null;
            }

            public void AddField(TLVDef field)
            {
                if (field == null)
                    throw new ArgumentNullException("field");
                if (this.fields.ContainsKey(field.Tag))
                    throw new Exception("Tag:" + field.Tag + ",标签定义已经存在。");
                this.fields.Add(field.Tag, field);
            }

            public void SaveToFile(string fileName)
            {
                string xml = this.ExportToXml();
                StreamWriter writer = new StreamWriter(fileName);
                writer.Write(xml);
                writer.Close();
            }

            private string ExportToXml()
            {
                TLVDef[] array = new TLVDef[this.fields.Count];
                int i = 0;
                foreach (KeyValuePair<string, TLVDef> kvp in this.fields)
                {
                    array[i++] = kvp.Value;
                }
                XmlSerializer serial = new XmlSerializer(typeof(TLVDef[]));
                StringBuilder sb = new StringBuilder();
                StringWriter writer = new StringWriter(sb);
                serial.Serialize(writer, array);
                return sb.ToString();
            }
        }

        private class TLVElement
        {
            public byte[] Tag;
            private int ValueLength;
            public byte[] Value;
            public int TotalLength;
            public bool R;//是否是16进制
            private static readonly int MaxLength = 256 * 256 - 1;
            private TLVDef mTLVDef;
            private byte[] mResult;
            private bool mParsed;

            public TLVElement(string tag, byte[] value, TLVDef def, bool isPack)
            {
                Tag = Utility.str2Bcd(tag);
                Value = value;
                mTLVDef = def;
                if ((def == null && isPack) || (def != null && def.ValueType == TLVDef.DataType.R && isPack))
                    R = true;
                else
                    R = false;

                if (!R && isPack)
                    throw new Exception("Tag:" + tag + ",内容格式不合法");
            }

            public TLVElement(string tag, string value, TLVDef def, bool isPack)
            {
                Tag = Utility.str2Bcd(tag);
                Value = Encoding.Default.GetBytes(value);
                mTLVDef = def;
                if (def == null || def.ValueType == TLVDef.DataType.A || def.ValueType == TLVDef.DataType.B)
                    R = false;
                else
                    R = true;

                if(R)
                    throw new Exception("Tag:" + tag + ",内容格式不合法");
            }

            public void UnPack()
            {
                if (!mParsed)
                {
                    unPack(this);
                    mParsed = true;
                }
            }

            private static void pack(TLVElement element)
            {
                if (element.Value.Length > MaxLength)
                    throw new Exception("值长度过长");
                byte[] result = element.Value;
                if (element.mTLVDef != null)
                {
                    int totalLength = element.Value.Length;
                    if (element.mTLVDef.LengthType == TLVDef.LengthFormat.Fix && element.mTLVDef.Length > element.Value.Length)
                    {
                        totalLength = element.mTLVDef.Length;
                    }
                    else if (element.mTLVDef.ValueType == TLVDef.DataType.B && element.Value.Length % 2 != 0)
                    {
                        totalLength += 1;
                    }
                    if (totalLength != element.Value.Length)
                    {
                        char padChar = '\0';
                        switch (element.mTLVDef.Padchar)
                        {
                            case TLVDef.PadType.Space:
                                padChar = ' ';
                                break;
                            case TLVDef.PadType.Zero:
                                padChar = '0';
                                break;
                        }
                        string padStr = new string(padChar, totalLength - element.Value.Length);
                        result = new byte[totalLength];
                        if (element.mTLVDef.Align == TLVDef.AlignType.Left)
                        {
                            Array.Copy(element.Value, result, element.Value.Length);
                            Array.Copy(Encoding.Default.GetBytes(padStr), 0, result, element.Value.Length, padStr.Length);
                        }
                        else
                        {
                            Array.Copy(Encoding.Default.GetBytes(padStr), result, padStr.Length);
                            Array.Copy(element.Value, 0, result, padStr.Length, element.Value.Length);
                        }
                    }
                    if (element.mTLVDef.ValueType == TLVDef.DataType.B)
                        result = Utility.str2Bcd(Encoding.Default.GetString(result));
                }
                element.Value = result;
                element.ValueLength = result.Length;
                element.TotalLength = element.Tag.Length + CalcLength(result) + element.ValueLength;
            }

            private static void unPack(TLVElement element)
            {
                if (element.Value.Length > MaxLength)
                    throw new Exception("值长度过长");
                byte[] result = element.Value;
                if (element.mTLVDef != null)
                {
                    if (element.mTLVDef.ValueType == TLVDef.DataType.B)
                        result = Encoding.Default.GetBytes(Utility.bcd2str(element.Value, element.Value.Length));
                }
                element.Value = result;
                element.ValueLength = result.Length;
                element.TotalLength = element.Tag.Length + CalcLength(result) + element.ValueLength;
            }

            public byte[] Pack()
            {
                if (mResult == null)
                {
                    pack(this);
                    byte[] len = null;
                    if (ValueLength < 128)
                    {
                        len = new byte[1];
                        len[0] = (byte)ValueLength;
                    }
                    else if (ValueLength >= 128 && ValueLength <= 255)
                    {
                        len = new byte[2];
                        len[0] = 0x81;
                        len[1] = (byte)ValueLength;
                    }
                    else
                    {
                        len = new byte[3];
                        len[0] = 0x82;
                        len[1] = (byte)(ValueLength / 256);
                        len[2] = (byte)(ValueLength % 256);
                    }

                    byte[] result = new byte[TotalLength];
                    Array.Copy(Tag, result, Tag.Length);
                    Array.Copy(len, 0, result, Tag.Length, len.Length);
                    Array.Copy(Value, 0, result, Tag.Length + len.Length, ValueLength);
                    mResult = result;
                }
                return mResult;
            }
        }

        private static int CalcLength(byte[] value)
        {
            int ret=1;
            if (value.Length > 127 && value.Length < 256)
                ret = 2;
            else if (value.Length >= 256)
                ret = 3;
            return ret;
        }

        private static int CalcLength(string value)
        {
            int ret = 1;
            if (value.Length > 127 && value.Length < 256)
                ret = 2;
            else if (value.Length >= 256)
                ret = 3;
            return ret;
        }

        private Dictionary<string, TLVElement> mTags;
        private TLVDefs mMap;
        private bool mNeedRecalc;
        private byte[] mResult;

        public TLVHandler()
        {
            mTags = new Dictionary<string, TLVElement>();
        }

        public TLVHandler(string schemaFile)
            : this()
        {
            mMap = new TLVDefs(schemaFile);
        }

        public void AddTLVDef(TLVDef field)
        {
            if (mMap == null)
                mMap = new TLVDefs();
            mMap.AddField(field);
        }

        public void SaveToFile(string fileName)
        {
            if (mMap != null)
                mMap.SaveToFile(fileName);
        }

        private TLVDef getTLVDefByTag(string tag)
        {
            if (mMap != null)
                return mMap.GetTLVDefByTag(tag);
            return null;
        }

        public void AddTag(string tag, byte[] value)
        {
            checkTag(tag);
            if (mTags.ContainsKey(tag))
                mTags.Remove(tag);
            mTags.Add(tag, new TLVElement(tag, value, getTLVDefByTag(tag), true));
            mNeedRecalc = true;
        }

        public void AddTag(string tag, string value)
        {
            checkTag(tag);
            if (mTags.ContainsKey(tag))
                mTags.Remove(tag);
            mTags.Add(tag, new TLVElement(tag, value, getTLVDefByTag(tag),true));
            mNeedRecalc = true;
        }

        public void RemoveTag(string tag)
        {
            checkTag(tag);
            if (mTags.ContainsKey(tag))
                mTags.Remove(tag);
            mNeedRecalc = true;
        }

        public byte[] GetTLV()
        {
            if (mNeedRecalc)
            {
                int length = 0;
                foreach (KeyValuePair<string, TLVElement> tmp in mTags)
                {
                    length += tmp.Value.Pack().Length;
                }
                mNeedRecalc = false;
                byte[] result = new byte[length];
                length = 0;
                foreach (KeyValuePair<string, TLVElement> tmp in mTags)
                {
                    Array.Copy(tmp.Value.Pack(), 0, result, length, tmp.Value.TotalLength);
                    length += tmp.Value.TotalLength;
                }
                mResult = result;
            }
            return mResult;
        }

        public byte[] GetTLVWithLength(int length)
        {
            byte[] content = GetTLV();
            byte[] result = new byte[content.Length + length];
            Array.Copy(Encoding.Default.GetBytes(content.Length.ToString().PadLeft(length, '0')), result, length);
            Array.Copy(content, 0, result, length, content.Length);
            return result;
        }

        private static void checkTag(string tag)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(tag) && (tag.Length == 2 || tag.Length == 4))
            {
                byte[] tmp = Utility.str2Bcd(tag);
                if (tmp.Length == 1)
                {
                    if ((tmp[0] & 0x0F) != 0x0F)
                        ret = true;
                }
                else
                {
                    if ((tmp[0] & 0x0F) == 0x0F)
                        ret = true;
                }
            }
            if (!ret)
                throw new Exception("Tag:" + tag + ",无效的TAG");
        }

        public void ParseTLV(byte[] content)
        {
            Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
            ParseTLV(content, result);
            mTags.Clear();
            foreach (KeyValuePair<string, byte[]> tmp in result)
            {
                TLVElement element = new TLVElement(tmp.Key, tmp.Value, getTLVDefByTag(tmp.Key), false);
                element.UnPack();
                mTags.Add(tmp.Key, element);
            }
        }

        public byte[] GetBytesValue(string tag)
        {
            if (mTags.ContainsKey(tag))
                return mTags[tag].Value;
            return null;
        }

        public string GetStringValue(string tag)
        {
            if (mTags.ContainsKey(tag))
            {
                if (!mTags[tag].R)
                    return Encoding.Default.GetString(mTags[tag].Value);
                else
                    throw new Exception("Tag:" + tag + ",格式不符,应调用GetBytesValue取值");
            }
            return null;
        }

        public static void ParseTLV(byte[] content,Dictionary<string, byte[]> result)
        {
            if (result == null)
                result = new Dictionary<string, byte[]>();
            string tag;
            int len;
            byte[] value = null;
            byte[] tag1 = new byte[1];
            byte[] tag2 = new byte[2];
            byte[] tagb = null;
            int step = 1;

            for (int pos = 0; pos < content.Length; )
            {
                if ((content[pos] & 0x0F) == 0x0F)
                    tagb = tag2;
                else
                    tagb = tag1;
                Array.Copy(content, pos, tagb, 0, tagb.Length);
                tag = Utility.bcd2str(tagb, tagb.Length);
                checkTag(tag);
                pos += tagb.Length;
                if ((content[pos] & 0x80) > 0)//长度大于1个字节
                {
                    len = 0;
                    step = 1;
                    int tmpLen = content[pos] & 0x7F;//几个字节
                    pos += 1;
                    for (int i = tmpLen + pos - 1; i >= pos; i--)
                    {
                        len += content[i] * step;
                        step *= 256;
                    }
                    pos += tmpLen;
                }
                else
                {
                    len = content[pos];
                    pos += 1;
                }
                value = new byte[len];
                Array.Copy(content, pos, value, 0, len);
                result.Add(tag, value);
                pos += len;
            }
        }
    }
}
