using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using log4net;
using log4net.Config;
using ZLBase.SharedLibrary.Interface;

namespace FanJun.P2PSample.Server
{
    public class Log4NetLoggerFactory : ILoggerFactory
    {
        public Log4NetLoggerFactory()
        {
            //FileInfo file;
            //if (HttpContext.Current != null)
            //{
            //    file = new FileInfo(HttpRuntime.AppDomainAppPath + "log4net.config");
            //}
            //else
            //{
            //    file = new FileInfo("log4net.config");
            //}
            //XmlConfigurator.ConfigureAndWatch(file);
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
        }

        public ILogger CreateLogger<T>(string group)
        {
            return new Log4NetLogger(typeof(T), group);
        }

        public ILogger CreateLogger<T>()
        {
            return new Log4NetLogger(typeof(T), String.Empty);
        }

        public ILogger CreateLogger(Type type, string loggingPath)
        {
            return new Log4NetLogger(type, String.Empty);
        }

        public ILogger CreateLogger(Type type)
        {
            return new Log4NetLogger(type, String.Empty);
        }
    }
}
