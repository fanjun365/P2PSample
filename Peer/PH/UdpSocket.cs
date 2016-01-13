using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FanJun.P2PSample.Peer
{
    public class UdpSocket : IP2PSocket
    {

        Socket udpSocket = null;
        EndPoint epRemote = null;
        EndPoint tEp = new IPEndPoint(IPAddress.Any, 0) as EndPoint;
        bool isService = false;
        public UdpSocket(Socket udpSocket, EndPoint epRemote, bool isService)
        {
            this.isService = isService;
            this.udpSocket = udpSocket;
            this.epRemote = epRemote;
        }

        #region ISocket 成员

        public IPEndPoint UDPRemoteEndPoint
        {
            get
            {
                return epRemote as IPEndPoint;
            }
        }

        public System.Net.Sockets.Socket P2PSocket
        {
            get { return udpSocket; }
        }

        public int Receive(byte[] buffer)
        {
            try
            {
                int len = udpSocket.ReceiveFrom(buffer, ref tEp);
                return len;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Receive(Encoding encoding)
        {
            throw new Exception("方法未实现");
        }

        public string Receive()
        {
            if (udpSocket == null)
            {
                return "";
            }
            string msg = "";
            try
            {
                byte[] buffer = new byte[1024];
                udpSocket.ReceiveFrom(buffer, ref tEp);
                msg = Encoding.UTF8.GetString(buffer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return msg;
        }



        public void Send(byte[] buffer)
        {
            if (udpSocket == null)
            {
                return;
            }
            SendTo(buffer, false);
        }

        public void Send(string data, Encoding encoding)
        {
            throw new Exception("方法未实现");
        }

        public void Send(string data)
        {
            if (udpSocket == null)
            {
                return;
            }
            byte[] msgByte = Encoding.UTF8.GetBytes(data);
            try
            {
                udpSocket.SendTo(msgByte, epRemote);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SendTo(byte[] buffer, bool isConfirm)
        {
            if (buffer.Length > 3 && isConfirm == false && buffer[3] > 64)
            {
                throw new Exception("普通消息，第四字节不能大于64");
            }
            try
            {
                udpSocket.SendTo(buffer, epRemote);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public event EventHandler SocketEvent;

        public P2PSocketType Type
        {
            get { return P2PSocketType.UDP; }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            //throw new Exception("The method or operation is not implemented.11");
        }

        #endregion

        bool shakeComplete = false;
        bool ShakeComplete
        {
            get { return shakeComplete; }
            set { shakeComplete = value; }
        }
        System.Threading.AutoResetEvent autoReset = new System.Threading.AutoResetEvent(false);

        bool timeOut = false;
        Thread udpReceiveThread = null;

        /// <summary>
        /// 初始化UDP，并进行打洞连接
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            if (this.udpSocket == null)
                throw new Exception("使用了空的Socket");
            if (this.epRemote == null)
                throw new Exception("使用了空的目标地址信息");

            /*
             * 连接建立过程：
             * 1.启动一个线程一直接受信息
             * 2.启动另外一个线程设置超时时间。
             * 3.由于需要向外打洞，所以A/B两端都必须向外发送数据，但是B端接收到消息，回应OK，A端不做任何回应。
             */
            //接收握手消息线程
            udpReceiveThread = new System.Threading.Thread(udpReceive);
            udpReceiveThread.IsBackground = true;
            udpReceiveThread.Name = "udpReceiveThread";
            udpReceiveThread.Start();

            Thread waitConnectThread = new Thread(WaitP2PConnect);
            waitConnectThread.IsBackground = true;
            waitConnectThread.Name = "waitConnectThread";
            waitConnectThread.Start();

            while (true)
            {
                Send("udp/");
                Thread.Sleep(300);
                if (ShakeComplete)
                {
                    autoReset.Set();
                    return true;
                }
                if (timeOut)
                {
                    return false;
                }
            }
        }

        private void udpReceive()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        string[] msg = this.Receive().Split('/');
                        if (ResolveMsg(msg))
                            break;
                    }
                    catch
                    {
                        timeOut = true;
                        break;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            catch { }
        }

        private bool ResolveMsg(string[] msg)
        {
            switch (msg[0])
            {
                case "udp":
                    if (!this.isService)
                    {
                        Send("ok/");
                    }
                    break;
                case "ok":
                    lock (this)
                    {
                        ShakeComplete = true;
                    }
                    if (this.isService)
                    {
                        Send("ok/");
                    }
                    return true;
                default:
                    break;
            }
            return false;
        }

        private void WaitP2PConnect()
        {
            try
            {
                autoReset.WaitOne(2000); //设置握手超时时间为2S
                lock (this)
                {
                    if (ShakeComplete == false)
                    {
                        if (udpReceiveThread != null)
                        {
                            if (udpSocket != null)
                            {
                                udpSocket.Shutdown(SocketShutdown.Both);
                                udpSocket.Close();
                            }
                            udpReceiveThread.Abort();
                        }
                    }
                    timeOut = true;
                }
            }
            catch { }
        }

        public void ConnectDistanceRemoteEndPoint(object sender, EventArgs e)
        {
            lock (this)
            {
                ShakeComplete = true;
                //接收端在接收到完成事件通知后，结束接收线程
                if (udpReceiveThread != null)
                {
                    try
                    {
                        udpReceiveThread.Abort();
                    }
                    catch { }
                }
            }
        }
    }
}
