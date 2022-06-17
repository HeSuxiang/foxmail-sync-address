using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

//检测进程使用Process
using System.Diagnostics;

namespace foxmail_sync_address
{
    public partial class Form1 : Form
    {

        public string FoxmailProcessName = "Foxmail";

        public Form1()
        {
            InitializeComponent();
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
