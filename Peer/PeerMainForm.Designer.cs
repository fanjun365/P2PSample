namespace FanJun.P2PSample.Peer
{
    partial class PeerMainForm
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
            this.btnSend = new System.Windows.Forms.Button();
            this.rtbSend = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rtbReceived = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoUDP = new System.Windows.Forms.RadioButton();
            this.rdoTCP = new System.Windows.Forms.RadioButton();
            this.btnCheck = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtServerMinorPort = new System.Windows.Forms.TextBox();
            this.txtServerPrimaryPort = new System.Windows.Forms.TextBox();
            this.txtUserPwd = new System.Windows.Forms.TextBox();
            this.txtServerAddress = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(501, 78);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(55, 305);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "&Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // rtbSend
            // 
            this.rtbSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbSend.Location = new System.Drawing.Point(0, 0);
            this.rtbSend.Name = "rtbSend";
            this.rtbSend.Size = new System.Drawing.Size(483, 63);
            this.rtbSend.TabIndex = 1;
            this.rtbSend.Text = "";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(12, 78);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rtbReceived);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rtbSend);
            this.splitContainer1.Size = new System.Drawing.Size(483, 305);
            this.splitContainer1.SplitterDistance = 238;
            this.splitContainer1.TabIndex = 2;
            // 
            // rtbReceived
            // 
            this.rtbReceived.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbReceived.Location = new System.Drawing.Point(0, 0);
            this.rtbReceived.Name = "rtbReceived";
            this.rtbReceived.ReadOnly = true;
            this.rtbReceived.Size = new System.Drawing.Size(483, 238);
            this.rtbReceived.TabIndex = 2;
            this.rtbReceived.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoUDP);
            this.groupBox1.Controls.Add(this.rdoTCP);
            this.groupBox1.Controls.Add(this.btnCheck);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Controls.Add(this.btnLogin);
            this.groupBox1.Controls.Add(this.txtServerMinorPort);
            this.groupBox1.Controls.Add(this.txtServerPrimaryPort);
            this.groupBox1.Controls.Add(this.txtUserPwd);
            this.groupBox1.Controls.Add(this.txtServerAddress);
            this.groupBox1.Controls.Add(this.txtUser);
            this.groupBox1.Location = new System.Drawing.Point(12, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(544, 69);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // rdoUDP
            // 
            this.rdoUDP.AutoSize = true;
            this.rdoUDP.Location = new System.Drawing.Point(408, 41);
            this.rdoUDP.Name = "rdoUDP";
            this.rdoUDP.Size = new System.Drawing.Size(41, 16);
            this.rdoUDP.TabIndex = 3;
            this.rdoUDP.Text = "UDP";
            this.rdoUDP.UseVisualStyleBackColor = true;
            // 
            // rdoTCP
            // 
            this.rdoTCP.AutoSize = true;
            this.rdoTCP.Checked = true;
            this.rdoTCP.Location = new System.Drawing.Point(408, 19);
            this.rdoTCP.Name = "rdoTCP";
            this.rdoTCP.Size = new System.Drawing.Size(41, 16);
            this.rdoTCP.TabIndex = 3;
            this.rdoTCP.TabStop = true;
            this.rdoTCP.Text = "TCP";
            this.rdoTCP.UseVisualStyleBackColor = true;
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(489, 15);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(55, 46);
            this.btnCheck.TabIndex = 2;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.PaleTurquoise;
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConnect.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.btnConnect.Location = new System.Drawing.Point(273, 15);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(129, 46);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "&Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.DarkSlateGray;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.ForeColor = System.Drawing.Color.PowderBlue;
            this.btnLogin.Location = new System.Drawing.Point(218, 15);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(49, 46);
            this.btnLogin.TabIndex = 1;
            this.btnLogin.Text = "&Login";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtServerMinorPort
            // 
            this.txtServerMinorPort.BackColor = System.Drawing.Color.LightGray;
            this.txtServerMinorPort.Location = new System.Drawing.Point(164, 15);
            this.txtServerMinorPort.Name = "txtServerMinorPort";
            this.txtServerMinorPort.Size = new System.Drawing.Size(48, 21);
            this.txtServerMinorPort.TabIndex = 0;
            this.txtServerMinorPort.Text = "6665";
            // 
            // txtServerPrimaryPort
            // 
            this.txtServerPrimaryPort.BackColor = System.Drawing.Color.LightGray;
            this.txtServerPrimaryPort.Location = new System.Drawing.Point(112, 15);
            this.txtServerPrimaryPort.Name = "txtServerPrimaryPort";
            this.txtServerPrimaryPort.Size = new System.Drawing.Size(48, 21);
            this.txtServerPrimaryPort.TabIndex = 0;
            this.txtServerPrimaryPort.Text = "6666";
            // 
            // txtUserPwd
            // 
            this.txtUserPwd.BackColor = System.Drawing.Color.LightGray;
            this.txtUserPwd.Location = new System.Drawing.Point(112, 40);
            this.txtUserPwd.Name = "txtUserPwd";
            this.txtUserPwd.PasswordChar = '*';
            this.txtUserPwd.Size = new System.Drawing.Size(100, 21);
            this.txtUserPwd.TabIndex = 0;
            this.txtUserPwd.Text = "123";
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.BackColor = System.Drawing.Color.LightGray;
            this.txtServerAddress.Location = new System.Drawing.Point(6, 15);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.Size = new System.Drawing.Size(100, 21);
            this.txtServerAddress.TabIndex = 0;
            this.txtServerAddress.Text = "218.70.26.146";
            // 
            // txtUser
            // 
            this.txtUser.BackColor = System.Drawing.Color.LightGray;
            this.txtUser.Location = new System.Drawing.Point(6, 40);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(100, 21);
            this.txtUser.TabIndex = 0;
            this.txtUser.Text = "u01";
            // 
            // PeerMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 395);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnSend);
            this.Name = "PeerMainForm";
            this.Text = "Peer";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.RichTextBox rtbSend;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox rtbReceived;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtUserPwd;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtServerPrimaryPort;
        private System.Windows.Forms.TextBox txtServerAddress;
        private System.Windows.Forms.TextBox txtServerMinorPort;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.RadioButton rdoTCP;
        private System.Windows.Forms.RadioButton rdoUDP;
    }
}

