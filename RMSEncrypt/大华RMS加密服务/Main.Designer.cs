namespace 大华RMS加密服务
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lkbViewHistory = new System.Windows.Forms.LinkLabel();
            this.lkbViewError = new System.Windows.Forms.LinkLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tslRunTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.labTotalCount = new System.Windows.Forms.Label();
            this.labRightCount = new System.Windows.Forms.Label();
            this.labErrorCount = new System.Windows.Forms.Label();
            this.labRunCount = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.显示加密窗口ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.隐藏加密窗口ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查看加密记录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查看加密任务ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查看错误日志ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出加密服务ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lkbViewTask = new System.Windows.Forms.LinkLabel();
            this.tbxTimeSpan = new 大华RMS加密服务.TextBoxEx();
            this.label1 = new System.Windows.Forms.Label();
            this.labCurrentStatus = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "加密总数：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "加密成功：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "加密出错：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "执行次数：";
            // 
            // lkbViewHistory
            // 
            this.lkbViewHistory.AutoSize = true;
            this.lkbViewHistory.Location = new System.Drawing.Point(33, 303);
            this.lkbViewHistory.Name = "lkbViewHistory";
            this.lkbViewHistory.Size = new System.Drawing.Size(77, 12);
            this.lkbViewHistory.TabIndex = 5;
            this.lkbViewHistory.TabStop = true;
            this.lkbViewHistory.Text = "查看加密历史";
            this.lkbViewHistory.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lkbViewHistory_LinkClicked);
            // 
            // lkbViewError
            // 
            this.lkbViewError.AutoSize = true;
            this.lkbViewError.Location = new System.Drawing.Point(33, 393);
            this.lkbViewError.Name = "lkbViewError";
            this.lkbViewError.Size = new System.Drawing.Size(77, 12);
            this.lkbViewError.TabIndex = 6;
            this.lkbViewError.TabStop = true;
            this.lkbViewError.Text = "查看错误日志";
            this.lkbViewError.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lkbViewError_LinkClicked);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslRunTime});
            this.statusStrip1.Location = new System.Drawing.Point(0, 459);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(408, 22);
            this.statusStrip1.TabIndex = 7;
            // 
            // tslRunTime
            // 
            this.tslRunTime.Name = "tslRunTime";
            this.tslRunTime.Size = new System.Drawing.Size(104, 17);
            this.tslRunTime.Text = "加密服务正在运行";
            // 
            // labTotalCount
            // 
            this.labTotalCount.AutoSize = true;
            this.labTotalCount.Location = new System.Drawing.Point(104, 33);
            this.labTotalCount.Name = "labTotalCount";
            this.labTotalCount.Size = new System.Drawing.Size(23, 12);
            this.labTotalCount.TabIndex = 8;
            this.labTotalCount.Text = "0个";
            // 
            // labRightCount
            // 
            this.labRightCount.AutoSize = true;
            this.labRightCount.Location = new System.Drawing.Point(104, 78);
            this.labRightCount.Name = "labRightCount";
            this.labRightCount.Size = new System.Drawing.Size(23, 12);
            this.labRightCount.TabIndex = 9;
            this.labRightCount.Text = "0个";
            // 
            // labErrorCount
            // 
            this.labErrorCount.AutoSize = true;
            this.labErrorCount.Location = new System.Drawing.Point(104, 123);
            this.labErrorCount.Name = "labErrorCount";
            this.labErrorCount.Size = new System.Drawing.Size(23, 12);
            this.labErrorCount.TabIndex = 10;
            this.labErrorCount.Text = "0个";
            // 
            // labRunCount
            // 
            this.labRunCount.AutoSize = true;
            this.labRunCount.Location = new System.Drawing.Point(104, 168);
            this.labRunCount.Name = "labRunCount";
            this.labRunCount.Size = new System.Drawing.Size(23, 12);
            this.labRunCount.TabIndex = 11;
            this.labRunCount.Text = "0个";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "大华RMS加密服务";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.显示加密窗口ToolStripMenuItem,
            this.隐藏加密窗口ToolStripMenuItem,
            this.查看加密记录ToolStripMenuItem,
            this.查看加密任务ToolStripMenuItem,
            this.查看错误日志ToolStripMenuItem,
            this.退出加密服务ToolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 136);
            // 
            // 显示加密窗口ToolStripMenuItem
            // 
            this.显示加密窗口ToolStripMenuItem.Name = "显示加密窗口ToolStripMenuItem";
            this.显示加密窗口ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.显示加密窗口ToolStripMenuItem.Text = "显示加密窗口";
            this.显示加密窗口ToolStripMenuItem.Click += new System.EventHandler(this.显示加密窗口ToolStripMenuItem_Click);
            // 
            // 隐藏加密窗口ToolStripMenuItem
            // 
            this.隐藏加密窗口ToolStripMenuItem.Name = "隐藏加密窗口ToolStripMenuItem";
            this.隐藏加密窗口ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.隐藏加密窗口ToolStripMenuItem.Text = "隐藏加密窗口";
            this.隐藏加密窗口ToolStripMenuItem.Click += new System.EventHandler(this.隐藏加密窗口ToolStripMenuItem_Click);
            // 
            // 查看加密记录ToolStripMenuItem
            // 
            this.查看加密记录ToolStripMenuItem.Name = "查看加密记录ToolStripMenuItem";
            this.查看加密记录ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.查看加密记录ToolStripMenuItem.Text = "查看加密记录";
            this.查看加密记录ToolStripMenuItem.Click += new System.EventHandler(this.查看加密记录ToolStripMenuItem_Click);
            // 
            // 查看加密任务ToolStripMenuItem
            // 
            this.查看加密任务ToolStripMenuItem.Name = "查看加密任务ToolStripMenuItem";
            this.查看加密任务ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.查看加密任务ToolStripMenuItem.Text = "查看加密任务";
            this.查看加密任务ToolStripMenuItem.Click += new System.EventHandler(this.查看加密任务ToolStripMenuItem_Click);
            // 
            // 查看错误日志ToolStripMenuItem
            // 
            this.查看错误日志ToolStripMenuItem.Name = "查看错误日志ToolStripMenuItem";
            this.查看错误日志ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.查看错误日志ToolStripMenuItem.Text = "查看错误日志";
            this.查看错误日志ToolStripMenuItem.Click += new System.EventHandler(this.查看错误日志ToolStripMenuItem_Click);
            // 
            // 退出加密服务ToolStripMenuItem1
            // 
            this.退出加密服务ToolStripMenuItem1.Name = "退出加密服务ToolStripMenuItem1";
            this.退出加密服务ToolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
            this.退出加密服务ToolStripMenuItem1.Text = "退出加密服务";
            this.退出加密服务ToolStripMenuItem1.Click += new System.EventHandler(this.退出加密服务ToolStripMenuItem1_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(33, 213);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 12;
            this.label9.Text = "加密间隔：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(212, 213);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 14;
            this.label10.Text = "分钟";
            // 
            // lkbViewTask
            // 
            this.lkbViewTask.AutoSize = true;
            this.lkbViewTask.Location = new System.Drawing.Point(33, 348);
            this.lkbViewTask.Name = "lkbViewTask";
            this.lkbViewTask.Size = new System.Drawing.Size(77, 12);
            this.lkbViewTask.TabIndex = 15;
            this.lkbViewTask.TabStop = true;
            this.lkbViewTask.Text = "查看加密任务";
            this.lkbViewTask.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lkbViewTask_LinkClicked);
            // 
            // tbxTimeSpan
            // 
            this.tbxTimeSpan.Location = new System.Drawing.Point(106, 208);
            this.tbxTimeSpan.Name = "tbxTimeSpan";
            this.tbxTimeSpan.Size = new System.Drawing.Size(100, 21);
            this.tbxTimeSpan.TabIndex = 13;
            this.tbxTimeSpan.Text = "1";
            this.tbxTimeSpan.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxTimeSpan_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 258);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 16;
            this.label1.Text = "当前状态：";
            // 
            // labCurrentStatus
            // 
            this.labCurrentStatus.AutoSize = true;
            this.labCurrentStatus.Location = new System.Drawing.Point(104, 258);
            this.labCurrentStatus.Name = "labCurrentStatus";
            this.labCurrentStatus.Size = new System.Drawing.Size(53, 12);
            this.labCurrentStatus.TabIndex = 17;
            this.labCurrentStatus.Text = "正在加密";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 481);
            this.Controls.Add(this.labCurrentStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lkbViewTask);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbxTimeSpan);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.labRunCount);
            this.Controls.Add(this.labErrorCount);
            this.Controls.Add(this.labRightCount);
            this.Controls.Add(this.labTotalCount);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lkbViewError);
            this.Controls.Add(this.lkbViewHistory);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "大华RMS加密服务";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel lkbViewHistory;
        private System.Windows.Forms.LinkLabel lkbViewError;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tslRunTime;
        private System.Windows.Forms.Label labTotalCount;
        private System.Windows.Forms.Label labRightCount;
        private System.Windows.Forms.Label labErrorCount;
        private System.Windows.Forms.Label labRunCount;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 显示加密窗口ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 查看加密记录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 查看错误日志ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出加密服务ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 隐藏加密窗口ToolStripMenuItem;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private TextBoxEx tbxTimeSpan;
        private System.Windows.Forms.LinkLabel lkbViewTask;
        private System.Windows.Forms.ToolStripMenuItem 查看加密任务ToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labCurrentStatus;
    }
}

