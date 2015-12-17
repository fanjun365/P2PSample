using System;
using System.Collections.Generic;
using ZLBase.Communicate;

namespace FanJun.P2PSample.Server
{
    public class TcpCrossCommandHandler : ClientCommandHandlerBase
    {
        private OutputDelegate m_dlgOutput;
        private ZLBase.Communicate.ServerApplication m_mainApp;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dlgOutput"></param>
        /// <param name="mainApp"></param>
        public TcpCrossCommandHandler(OutputDelegate dlgOutput, ZLBase.Communicate.ServerApplication mainApp)
            : base(MockMembershipService.Instance)
        {
            this.m_dlgOutput = dlgOutput;
            this.m_mainApp = mainApp;
        }

        protected override bool DealwithRequestCore(string data, string identify, bool needResponse, string commandID, 
            SessionInfo session, out string result, out string errMsg)
        {
            try
            {
                if (this.m_dlgOutput != null)
                {
                    this.m_dlgOutput(string.Format("#TcpCross#接收到[{0}][{1}]的消息: {2}",
                        session == null ? "" : session.Username,
                        session == null ? "" : session.ClientIpAddress,
                        data));
                }

                switch (identify)
                {
                    case "PEER_A":
                    case "PEER_B":
                        string sid = this.m_mainApp.SessionManager.GetSessionIDByAccount(session.Username)[0];
                        string[] parameters = data.Split('|');
                        string key = parameters[0];
                        string clientLocalAddress = parameters[1];
                        int clientLocalPort = int.Parse(parameters[2]);

                        P2PMatchPair mp = P2PMatchManager.Get(key);
                        if (identify == "PEER_A")
                        {
                            mp.SetSourceInfo(sid, clientLocalAddress, clientLocalPort, 
                                session.ClientIpAddress, session.ClientPort);
                        }
                        else
                        {
                            mp.SetTargetInfo(sid, clientLocalAddress, clientLocalPort, 
                                session.ClientIpAddress, session.ClientPort);
                        }

                        if (mp.IsMatched)
                        {
                            SessionStateInfo source_ssi = this.m_mainApp.SessionManager.GetSession(mp.SourceSID);
                            PushingMessageCommand source_cmd = new PushingMessageCommand();
                            source_cmd.Message = string.Format("TCP_MATCHED_A|{0}|{1}|{2}|{3}|{4}|{5}", 
                                mp.TargetRemoteAddress, mp.TargetRemotePort,
                                mp.TargetLocalAddress, mp.TargetLocalPort,
                                mp.SourceRemoteAddress, mp.SourceRemotePort);
                            source_cmd.SessionID = source_ssi.SessionInfo.SessionID;
                            source_ssi.Connection.SendResponse(source_cmd);

                            SessionStateInfo target_ssi = this.m_mainApp.SessionManager.GetSession(mp.TargetSID);
                            PushingMessageCommand target_cmd = new PushingMessageCommand();
                            target_cmd.Message = string.Format("TCP_MATCHED_B|{0}|{1}|{2}|{3}|{4}|{5}",
                                mp.SourceRemoteAddress, mp.SourceRemotePort,
                                mp.SourceLocalAddress, mp.SourceLocalPort,
                                mp.TargetRemoteAddress, mp.TargetRemotePort);
                            target_cmd.SessionID = target_ssi.SessionInfo.SessionID;
                            target_ssi.Connection.SendResponse(target_cmd);
                        }
                        break;
                    default:
                        break;
                }

                if (needResponse)
                    result = DateTime.Now.ToString();
                else
                    result = null;
                errMsg = null;
                return true;
            }
            catch (Exception ex)
            {
                result = null;
                errMsg = ex.Message;
                return false;
            }
        }

        protected override bool InvokeSimpleCustomServiceCore(string methodName, string[] parameters, byte[][] attachDatas,
            string sessionId, out string result, out string errMsg)
        {
            try
            {
                string str = string.Empty;
                if (attachDatas != null)
                {
                    for (int i = 0; i < attachDatas.Length; i++)
                    {
                        str += attachDatas[i].Length + ",";
                    }
                }

                if (this.m_dlgOutput != null)
                {
                    this.m_dlgOutput(string.Format(">>#TcpCross# Calling [{0}][{1}]",
                        methodName, parameters == null ? "null" : string.Join("^", parameters)));
                }

                result = DateTime.Now.ToString();
                errMsg = null;
                return true;
            }
            catch (Exception ex)
            {
                result = null;
                errMsg = ex.Message;
                return false;
            }
        }
    }
}
