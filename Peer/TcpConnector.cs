using System;
using System.Collections.Generic;
using System.Net;

namespace FanJun.P2PSample.Peer
{
    public class TcpConnector
    {
        static TcpConnector()
        {
        }


        public void Step1_ConnectToServer(string address, int port)
        {
            IPEndPoint sPoint = new IPEndPoint(IPAddress.Parse(address), port);
        }
    }
}
