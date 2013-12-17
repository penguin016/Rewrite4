using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Rewrite4
{
    public partial class Form1 : Form
    {
        public string processname;
        public string username;
        public string cpuperformance;
        public string memory;
        public string description;
        public string processid;

        public Form1()
        {
            InitializeComponent();
            processname = null;
            username = null;
            cpuperformance = null;
            memory = null;
            description = null;
            processid = null;
        }

        public bool getDoubleBuffer()
        {
            return base.DoubleBuffered;
        }

        public void setDoubleBuffer(bool value)
        {
            base.DoubleBuffered = value;
        }
    
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (processname != null &&
                username != null &&
                cpuperformance != null &&
                memory != null &&
                description != null &&
                processid != null)
            {
                //弹出对话框
                Process process = Process.GetProcessById(Convert.ToInt32(processid));
                int pid = Convert.ToInt32(processid);
                SaveFileDialog save = new SaveFileDialog();
                save.Title = "文件另存为";
                save.InitialDirectory = @"C:\Users\wsx\Documents\Visual Studio 2012\Projects";
                save.FileName = processname + "_" + processid;
                save.Filter = "上下文文件(*.context)|*.context";
                save.DefaultExt = "context";
                save.OverwritePrompt = true;
                save.RestoreDirectory = true;
                save.OverwritePrompt = true;

               if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = save.FileName.ToString();
 
                   checkpoint check = new checkpoint();

                   check.DumpProcState(pid, path);

                   processname = null;
                   username = null;
                   cpuperformance = null;
                   memory = null;
                   description = null;
                   processid = null;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Nothing available!");
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection collection = this.listView1.SelectedItems;
            foreach (ListViewItem item in collection)
            {
                processname = item.SubItems[0].Text;
                username = item.SubItems[1].Text;
                cpuperformance = item.SubItems[2].Text;
                memory = item.SubItems[3].Text;
                description = item.SubItems[4].Text;
                processid = item.SubItems[5].Text;
            }
        }

        private void process1_Exited(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (processname != null &&
                username != null &&
                cpuperformance != null &&
                memory != null &&
                description != null &&
                processid != null)
            {
                Process[] process = Process.GetProcessesByName(processname);
                for (int i = 0; i < process.Length; i++)
                {
                    try
                    {
                        process[i].Kill();
                    }
                    catch
                    {

                    }
                }
                processname = null;
                username = null;
                cpuperformance = null;
                memory = null;
                description = null;
                processid = null;
            }
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
        
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.AddExtension = true;
            open.Filter = "上下文文件(*.context)|*.context";
            open.DefaultExt = "context";
            open.RestoreDirectory = true;
            open.Multiselect = false;
            open.InitialDirectory = @"C:\Users\wsx\Documents\Visual Studio 2012\Projects";
            open.Title = "选择一个上下文文件";

            checkpoint check = new checkpoint();

            if (open.ShowDialog() == DialogResult.OK)
            {
                string path = open.FileName;
                int start = path.IndexOf('_');
                int end = path.IndexOf('.');
                int pid = Convert.ToInt32(path.Substring(start+1, end - start-1));
                check.ResumeProcState(pid, path);
            }

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void 预处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.AddExtension = true;
            open.Filter = "exe文件(*.exe)|*.exe";
            open.DefaultExt = "exe";
            open.RestoreDirectory = false;
            open.Multiselect = false;
            open.InitialDirectory = @"C:\Users\wsx\Desktop";
            open.Title = "选择一个exe文件";

            SetDLL setdll = new SetDLL();

            if (open.ShowDialog() == DialogResult.OK)
            {
                string path = open.FileName;
                
                setdll.Pretreatment(path);
            }
        }

        private void 新建任务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.AddExtension = true;
            open.Filter = "exe文件(*.exe)|*.exe|exe~文件(*.exe~)|*.exe~";
            open.DefaultExt = "exe";
            open.RestoreDirectory = true;
            open.Multiselect = false;
            open.InitialDirectory = @"C:\Users\wsx\Desktop";
            open.Title = "选择一个可执行文件";

            SetDLL setdll = new SetDLL();

            if (open.ShowDialog() == DialogResult.OK)
            {
                string path = open.FileName;

                setdll.NewTask(path);
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "Do you want exit this program?";
            string caption = "Exit";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
                this.Close();
        }
    }
}
