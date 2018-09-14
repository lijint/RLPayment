using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks
{
    public class Singleton<T> where T : new()
    {
        private static readonly object mLock = new object();
        private static T instance;
        protected Singleton() { }

        public static T GetInstance()
        {
            if (instance == null)
            {
                lock (mLock)
                {
                    if (instance == null)
                        instance = new T();
                }
            }
            return instance;
        }
    }
}
