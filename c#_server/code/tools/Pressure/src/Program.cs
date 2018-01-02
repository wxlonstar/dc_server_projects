using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace dc
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool show_console = false;

            Master.Instance.Setup();
            if(ServerConfig.net_info.console != 0)
            {
                AllocConsole();//调用系统API，调用控制台窗口
                show_console = true;
            }
            Master.Instance.Start();

            Application.Run(new MainForm());
            if (show_console)
            {
                FreeConsole();//释放控制台
            }
        }
    }
}
