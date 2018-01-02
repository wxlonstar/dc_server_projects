using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace dc
{
    public class ClientUtils
    {
        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        private static bool m_is_show_console = false;

        public static void ShowConsole()
        {
            if (m_is_show_console) return;
            AllocConsole();//调用系统API，调用控制台窗口
            m_is_show_console = true;
        }
        public static void CloseConsole()
        {
            if (!m_is_show_console) return;
            FreeConsole();//释放控制台
            m_is_show_console = false;
        }
        public static void SwitchConsole()
        {
            if (!m_is_show_console)
            {
                ShowConsole();
            }
            else
            {
                CloseConsole();
            }
        }
    }
}
