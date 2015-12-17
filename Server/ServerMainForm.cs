using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FanJun.P2PSample.Server
{
    public partial class ServerMainForm : Form
    {
        private ZLBase.Communicate.ServerApplication m_app_primary = null;
        private ZLBase.Communicate.ServerApplication m_app_minor = null;

        public ServerMainForm()
        {
            InitializeComponent();

            Log4NetLogEventSourceAppender.OnLog += new EventHandler<OnLog4NetLogEventArgs>(Log4Net_OnLog);
            this.btnStart.Click += btnStart_Click;
        }

        private void Log4Net_OnLog(object sender, OnLog4NetLogEventArgs e)
        {
            this.AppendTextLine(e.LoggingEvent.Level.ToString() + (e.LoggingEvent.MessageObject as string));
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtPrimaryPort.Text.Trim()))
                return;
            if (string.IsNullOrEmpty(this.txtMinorPortTcp.Text.Trim()))
                return;
            int portPrimary, portMinorTcp, portMinorUdp;
            if (!int.TryParse(this.txtPrimaryPort.Text.Trim(), out portPrimary))
                return;
            if (!int.TryParse(this.txtMinorPortTcp.Text.Trim(), out portMinorTcp))
                return;
            if (!int.TryParse(this.txtMinorPortUdp.Text.Trim(), out portMinorUdp))
                return;

            string dirData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            this.groupBox1.Enabled = false;
            try
            {
                if (m_app_primary == null)
                    m_app_primary = ZLBase.Communicate.ServerApplication.New(true, "Primary", dirData);
                CommandHandler handler = new CommandHandler(this.AppendTextLine, m_app_primary);
                //handler.ServiceContainer.RegistService<Interface.IMyService1>(new Service.MyService());
                m_app_primary.Start(portPrimary, handler);

                //TCP辅助打洞端口
                if (m_app_minor == null)
                    m_app_minor = ZLBase.Communicate.ServerApplication.New(false, "Minor", dirData);
                m_app_minor.Start(portMinorTcp, new TcpCrossCommandHandler(this.AppendTextLine, m_app_primary));

                //UDP辅助打洞端口
                StartUdpServer(portMinorUdp);
            }
            catch (Exception ex)
            {
                this.AppendTextLine(ex.ToString());
                this.groupBox1.Enabled = true;
            }
        }

        private void linkClearLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.rtbLog.Clear();
        }



        public void NotifyMatchSuccedd(P2PMatchPair mp)
        {
            m_app_primary.PushMessageToClientByAccount(
                new string[] { mp.SourceUser, mp.TargetUser }, 
                "CONNECT_SUCCEED|" + mp.Key);
        }




        #region UDP Server

        private Socket m_udpSocket = null;
        private Thread m_udpReceiveThread = null;

        private void StartUdpServer(int port)
        {
            m_udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress address = this.GetLocalIP();
            if (address == null)
                throw new Exception("获取本地IP失败");

            IPEndPoint point = new IPEndPoint(address, port);
            m_udpSocket.Bind(point);

            m_udpReceiveThread = new Thread(UDPReceive);
            m_udpReceiveThread.Name = "UdpReceiveThread";
            m_udpReceiveThread.IsBackground = true;

            m_udpReceiveThread.Start();
            AppendTextLine("协助监听UDP: " + point.ToString());
        }

        /// <summary>
        /// 接收UDP发送的数据
        /// </summary>
        private void UDPReceive()
        {
            while (m_udpSocket != null)
            {
                try
                {
                    byte[] buf = new byte[100];
                    EndPoint iep = new IPEndPoint(IPAddress.Any, 0);
                    m_udpSocket.ReceiveFrom(buf, ref iep);

                    object[] arg = new object[2];
                    arg[0] = buf;
                    arg[1] = iep;
                    object obj = arg;

                    ThreadPool.QueueUserWorkItem(new WaitCallback(ResolveUdpMsg), obj);
                }
                catch (Exception ex)
                {
                    AppendTextLine("接收UDP信息异常: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// 接收并解析UDP消息，
        /// 在ClientList中进行匹配
        /// </summary>
        /// <param name="buffer"></param>
        private void ResolveUdpMsg(object obj)
        {
            object[] arg = (object[])obj;
            byte[] buffer = (byte[])arg[0];
            IPEndPoint iepRemote = arg[1] as IPEndPoint;
            if (CheckUdpReceive(buffer) == false)
            {
                return;
            }
            string strInfo = Encoding.UTF8.GetString(buffer, 4, 39);
            string[] infos = strInfo.Split('|');
            P2PMatchPair mp = P2PMatchManager.Get(infos[0]);
            if (infos[1] == "PEER_A")
            {
                mp.SetSourceInfo(infos[1], "", 0, iepRemote.Address.ToString(), iepRemote.Port);
            }
            else
            {
                mp.SetTargetInfo(infos[1], "", 0, iepRemote.Address.ToString(), iepRemote.Port);
            }

            if (mp.IsMatched)
            {
                //相互通知。
                InformIPEndPointUdp(mp.SourceRemoteAddress, mp.SourceRemotePort, 
                    new IPEndPoint(IPAddress.Parse(mp.TargetRemoteAddress), mp.TargetRemotePort));
                InformIPEndPointUdp(mp.TargetRemoteAddress, mp.TargetRemotePort,
                    new IPEndPoint(IPAddress.Parse(mp.SourceRemoteAddress), mp.SourceRemotePort));
            }
        }

        private byte[] acceptMsgUdp = new byte[] { 255, 255, 255, 166 };
        private void InformIPEndPointUdp(string sourceAddress, int sourcePort, IPEndPoint target)
        {
            string msg = sourceAddress + "/" + sourcePort + "/";
            byte[] tMsg = Encoding.UTF8.GetBytes(msg);
            byte[] iepMsg = new byte[tMsg.Length + 4];
            //标识  255, 255, 255, 166
            iepMsg[0] = acceptMsgUdp[0];
            iepMsg[1] = acceptMsgUdp[1];
            iepMsg[2] = acceptMsgUdp[2];
            iepMsg[3] = acceptMsgUdp[3];
            //IEP信息
            for (int i = 0; i < tMsg.Length; i++)
            {
                iepMsg[4 + i] = tMsg[i];
            }

            try
            {
                m_udpSocket.SendTo(iepMsg, target);
            }
            catch (Exception ex)
            {
                AppendTextLine("通知对方UPD端口信息异常: " + ex.Message);
            }
        }

        private bool CheckUdpReceive(byte[] buffer)
        {
            if (buffer.Length < 40 || buffer[0] != 255 || buffer[3] != 166)
            {
                return false;
            }
            return true;
        }



        private IPAddress GetLocalIP()
        {
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            for (int i = 0; i < IpEntry.AddressList.Length; i++)
            {
                if (IpEntry.AddressList[i].AddressFamily.ToString() == "InterNetwork")
                {
                    return IpEntry.AddressList[i];
                }
            }
            return null;
        }

        #endregion








        private int maxConnection = 100;
        private Thread TcpListenThread = null;
        private Socket serverListenSocket = null;
            //初始化接收信息成功信号,168标识TCP，166标识UDP
        private byte[] acceptMsgTcp = new byte[] { 255, 255, 255, 168 };
        /// <summary>
        /// 开启TCP打洞协助端口
        /// </summary>
        /// <returns></returns>
        public void StartTCPServer(int port)
        {
            serverListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress localIPAddress = this.GetLocalIPAddress();
            if (localIPAddress == null)
                throw new Exception("获取本地IP失败");

            IPEndPoint serverListenEndPoint = new IPEndPoint(localIPAddress, port);
            serverListenSocket.Bind(serverListenEndPoint);
            serverListenSocket.Listen(maxConnection);

            TcpListenThread = new Thread(new ThreadStart(TCPListen));
            TcpListenThread.Name = "TcpListenThread";
            TcpListenThread.IsBackground = true;
            TcpListenThread.Start();

            AppendTextLine("已开启TCP打洞协助监听: " + serverListenEndPoint.ToString());
        }
        /// <summary>
        /// 一直监听端口，如果有连接则将其添加到队列中。
        /// 在监听之后，需要马上接收一次数据，匹配A\B两端
        /// </summary>
        private void TCPListen()
        {
            while (true)
            {
                if (serverListenSocket != null)
                {
                    try
                    {
                        Socket client = serverListenSocket.Accept();
                        //使用线程池异步执行
                        ThreadPool.QueueUserWorkItem(new WaitCallback(MatchTcpClient), client);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// （异步执行）在TCP连接中，将获取的数据在队列中匹配
        /// 1.先向客户端发送连接消息。
        /// 2.接收客户端发送的IDKey码(GUID)
        /// 3.如果客户端断开连接，则在客户端队列中删除
        /// </summary>
        private void MatchTcpClient(object client)
        {
            Socket sClient = client as Socket;
            //开始与客户端交互信息，发送服务器接收成功信息
            if (Send(acceptMsgTcp, sClient) == false)
            {
                ShutDownSocket(sClient);
                return;
            }

            //接收客户端反馈信息
            byte[] msg = new byte[100];
            if (Receive(msg, sClient) == false)
            {
                ShutDownSocket(sClient);
                return;
            }

            //检查信息是否符合约定,反馈信息前4字节为255,255,255,168
            if (CheckTcpReceive(msg) == false)
            {
                ShutDownSocket(sClient);
                return;
            }

            IPEndPoint iepLocal = null;
            //从反馈消息中获取匹配值
            string iDKey = GetIDKey(msg, out iepLocal);
            if (iDKey.Length < 1)
            {
                ShutDownSocket(sClient);
                return;
            }

            P2PSession curClient = FindClientFromClientList(iDKey);
            if (curClient == null)//未找到对象
            {
                ShutDownSocket(sClient);
                return;
            }
            else if (curClient.SocketA == null)
            {
                curClient.SocketA = sClient;
                curClient.IEPLocalA = iepLocal;
            }
            else if (curClient.SocketB == null)//匹配成功
            {
                curClient.SocketB = sClient;
                curClient.IEPLocalB = iepLocal;
                //匹配成功之后将服务器监听到的信息，相互通知
                InformIPEndPointTcp(curClient.SocketA, curClient.IEPLocalA, curClient.SocketB);
                InformIPEndPointTcp(curClient.SocketB, curClient.IEPLocalB, curClient.SocketA);
            }
            else
            {
                ShutDownSocket(sClient);
                //PrivateLogger.Info("P2P匹配中出现客户端重复。");
            }
        }


        /// <summary>
        /// 检查信息是否符合规范
        /// </summary>
        private bool CheckTcpReceive(byte[] msg)
        {
            if (msg[0] == 255 && msg[1] == 255 && msg[2] == 255 && msg[3] == 168)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 从反馈消息中获取IDKey，也就是匹配键。后36位
        /// </summary>
        private string GetIDKey(byte[] msg, out IPEndPoint point)
        {
            try
            {
                string[] msgQue = Encoding.UTF8.GetString(msg, 4, msg.Length - 4).Split('/');
                point = new IPEndPoint(IPAddress.Parse(msgQue[1]), Convert.ToInt32(msgQue[2]));
                return msgQue[0];
            }
            catch
            {
                point = null;
                return "";
            }
        }
        /// <summary>
        /// 在ClientList中查找匹配对象
        /// </summary>
        private P2PSession FindClientFromClientList(string idKey)
        {
            P2PSession client = null;
            //lock (P2PSession.lockObj)
            //{
            //    client = SessionList.Find(delegate(P2PSession obj)
            //    {
            //        if (obj.IDKey == idKey)
            //        {
            //            return true;
            //        }
            //        return false;
            //    });
            //}
            return client;
        }
        /// <summary>
        /// 获取source中的RemoteEndPoint发送给Target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool InformIPEndPointTcp(Socket source, IPEndPoint sourceIEP, Socket target)
        {
            if (source == null || target == null)
            {
                return false;
            }
            IPEndPoint iepSource = source.RemoteEndPoint as IPEndPoint;
            string msg = iepSource.Address.ToString() + "/" + iepSource.Port + "/" + sourceIEP.Address + "/" + sourceIEP.Port + "/";
            byte[] tMsg = Encoding.UTF8.GetBytes(msg);
            byte[] iepMsg = new byte[tMsg.Length + 4];
            //标识  255, 255, 255, 168
            iepMsg[0] = acceptMsgTcp[0];
            iepMsg[1] = acceptMsgTcp[1];
            iepMsg[2] = acceptMsgTcp[2];
            iepMsg[3] = acceptMsgTcp[3];
            //IEP信息
            for (int i = 0; i < tMsg.Length; i++)
            {
                iepMsg[4 + i] = tMsg[i];
            }
            //发送信息
            try
            {
                target.Send(iepMsg);
            }
            catch
            {
                //发送信息失败
                return false;
            }
            return true;
        }


        /// <summary>
        /// TCP发送数据
        /// </summary>
        private bool Send(byte[] msg, Socket socket)
        {
            try
            {
                socket.Send(msg);
            }
            catch
            {
                return false;
            }
            return true;
        }
        private bool Receive(byte[] msg, Socket socket)
        {
            try
            {
                socket.Receive(msg);
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 关闭当前socket
        /// </summary>
        /// <param name="socket"></param>
        private void ShutDownSocket(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch { }
        }




        #region >> 辅助 <<

        private void AppendTextLine(string text, bool outputTime)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(delegate { this.AppendTextLine(text, outputTime); }));
            }
            else
            {
                string textLine;
                if (outputTime)
                {
                    textLine = string.Format("[{0}]{1}\n",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffffff"), text ?? string.Empty);
                }
                else
                {
                    textLine = (text ?? "") + "\n";
                }

                this.WriteFileLog(textLine);

                if (this.chkShowLog.Checked)
                {
                    this.rtbLog.Text += textLine;
                    this.rtbLog.SelectionStart = this.rtbLog.Text.Length - 1;
                    this.rtbLog.SelectionLength = 0;
                    this.rtbLog.ScrollToCaret();
                }
            }
        }
        private void AppendTextLine(string text)
        {
            this.AppendTextLine(text, true);
        }

        private string m_LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "Log\\log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
        private StreamWriter m_logWriter;
        private void WriteFileLog(string text)
        {
            if (text == null || !text.Contains("<异常>"))
                return;

            try
            {
                if (this.m_logWriter == null)
                {
                    lock (this.m_LogFilePath)
                    {
                        if (this.m_logWriter == null)
                        {
                            string dirPath = Path.GetDirectoryName(this.m_LogFilePath);
                            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

                            this.m_logWriter = new System.IO.StreamWriter(this.m_LogFilePath, true,
                                System.Text.Encoding.Default);
                        }
                    }
                }

                this.m_logWriter.Write(text);
            }
            catch { }
        }

        #endregion

        /// <summary>
        /// 兼容XP,WIN7获取IP4
        /// </summary>
        /// <returns></returns>
        private IPAddress GetLocalIPAddress()
        {
            string HostName = Dns.GetHostName();
            IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
            IPAddress ip = null;
            for (int i = 0; i < IpEntry.AddressList.Length; i++)
            {
                if (IpEntry.AddressList[i].AddressFamily.ToString() == "InterNetwork")
                {
                    ip = IpEntry.AddressList[i];
                    break;
                }
            }
            return ip;
        }
    }




    /// <summary>
    /// 匹配对象
    /// </summary>
    internal class P2PSession
    {
        /// <summary>
        /// 用于同步的对象
        /// </summary>
        public static object lockObj;
        static P2PSession()
        {
            lockObj = new object();
        }

        /// <summary>
        /// 客户端A
        /// </summary>
        public Socket SocketA = null;
        /// <summary>
        /// 客户端A的本地地址
        /// </summary>
        public IPEndPoint IEPLocalA = null;
        /// <summary>
        /// 客户端B
        /// </summary>
        public Socket SocketB = null;
        /// <summary>
        /// 客户端B的本地地址
        /// </summary>
        public IPEndPoint IEPLocalB = null;
        public bool IsTimeOut = false;
        /// <summary>
        ///匹配键，在服务端请求P2P连接时生成的GUID 
        /// </summary>
        public string IDKey = string.Empty;

        private string userAccount;
        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserAccount
        {
            get { return userAccount; }
            set { userAccount = value; }
        }

        private string targetAccount;
        /// <summary>
        /// 对方用户账号
        /// </summary>
        public string TargetAccount
        {
            get { return targetAccount; }
            set { targetAccount = value; }
        }

        private DateTime time;
        /// <summary>
        /// 初始化时间，用于超时对比
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                return time;
            }
        }
        public P2PSession(string idKey, string userAccount)
        {
            this.IDKey = idKey;
            time = DateTime.Now;
            this.userAccount = userAccount;
        }

        ~P2PSession()
        {
            if (SocketA != null)
            {
                try
                {
                    SocketA.Close();
                }
                catch
                {
                }
            }

            if (SocketB != null)
            {
                try
                {
                    SocketB.Close();
                }
                catch { }
            }
        }
    }
}
