using System;
using System.Collections.Generic;
using System.Net;

namespace FanJun.P2PSample.Server
{
    public class P2PMatchPair
    {
        private string m_key;
        public string Key { get { return this.m_key; } }

        private string m_sourceUser;
        public string SourceUser { get { return this.m_sourceUser; } }

        private string m_targetUser;
        public string TargetUser { get { return this.m_targetUser; } }

        private string m_sourceSID;
        public string SourceSID { get { return this.m_sourceSID; } }

        private string m_targetSID;
        public string TargetSID { get { return this.m_targetSID; } }

        private string m_sourceLocalAddress;
        public string SourceLocalAddress { get { return this.m_sourceLocalAddress; } }

        private int m_sourceLocalPort;
        public int SourceLocalPort { get { return this.m_sourceLocalPort; } }
        
        private string m_sourceRemoteAddress;
        public string SourceRemoteAddress { get { return this.m_sourceRemoteAddress; } }

        private int m_sourceRemotePort;
        public int SourceRemotePort { get { return this.m_sourceRemotePort; } }

        private string m_targetLocalAddress;
        public string TargetLocalAddress { get { return this.m_targetLocalAddress; } }

        private int m_targetLocalPort;
        public int TargetLocalPort { get { return this.m_targetLocalPort; } }

        private string m_targetRemoteAddress;
        public string TargetRemoteAddress { get { return this.m_targetRemoteAddress; } }

        private int m_targetRemotePort;
        public int TargetRemotePort { get { return this.m_targetRemotePort; } }




        public bool IsMatched { get { return !string.IsNullOrEmpty(this.m_sourceSID) && !string.IsNullOrEmpty(this.m_targetSID); } }


        public P2PMatchPair(string key, string sourceUser, string targetUser)
        {
            this.m_key = key;
            this.m_sourceUser = sourceUser;
            this.m_targetUser = targetUser;
        }


        public void SetSourceInfo(string sid, string localAddress, int localPort, string remoteAddress, int remotePort)
        {
            if (!string.IsNullOrEmpty(this.m_sourceSID))
                throw new Exception("不允许重复设置");
            this.m_sourceSID = sid;
            this.m_sourceLocalAddress = localAddress;
            this.m_sourceLocalPort = localPort;
            this.m_sourceRemoteAddress = remoteAddress;
            this.m_sourceRemotePort = remotePort;
        }
        public void SetTargetInfo(string sid, string localAddress, int localPort, string remoteAddress, int remotePort)
        {
            if (!string.IsNullOrEmpty(this.m_targetSID))
                throw new Exception("不允许重复设置");
            this.m_targetSID = sid;
            this.m_targetLocalAddress = localAddress;
            this.m_targetLocalPort = localPort;
            this.m_targetRemoteAddress = remoteAddress;
            this.m_targetRemotePort = remotePort;
        }
    }
}
