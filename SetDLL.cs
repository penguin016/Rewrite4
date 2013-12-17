using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace Rewrite4
{
    class SetDLL
    {
        public enum HookType : int //挂钩类型
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [Flags]
        enum CreateProcessFlags : uint
        {
            DEBUG_PROCESS = 0x00000001,
            DEBUG_ONLY_THIS_PROCESS = 0x00000002,
            CREATE_SUSPENDED = 0x00000004,
            DETACHED_PROCESS = 0x00000008,
            CREATE_NEW_CONSOLE = 0x00000010,
            NORMAL_PRIORITY_CLASS = 0x00000020,
            IDLE_PRIORITY_CLASS = 0x00000040,
            HIGH_PRIORITY_CLASS = 0x00000080,
            REALTIME_PRIORITY_CLASS = 0x00000100,
            CREATE_NEW_PROCESS_GROUP = 0x00000200,
            CREATE_UNICODE_ENVIRONMENT = 0x00000400,
            CREATE_SEPARATE_WOW_VDM = 0x00000800,
            CREATE_SHARED_WOW_VDM = 0x00001000,
            CREATE_FORCEDOS = 0x00002000,
            BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,
            ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,
            INHERIT_PARENT_AFFINITY = 0x00010000,
            INHERIT_CALLER_PRIORITY = 0x00020000,
            CREATE_PROTECTED_PROCESS = 0x00040000,
            EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
            PROCESS_MODE_BACKGROUND_BEGIN = 0x00100000,
            PROCESS_MODE_BACKGROUND_END = 0x00200000,
            CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
            CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
            CREATE_DEFAULT_ERROR_MODE = 0x04000000,
            CREATE_NO_WINDOW = 0x08000000,
            PROFILE_USER = 0x10000000,
            PROFILE_KERNEL = 0x20000000,
            PROFILE_SERVER = 0x40000000,
            CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000,
        }

        delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(HookType hookType, 
            HookProc lpfn, 
            IntPtr hMod, 
            uint dwThreadId);

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

          [Flags]
        enum ProcessAccessFlags : uint //声明枚举类型ProcessAccessFlags
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [Flags]
        public enum FreeType
        {
            Decommit = 0x4000,
            Release = 0x8000,
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            [In] ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(
            ProcessAccessFlags dwDesiredAccess, 
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, 
            int dwProcessId);

        [DllImport("kernel32.dll", SetLastError=true, ExactSpelling=true)]
        static extern IntPtr VirtualAllocEx(
            IntPtr hProcess, 
            IntPtr lpAddress,
            uint dwSize, 
            AllocationType flAllocationType, 
            MemoryProtection flProtect);

        [DllImport("kernel32.dll",SetLastError = true)]
        static extern bool WriteProcessMemory(
            IntPtr hProcess, 
            IntPtr lpBaseAddress, 
            byte [] lpBuffer, 
            uint nSize, 
            out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32", CharSet=CharSet.Ansi, ExactSpelling=true, SetLastError=true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes, 
            uint dwStackSize, 
            ThreadstartDelegate lpStartAddress, 
            IntPtr lpParameter, 
            uint dwCreationFlags, 
            IntPtr lpThreadId);
        public delegate void ThreadstartDelegate();

        [DllImport("kernel32.dll", SetLastError=true)]
        static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError=true, ExactSpelling=true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
             int dwSize, FreeType dwFreeType);

        [DllImport("kernel32.dll", SetLastError=true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        static extern uint ResumeThread(IntPtr hThread);

        public int Pretreatment(string filename)
        {
            SECURITY_ATTRIBUTES process = new SECURITY_ATTRIBUTES();
            SECURITY_ATTRIBUTES thread = new SECURITY_ATTRIBUTES();
            STARTUPINFO info = new STARTUPINFO();
            PROCESS_INFORMATION proinfo = new PROCESS_INFORMATION();
            if (CreateProcess(
                null,
                Pinjie(filename),
                ref process,
                ref thread,
                false,
                (uint)CreateProcessFlags.BELOW_NORMAL_PRIORITY_CLASS,
                (IntPtr)null,
                null,
                ref info,
                out proinfo))
            {
                System.Windows.Forms.MessageBox.Show("Pretreatment success!");
                return 0;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Something is wrong, pretreament failed!");
                return -1;
            }
        }

        public string Pinjie(string filename)
        {

            string result = @"C:\Program Files\Microsoft Visual Studio 11.0\VC\Detours Express 3.0\bin.X86\setdll " + "/d:" +"MyDLL.dll " + filename;
            return result;
        }

        public int NewTask(string filename)
        {
            int dex = filename.IndexOf(".");
            string suffix = filename.Substring(dex + 1);
            if (suffix.CompareTo("exe~") == 0)
            {
                SECURITY_ATTRIBUTES process = new SECURITY_ATTRIBUTES();
                SECURITY_ATTRIBUTES thread = new SECURITY_ATTRIBUTES();
                STARTUPINFO info = new STARTUPINFO();
                PROCESS_INFORMATION proinfo = new PROCESS_INFORMATION();
                if (CreateProcess(
                    null,
                    filename,
                    ref process,
                    ref thread,
                    false,
                    (uint)CreateProcessFlags.BELOW_NORMAL_PRIORITY_CLASS,
                    (IntPtr)null,
                    null,
                    ref info,
                    out proinfo))
                {
                    MessageBox.Show("New task sucessfully!");
                    return 0;
                }
                else
                {
                    MessageBox.Show("New task failed!");
                    return 1;
                }
            }
            if(suffix.CompareTo("exe") == 0)
            {
                string message = "You can run this exe in a safe way, do you want do this?";
                string caption = "Choose a maneer";
                MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);
 
                if(result == DialogResult.Yes)
                {
                    //使用远程线程来注入DLL
                    //新建进程，直接挂起
                    SECURITY_ATTRIBUTES process = new SECURITY_ATTRIBUTES();
                    SECURITY_ATTRIBUTES thread = new SECURITY_ATTRIBUTES();
                    STARTUPINFO info = new STARTUPINFO();
                    PROCESS_INFORMATION proinfo = new PROCESS_INFORMATION();
                    if (CreateProcess(
                        null,
                        filename,
                        ref process,
                        ref thread,
                        false,
                        (uint)CreateProcessFlags.CREATE_SUSPENDED,
                        (IntPtr)null,
                        null,
                        ref info,
                        out proinfo))
                    {
                        //获取远程进程句柄
                        IntPtr prohandle = OpenProcess(ProcessAccessFlags.QueryInformation |
                             ProcessAccessFlags.VMOperation | ProcessAccessFlags.VMRead |
                              ProcessAccessFlags.VMWrite, false, proinfo.dwProcessId);
                        if(prohandle == null)
                        {
                            MessageBox.Show("Cannot open process!");
                            return 0;
                        }
                        //在远程进程的地址空间中分配一块内存
                        string mydll = @"C:\Users\wsx\Documents\Visual Studio 2012\Projects\Rewrite4\Rewrite4\bin\Debug";
                        byte[] bytemydll = new byte[mydll.Length];
                        char[] charmydll = mydll.ToCharArray();
                        for(int i = 0; i < mydll.Length; i++)
                        {
                            bytemydll[i] = Convert.ToByte(charmydll[i]);
                        }
                        IntPtr virual = VirtualAllocEx(prohandle, 
                            (IntPtr)null, (uint)bytemydll.Length, 
                            AllocationType.Commit, MemoryProtection.ReadWrite);
                        if(virual == null)
                        {
                            MessageBox.Show("Virtualalloc failed!");
                            return 1;
                        }
                        //往内存写入dll路径
                        UIntPtr bytewrite;
                        WriteProcessMemory(prohandle, virual, bytemydll, (uint)bytemydll.Length, out bytewrite);
                        //获得LoadLibraryW在Kernel32.dll中的实际地址
                        IntPtr getadd = GetProcAddress(GetModuleHandle("Kernel32.dll"), "LoadLibraryW");
                        if(getadd == null)
                        {
                            MessageBox.Show("Get LoadlibraryW address failed!");
                            return 1;
                        }
                        //创建远程线程调用LoadLibraryW加载dll
                        ThreadstartDelegate threadfunc = (ThreadstartDelegate)Marshal.GetDelegateForFunctionPointer(getadd, typeof(ThreadstartDelegate));
                        IntPtr hthread = CreateRemoteThread(prohandle, (IntPtr)null, 0, threadfunc, virual, 0, (IntPtr)null);
                        if(hthread == null)
                        {
                            MessageBox.Show("CreateRemoteThread failed!");
                            return 1;
                        }
                        //等待远程线程结束
                        const UInt32 INFINITE = 0xFFFFFFFF;
                        WaitForSingleObject(hthread, INFINITE);

                        MessageBox.Show("New task run sucessfully. Dll inject completely!");

                        //回收资源
                        if(virual != null)
                            VirtualFreeEx(prohandle, virual, 0, FreeType.Release);
                        if(hthread != null)
                            CloseHandle(hthread);
                        if(prohandle != null)
                            CloseHandle(prohandle);
                        //恢复远程进程运行
                        ResumeThread(proinfo.hThread);
                        return 0;
                    }
                    else
                    {
                        MessageBox.Show("New task failed!");
                        return 1;
                    }
                }
                else if(result == DialogResult.No)
                {
                    SECURITY_ATTRIBUTES process = new SECURITY_ATTRIBUTES();
                    SECURITY_ATTRIBUTES thread = new SECURITY_ATTRIBUTES();
                    STARTUPINFO info = new STARTUPINFO();
                    PROCESS_INFORMATION proinfo = new PROCESS_INFORMATION();
                    if (CreateProcess(
                        null,
                        filename,
                        ref process,
                        ref thread,
                        false,
                        (uint)CreateProcessFlags.BELOW_NORMAL_PRIORITY_CLASS,
                        (IntPtr)null,
                        null,
                        ref info,
                        out proinfo))
                    {
                        return 0;
                    }
                    else
                        MessageBox.Show("New task failed! Something is wrong!");
                }
                else
                {
                    return 0;
                }
            }
            else
                return 0;
            return 1;
        }
    }
}
