using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MailHelper mail = MailHelper.GetMailHelper("ltbluear@hotmail.com", "刘涛", "测试文件", "2017-8-23 17:00", "File was already encrypted by using RMS Protocol before, do not support");
            mail.Send();
        }
    }
}
