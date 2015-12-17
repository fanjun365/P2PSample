namespace FanJun.P2PSample.Server
{
    partial class ServerMainForm
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
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.linkClearLog = new System.Windows.Forms.LinkLabel();
            this.chkShowLog = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPrimaryPort = new System.Windows.Forms.TextBox();
            this.txtMinorPortTcp = new System.Windows.Forms.TextBox();
            this.txtMinorPortUdp = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbLog
            // 
            this.rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLog.Location = new System.Drawing.Point(12, 93);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(660, 248);
            this.rtbLog.TabIndex = 3;
            this.rtbLog.Text = "";
            // 
            // linkClearLog
            // 
            this.linkClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkClearLog.AutoSize = true;
            this.linkClearLog.Location = new System.Drawing.Point(625, 344);
            this.linkClearLog.Name = "linkClearLog";
            this.linkClearLog.Size = new System.Drawing.Size(47, 12);
            this.linkClearLog.TabIndex = 4;
            this.linkClearLog.TabStop = true;
            this.linkClearLog.Text = "清空(&C)";
            this.linkClearLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkClearLog_LinkClicked);
            // 
            // chkShowLog
            // 
            this.chkShowLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowLog.AutoSize = true;
            this.chkShowLog.Checked = true;
            this.chkShowLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowLog.Location = new System.Drawing.Point(12, 343);
            this.chkShowLog.Name = "chkShowLog";
            this.chkShowLog.Size = new System.Drawing.Size(72, 16);
            this.chkShowLog.TabIndex = 5;
            this.chkShowLog.Text = "显示日志";
            this.chkShowLog.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtPrimaryPort);
            this.groupBox1.Controls.Add(this.txtMinorPortUdp);
            this.groupBox1.Controls.Add(this.txtMinorPortTcp);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(660, 75);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(170, 16);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 48);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "启动";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "次端口";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "主端口";
            // 
            // txtPrimaryPort
            // 
            this.txtPrimaryPort.Location = new System.Drawing.Point(58, 16);
            this.txtPrimaryPort.Name = "txtPrimaryPort";
            this.txtPrimaryPort.Size = new System.Drawing.Size(106, 21);
            this.txtPrimaryPort.TabIndex = 3;
            this.txtPrimaryPort.Text = "6666";
            // 
            // txtMinorPortTcp
            // 
            this.txtMinorPortTcp.Location = new System.Drawing.Point(58, 43);
            this.txtMinorPortTcp.Name = "txtMinorPortTcp";
            this.txtMinorPortTcp.Size = new System.Drawing.Size(50, 21);
            this.txtMinorPortTcp.TabIndex = 4;
            this.txtMinorPortTcp.Text = "6665";
            // 
            // txtMinorPortUdp
            // 
            this.txtMinorPortUdp.Location = new System.Drawing.Point(114, 43);
            this.txtMinorPortUdp.Name = "txtMinorPortUdp";
            this.txtMinorPortUdp.Size = new System.Drawing.Size(50, 21);
            this.txtMinorPortUdp.TabIndex = 4;
            this.txtMinorPortUdp.Text = "6664";
            // 
            // ServerMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 361);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkShowLog);
            this.Controls.Add(this.linkClearLog);
            this.Controls.Add(this.rtbLog);
            this.Name = "ServerMainForm";
            this.Text = "P2P.Server";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.LinkLabel linkClearLog;
        private System.Windows.Forms.CheckBox chkShowLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPrimaryPort;
        private System.Windows.Forms.TextBox txtMinorPortTcp;
        private System.Windows.Forms.TextBox txtMinorPortUdp;
    }
}

