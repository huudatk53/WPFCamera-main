using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Common
{
    public static class RamHelper
    {
        public static void FushRam()
        {
            try
            {
                GC.Collect();
                SetProcessWorkingSetSize32Bit(GetCurrentProcess(), -1, -1);
            }
            catch
            {

            }
        }
        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize32Bit(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);
        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetCurrentProcess();
    }

}
