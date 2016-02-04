using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Download5sing
{
    public delegate void SetDownLoadStatus(object sender, AsyncCompletedEventArgs e);

    public partial class Form1 : Form
    {

        private int success, fail, total;

        public Form1()
        {
            InitializeComponent();
            textBox3.Text = Environment.CurrentDirectory;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = CatchCommand.Login(textBox1.Text, textBox2.Text).ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK
                && folderBrowserDialog1.SelectedPath.Trim() != "")
                textBox3.Text = folderBrowserDialog1.SelectedPath.Trim();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!CatchCommand.CheckCookie())
                textBox5.Text = "无法获取登录状态";
            else if (numericUpDown1.Value > numericUpDown2.Value)
                textBox5.Text = "起始编号需小于等于终止编号";
            else if (textBox3.Text == "")
                textBox5.Text = "无效的本地路径";
            else
            {
                button2.Enabled = false;
                List<String[]> list = CatchCommand.GetCollectList((int)numericUpDown1.Value-1, (int)numericUpDown2.Value-1);
                success = 0;
                fail = 0;
                total = (int)(numericUpDown2.Value - numericUpDown1.Value + 1);
                textBox5.Text = "下载中";
                textBox5.Update();
                foreach (String[] s in list)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(new Download(s[0], s[1], textBox3.Text, AddFinishNum).StartDownload), null);
                }
                timer1.Start();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!CatchCommand.CheckCookie())
                textBox5.Text = "无法获取登录状态";
            else if (numericUpDown1.Value > numericUpDown2.Value)
                textBox5.Text = "起始编号需小于等于终止编号";
            else if (textBox3.Text == "")
                textBox5.Text = "无效的本地路径";
            else
            {
                textBox5.Text = "检查未下载歌曲中\r\n";
                String rootPath = textBox3.Text;
                int index = (int)numericUpDown1.Value;
                List<String[]> list = CatchCommand.GetCollectList((int)numericUpDown1.Value - 1, (int)numericUpDown2.Value - 1);
                foreach (String[] s in list)
                {
                    if (!new Download(s[0], s[1], rootPath, AddFinishNum).CheckExist())
                    {
                        textBox5.Text += index + " " + s[1] + " " + s[0] + "\r\n";
                        textBox5.Update();
                    }
                    index++;
                }
                textBox5.Text = textBox5.Text.Replace("检查未下载歌曲中", "未下载歌曲检查完毕");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GetSong(false);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            GetSong(true);
        }

        private void GetSong(bool isSave)
        {
            if (!CatchCommand.CheckCookie())
            {
                label7.Text = "无法获取登录状态";
                return;
            }
            String id, type, url;
            url = textBox4.Text;
            if (url.Contains("yc"))
                type = "yc";
            else if (url.Contains("fc"))
                type = "fc";
            else if (url.Contains("bz"))
                type = "bz";
            else
            {
                label7.Text = "不正确的网址";
                return;
            }
            int index1 = url.LastIndexOf("/") + 1;
            int index2 = url.LastIndexOf(".");
            if (index1 == -1 || index2 == -1 || index1 > index2)
            {
                label7.Text = "不正确的网址";
                return;
            }
            id = url.Substring(index1, index2 - index1);

            SongInformation inf = CatchCommand.GetSongInformation(id, type);
            if (inf.fileName != "")
            {
                label7.Text = inf.authorName + " - " + inf.songName + "\n" + inf.fileName;
                if(isSave)
                {
                    if (textBox3.Text != "")
                    {
                        inf = CatchCommand.AddDownloadPath(inf, textBox3.Text);
                        CatchCommand.DownloadSong(inf, new AsyncCompletedEventHandler(DownloadFinish));
                        label7.Text = inf.path + " - " + inf.file + "下载中";
                    }
                    else
                        label7.Text = "无效的网络路径或本地路径";
                }
            }
            else
                label7.Text = "获取时出错";
        }

        public void DownloadFinish(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
                label7.Text = "下载失败";
            else
                label7.Text = "已下载到" + textBox3.Text;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (success != 0 || fail != 0)
                textBox5.Text = "下载成功" + success + "首\r\n下载失败" + fail + "首\r\n";
            if (success + fail >= total)
            {
                success = 0;
                fail = 0;
                total = 0;
                timer1.Stop();
                button2.Enabled = true;
                textBox5.Text += "下载完成";
            }
        }

        public void AddFinishNum(bool isSuccess)
        {
            if (isSuccess)
                success++;
            else
                fail++;
        }

    }
}
