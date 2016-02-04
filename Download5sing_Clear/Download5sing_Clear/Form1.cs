using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Download5sing_Clear
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox3.Text = Environment.CurrentDirectory;
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
                label1.Text = "无法获取登录状态";
            else if (numericUpDown1.Value > numericUpDown2.Value)
                label1.Text = "起始编号需小于等于终止编号";
            else if (textBox3.Text == "")
                label1.Text = "无效的本地路径";
            else
            {
                Clear clear = new Clear((int)numericUpDown1.Value, (int)numericUpDown2.Value, textBox3.Text);
                clear.DoClear();
                label1.Text = "清除完毕";
            }
        }
    }
}
