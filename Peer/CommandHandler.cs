using System;
using System.Collections.Generic;
using ZLBase.Communicate;

namespace FanJun.P2PSample.Peer
{
    public class CommandHandler : ClientCommandHandlerBase
    {
        static CommandHandler s_instance = new CommandHandler();
        /// <summary>
        /// 实例
        /// </summary>
        public static CommandHandler Instance
        {
            get { return s_instance; }
        }
    }
}
