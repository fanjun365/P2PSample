using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FanJun.P2PSample.Server
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ZLBase.Communicate.CommunicateBaseContext.LoggerFactory = new Log4NetLoggerFactory();
            Application.Run(new MainForm());
            //Application.Run(new ServerForm());
        }
    }
}
