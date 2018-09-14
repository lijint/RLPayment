using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks
{
    public class KeyManager : Singleton<KeyManager>
    {
        private class KeyItem
        {
            public string SectionName;
            public byte[] EnMacKey;
            public byte[] DeMacKey;
            public byte[] EnPinKey;
            public byte[] DePinKey;

            public KeyItem(string sectionName)
            {
                SectionName = sectionName;
            }
        }

        public KeyManager()
        {
            mKeyItems = new Dictionary<string, KeyItem>();
        }

        private Dictionary<string, KeyItem> mKeyItems;

        private KeyItem getKeyItem(string sectionName)
        {
            if (mKeyItems.ContainsKey(sectionName))
                return mKeyItems[sectionName];
            else
            {
                KeyItem item = new KeyItem(sectionName);
                mKeyItems.Add(sectionName, item);
                return item;
            }
        }

        public static void SetEnMacKey(string sectionName, byte[] key)
        {
            GetInstance().getKeyItem(sectionName).EnMacKey = key;
        }

        public static void SetEnPinKey(string sectionName, byte[] key)
        {
            GetInstance().getKeyItem(sectionName).EnPinKey = key;
        }

        public static void SetDeMacKey(string sectionName, byte[] key)
        {
            GetInstance().getKeyItem(sectionName).DeMacKey = key;
        }

        public static void SetDePinKey(string sectionName, byte[] key)
        {
            GetInstance().getKeyItem(sectionName).DePinKey = key;
        }

        public static byte[] GetEnMacKey(string sectionName)
        {
            byte[] key = GetInstance().getKeyItem(sectionName).EnMacKey;
            if (key == null)
                throw new Exception(sectionName + ":EnMacKey不存在");
            else
                return key;
        }

        public static byte[] GetEnPinKey(string sectionName)
        {
            byte[] key = GetInstance().getKeyItem(sectionName).EnPinKey;
            if (key == null)
                throw new Exception(sectionName + ":EnPinKey不存在");
            else
                return key;
        }

        public static byte[] GetDeMacKey(string sectionName)
        {
            byte[] key = GetInstance().getKeyItem(sectionName).DeMacKey;
            if (key == null)
                throw new Exception(sectionName + ":DeMacKey不存在");
            else
                return key;
        }

        public static byte[] GetDePinKey(string sectionName)
        {
            byte[] key = GetInstance().getKeyItem(sectionName).DePinKey;
            if (key == null)
                throw new Exception(sectionName + ":DePinKey不存在");
            else
                return key;
        }

        public static void ListAllKeys()
        {
            Dictionary<string, KeyItem> keyItems = GetInstance().mKeyItems;
            if (keyItems.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, KeyItem> keyItem in keyItems)
                {
                    sb.AppendLine("Keys of <" + keyItem.Key + ">:");
                    if (keyItem.Value.EnMacKey != null)
                        sb.AppendLine("EnMacKey:" + Utility.bcd2str(keyItem.Value.EnMacKey, keyItem.Value.EnMacKey.Length));
                    else
                        sb.AppendLine("EnMacKey:null");
                    if (keyItem.Value.DeMacKey != null)
                        sb.AppendLine("DeMacKey:" + Utility.bcd2str(keyItem.Value.DeMacKey, keyItem.Value.DeMacKey.Length));
                    else
                        sb.AppendLine("DeMacKey:null");
                    if (keyItem.Value.EnPinKey != null)
                        sb.AppendLine("EnPinKey:" + Utility.bcd2str(keyItem.Value.EnPinKey, keyItem.Value.EnPinKey.Length));
                    else
                        sb.AppendLine("EnPinKey:null");
                    if (keyItem.Value.DePinKey != null)
                        sb.Append("DePinKey:" + Utility.bcd2str(keyItem.Value.DePinKey, keyItem.Value.DePinKey.Length));
                    else
                        sb.Append("DePinKey:null");
                }
                Log.Debug(sb.ToString());
            }
        }

        public static void ListKeysByName(string sectionName)
        {
            Dictionary<string, KeyItem> keyItems = GetInstance().mKeyItems;
            if (keyItems.ContainsKey(sectionName))
            {
                KeyItem keyItem = keyItems[sectionName];
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Keys of <" + keyItem.SectionName + ">:");
                if (keyItem.EnMacKey != null)
                    sb.AppendLine("EnMacKey:" + Utility.bcd2str(keyItem.EnMacKey, keyItem.EnMacKey.Length));
                else
                    sb.Append("EnMacKey:null");
                if (keyItem.DeMacKey != null)
                    sb.AppendLine("DeMacKey:" + Utility.bcd2str(keyItem.DeMacKey, keyItem.DeMacKey.Length));
                else
                    sb.Append("DeMacKey:null");
                if (keyItem.EnPinKey != null)
                    sb.AppendLine("EnPinKey:" + Utility.bcd2str(keyItem.EnPinKey, keyItem.EnPinKey.Length));
                else
                    sb.Append("EnPinKey:null");
                if (keyItem.DePinKey != null)
                    sb.Append("DePinKey:" + Utility.bcd2str(keyItem.DePinKey, keyItem.DePinKey.Length));
                else
                    sb.Append("DePinKey:null");
                Log.Debug(sb.ToString());
            }
        }
    }
}
