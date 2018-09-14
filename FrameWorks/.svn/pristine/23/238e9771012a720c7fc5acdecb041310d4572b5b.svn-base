using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks
{
    public static class HardwareManager
    {
        private enum CheckType
        {
            One,All,
        }

        private class Item
        {
            public string Name;
            public IManagedHardware Hardware;
            public object LastCheckResult;
            public bool Error;
        }

        private static bool mHardWareError;

        private static readonly object mLock = new object();
        private static Dictionary<string, Item> mItemMap = new Dictionary<string, Item>();
        private static List<Item> mHardWareItems = new List<Item>();

        private interface Handler
        {
            object Handle(IManagedHardware ins);
        }

        private class OpenHandler : Handler
        {
            public object Handle(IManagedHardware ins)
            {
                ins.Close();
                return ins.Open();
            }
        }

        private class CloseHandler : Handler
        {
            public object Handle(IManagedHardware ins)
            {
                return ins.Close();
            }
        }

        private class CheckHandler : Handler
        {
            public object Handle(IManagedHardware ins)
            {
                return ins.CheckStatus();
            }
        }

        private static Handler[] sHandlers = { new OpenHandler(), new CloseHandler(), new CheckHandler() };

        internal static void AddHardWare(string name, IManagedHardware hardWare)
        {
            lock (mLock)
            {
                if (!mItemMap.ContainsKey(name))
                {
                    Item item = new Item();
                    item.Name = name;
                    item.Hardware = hardWare;
                    mHardWareItems.Add(item);
                    mItemMap.Add(name, item);
                }
            }
        }

        internal static void RemoveHardWare(string name, IManagedHardware hardWare)
        {
            lock (mLock)
            {
                if (mItemMap.ContainsKey(name))
                {
                    Item item = mItemMap[name];
                    mItemMap.Remove(name);
                    mHardWareItems.Remove(item);
                }
            }
        }

        internal static string Dump()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mHardWareItems.Count; i++)
            {
                sb.Append("Name:" + mHardWareItems[i].Name);
                sb.Append(" LastState:" + mHardWareItems[i].LastCheckResult);
                sb.AppendLine();
            }
            Log.Info(sb.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// 检查所有或单个硬件设备状态
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        /// <returns>是否有硬件故障true:无故障</returns>
        private static bool check(CheckType type, string name,Handler handler)
        {
            lock (mLock)
            {
                if (type == CheckType.One)
                {
                    Item item = null;
                    if (mItemMap.ContainsKey(name))
                    {
                        item = mItemMap[name];
                        object ret = handler.Handle(item.Hardware);
                        item.LastCheckResult = ret;
                        if (item.Hardware.MeansError(ret))
                        {
                            item.Error = true;
                            mHardWareError = true;
                        }
                        else
                        {
                            item.Error = false;
                            int i = 0;
                            if (mHardWareError)
                            {
                                for (; i < mHardWareItems.Count; i++)
                                {
                                    if (mHardWareItems[i].Error)
                                        break;
                                }
                                if (i == mHardWareItems.Count)
                                    mHardWareError = false;
                            }
                        }
                    }
                    if (item == null || !item.Error)
                        return true;
                    else
                        return false;
                }

                mHardWareError = false;
                for (int i = 0; i < mHardWareItems.Count; i++)
                {
                    object ret = handler.Handle(mHardWareItems[i].Hardware);
                    mHardWareItems[i].LastCheckResult = ret;
                    if (mHardWareItems[i].Hardware.MeansError(ret))
                    {
                        mHardWareItems[i].Error = true;
                        mHardWareError = true;
                    }
                    else
                    {
                        mHardWareItems[i].Error = false;
                    }
                }

                Dump();//添加输入日志
                return !mHardWareError;
            }
        }

        internal static object GetState(string name)
        {
            lock (mLock)
            {
                object result = 0;
                if (mItemMap.ContainsKey(name))
                    result = mItemMap[name].LastCheckResult;
                return result;
            }
        }

        public static bool HardWareError()
        {
            lock (mLock)
            {
                return mHardWareError;
            }
        }

        internal static bool ExistError(string name)
        {
            lock (mLock)
            {
                if (mItemMap.ContainsKey(name))
                    return mItemMap[name].Error;
                else
                    return false;
            }
        }

        internal static bool CheckOne(string name)
        {
            return check(CheckType.One, name, sHandlers[2]);
        }

        public static bool CheckAll()
        {
            return check(CheckType.All, null, sHandlers[2]);
        }

        internal static bool OpenOne(string Name)
        {
            return check(CheckType.One, Name, sHandlers[0]);
        }

        public static bool OpenAll()
        {
            return check(CheckType.All, null, sHandlers[0]);
        }

        public static bool CloseAll()
        {
            return check(CheckType.All, null, sHandlers[1]);
        }

        internal static bool CloseOne(string Name)
        {
            return check(CheckType.One, Name, sHandlers[1]);
        }
    }
}
