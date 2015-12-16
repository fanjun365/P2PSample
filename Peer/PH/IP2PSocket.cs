using System;
using System.Collections.Generic;
using System.Text;

namespace FanJun.P2PSample.Peer
{
    public interface IP2PSocket : IDisposable
    {
        int Receive(byte[] buffer);
        string Receive(System.Text.Encoding encoding);
        string Receive();
        void Send(byte[] buffer);
        void Send(string data, System.Text.Encoding encoding);
        void Send(string data);
        event EventHandler SocketEvent;
        P2PSocketType Type { get; }
        System.Net.Sockets.Socket P2PSocket { get; }
        System.Net.IPEndPoint UDPRemoteEndPoint { get; }
    }


    public enum P2PSocketType
    {
        UDP = 0,
        TCP = 1,
        ANY = 2,
    }
}
