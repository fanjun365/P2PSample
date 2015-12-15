using LumiSoft.Net;
using LumiSoft.Net.TCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace FanJun.P2PSample.Server
{
    public partial class ServerForm : Form
    {
        private TCPServer m_tcpServer;

        public ServerForm()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (m_tcpServer == null)
            {
                m_tcpServer = new TCPServer();
                m_tcpServer.SessionCreated += m_tcpServer_SessionCreated;
                m_tcpServer.Bindings = new IPBindInfo[] { new IPBindInfo("192.168.100.169", BindInfoProtocol.TCP, IPAddress.Any, 6665) };
            }
            m_tcpServer.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TCP_ServerSession[] array = m_tcpServer.Sessions.ToArray();
        }


        void m_tcpServer_SessionCreated(object sender, TCP_ServerSessionEventArgs<TCPServerSession> e)
        {
            //e.Session..PacketReceived += new TCP_ServerSession.PacketReceivedEventHandler(fileSession_PacketReceived);
            //e.Session.IsAuthenticated = true;
        }



        public TCP_ServerSession[] GetTcpServerSession()
        {
            return m_tcpServer.Sessions.ToArray();
        }
        public void BroadcastingMessage(string msg)
        {
            m_tcpServer.BroadcastingMessage(msg);
        }
    }
}
