using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Timers;

namespace Rewrite4
{
    public static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        

        public static Form1 form = new Form1();

        public static void readdata()
        {
            readproc procinfo = new readproc();
            Process[] proclist = new Process[200];
            proclist = procinfo.getprocesses();

            for (int i = 0; i < proclist.Length; i++)
            {
                try
                {
                    System.Windows.Forms.ListViewItem listviewitem = new System.Windows.Forms.ListViewItem(new string[] {
                     proclist[i].ProcessName,
                     procinfo.GetProcessUserName(proclist[i].Id),
                     procinfo.GetCpuPerformance(proclist[i].ProcessName),
                     proclist[i].MainModule.ModuleMemorySize.ToString(),
                     proclist[i].MainModule.FileName,
                     Convert.ToString(proclist[i].Id)}, -1);
                    form.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[]{listviewitem});

                }
                catch
                {
                    continue;
                }
            }
            
        } 
       [STAThread]
       static void Main()
        {
            Application.EnableVisualStyles();
          // Application.SetCompatibleTextRenderingDefault
           // Application.SetCompatibleTextRenderingDefault(false);
            //Form1 form = new Form1();
            //Thread thread = new Thread(new ThreadStart(readdata));
            //thread.Start();

            System.Timers.Timer aTimer = new System.Timers.Timer(1000);
            //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
           // aTimer.Interval = 10000;
           // aTimer.Enabled = true;
           // form.setDoubleBuffer(true);
            readdata();
            Application.Run(form);
        }

       private static void OnTimedEvent(object source, ElapsedEventArgs e)
       {
           readproc procinfo = new readproc();
           Process[] proclist = new Process[200];
           proclist = procinfo.getprocesses();

           //System.Windows.Forms.ListViewItem[] listviewitem1 = null;

           //form.listView1.BeginUpdate();
           form.listView1.Items.Clear();
           for (int i = 0; i < proclist.Length; i++)
           {
               try
               {
                   System.Windows.Forms.ListViewItem listviewitem = new System.Windows.Forms.ListViewItem(new string[] {
                     proclist[i].ProcessName,
                     procinfo.GetProcessUserName(proclist[i].Id),
                     procinfo.GetCpuPerformance(proclist[i].ProcessName),
                     proclist[i].MainModule.ModuleMemorySize.ToString(),
                     proclist[i].MainModule.FileName}, -1);
                    
                   /*listviewitem1[i] = new System.Windows.Forms.ListViewItem(new string[] {
                     proclist[i].ProcessName,
                     procinfo.GetProcessUserName(proclist[i].Id),
                     procinfo.GetCpuPerformance(proclist[i].ProcessName),
                     proclist[i].MainModule.ModuleMemorySize.ToString(),
                     proclist[i].MainModule.FileName}, -1);
                   */
                   form.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[]{listviewitem});

               }
               catch
               {
                   continue;
               }
           }
           //form.listView1.Items.AddRange(listviewitem1);
           //form.listView1.EndUpdate();
       }

    }
}