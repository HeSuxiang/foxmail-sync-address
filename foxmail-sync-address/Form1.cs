using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;

//检测进程使用Process
using System.Diagnostics;

namespace foxmail_sync_address
{
    public partial class Form1 : Form
    {
        //进程名
        private string FoxmailProcessName = "Foxmail";
        //文件名
        private string FoxmailAppName = "Foxmail.exe";
        

        //服务器路径
        string ServerPath = @"\\192.168.9.233\foxmail";

        //本地foxmail文件夹路径
        string FoxmailLocalPath = null;

        public Form1()
        {
            InitializeComponent();
        }

      
        private void Form1_Load(object sender, EventArgs e)
        {
            //防止多开

            //获取当前活动进程的模块名称
            string moduleName = Process.GetCurrentProcess().MainModule.ModuleName;
            //返回指定路径字符串的文件名
            string processName = System.IO.Path.GetFileNameWithoutExtension(moduleName);
            //根据文件名创建进程资源数组
            Process[] processes = Process.GetProcessesByName(processName);
            //如果该数组长度大于1，说明多次运行
            if (processes.Length > 1)
            {
                //MessageBox.Show("本程序一次只能运行一个实例！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);//弹出提示信息
                this.Close();//关闭当前窗体
            }


            //获取程序目录
            //初始化foxmail本地路径
            FoxmailLocalPath = System.Threading.Thread.GetDomain().BaseDirectory;
            //开始同步
            SyncFoxmailAddress();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            SyncFoxmailAddress();
        }


        //同步地址薄文件夹
        void SyncFoxmailAddress()
        {
            if (isFoxmailRuning())
            {
                ShowInfo("Foxmail运行中,本次不更新邮箱地址");
                MessageBox.Show("Foxmail运行中,本次不更新邮箱地址");

                //启动Foxmail
                Process.Start(FoxmailAppName);
                //退出程序
                Application.Exit();
            }


            try
            {
                //SharedTool tool = new SharedTool("foxmail", "zs2022", "192.168.9.233");
                //DebugSharedTool();
                ShowInfo("开始更新邮箱地址");

                SharedTool tool = new SharedTool("guest", "", "192.168.9.233");
                CopyFolder(ServerPath, FoxmailLocalPath);
                MessageBox.Show("Foxmail邮箱地址地址薄已自动更新");
                //更新后启动Foxmail
                Process.Start(FoxmailAppName);
            }
            catch (Exception ex)
            {
                ShowInfo("连接到服务器时错误:" + ex.Message);
                MessageBox.Show("连接到更新服务器时错误");
            }

            //退出程序
            Application.Exit();
        }

        //调试输出
        void DebugSharedTool()
        {
            var dicInfo = new DirectoryInfo(ServerPath);//选择的目录信息  

            DirectoryInfo[] dic = dicInfo.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
            foreach (DirectoryInfo temp in dic)
            {
                Console.WriteLine(temp.FullName);
            }

            Console.WriteLine("---------------------------");
            FileInfo[] textFiles = dicInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);//获取所有目录包含子目录下的文件  
            foreach (FileInfo temp in textFiles)
            {
                Console.WriteLine(temp.Name);
            }

        }


        //https://www.cnblogs.com/fps2tao/p/14965561.html
        //C#中复制文件夹及文件的两种方法
        //方法一:
        /// <summary>
        /// 复制文件夹及文件
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        public int CopyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                //如果目标路径不存在,则创建目标路径
                if (!System.IO.Directory.Exists(destFolder))
                {
                    System.IO.Directory.CreateDirectory(destFolder);
                }
                //得到原文件根目录下的所有文件
                string[] files = System.IO.Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = System.IO.Path.GetFileName(file);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    System.IO.File.Copy(file, dest, true);//复制文件, 允许覆盖同名的文件。
                }
                //得到原文件根目录下的所有文件夹
                string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = System.IO.Path.GetFileName(folder);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);//构建目标路径,递归复制文件
                }
                return 1;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                ShowInfo("复制文件时出错");
                //MessageBox.Show("更新邮箱地址出错");
                //抛出异常
                throw (exception);

            }

        }



        //检测运行进程
        bool isFoxmailRuning()
        {
            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
            {
                string info = "";
                try
                {
                    info = p.ProcessName;
                    if (FoxmailProcessName.Equals(info))
                    {
                        ShowInfo("检测到进程" + info);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    info = e.Message;
                }

            }
            return false;
        }


        //显示消息在文本框中
        private void ShowInfo(string msg)
        {
            msg += "\r\n";
            //textBox4.Text = msg + textBox4.Text;
            textBox1.Text += msg;
        }

        //C#中实现文本框的滚动条自动滚到最底端
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();

        }

    }
}
