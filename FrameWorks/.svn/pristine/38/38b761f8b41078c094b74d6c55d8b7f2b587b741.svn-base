using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks
{
    public class Intent:Object
    {
        public string ActivityName;
        public string ActivityClassName;
        public string PageFileName;
        public static string PageFilePath;
        public bool AlonePage;
        public int TimeOut;
        public string NextActivity;
        internal bool ShowTimerCount;
        public string SoundPath;

        private Bundle mExtra;

        public Intent() { }

        public Intent(string name)
        {
            ActivityName = name;
        }

        public Intent(Intent intent)
        {
            this.ActivityName = intent.ActivityName;
            this.ActivityClassName = intent.ActivityClassName;
            this.PageFileName = intent.PageFileName;
            this.AlonePage = intent.AlonePage;
            this.TimeOut = intent.TimeOut;
            this.NextActivity = intent.NextActivity;
            this.ShowTimerCount = intent.ShowTimerCount;
            this.SoundPath = intent.SoundPath;
            this.mExtra = intent.mExtra;
        }

        public void Clear()
        {
            ClearExtra();
            this.ActivityName = null;
            this.ActivityClassName = null;
            this.PageFileName = null;
            this.AlonePage = false;
            this.TimeOut = 0;
            this.NextActivity = null;
            this.ShowTimerCount = false;
            SoundPath = null;
        }

        public void ClearExtra()
        {
            if (mExtra != null)
                mExtra.Clear();
        }

        public void CopyExtraFrom(Intent intent)
        {
            mExtra = intent.mExtra;
        }

        public Intent PutExtra(string key, object value)
        {
            if (mExtra == null)
                mExtra = new Bundle();
            mExtra.Put(key, value);
            return this;
        }

        public Intent PutExtra(string key,string value)
        {
            if (mExtra == null)
                mExtra = new Bundle();
            mExtra.PutString(key, value);
            return this;
        }

        public Intent PutExtra(string key, int value)
        {
            if (mExtra == null)
                mExtra = new Bundle();
            mExtra.PutInt(key, value);
            return this;
        }

        public Intent PutExtra(string key, Boolean value)
        {
            if (mExtra == null)
                mExtra = new Bundle();
            mExtra.PutBoolean(key, value);
            return this;
        }

        public Intent PutExtra(string key, double value)
        {
            if (mExtra == null)
                mExtra = new Bundle();
            mExtra.PutDouble(key, value);
            return this;
        }

        public Intent PutExtra(string key, string[] value)
        {
            if (mExtra == null)
                mExtra = new Bundle();
            mExtra.PutStringArray(key, value);
            return this;
        }

        public Intent PutExtra(string key, int[] value)
        {
            if (mExtra == null)
                mExtra = new Bundle();
            mExtra.PutIntArray(key, value);
            return this;
        }

        public object GetExtra(string key)
        {
            if (mExtra != null)
                return mExtra.Get(key);
            return null;
        }

    }
}
