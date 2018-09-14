using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Landi.FrameWorks
{
    public class DllImport
    {
        [DllImport("user32.dll", EntryPoint = "GetKeyState", CallingConvention = CallingConvention.StdCall)]
        public static extern short GetKeyState(int vKey);

        [DllImport("Kernel32.dll")]
        public static extern bool SetSystemTime([In, Out] SystemTime st);

        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime([In, Out] SystemTime sysTime);

        [DllImport("Kernel32.dll", EntryPoint = "GetDriveTypeA")]
        public static extern long GetDriveType(string nDrive);

        [DllImport("kernel32.dll")]
        public static extern int GetTickCount();

        //#region hook
        //public struct KBDLLHOOKSTRUCT
        //{
        //    public int vkCode;
        //    int scanCode;
        //    public int flags;
        //    int time;
        //    int dwExtraInfo;
        //}

        //public delegate int LowLevelKeyboardProcDelegate(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
        //public delegate int KeyboardProcDelegate(int nCode, int wParam);
        //private static KeyboardProcDelegate mHandler; 

        //private static int defaultProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam)
        //{
        //    if (mHandler != null)
        //    {
        //        return mHandler(nCode, wParam);
        //    }
        //    return 0;
        //}

        //[DllImport("user32.dll")]
        //private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, IntPtr hMod, int dwThreadId);

        //[DllImport("user32.dll")]
        //private static extern bool UnhookWindowsHookEx(IntPtr hHook);


        //[DllImport("user32.dll")]
        //public static extern int CallNextHookEx(int hHook, int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);

        //[DllImport("kernel32.dll")]
        //private static extern IntPtr GetModuleHandle(IntPtr path);

        //private const int WH_KEYBOARD_LL = 13;

        //public static IntPtr SetWindowsHook(KeyboardProcDelegate hookProc)
        //{
        //    if (hookProc != null)
        //    {
        //        mHandler = hookProc;
        //        return SetWindowsHook(defaultProc);
        //    }
        //    else
        //        return IntPtr.Zero;
        //}

        //public static IntPtr SetWindowsHook(LowLevelKeyboardProcDelegate hookProc)
        //{
        //    if (hookProc != null)
        //    {
        //        IntPtr hModule = GetModuleHandle(IntPtr.Zero);
        //        IntPtr ret = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hModule, 0);
        //        if (ret == IntPtr.Zero)
        //            throw new Exception("Failed to set hook, error = " + Marshal.GetLastWin32Error());
        //        else
        //            return ret;
        //    }
        //    return IntPtr.Zero;
        //}

        //public static void UnhookWindowsHook(IntPtr hHook)
        //{
        //    UnhookWindowsHookEx(hHook);
        //}
        //#endregion

        public struct SoundFlag
        {
            public const int SND_SYNC = 0x0000;/* play synchronously (default) */
            public const int SND_ASYNC = 0x0001;/* play asynchronously */
            public const int SND_NODEFAULT = 0x0002;/* silence (!default) if sound not found */
            public const int SND_MEMORY = 0x0004;/* pszSound points to a memory file */
            public const int SND_LOOP = 0x0008;/* loop the sound until next sndPlaySound */
            public const int SND_NOSTOP = 0x0010;/* don't stop any currently playing sound */
            public const int SND_NOWAIT = 0x00002000;/* don't wait if the driver is busy */
            public const int SND_ALIAS = 0x00010000;/* name is a registry alias */
            public const int SND_ALIAS_ID = 0x00110000;/* alias is a predefined ID */
            public const int SND_FILENAME = 0x00020000;/* name is file name */
            public const int SND_RESOURCE = 0x00040004;/* name is resource name or atom */
        }

        [DllImport("winmm.dll")]
        public static extern bool PlaySound(string pszSound, int hmod, int fdwSound);

        [DllImport("user32.dll")]
        public static extern void keybd_event(
            byte bVk,    //虚拟键值
            byte bScan,// 一般为0
            int dwFlags,  //这里是整数类型  0 为按下，2为释放
            int dwExtraInfo  //这里是整数类型 一般情况下设成为 0
        );        
    }
}
