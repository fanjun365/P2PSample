using System;
using System.Collections.Generic;
using log4net.Appender;
using log4net.Core;

namespace FanJun.P2PSample.Server
{
    public class Log4NetLogEventSourceAppender : AppenderSkeleton
    {
        private Object _syncRoot;

        static Log4NetLogEventSourceAppender()
        {
        }

        public Log4NetLogEventSourceAppender()
        {
            _syncRoot = new object();
        }

        /// <summary>
        /// Occurs when [on log].
        /// </summary>
        public static event EventHandler<OnLog4NetLogEventArgs> OnLog;

        protected override void Append(LoggingEvent loggingEvent)
        {
            EventHandler<OnLog4NetLogEventArgs> temp = OnLog;
            if (temp != null)
            {
                lock (_syncRoot)
                {
                    temp(null, new OnLog4NetLogEventArgs(loggingEvent));
                }
            }
        }

    }

    public class OnLog4NetLogEventArgs : EventArgs
    {
        public LoggingEvent LoggingEvent;

        public OnLog4NetLogEventArgs(LoggingEvent loggingEvent)
        {
            LoggingEvent = loggingEvent;
        }
    }
}

