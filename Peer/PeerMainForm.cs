using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZLBase.Communicate;

namespace FanJun.P2PSample.Peer
{
    public partial class PeerMainForm : Form
    {
        private ZLBase.Communicate.CommunicateProxy m_tcpClient;

        public PeerMainForm()
        {
            InitializeComponent();

            this.txtServerAddress.Text = System.Configuration.ConfigurationManager.AppSettings["ServerAddress"];
            this.txtUser.Text = System.Configuration.ConfigurationManager.AppSettings["User"];
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string address = this.txtServerAddress.Text.Trim();
            if (string.IsNullOrEmpty(address))
                return;
            int portPrimary, portMinor;
            if (!int.TryParse(this.txtServerPrimaryPort.Text.Trim(), out portPrimary))
                return;
            if (!int.TryParse(this.txtServerMinorPort.Text.Trim(), out portMinor))
                return;

            //IPAddress[] ma = Dns.GetHostAddresses(address);
            //IPEndPoint ipe = new IPEndPoint(ma[0], port);
            //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //socket.Connect(ipe);
            //Thread.Sleep(3000);
            //socket.Disconnect(false);
            //socket.Shutdown(SocketShutdown.Both);
            //socket.Close();
            //return;

            if (this.m_tcpClient == null)
            {
                this.m_tcpClient = new ZLBase.Communicate.CommunicateProxy(address, portPrimary, -1);
                this.m_tcpClient.MessageReceived += new EventHandler<CustomEventArgs<object>>(m_tcpClient_MessageReceived);
                this.m_tcpClient.ErrorRecived += new EventHandler<CustomEventArgs<string>>(m_tcpClient_ErrorRecived);
                this.m_tcpClient.SessionKicked += new EventHandler<CustomEventArgs<string>>(m_tcpClient_SessionKicked);
                this.m_tcpClient.ServerConnectStateChanged +=
                    new EventHandler<CustomEventArgs<bool>>(m_tcpClient_ServerConnectStateChanged);

                //预初始化代理服务
                this.m_tcpClient.ServiceContainer.PreInit();
            }
            else
            {
                this.m_tcpClient.ChangeServer(address, portPrimary);
            }

            string user = this.txtUser.Text.Trim();
            string pwd = this.txtUserPwd.Text.Trim();
            try
            {
                this.m_tcpClient.Login(user, pwd, false, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            this.AppendTextLine("[主连接]{0}:{1}", this.m_tcpClient.LocalIP, this.m_tcpClient.LocalPort);

            this.txtServerAddress.Enabled = false;
            this.txtServerPrimaryPort.Enabled = false;
            this.txtServerMinorPort.Enabled = false;
            this.txtUser.Enabled = false;
            this.txtUserPwd.Enabled = false;
            this.btnLogin.Enabled = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string key = this.m_tcpClient.InvokeSimpleCustomService("ASK_FOR_CONNECT", new string[] { "u01", "u02" });

            TryConnServer(key, "PEER_A");

            //this.btnConnect.Enabled = false;
            //this.btnSend.Enabled = true;
            //this.splitContainer1.Enabled = true;
        }
        
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (this.rdoUDP.Checked && this.m_udpSocket != null)
            {
                this.m_udpSocket.SendTo(Encoding.UTF8.GetBytes(this.rtbSend.Text), this.m_udpTargetPoint);
                this.rtbSend.Clear();
            }
        }


        private string m_localAddress;
        private int m_localPort;

        Socket m_udpSocket;
        EndPoint m_udpTargetPoint;
        Thread m_udpReceiveThread = null;

        private void TryConnServer(string key, string arg1)
        {
            int serverMinorPort = int.Parse(this.txtServerMinorPort.Text.Trim());
            string serverAddress = this.txtServerAddress.Text.Trim();

            if (this.rdoTCP.Checked)
            {
                string user = this.txtUser.Text.Trim();
                string pwd = this.txtUserPwd.Text.Trim();

                ZLBase.Communicate.CommunicateProxy tempTcp = new ZLBase.Communicate.CommunicateProxy(serverAddress, serverMinorPort, -1);
                tempTcp.Login(user, pwd, false, true);
                m_localAddress = tempTcp.LocalIP;
                m_localPort = int.Parse(tempTcp.LocalPort);
                AppendTextLine("[打洞连接]{0}:{1}", m_localAddress, m_localPort.ToString());
                tempTcp.SendMessage(string.Format("{0}|{1}|{2}", key, tempTcp.LocalIP, tempTcp.LocalPort), arg1);
                //tempTcp.Logout();
            }
            else
            {
                byte[] bytesKey = Encoding.UTF8.GetBytes(string.Format("{0}|{1}", key, arg1));
                byte[] bytesMsg = new byte[4 + bytesKey.Length];
                bytesMsg[0] = 255;
                bytesMsg[1] = 255;
                bytesMsg[2] = 255;
                bytesMsg[3] = 166;
                for (int i = 0; i < bytesKey.Length; i++)
                    bytesMsg[4 + i] = bytesKey[i];

                IPEndPoint serverPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverMinorPort);
                m_udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                m_udpSocket.SendTo(bytesMsg, serverPoint);

                IPEndPoint targetPoint;
                EndPoint tIep = new IPEndPoint(IPAddress.Any, 0) as EndPoint;
                byte[] buffer = new byte[100];
                m_udpSocket.ReceiveFrom(buffer, ref tIep);
                string[] tMsg = Encoding.UTF8.GetString(buffer, 4, buffer.Length - 4).Split('/');
                targetPoint = new IPEndPoint(IPAddress.Parse(tMsg[0]), Convert.ToInt32(tMsg[1]));

                UdpSocket udp = new UdpSocket(m_udpSocket, targetPoint, false);
                if (udp.Initialize())
                {
                    AppendTextLine("P2P连接成功-UDP:" + targetPoint.ToString());
                    m_udpTargetPoint = targetPoint;

                    m_udpReceiveThread = new Thread(UDPReceive);
                    m_udpReceiveThread.Name = "UdpReceiveThread";
                    m_udpReceiveThread.IsBackground = true;

                    m_udpReceiveThread.Start();
                }
                else
                {
                    AppendTextLine("P2P连接失败-UDP");
                }
            }
        }

        private void UDPReceive()
        {
            while (m_udpSocket != null)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    EndPoint iep = new IPEndPoint(IPAddress.Any, 0);
                    m_udpSocket.ReceiveFrom(buffer, ref iep);

                    AppendTextLine(Encoding.UTF8.GetString(buffer));
                    AppendTextLine("");
                }
                catch (Exception ex)
                {
                    AppendTextLine("接收UDP信息异常: " + ex.Message);
                }
            }
        }

        private void ConnectEndPoint(IPEndPoint localPoint, string address, int port)
        {
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(address), port);
            m_remotePoint = point;

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            
            socket.Bind(localPoint);
            socket.BeginConnect(point, AsyncCallback_Connected, socket);

            //m_p2pSocket = socket;
            //ListenForData();
            
            //if (this.m_args[0] == "TCP_MATCHED_A")
            //    this.m_tcpClient.InvokeSimpleCustomService("READY_A", new string[] { });
            //else
            //    this.m_tcpClient.InvokeSimpleCustomService("READY_B", new string[] { });
        }

        int m_retryTimes = 0;
        IPEndPoint m_remotePoint;
        Socket m_p2pSocket;
        private void AsyncCallback_Connected(IAsyncResult result)
        {
            Socket socket = result.AsyncState as Socket;
            try
            {
                socket.EndConnect(result);
            }
            catch (Exception ex)
            {
                AppendTextLine("P2P连接失败:" + ex.Message);
                
                if (m_remotePoint != null)
                {
                    if (m_retryTimes > 2) return;

                    AppendTextLine("等待后尝试重连...");
                    Thread.Sleep(3000);
                    socket.BeginConnect(m_remotePoint, AsyncCallback_Connected, socket);
                    m_retryTimes++;
                }
                return;
            }

            m_p2pSocket = socket;
            if (m_p2pSocket == null)
            {
                AppendTextLine("P2P连接失败");
            }
            else
            {
                AppendTextLine("P2P连接成功 [{0}]<-->[{1}]", m_p2pSocket.LocalEndPoint.ToString(), m_p2pSocket.RemoteEndPoint.ToString());

                if (this.m_args[0] == "TCP_MATCHED_B")
                {
                    ListenForData();
                    this.m_tcpClient.InvokeSimpleCustomService("READY", new string[] { });
                }
            }
        }
        
        private Object myLock1 = new Object();
        private Object myLock2 = new Object();
        private void ListenForData()
        {
            lock (this.myLock1)
            {
                try
                {
                    if (m_p2pSocket != null && m_p2pSocket.Connected)
                    {
                        //if (currentFilePos < fileLength)
                        //{
                            byte[] bs = new byte[1024 * 1024];

                            SocketAsyncEventArgs recArgs = new SocketAsyncEventArgs();
                            recArgs.SetBuffer(bs, 0, bs.Length);
                            recArgs.Completed += SocketRecivered;

                            ExtensionMethods.InvokeAsyncMethod(m_p2pSocket, m_p2pSocket.ReceiveAsync, SocketRecivered, recArgs);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    //SocketError("文件接受出错:" + ex.Message, null);
                    //ClientLogger.Error("接受数据出错:" + ex.Message, ex);
                }
            }
        }        
        private void SocketRecivered(object sender, SocketAsyncEventArgs e)
        {
            lock (myLock2)
            {
                try
                {
                    if (e.BytesTransferred == 0) return;
                    byte[] bs = e.Buffer;
                    string str = Encoding.UTF8.GetString(bs);
                    AppendTextLine(str);
                    //if (tmpmsLength - tmpIndex <= e.BytesTransferred) //缓冲区过小,tmpmsLength为定义的为byte[]的缓存
                    //{
                    //    using (FileStream fileStream = new FileStream(fileInfo.fileName, FileMode.OpenOrCreate))
                    //    {
                    //        fileStream.Seek(currentFilePos - tmpIndex, SeekOrigin.Begin);
                    //        fileStream.Write(ms, 0, tmpIndex);
                    //        fileStream.Flush();
                    //        fileStream.Close();

                    //        tmpIndex = 0;
                    //    }
                    //}

                    //Array.Copy(bs, 0, ms, tmpIndex, e.BytesTransferred);
                    //currentFilePos += e.BytesTransferred;
                    //tmpIndex += e.BytesTransferred;
                    //if (FileTransfering != null)
                    //    FileTransfering(currentFilePos);
                    //e.Dispose();
                    //if (currentFilePos < fileLength)
                    //    ListenForData();
                    //else
                    //{
                    //    if (tmpIndex != 0)
                    //    {
                    //        using (FileStream fileStream = new FileStream(fileInfo.fileName, FileMode.OpenOrCreate))
                    //        {
                    //            fileStream.Seek(currentFilePos - tmpIndex, SeekOrigin.Begin);
                    //            fileStream.Write(ms, 0, tmpIndex);
                    //            fileStream.Flush();
                    //            fileStream.Close();
                    //            tmpIndex = 0;
                    //        }
                    //    }
                    //    if (FileTransferComplete != null)
                    //        FileTransferComplete(null);
                    //}
                }
                catch (Exception ex)
                {
                    //SocketError("文件接受出错:" + ex.Message, null);
                    //ClientLogger.Error("接受数据出错:" + ex.Message, ex);
                }
            }
        }




        private void AppendTextLine(string text, params string[] args)
        {
            if (this.rtbReceived.InvokeRequired)
            {
                this.rtbReceived.Invoke(new EventHandler(delegate
                {
                    this.AppendTextLine(text, args);
                }));
            }
            else
            {
                if (args == null || args.Length == 0)
                    this.rtbReceived.AppendText((text ?? string.Empty) + "\r\n");
                else
                    this.rtbReceived.AppendText(string.Format(text, args) + "\r\n");
            }
        }

        private string[] m_args;
        private void m_tcpClient_MessageReceived(object sender, CustomEventArgs<object> e)
        {
            string msg = e.Data as string;
            AppendTextLine("[MsgReceived]{0}", msg);

            if (string.IsNullOrEmpty(msg))
                return;

            string[] args = msg.Split('|');
            switch (args[0])
            {
                case "ASK_FOR_CONNECT":
                    TryConnServer(args[1], "PEER_B");
                    break;
                case "TCP_MATCHED_A":
                case "TCP_MATCHED_B":
                    bool isBehindNAT = args[1] == args[3] && args[2] == args[4] ? false : true;
                    AppendTextLine("Target is behind NAT:{0}", isBehindNAT.ToString());
                    m_args = args;
                    if (this.rdoTCP.Checked)
                    {
                        IPEndPoint localPoint = new IPEndPoint(IPAddress.Parse(this.m_localAddress), this.m_localPort);
                        //同一个内网,优先用内网连接
                        if (this.m_localAddress == args[3])
                            ConnectEndPoint(localPoint, args[3], int.Parse(args[4]));//RemoteInner
                        else
                            ConnectEndPoint(localPoint, args[1], int.Parse(args[2]));//RemoteOuter
                    }
                    else
                    {
                       
                    }

                    //TcpSocket tcps = new TcpSocket(localPoint,
                    //    new IPEndPoint(IPAddress.Parse(args[3]), int.Parse(args[4])),
                    //    new IPEndPoint(IPAddress.Parse(args[1]), int.Parse(args[2])));
                    //if (tcps.Initialize() == true)
                    //{
                    //    //this.clientSocket = tcps as IP2PSocket;
                    //    AppendTextLine("连接成功！");
                    //}
                    //else
                    //{
                    //    AppendTextLine("连接失败！");
                    //}
                    break;
                case "READY":
                    if (this.m_args[0] == "TCP_MATCHED_A")
                        m_p2pSocket.Send(Encoding.UTF8.GetBytes("Hello, I'm A!"));
                    else
                        m_p2pSocket.Send(Encoding.UTF8.GetBytes("Hello, I'm B!"));
                    break;
                case "READY_A":
                    if (this.m_args[0] == "TCP_MATCHED_B")
                    {
                        m_p2pSocket.BeginConnect(m_remotePoint, AsyncCallback_Connected, m_p2pSocket);
                    }
                    break;
                case "READY_B":
                    if (this.m_args[0] == "TCP_MATCHED_A")
                    {
                        m_p2pSocket.BeginConnect(m_remotePoint, AsyncCallback_Connected, m_p2pSocket);
                    }
                    break;
                default:
                    break;
            }
        }
        private void m_tcpClient_ErrorRecived(object sender, CustomEventArgs<string> e)
        {
            AppendTextLine("[ErrorRecived]{0}", e.Data);
        }
        private void m_tcpClient_SessionKicked(object sender, CustomEventArgs<string> e)
        {
            AppendTextLine("[SessionKicked]{0}", e.Data);
        }
        private void m_tcpClient_ServerConnectStateChanged(object sender, CustomEventArgs<bool> e)
        {
            AppendTextLine("[ConnectStateChanged]{0}", e.Data ? "已连接" : "已断开");

            if (e.Data)//服务器重新连接上
            {
                AutoConnect();
            }
        }

        private void AutoConnect()
        {
            try
            {
                this.m_tcpClient.ReLogin();
            }
            catch (Exception ex)
            {
                this.rtbReceived.Invoke(new EventHandler(delegate
                {
                    this.rtbReceived.AppendText(">>自动重连失败：" + ex.Message);
                }));
            }
        }


        /// <summary>
        /// 获取操作系统已用的端口号
        /// </summary>
        /// <returns></returns>
        public static List<int> GetPortsAreUsed()
        {
            //获取本地计算机的网络连接和通信统计数据的信息
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            //返回本地计算机上的所有Tcp监听程序
            IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();

            //返回本地计算机上的所有UDP监听程序
            IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();

            //返回本地计算机上的Internet协议版本4(IPV4 传输控制协议(TCP)连接的信息。
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            List<int> allPorts = new List<int>();
            foreach (IPEndPoint ep in ipsTCP) allPorts.Add(ep.Port);
            foreach (IPEndPoint ep in ipsUDP) allPorts.Add(ep.Port);
            foreach (TcpConnectionInformation conn in tcpConnInfoArray) allPorts.Add(conn.LocalEndPoint.Port);

            return allPorts;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            List<int> portList = GetPortsAreUsed();
            string str = string.Empty;
            foreach (int item in portList)
            {
                str += item.ToString() + ",";
            }
            AppendTextLine(str);
        }
    }
}

/*

            //string str = "";
            //List<int> portList = GetPortsAreUsed();
            //foreach (int item in portList)
            //{
            //    str += item + ",";
            //}
            //MessageBox.Show(str);
            //return;

            //IPAddress groupAddress = IPAddress.Parse("192.168.100.169");
            //Socket Listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //Listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            //Listener.Bind(new IPEndPoint(IPAddress.Any, 8411));
            //MulticastOption multicastOption = new MulticastOption(groupAddress);
            //Listener.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOption);



            Socket socket1;
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 6690);
            socket1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket1.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);
            socket1.Bind(localEP);

            Socket socket2;
            socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket1.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);
            socket2.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //请注意这一句。ReuseAddress选项设置为True将允许将套接字绑定到已在使用中的地址。 
            socket2.Bind(localEP);


            string dirData = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            //ZLBase.Communicate.ServerApplication app = ZLBase.Communicate.ServerApplication.New(false, "temp", dirData);
            //app.Start(localPort, CommandHandler.Instance);


            //this.m_tcpClient.SendMessage("", "connect");



//1、 S启动两个网络侦听，一个叫【主连接】侦听，一个叫【协助打洞】的侦听。
//2、 A和B分别与S的【主连接】保持联系。
//3、 当A需要和B建立直接的TCP连接时，首先连接S的【协助打洞】端口，并发送协助连接申请。同时在该端口号上启动侦听（保证net类型3也能成功）。注意由于要在相同的网络终端上绑定到不同的套接字上，所以必须为这些套接字设置 SO_REUSEADDR 属性（即允许重用），否则侦听会失败。
//4、 S的【协助打洞】连接收到A的申请后通过【主连接】通知B，并将A经过NAT-A转换后的公网IP地址和端口等信息告诉B。
//5、 B收到S的连接通知后首先与S的【协助打洞】端口连接，随便发送一些数据后立即断开，这样做的目的是让S能知道B经过NAT-B转换后的公网IP和端口号。
//6、 B尝试与A的经过NAT-A转换后的公网IP地址和端口进行connect（这就是所谓“打洞”），根据不同的路由器会有不同的结果，有些路由器在这个操作就能建立连接（例如我用的TPLink R402），大多数路由器对于不请自到的SYN请求包直接丢弃而导致connect失败，但NAT-A会纪录此次连接的源地址和端口号，为接下来真正的连接做好了准备，这就是所谓的打洞，即B向A打了一个洞，下次A就能直接连接到B刚才使用的端口号了。
//7、 客户端B打洞的同时在相同的端口上启动侦听。B在一切准备就绪以后通过与S的【主连接】回复消息“我已经准备好”，S在收到以后将B经过NAT-B转换后的公网IP和端口号告诉给A。
//8、 A收到S回复的B的公网IP和端口号等信息以后，开始连接到B公网IP和端口号，由于在步骤6中B曾经尝试连接过A的公网IP地址和端口，NAT-A纪录 了此次连接的信息，所以当A主动连接B时，NAT-B会认为是合法的SYN数据，并允许通过，从而直接的TCP连接建立起来了。


            string user = this.txtUser.Text.Trim();
            string pwd = this.txtUserPwd.Text.Trim();
            string address = this.txtServerAddress.Text.Trim();
            int portMinor = int.Parse(this.txtServerMinorPort.Text.Trim());

            //1.连接服务端的辅助端口
            ZLBase.Communicate.CommunicateProxy cp1 = new ZLBase.Communicate.CommunicateProxy(address, portMinor, -1);
            cp1.Login(user, pwd, false, true);
            int localPort = int.Parse(cp1.LocalPort);
            //2.在上个连接的端口号上启动侦听
            ZLBase.Communicate.ServerApplication app = ZLBase.Communicate.ServerApplication.New(false, "temp", dirData);
            app.Start(localPort, CommandHandler.Instance);
*/