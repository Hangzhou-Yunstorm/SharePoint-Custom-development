using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 大华RMS加密服务
{
    /// <summary>
    /// 重写TextBox，禁止右键复制粘贴
    /// </summary>
    public class TextBoxEx : System.Windows.Forms.TextBox
    {
        //
        //构造函数默认的
        //
        protected override void WndProc(ref   Message m)
        {
            if (m.Msg != 0x007B && m.Msg != 0x0301 && m.Msg != 0x0302)
            {
                base.WndProc(ref m);
            }
        }

    } 
}
