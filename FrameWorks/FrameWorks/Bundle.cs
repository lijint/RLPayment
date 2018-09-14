using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Landi.FrameWorks
{
    public class Bundle
    {
        private Dictionary<string ,object> mDictionary;

        public Bundle()
        {
            mDictionary = new Dictionary<string, object>();
        }

        public void PutString(string key,string value)
        {
            if (mDictionary.ContainsKey(key))
                mDictionary.Remove(key);
            mDictionary.Add(key,value);
        }

        public void PutInt(string key, int value)
        {
            if (mDictionary.ContainsKey(key))
                mDictionary.Remove(key);
            mDictionary.Add(key, value);
        }

        public void PutDouble(string key, double value)
        {
            if (mDictionary.ContainsKey(key))
                mDictionary.Remove(key);
            mDictionary.Add(key, value);
        }

        public void PutBoolean(string key, Boolean value)
        {
            if (mDictionary.ContainsKey(key))
                mDictionary.Remove(key);
            mDictionary.Add(key, value);
        }

        public void PutStringArray(string key, string[] value)
        {
            if (mDictionary.ContainsKey(key))
                mDictionary.Remove(key);
            mDictionary.Add(key, value);
        }

        public void PutIntArray(string key, int[] value)
        {
            if (mDictionary.ContainsKey(key))
                mDictionary.Remove(key);
            mDictionary.Add(key, value);
        }

        public void Put(string key, object value)
        {
            if (mDictionary.ContainsKey(key))
                mDictionary.Remove(key);
            mDictionary.Add(key, value);
        }

        public string GetString(string key)
        {
            if (!mDictionary.ContainsKey(key))
                return null;
            try
            {
                object obj = mDictionary[key];
                return (string)obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int GetInt(string key)
        {
            if (!mDictionary.ContainsKey(key))
                return 0;
            try
            {
                object obj = mDictionary[key];
                return (int)obj;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public double GetDouble(string key)
        {
            if (!mDictionary.ContainsKey(key))
                return 0;
            try
            {
                object obj = mDictionary[key];
                return (double)obj;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public Boolean GetBoolean(string key)
        {
            if (!mDictionary.ContainsKey(key))
                return false;
            try
            {
                object obj = mDictionary[key];
                return (Boolean)obj;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string[] GetStringArray(string key)
        {
            if (!mDictionary.ContainsKey(key))
                return null;
            try
            {
                object obj = mDictionary[key];
                return (string[])obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int[] GetIntArray(string key)
        {
            if (!mDictionary.ContainsKey(key))
                return null;
            try
            {
                object obj = mDictionary[key];
                return (int[])obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object Get(string key)
        {
            if (mDictionary.ContainsKey(key))
                return mDictionary[key];
            else
                return null;
        }

        public void Clear()
        {
            mDictionary.Clear();
        }

    }
}
