using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FanJun.P2PSample.Peer
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
            Application.Run(new MainForm());
            //Application.Run(new PeerForm());
        }
    }
}
