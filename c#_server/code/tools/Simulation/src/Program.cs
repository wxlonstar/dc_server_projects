using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace dc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Master.Instance.Setup();
            if (ServerConfig.net_info.console != 0)
            {
                ClientUtils.ShowConsole();
            }
            Master.Instance.Start();

            Application.Run(new MainForm());
            ClientUtils.CloseConsole();
        }
    }
}
