using System;
using System.Collections.Generic;
using ZLBase.Communicate;

namespace FanJun.P2PSample.Server
{
    public class CommandHandler : ClientCommandHandlerBase
    {
        private OutputDelegate m_dlgOutput;
        private ZLBase.Communicate.ServerApplication m_app;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dlgOutput"></param>
        /// <param name="app"></param>
        public CommandHandler(OutputDelegate dlgOutput, ZLBase.Communicate.ServerApplication app)
            : base(MockMembershipService.Instance)
        {
            this.m_dlgOutput = dlgOutput;
            this.m_app = app;
        }

        protected override bool DealwithRequestCore(string data, string identify, bool needResponse, string commandID, SessionInfo session,
            out string result, out string errMsg)
        {
            try
            {
                if (this.m_dlgOutput != null)
                {
                    this.m_dlgOutput(string.Format("接收到[{0}][{1}]的消息: {2}",
                        session == null ? "" : session.Username,
                        session == null ? "" : session.ClientIpAddress,
                        data));
                }
            }
            catch (Exception ex)
            {
                result = null;
                errMsg = ex.Message;
                return false;
            }

            result = string.Format("已收到[{0}][{1}]的消息\"{2}\"",
                session == null ? "" : session.Username,
                session == null ? "" : session.ClientIpAddress,
                data);
            errMsg = null;
            return true;
        }

        protected override bool InvokeSimpleCustomServiceCore(string methodName, string[] parameters, byte[][] attachDatas,
            out string result, out string errMsg)
        {
            string str = string.Empty;
            if (attachDatas != null)
            {
                for (int i = 0; i < attachDatas.Length; i++)
                {
                    str += attachDatas[i].Length + ",";
                }
            }

            this.m_dlgOutput(">> Calling [" + methodName + "][" + (parameters == null ? "null" : string.Join("^", parameters)) + "]");
            try
            {
                switch (methodName)
                {
                    case "ASK_FOR_CONNECT_TCP":
                    case "ASK_FOR_CONNECT_UDP":
                        if (parameters == null || parameters.Length < 2)
                        {
                            result = null;
                            errMsg = "参数个数不符合要求：需要2个参数";
                            return false;
                        }
                        else
                        {
                            string sourceUser = parameters[0];
                            string targetUser = parameters[1];
                            string key = P2PMatchManager.AddNew(sourceUser, targetUser);
                            m_app.PushMessageToClientByAccount(new string[] { targetUser }, methodName + "|" + key);
                            result = key;
                        }
                        break;
                    case "READY":
                        result = null;
                        m_app.PushMessageToAllClients("READY|");
                        break;
                    case "READY_A":
                    case "READY_B":
                        result = null;
                        m_app.PushMessageToAllClients(methodName + "|");
                        break;
                    default:
                        result = DateTime.Now.ToString();
                        break;
                }

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
