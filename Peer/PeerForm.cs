using LumiSoft.Net.TCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace FanJun.P2PSample.Peer
{
    public partial class PeerForm : Form
    {
        private TCP_Client m_tcpClient;

        public PeerForm()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (m_tcpClient == null)
            {
                m_tcpClient = new TCP_Client();
                m_tcpClient.Connect("192.168.100.169", 6665);
            }
            this.txtLocalEndPoint.Text = m_tcpClient.LocalEndPoint.ToString();
            this.txtRemoteEndPoint.Text = m_tcpClient.RemoteEndPoint.ToString();
        }

        private void btnConnectRemote_Click(object sender, EventArgs e)
        {
            if (m_tcpClient == null)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("192.168.100.169"), int.Parse(this.txtRemotePort.Text));
                m_tcpClient = new TCP_Client();
                m_tcpClient.Connect(remoteEP, false);
            }
            this.txtLocalEndPoint.Text = m_tcpClient.LocalEndPoint.ToString();
            this.txtRemoteEndPoint.Text = m_tcpClient.RemoteEndPoint.ToString();
        }
    }
}
