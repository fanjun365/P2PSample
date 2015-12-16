using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FanJun.P2PSample.Peer
{
    public class TcpSocket : IP2PSocket
    {

        //**********************************
        //对方本地端口
        private IPEndPoint iepRemoteInner = null;
        //对方外网端口
        private IPEndPoint iepRemoteOuter = null;
        //本地端口
        private IPEndPoint iepLocalInner = null;

        //记录是否已经连接成功
        bool hasConnction = false;
        //********************************
        public P2PSocketType Type
        {
            get { return P2PSocketType.TCP; }
        }

        private Socket p2PSocket;
        /// <summary>
        /// 本地用户传输的Socket对象
        /// </summary>
        public Socket P2PSocket
        {
            get { return p2PSocket; }
            set { p2PSocket = value; }
        }

        #region 事件

        public event EventHandler SocketEvent;
        #endregion



        #region ISocket 成员

        public IPEndPoint UDPRemoteEndPoint
        {
            get { return null; }
        }

        public void Send(byte[] buffer)
        {
            try
            {
                this.p2PSocket.Send(buffer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Send(string data, Encoding encoding)
        {
            try
            {
                byte[] buffer = encoding.GetBytes(data);
                this.p2PSocket.Send(buffer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Send(string data)
        {
            Send(data, Encoding.UTF8);
        }

        public int Receive(byte[] buffer)
        {
            try
            {
                return this.p2PSocket.Receive(buffer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Receive(Encoding encoding)
        {
            try
            {
                byte[] buffer = new byte[1024];
                this.p2PSocket.Receive(buffer);
                string msg = encoding.GetString(buffer);
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Receive()
        {
            try
            {
                byte[] buffer = new byte[1024];
                this.p2PSocket.Receive(buffer);
                string msg = Encoding.UTF8.GetString(buffer);
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void Dispose()
        {
            if (this.p2PSocket != null)
            {
                try
                {
                    this.p2PSocket.Close();
                }
                catch { }
            }
        }


        /// <summary>
        /// 初始化连接
        /// </summary>
        /// <param name="timeOut">等待超时时间,如果是 -1 则默认等待5秒。最少是2秒</param>
        /// <returns>true 代表TCP建立P2P连接成功，反之则失败</returns>
        public bool Initialize(int timeOut)
        {
            if (timeOut == -1)
            {
                timeOut = 5000;
            }
            else if (timeOut < 2000)
            {
                timeOut = 2000;
            }

            try
            {
                ConnectDistanceLocalEndPoint();
                //if (iepRemoteOuter != null)
                //{
                //    ConnectDistanceRemoteEndPoint();
                //}
                // AcceptDistanceConnection();
            }
            catch (Exception ex)
            {
                throw new Exception("TCP建立P2P发生异常 " + ex.Message);
            }
            autoEvent.WaitOne(timeOut);
            CloseSockets();
            return hasConnction;
        }

        /// <summary>
        /// 初始化连接,超时时间5秒
        /// </summary>
        public bool Initialize()
        {
            return Initialize(-1);
        }

        #endregion


        /// <summary>
        /// 初始化P2P实例,再调用Initialize方法进行P2P初始化
        /// </summary>
        /// <param name="iEPLocal">A方连接服务器的端口</param>
        /// <param name="iEPDistanceLocal">B方本地端口</param>
        /// <param name="iEPDistanceRemote">B方外网端口</param>
        public TcpSocket(IPEndPoint iEPLocal, IPEndPoint iEPDistanceLocal, IPEndPoint iEPDistanceRemote)
        {
            //通过客户端对象，获取客户端本地端口
            this.iepLocalInner = iEPLocal;
            this.iepRemoteInner = iEPDistanceLocal;
            this.iepRemoteOuter = iEPDistanceRemote;
        }

        /// <summary>
        /// 初始化P2P实例,再调用Initialize方法进行P2P初始化
        /// </summary>
        /// <param name="iEPLocal">A方连接服务器的端口</param>
        public TcpSocket(IPEndPoint iEPLocal, string addrDistanceLocal, int portDistanceLocal, string addrDistanceRemote, int portDistanceRemote)
        {
            //通过客户端对象，获取客户端本地端口
            this.iepLocalInner = iEPLocal;
            this.iepRemoteInner = new IPEndPoint(IPAddress.Parse(addrDistanceLocal), portDistanceLocal);
            //两个客户端都与服务器在局域网中，外网IP端口与内网IP端口一致。
            if (addrDistanceLocal == addrDistanceRemote && portDistanceLocal == portDistanceRemote)
            {
                this.iepRemoteOuter = null;
            }
            else
            {
                this.iepRemoteOuter = new IPEndPoint(IPAddress.Parse(addrDistanceRemote), portDistanceRemote);
            }
        }

        #region 私有方法

        Socket socketConDisLocal = null;
        Socket socketConDisRemote = null;
        Socket socketAcceptDisCon = null;
        System.Threading.AutoResetEvent autoEvent = new System.Threading.AutoResetEvent(false);
        /// <summary>
        /// 异步连接对方本地端口，如果成功则会执行AsyFun方法。
        /// ConnectDistanceLocalEndPoint、ConnectDistanceRemoteEndPoint、AcceptDistanceConnection如果连接成功
        /// 则有且只有一个方法执行了AsyFun。
        /// </summary>
        private void ConnectDistanceLocalEndPoint()
        {
            socketConDisLocal = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketConDisLocal.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socketConDisLocal.Bind(iepLocalInner);
            socketConDisLocal.BeginConnect(iepRemoteInner, AsyFun, socketConDisLocal);
        }

        /// <summary>
        /// 异步连接对方外网(服务器观察到的)端口，如果成功则会执行AsyFun方法。
        /// ConnectDistanceLocalEndPoint、ConnectDistanceRemoteEndPoint、AcceptDistanceConnection如果连接成功
        /// 则有且只有一个方法执行了AsyFun。
        /// </summary>
        private void ConnectDistanceRemoteEndPoint()
        {
            //socketConDisLocal.Disconnect(false);
            //socketConDisLocal.Close();
            //System.Threading.Thread.Sleep(1000);

            socketConDisRemote = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketConDisRemote.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socketConDisRemote.Bind(iepLocalInner);
            socketConDisRemote.BeginConnect(iepRemoteOuter, AsyFun, socketConDisRemote);
        }

        /// <summary>
        /// 异步接收来自外网的连接，如果成功则会执行AsyFun方法。
        /// ConnectDistanceLocalEndPoint、ConnectDistanceRemoteEndPoint、AcceptDistanceConnection如果连接成功
        /// 则有且只有一个方法执行了AsyFun。
        /// </summary>
        private void AcceptDistanceConnection()
        {
            socketAcceptDisCon = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketAcceptDisCon.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socketAcceptDisCon.Bind(iepLocalInner);
            socketAcceptDisCon.Listen(3);
            socketAcceptDisCon.BeginAccept(AsyFun2, socketAcceptDisCon);
        }


        int excp = 0;
        /// <summary>
        /// 获取到已经连接的SOCKET，并释放Initialize中的阻塞
        /// </summary>
        /// <param name="result"></param>
        private void AsyFun(IAsyncResult result)
        {

            Socket tSocket = result.AsyncState as Socket;
            try
            {
                tSocket.EndConnect(result);
            }
            catch
            {
                return;
            }
            p2PSocket = tSocket;
            if (p2PSocket == null)
            {
                return;
            }
            else
            {
                this.hasConnction = true;
            }
            //释放
            autoEvent.Set();
        }

        private void AsyFun2(IAsyncResult result)
        {
            Socket tSocket = result.AsyncState as Socket;
            try
            {
                Socket accSocket = tSocket.EndAccept(result);
                p2PSocket = accSocket;
                if (p2PSocket == null)
                {
                    return;
                }
                else
                {
                    this.hasConnction = true;
                }
                //释放
                autoEvent.Set();
            }
            catch
            {
                return;
            }
        }

        //private void AsyFun3(IAsyncResult result)
        //{
        //    if (excp++ > 0) return;
        //    this.p2PSocket = result.AsyncState as Socket;
        //    //localSocket是监听Socket
        //    if (this.p2PSocket == socketAcceptDisCon)
        //    {
        //        this.p2PSocket = this.p2PSocket.EndAccept(result);
        //    }

        //    if (p2PSocket == null)
        //    {
        //        this.hasConnction = false;
        //    }
        //    else
        //    {
        //        this.hasConnction = true;
        //    }
        //    //释放
        //    autoEvent.Set();
        //}


        /// <summary>
        /// 关闭无效Socket端口占用
        /// </summary>
        private void CloseSockets()
        {
            if (this.p2PSocket != socketConDisLocal && this.socketConDisLocal != null)
            {
                socketConDisLocal.Close();
            }

            if (this.p2PSocket != socketConDisRemote && this.socketConDisRemote != null)
            {
                socketConDisRemote.Close();
            }

            if (this.socketAcceptDisCon != null)
            {
                socketAcceptDisCon.Close();
            }
        }
        #endregion


        #region IP2PSocket 成员


        public bool SendConfirm(byte[] buffer, int timeOut)
        {
            Send(buffer);
            return true;
        }

        #endregion
    }
}
