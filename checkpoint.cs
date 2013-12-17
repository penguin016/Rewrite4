using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;

namespace Rewrite4
{
    class checkpoint
    {
        public enum CONTEXT_FLAGS : uint
        {
            CONTEXT_i386 = 0x10000,
            CONTEXT_i486 = 0x10000,   //  same as i386
            CONTEXT_CONTROL = CONTEXT_i386 | 0x01, // SS:SP, CS:IP, FLAGS, BP
            CONTEXT_INTEGER = CONTEXT_i386 | 0x02, // AX, BX, CX, DX, SI, DI
            CONTEXT_SEGMENTS = CONTEXT_i386 | 0x04, // DS, ES, FS, GS
            CONTEXT_FLOATING_POINT = CONTEXT_i386 | 0x08, // 387 state
            CONTEXT_DEBUG_REGISTERS = CONTEXT_i386 | 0x10, // DB 0-3,6,7
            CONTEXT_EXTENDED_REGISTERS = CONTEXT_i386 | 0x20, // cpu specific extensions
            CONTEXT_FULL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS,
            CONTEXT_ALL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS | CONTEXT_FLOATING_POINT | CONTEXT_DEBUG_REGISTERS | CONTEXT_EXTENDED_REGISTERS
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct FLOATING_SAVE_AREA
        {
            public uint ControlWord;
            public uint StatusWord;
            public uint TagWord;
            public uint ErrorOffset;
            public uint ErrorSelector;
            public uint DataOffset;
            public uint DataSelector;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
            public byte[] RegisterArea;
            public uint Cr0NpxState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CONTEXT
        {
            public uint ContextFlags; //set this to an appropriate value 指示要获取哪些寄存器的内容
            // Retrieved by CONTEXT_DEBUG_REGISTERS CPU调试寄存器
            public uint Dr0;
            public uint Dr1;
            public uint Dr2;
            public uint Dr3;
            public uint Dr6;
            public uint Dr7;
            // Retrieved by CONTEXT_FLOATING_POINT CPU浮点寄存器
            public FLOATING_SAVE_AREA FloatSave;
            // Retrieved by CONTEXT_SEGMENTS CPU段寄存器
            public uint SegGs;
            public uint SegFs;
            public uint SegEs;
            public uint SegDs;
            // Retrieved by CONTEXT_INTEGER CPU整数寄存器
            public uint Edi;
            public uint Esi;
            public uint Ebx;
            public uint Edx;
            public uint Ecx;
            public uint Eax;
            // Retrieved by CONTEXT_CONTROL CPU控制寄存器：指令指针、栈指针、标志和函数返回地址
            public uint Ebp;
            public uint Eip;
            public uint SegCs;
            public uint EFlags;
            public uint Esp;
            public uint SegSs;
            // Retrieved by CONTEXT_EXTENDED_REGISTERS CPU扩展寄存器
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] ExtendedRegisters;
            private int count;
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

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION //内存信息结构体
        {
            public IntPtr BaseAddress; //区域基地址
            public IntPtr AllocationBase; //分配基地址
            public uint AllocationProtect; //区域被初次保留时赋予的保护属性
            public IntPtr RegionSize; //区域大小，以字节为计量单位
            public uint State; //状态，MEM_FREE, MEM_RESERVE或MEM_COMMIT    
            public uint Protect; //保护属性
            public uint Type; //类型
        }

        public enum StateEnum : uint
        {
            MEM_COMMIT = 0x1000,
            MEM_FREE = 0x10000,
            MEM_RESERVE = 0x2000
        }

        public enum AllocationProtect : uint //页面保护属性
        {
            PAGE_EXECUTE = 0x00000010, //只允许执行代码，对该区域试图进行读写操作将引发访问违规。
            PAGE_EXECUTE_READ = 0x00000020, //允许执行和读取。
            PAGE_EXECUTE_READWRITE = 0x00000040, //允许读写和执行代码。

            /*对于该地址空间的区域，不管执行什么操作，
             * 都不会引发访问违规。
             * 如果试图写入页面将使系统为进程单独创建一份该页面的私有副本（以页交换文件为后备存储器）
            */
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001, //试图读取页面、写入页面或执行页面中的代码将引发访问违规
            PAGE_READONLY = 0x00000002, //试图写入页面或执行页面中的代码将引发访问违规
            PAGE_READWRITE = 0x00000004, //试图执行页面中的代码将引发访问违规
            PAGE_WRITECOPY = 0x00000008, //试图执行页面中的代码将引发访问违规。试图写入页面将使系统为进程单独创建一份该页面的私有副本（以页交换文件为后备存储器）。 
            PAGE_GUARD = 0x00000100, //在页面上写入一个字节时使应用程序收到一个通知（通过一个异常条件）。
            PAGE_NOCACHE = 0x00000200, // 停用已提交页面的高速缓存。
            PAGE_WRITECOMBINE = 0x00000400
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct _PROCESSOR_INFO_UNION
        {
            [FieldOffset(0)]
            internal uint dwOemId;
            [FieldOffset(0)]
            internal ushort wProcessorArchitecture; //处理器的体系架构
            [FieldOffset(2)]
            internal ushort wReserved; //留用
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            internal _PROCESSOR_INFO_UNION uProcessorInfo;
            public uint dwPageSize; //表示CPU页面大小
            public IntPtr lpMinimumApplicationAddress; //给出每个进程可用地址空间中最小的内存地址
            public IntPtr lpMaximumApplicationAddress; //给出每个进程私有地址空间中最大的可用内存地址
            public IntPtr dwActiveProcessorMask; //一个位掩码，用来表示哪些CPU处于活动状态
            public uint dwNumberOfProcessors; //机器中CPU的数量
            public uint dwProcessorType; //已作废，勿使用
            public uint dwAllocationGranularity; //表示用于预定地址空间区域的分配力度
            public ushort dwProcessorLevel; //进一步细分处理器的体系结构
            public ushort dwProcessorRevision; //在对dwProcessorLevel进行细分
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
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

        [DllImport("kernel32.dll")]
        static extern bool GetThreadContext(IntPtr hThread, ref CONTEXT lpContext);

        [DllImport("kernel32.dll")]
        static extern bool SetThreadContext(IntPtr hThread, [In] ref CONTEXT lpContext);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(
            ThreadAccess dwDesiredAccess, 
            bool bInheritHandle,
            uint dwThreadId);

        [DllImport("kernel32.dll", SetLastError=true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll",SetLastError=true)]
        static extern int SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        static extern uint ResumeThread(IntPtr hThread);

        [DllImport("kernel32")]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll",SetLastError = true)]
        static extern bool WriteProcessMemory(
            IntPtr hProcess, 
            IntPtr lpBaseAddress, 
            byte [] lpBuffer, 
            uint nSize, 
            out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(
            ProcessAccessFlags dwDesiredAccess, 
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, 
            int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern int VirtualQueryEx(IntPtr hProcess, 
            IntPtr lpAddress, 
            out MEMORY_BASIC_INFORMATION lpBuffer, 
            uint dwLength);

        [DllImport("kernel32.dll")]
        static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError=true)]
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

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private unsafe static extern uint CreateThread(
           uint* lpThreadAttributes,
           uint dwStackSize,
           StartThread lpStartAddress,
           uint* lpParameter,
           uint dwCreationFlags,
           out uint lpThreadId);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes, 
            uint dwStackSize, 
            ThreadStartDelegate lpStartAddress, 
            IntPtr lpParameter, 
            uint dwCreationFlags, 
            IntPtr lpThreadId);


        public delegate void StartThread();
        public delegate void ThreadStartDelegate();

        public int DumpProcState(int pid, string filename)
        {
            AllocConsole();
            //查找进程相关的所有线程
            System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(pid);
            System.Diagnostics.ProcessThread[] processthread = new System.Diagnostics.ProcessThread[500];
            System.Diagnostics.ProcessThreadCollection threadcollection = new System.Diagnostics.ProcessThreadCollection(processthread);
            threadcollection = process.Threads;
            int count = threadcollection.Count;
            CONTEXT[] context = new CONTEXT[count];
            //间文件流
            System.IO.FileStream stream = null;
           
            stream = new System.IO.FileStream(filename, System.IO.FileMode.Create);

            string procdescription = process.MainModule.FileName;
            char[] description = procdescription.ToCharArray();
            byte[] bytedescription = new byte[procdescription.Length];
            for (int j = 0; j < procdescription.Length; j++)
            {
                bytedescription[j] = Convert.ToByte(description[j]);
            }
            byte[] descriptioncount = new byte[1];
            descriptioncount[0] = Convert.ToByte(procdescription.Length);
            //写入exe文件路径的长度
            stream.Write(descriptioncount, 0, descriptioncount.Length);
            //写入exe文件路径
            stream.Write(bytedescription, 0, bytedescription.Length);

            byte[] bytecount = new byte[1];
            bytecount[0] = Convert.ToByte(count);
            //写入线程数
            stream.Write(bytecount, 0, bytecount.Length);

            //挂起所有线程
            for (int ii = 0; ii < count; ii++)
            {
                IntPtr ptr = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)threadcollection[ii].Id);
                SuspendThread(ptr);
                CloseHandle(ptr);
            }

            for (int i = 0; i < count; i++)
            {
                context[i].ContextFlags = (uint)CONTEXT_FLAGS.CONTEXT_ALL;
                IntPtr hThread = OpenThread(ThreadAccess.GET_CONTEXT, false, (uint)(threadcollection[i].Id));
                Console.WriteLine(hThread);
                string stringhandle = Convert.ToString(hThread);
                char[] charhandle = stringhandle.ToCharArray();
                byte[] byteahandle = new byte[stringhandle.Length+1];
                byteahandle[0] = Convert.ToByte(stringhandle.Length);
                for (int bi = 0; bi < stringhandle.Length; bi++)
                {
                    byteahandle[bi + 1] = Convert.ToByte(charhandle[bi]);
                }
                //写入线程句柄
                stream.Write(byteahandle, 0, byteahandle.Length);

                if (GetThreadContext(hThread, ref context[i]))
                {
                    Console.WriteLine("Ebp    : {0}", context[i].Ebp);
                    Console.WriteLine("Eip    : {0}", context[i].Eip);
                    Console.WriteLine("SegCs  : {0}", context[i].SegCs);
                    Console.WriteLine("EFlags : {0}", context[i].EFlags);
                    Console.WriteLine("Esp    : {0}", context[i].Esp);
                    Console.WriteLine("SegSs  : {0}", context[i].SegSs);
                     
                    byte[] bydata = Serialize(context[i]);
                    //写入上写文
                    stream.Write(bydata, 0, bydata.Length);
                }
                else
                {
                    Console.WriteLine("A problem occurred!");
                }
                CloseHandle(hThread);
            }

            stream.Close();
            Console.WriteLine("read over!");

            //dump内存
            DumpProcMemory(pid, filename);

            //恢复线程
            for (int iii = 0; iii < count; iii++)
            {
                IntPtr ptrr = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)threadcollection[iii].Id);
                ResumeThread(ptrr);
                CloseHandle(ptrr);
            }

            return 0;
        }

        public int DumpProcMemory(int pid, string filename)
        {
            //打开进程
            IntPtr handle = OpenProcess(ProcessAccessFlags.QueryInformation
                                        | ProcessAccessFlags.VMOperation
                                        | ProcessAccessFlags.VMRead
                                        | ProcessAccessFlags.VMWrite, false, pid);

            SYSTEM_INFO systeminfo = new SYSTEM_INFO();
            GetSystemInfo(out systeminfo);

            System.IO.FileStream stream = null;
           
            stream = new System.IO.FileStream(filename, System.IO.FileMode.Append);

            long MaxAddress = (long)systeminfo.lpMaximumApplicationAddress;
            long address = 0;
            int countcount = 0;
            do
            {
                MEMORY_BASIC_INFORMATION memory;
                int result = VirtualQueryEx(handle, (IntPtr)address, out memory, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if (address == (long)memory.BaseAddress + (long)memory.RegionSize)
                    break;
                //保存具有读写属性且已提交的虚存
                if (memory.State == (uint)StateEnum.MEM_COMMIT)
                {
                    switch (memory.AllocationProtect)
                    {
                        case (uint)AllocationProtect.PAGE_READWRITE:
                            byte[] buffer = new byte[(int)memory.RegionSize];
                            IntPtr byteread;
                            ReadProcessMemory(handle, memory.BaseAddress, buffer, (int)memory.RegionSize, out byteread);
                            //写入文件
                            stream.Write(buffer, 0, buffer.Length);
                            countcount++;
                            break;
                        default:
                            break;
                    }
                }
                address = (long)memory.BaseAddress + (long)memory.RegionSize;
            }
            while (address <= MaxAddress);

            stream.Close();
            CloseHandle(handle);
            Console.WriteLine("read over!");
            Console.WriteLine(countcount);
            return 0;
        }

        public int ResumeProcState(int pid, string filename)
        {
            AllocConsole();
            //建文件流
            System.IO.FileStream stream = null;

            stream = new System.IO.FileStream(filename, System.IO.FileMode.Open);

            byte[] descriptioncount = new byte[1];
            //读出exe文件路径长度
            stream.Read(descriptioncount, 0, descriptioncount.Length);
            int descount = Convert.ToInt32(descriptioncount[0]);
            char[] procdescription = new char[descount];
            byte[] bytedescription = new byte[descount];
            //读出exe文件路径
            stream.Read(bytedescription, 0, bytedescription.Length);
            for (int j = 0; j < descount; j++)
            { 
                procdescription[j] = Convert.ToChar(bytedescription[j]);
            }
            string description = new string(procdescription);
            
            //新建进程
            SECURITY_ATTRIBUTES process = new SECURITY_ATTRIBUTES();
            SECURITY_ATTRIBUTES thread = new SECURITY_ATTRIBUTES();
            STARTUPINFO info = new STARTUPINFO();
            PROCESS_INFORMATION proinfo = new PROCESS_INFORMATION();
            if (CreateProcess(
                null,
                description,
                ref process,
                ref thread,
                false,
                (uint)CreateProcessFlags.NORMAL_PRIORITY_CLASS,
                (IntPtr)null,
                null,
                ref info,
                out proinfo))
            {
                IntPtr prohandle = proinfo.hProcess;
                IntPtr thrhandle = proinfo.hThread;
                System.Threading.Thread.Sleep(5000);
                SuspendThread(thrhandle);
                //打印信息
                CONTEXT tt = new CONTEXT();
                tt.ContextFlags = (uint)CONTEXT_FLAGS.CONTEXT_ALL;
                GetThreadContext(thrhandle, ref tt);
                Console.WriteLine(thrhandle);
                Console.WriteLine("Ebp    : {0}", tt.Ebp);
                Console.WriteLine("Eip    : {0}", tt.Eip);
                Console.WriteLine("SegCs  : {0}", tt.SegCs);
                Console.WriteLine("EFlags : {0}", tt.EFlags);
                Console.WriteLine("Esp    : {0}", tt.Esp);
                Console.WriteLine("SegSs  : {0}", tt.SegSs);
                Console.WriteLine("Dr0    : {0}", tt.Dr0);
                Console.WriteLine("Dr1    : {0}", tt.Dr1);
                Console.WriteLine("Dr2    : {0}", tt.Dr2);
                Console.WriteLine("Dr3    : {0}", tt.Dr3);
                Console.WriteLine("Dr6    : {0}", tt.Dr6);
                Console.WriteLine("Dr7    : {0}", tt.Dr7);
                Console.WriteLine("SegGs    : {0}", tt.SegGs);
                Console.WriteLine("SegFs    : {0}", tt.SegFs);
                Console.WriteLine("Seges    : {0}", tt.SegEs);
                Console.WriteLine("SegDs    : {0}", tt.SegDs);
                Console.WriteLine("Edi     : {0}", tt.Edi);
                Console.WriteLine("Esi     : {0}", tt.Esi);
                Console.WriteLine("Ebx     : {0}", tt.Ebx);
                Console.WriteLine("Edx     : {0}", tt.Edx);
                Console.WriteLine("Ecx     : {0}", tt.Ecx);
                Console.WriteLine("Eax     : {0}", tt.Eax);
                ResumeThread(thrhandle);
                System.Threading.Thread.Sleep(5000);
                SuspendThread(thrhandle);
                CONTEXT ttt = new CONTEXT();
                ttt.ContextFlags = (uint)CONTEXT_FLAGS.CONTEXT_ALL;
                GetThreadContext(thrhandle, ref ttt);
                Console.WriteLine(thrhandle);
                Console.WriteLine("Ebp    : {0}", ttt.Ebp);
                Console.WriteLine("Eip    : {0}", ttt.Eip);
                Console.WriteLine("SegCs  : {0}", ttt.SegCs);
                Console.WriteLine("EFlags : {0}", ttt.EFlags);
                Console.WriteLine("Esp    : {0}", ttt.Esp);
                Console.WriteLine("SegSs  : {0}", ttt.SegSs);
                Console.WriteLine("Dr0    : {0}", ttt.Dr0);
                Console.WriteLine("Dr1    : {0}", ttt.Dr1);
                Console.WriteLine("Dr2    : {0}", ttt.Dr2);
                Console.WriteLine("Dr3    : {0}", ttt.Dr3);
                Console.WriteLine("Dr6    : {0}", ttt.Dr6);
                Console.WriteLine("Dr7    : {0}", ttt.Dr7);
                Console.WriteLine("SegGs    : {0}", ttt.SegGs);
                Console.WriteLine("SegFs    : {0}", ttt.SegFs);
                Console.WriteLine("Seges    : {0}", ttt.SegEs);
                Console.WriteLine("SegDs    : {0}", ttt.SegDs);
                Console.WriteLine("Edi     : {0}", ttt.Edi);
                Console.WriteLine("Esi     : {0}", ttt.Esi);
                Console.WriteLine("Ebx     : {0}", ttt.Ebx);
                Console.WriteLine("Edx     : {0}", ttt.Edx);
                Console.WriteLine("Ecx     : {0}", ttt.Ecx);
                Console.WriteLine("Eax     : {0}", ttt.Eax);
                //读取线程数
                byte[] bytecount = new byte[1];
                stream.Read(bytecount, 0, bytecount.Length);
                int count = Convert.ToInt32(bytecount[0]);
                int threadid = proinfo.dwThreadId;

                CONTEXT[] context = new CONTEXT[count];
                for (int i = 0; i < count; i++)
                {
                    //读取原先的线程句柄
                    byte[] byteAgohandlecount = new byte[1];
                    stream.Read(byteAgohandlecount, 0, byteAgohandlecount.Length);
                    int Agohandlecount = Convert.ToInt32(byteAgohandlecount[0]);
                    Console.WriteLine(Agohandlecount);
                    byte[] byteAgohandle = new byte[Agohandlecount];
                    stream.Read(byteAgohandle, 0, byteAgohandle.Length);
                    char[] charAgohandle = new char[Agohandlecount];
                    for (int bi = 0; bi < Agohandlecount; bi++)
                    {
                        charAgohandle[bi] = Convert.ToChar(byteAgohandle[bi]);
                    }
                    string stringAgohandle = new string(charAgohandle);
                    IntPtr Agohandle = (IntPtr)Convert.ToInt32(stringAgohandle);
                    Console.WriteLine(Agohandle);
                    //读上下文
                    byte[] bydata = new byte[Marshal.SizeOf(context[i])];
                    stream.Read(bydata, 0, bydata.Length);
                    context[i] = Deserialize(bydata);
                    
                    Console.WriteLine("Ebp    : {0}", context[i].Ebp);
                    Console.WriteLine("Eip    : {0}", context[i].Eip);
                    Console.WriteLine("SegCs  : {0}", context[i].SegCs);
                    Console.WriteLine("EFlags : {0}", context[i].EFlags);
                    Console.WriteLine("Esp    : {0}", context[i].Esp);
                    Console.WriteLine("SegSs  : {0}", context[i].SegSs);
                    Console.WriteLine("Dr0    : {0}", context[i].Dr0);
                    Console.WriteLine("Dr1    : {0}", context[i].Dr1);
                    Console.WriteLine("Dr2    : {0}", context[i].Dr2);
                    Console.WriteLine("Dr3    : {0}", context[i].Dr3);
                    Console.WriteLine("Dr6    : {0}", context[i].Dr6);
                    Console.WriteLine("Dr7    : {0}", context[i].Dr7);
                    Console.WriteLine("SegGs    : {0}", context[i].SegGs);
                    Console.WriteLine("SegFs    : {0}", context[i].SegFs);
                    Console.WriteLine("Seges    : {0}", context[i].SegEs);
                    Console.WriteLine("SegDs    : {0}", context[i].SegDs);
                    Console.WriteLine("Edi     : {0}", context[i].Edi);
                    Console.WriteLine("Esi     : {0}", context[i].Esi);
                    Console.WriteLine("Ebx     : {0}", context[i].Ebx);
                    Console.WriteLine("Edx     : {0}", context[i].Edx);
                    Console.WriteLine("Ecx     : {0}", context[i].Ecx);
                    Console.WriteLine("Eax     : {0}", context[i].Eax);

                    context[i].ContextFlags = (uint)CONTEXT_FLAGS.CONTEXT_ALL;
                    IntPtr Nowhandle = OpenThread(ThreadAccess.SET_CONTEXT, false, (uint)threadid);
                   // Console.WriteLine(Nowhandle);
                   // context[i] = HandleToHandle(Agohandle, Nowhandle, context[i]);

                    tt.Eax = context[i].Eax;
                    tt.Ebx = context[i].Ebx;
                    tt.Ecx = context[i].Ecx;
                    tt.Edx = context[i].Edx;

                    //tt.Ebp = context[i].Ebp;
                    //tt.Esp = context[i].Esp;
                    //tt.Esi = context[i].Esi;

                    //SetThreadContext(Agohandle, ref tt);
                    CloseHandle(Nowhandle);
                    //建立新线程
                    if (i + 1 < count)
                    {
                        IntPtr lpThreadID = (IntPtr)0;
                        StartThread threadfunc = null;
                        ThreadStartDelegate threadFunc = null;
                        unsafe
                        {
                            thrhandle = CreateRemoteThread(prohandle, (IntPtr)null, 0, threadFunc, (IntPtr)null, 
                                (uint)CreateProcessFlags.CREATE_SUSPENDED, lpThreadID);
                        }
                        threadid = (int)lpThreadID;
                    }
                }

               // ResumeThread(thrhandle);

               //读入内存状态
                SYSTEM_INFO systeminfo = new SYSTEM_INFO();
                GetSystemInfo(out systeminfo);

                long MaxAddress = (long)systeminfo.lpMaximumApplicationAddress;
                long address = 0;
                int countcount = 0;
                do
                {
                    MEMORY_BASIC_INFORMATION memory;
                    int result = VirtualQueryEx(prohandle, (IntPtr)address, out memory, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                    if (address == (long)memory.BaseAddress + (long)memory.RegionSize)
                        break;
                    if (memory.State == (uint)StateEnum.MEM_COMMIT)
                    {
                        switch (memory.AllocationProtect)
                        {
                            case (uint)AllocationProtect.PAGE_READWRITE:
                                byte[] buffer = new byte[(int)memory.RegionSize];
                                Console.WriteLine("now");
                                Console.WriteLine(memory.BaseAddress);
                                Console.WriteLine(memory.AllocationBase);
                                Console.WriteLine(memory.RegionSize);
                                Console.WriteLine(memory.Type);
                                Console.WriteLine(memory.Protect);
                                stream.Read(buffer, 0, buffer.Length);
                                UIntPtr byteread;
                                WriteProcessMemory(prohandle, memory.BaseAddress, buffer, (uint)memory.RegionSize, out byteread);
                                Console.WriteLine("ago");
                                Console.WriteLine(memory.BaseAddress);
                                Console.WriteLine(memory.AllocationBase);
                                Console.WriteLine(memory.RegionSize);
                                Console.WriteLine(memory.Type);
                                Console.WriteLine(memory.Protect);
                                countcount++;
                                break;
                            default:
                                break;
                        }
                    }
                    address = (long)memory.BaseAddress + (long)memory.RegionSize;
                }
                while (address <= MaxAddress);

                stream.Close();
                CloseHandle(prohandle);
                Console.WriteLine("write over!");
                Console.WriteLine(countcount);
             
            }
           
            //恢复线程运行
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetProcessById(proinfo.dwProcessId);
            System.Diagnostics.ProcessThread[] processthread = new System.Diagnostics.ProcessThread[500];
            System.Diagnostics.ProcessThreadCollection threadcollection = new System.Diagnostics.ProcessThreadCollection(processthread);
            threadcollection = proc.Threads;
            for (int k = 0; k < threadcollection.Count; k++)
            {
                IntPtr ptrr = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)threadcollection[k].Id);
                ResumeThread(ptrr);
                CloseHandle(ptrr);
            }
                return 0;
        }

        public CONTEXT HandleToHandle(IntPtr Agohandle, IntPtr Nowhandle, CONTEXT context)
        {
            uint offset = (uint)Nowhandle - (uint)Agohandle;
            context.Ebp = context.Ebp + offset;
            context.Eip = context.Eip + offset;
            context.SegCs = context.SegCs + offset;
            context.EFlags = context.EFlags + offset;
            context.Esp = context.Esp + offset;
            context.SegSs = context.SegSs + offset;

            return context;
        }

        //结构体序列化
        public byte[] Serialize(object obj)
        {
            int size = Marshal.SizeOf(obj);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, buffer, false);
            byte[] datas = new byte[size];
            Marshal.Copy(buffer, datas, 0, size);
            Marshal.FreeHGlobal(buffer);
            return datas;
        }

        //反序列化结构体
        public CONTEXT Deserialize(byte[] datas)
        {
            Type anytype = typeof(CONTEXT);
            int size = Marshal.SizeOf(anytype);
            if (size > datas.Length)
                return new CONTEXT();
            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.Copy(datas, 0, buffer, size);
            object retobj = Marshal.PtrToStructure(buffer, anytype);
            Marshal.FreeHGlobal(buffer);
            return (CONTEXT)retobj;
        }
    }
}
