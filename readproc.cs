using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;

namespace Rewrite4
{
    class readproc
    {
        
        public Process[] getprocesses()
        {
            Process[] proclist = new Process[200];
            proclist = Process.GetProcesses();
            return proclist;
        }

        public string GetProcessUserName(int pid)
        {
            string text1 = null;
            SelectQuery query = new SelectQuery("Select * from Win32_Process WHERE processID=" + pid);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            try
            {
                foreach (ManagementObject disk in searcher.Get())
                {
                    ManagementBaseObject inPar = null;
                    ManagementBaseObject outPar = null;

                    inPar = disk.GetMethodParameters("GetOwner");
                    outPar = disk.InvokeMethod("GetOwner", inPar, null);

                    text1 = outPar["User"].ToString();
                    break;
                }
            }
            catch
            {
                text1 = "SYSTEM";
            }
            return text1;
        }

        public string GetCpuPerformance(string processname)
        {
            PerformanceCounter cpuUsage = new PerformanceCounter("Process", "% Processor Time", processname);
            return Math.Round(cpuUsage.NextValue(), 2).ToString();
        }
    }
}
