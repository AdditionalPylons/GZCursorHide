using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GZInjector
{
    public class HookInject
    {
        public static bool Hook()
        {
            string dllPath = Assembly.GetExecutingAssembly().Location;
            dllPath = Path.Combine(Path.GetDirectoryName(dllPath), "GZCursorHide.dll");
            if (!File.Exists(dllPath))
            {
                throw new Exception("GZCursorHide.dll not found");
            }

            IntPtr window = Native.FindWindow(null, "METAL GEAR SOLID V: GROUND ZEROES");
            if (window.ToInt64() == 0)
            {
                return false;
            }

            uint pid;
            Native.GetWindowThreadProcessId(window, out pid);
            if (pid == 0)
            {
                throw new Exception("Failed to get pid for window");
            }

            IntPtr processHandle = Native.OpenProcess(Native.ProcessAccessFlags.All, false, pid);
            if (processHandle.ToInt64() == 0)
            {
                throw new Exception("Failed to get handle for pid");
            }

            IntPtr loadLibraryAddr = Native.GetProcAddress(Native.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (loadLibraryAddr.ToInt64() == 0)
            {
                throw new Exception("Failed to get LoadLibraryA address");
            }

            IntPtr allocated = Native.VirtualAllocEx(processHandle, IntPtr.Zero, (uint)dllPath.Length, Native.AllocationType.Reserve | Native.AllocationType.Commit, Native.MemoryProtection.ReadWrite);
            if (allocated.ToInt64() == 0)
            {
                throw new Exception("Failed to allocate memory in remote thread");
            }

            IntPtr bytesWritten;
            if (!Native.WriteProcessMemory(processHandle, allocated, Encoding.UTF8.GetBytes(dllPath), dllPath.Length, out bytesWritten))
            {
                throw new Exception("Failed to write memory to process");
            }

            IntPtr threadId = Native.CreateRemoteThread(processHandle, IntPtr.Zero, 0, loadLibraryAddr, allocated, 0, IntPtr.Zero);
            if (threadId.ToInt64() == 0)
            {
                throw new Exception("Failed to create new thread");
            }

            Native.CloseHandle(processHandle);

            return true;
        }
    }
}
