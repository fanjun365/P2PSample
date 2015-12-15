namespace FanJun.P2PSample.Peer
{
    partial class PeerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtLocalEndPoint = new System.Windows.Forms.TextBox();
            this.btnConnectRemote = new System.Windows.Forms.Button();
            this.txtRemotePort = new System.Windows.Forms.TextBox();
            this.txtRemoteEndPoint = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtLocalEndPoint
            // 
            this.txtLocalEndPoint.Location = new System.Drawing.Point(12, 41);
            this.txtLocalEndPoint.Name = "txtLocalEndPoint";
            this.txtLocalEndPoint.Size = new System.Drawing.Size(120, 21);
            this.txtLocalEndPoint.TabIndex = 1;
            // 
            // btnConnectRemote
            // 
            this.btnConnectRemote.Location = new System.Drawing.Point(118, 130);
            this.btnConnectRemote.Name = "btnConnectRemote";
            this.btnConnectRemote.Size = new System.Drawing.Size(164, 23);
            this.btnConnectRemote.TabIndex = 2;
            this.btnConnectRemote.Text = "ConnectRemote";
            this.btnConnectRemote.UseVisualStyleBackColor = true;
            this.btnConnectRemote.Click += new System.EventHandler(this.btnConnectRemote_Click);
            // 
            // txtRemotePort
            // 
            this.txtRemotePort.Location = new System.Drawing.Point(12, 130);
            this.txtRemotePort.Name = "txtRemotePort";
            this.txtRemotePort.Size = new System.Drawing.Size(100, 21);
            this.txtRemotePort.TabIndex = 3;
            // 
            // txtRemoteEndPoint
            // 
            this.txtRemoteEndPoint.Location = new System.Drawing.Point(12, 68);
            this.txtRemoteEndPoint.Name = "txtRemoteEndPoint";
            this.txtRemoteEndPoint.Size = new System.Drawing.Size(120, 21);
            this.txtRemoteEndPoint.TabIndex = 1;
            // 
            // PeerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 434);
            this.Controls.Add(this.txtRemotePort);
            this.Controls.Add(this.btnConnectRemote);
            this.Controls.Add(this.txtRemoteEndPoint);
            this.Controls.Add(this.txtLocalEndPoint);
            this.Controls.Add(this.btnConnect);
            this.Name = "PeerForm";
            this.Text = "PeerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtLocalEndPoint;
        private System.Windows.Forms.Button btnConnectRemote;
        private System.Windows.Forms.TextBox txtRemotePort;
        private System.Windows.Forms.TextBox txtRemoteEndPoint;
    }
}