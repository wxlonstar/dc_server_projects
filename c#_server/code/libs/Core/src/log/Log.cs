using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// 日志输出
    /// @author hannibal
    /// @time 2014-11-24
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 日志回调
        /// </summary>
        public delegate void RegistFunction(string msg);
        private static RegistFunction m_MsgFun = null;		//日志监视
        /// <summary>
        /// 日志等级
        /// </summary>
        private static eLogLevel m_LogLv = eLogLevel.LV_DEBUG;
        private static bool[] m_EnableType = { true, true, true, true, true, true };
        /// <summary>
        /// 是否写入文件
        /// </summary>
        private static bool m_Log2File = true;

        private static StringBuilder tmp_st = new StringBuilder();

        #region 输出
        /// <summary>
        /// 临时或测试数据使用：正式游戏后会关闭
        /// </summary>
        static public void Debug(params string[] msg)
        {
            if (m_LogLv > eLogLevel.LV_DEBUG) return;
            if (!m_EnableType[(int)eLogLevel.LV_DEBUG]) return;

            tmp_st.Clear();
            for (int i = 0; i < msg.Length; ++i)
            {
                tmp_st = tmp_st.Append(msg[i]);
            }
            string log = "[debug]" + tmp_st.ToString();
            Console.WriteLine(log);
            if (m_MsgFun != null) m_MsgFun(log);
        }
        /// <summary>
        /// 临时或测试数据使用：正式游戏后会关闭
        /// </summary>
        static public void Debug(string msg)
        {
            if (m_LogLv > eLogLevel.LV_DEBUG) return;
            if (!m_EnableType[(int)eLogLevel.LV_DEBUG]) return;

            string log = "[debug]" + msg;
            Console.WriteLine(log);
            if (m_MsgFun != null) m_MsgFun(log);
        }

        static public void Info(params string[] msg)
        {
            if (m_LogLv > eLogLevel.LV_INFO) return;
            if (!m_EnableType[(int)eLogLevel.LV_INFO]) return;

            tmp_st.Clear();
            for (int i = 0; i < msg.Length; ++i)
            {
                tmp_st = tmp_st.Append(msg[i]);
            }
            string log = "[info]" + tmp_st.ToString();
            Console.WriteLine(log);
            if (m_Log2File) Log4Helper.Info(log);
            if (m_MsgFun != null) m_MsgFun(log);
        }
        static public void Info(string msg)
        {
            if (m_LogLv > eLogLevel.LV_INFO) return;
            if (!m_EnableType[(int)eLogLevel.LV_INFO]) return;

            string log = "[info]" + msg;
            Console.WriteLine(log);
            if (m_Log2File) Log4Helper.Info(log);
            if (m_MsgFun != null) m_MsgFun(log);
        }
        /// <summary>
        /// 警告
        /// </summary>
        static public void Warning(params string[] msg)
        {
            if (m_LogLv > eLogLevel.LV_WARNING) return;
            if (!m_EnableType[(int)eLogLevel.LV_WARNING]) return;

            tmp_st.Clear();
            for (int i = 0; i < msg.Length; ++i)
            {
                tmp_st = tmp_st.Append(msg[i]);
            }
            string log = "[warning]" + tmp_st.ToString();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(log);
            Console.ForegroundColor = ConsoleColor.White;
            if (m_Log2File) Log4Helper.Warning(log);
            if (m_MsgFun != null) m_MsgFun(log);
        }
        /// <summary>
        /// 警告
        /// </summary>
        static public void Warning(string msg)
        {
            if (m_LogLv > eLogLevel.LV_WARNING) return;
            if (!m_EnableType[(int)eLogLevel.LV_WARNING]) return;

            string log = "[warning]" + msg;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(log);
            Console.ForegroundColor = ConsoleColor.White;
            if (m_Log2File) Log4Helper.Warning(log);
            if (m_MsgFun != null) m_MsgFun(log);
        }
        /// <summary>
        /// 错误
        /// </summary>
        static public void Error(params string[] msg)
        {
            if (m_LogLv > eLogLevel.LV_ERROR) return;
            if (!m_EnableType[(int)eLogLevel.LV_ERROR]) return;

            tmp_st.Clear();
            for (int i = 0; i < msg.Length; ++i)
            {
                tmp_st = tmp_st.Append(msg[i]);
            }
            string log = "[error]" + tmp_st.ToString(); 
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(log);
            Console.ForegroundColor = ConsoleColor.White;
            if (m_Log2File) Log4Helper.Error(log);
            if (m_MsgFun != null) m_MsgFun(log);
        }
        /// <summary>
        /// 错误
        /// </summary>
        static public void Error(string msg)
        {
            if (m_LogLv > eLogLevel.LV_ERROR) return;
            if (!m_EnableType[(int)eLogLevel.LV_ERROR]) return;

            string log = "[error]" + msg;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(log);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Utils.GetStackTrace());
            if (m_Log2File) Log4Helper.Error(log);
            if (m_MsgFun != null) m_MsgFun(log);
        }
        /// <summary>
        /// 抛出异常
        /// </summary>
        static public void Exception(Exception e)
        {
            if (m_LogLv > eLogLevel.LV_EXCEPTION) return;
            if (!m_EnableType[(int)eLogLevel.LV_EXCEPTION]) return;

            Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
            if (m_Log2File) Log4Helper.Exception(e);
            if (m_MsgFun != null) m_MsgFun(e.Message);
        }
        #endregion

        #region 属性
        public static void SetMsgCallback(RegistFunction fun)
        {
            m_MsgFun = fun;
        }
        public static void SetLogLv(eLogLevel lv)
        {
            if (lv >= eLogLevel.LV_DEBUG && lv < eLogLevel.LV_MAX)
            {
                m_LogLv = lv;
            }
        }
        public static void SetEnableType(eLogLevel lv, bool b)
        {
            if(lv >= eLogLevel.LV_DEBUG && lv < eLogLevel.LV_MAX)
            {
                m_EnableType[(int)lv] = b;
            }
        }
        public static void SetLog2File(bool b)
        {
            m_Log2File = b;
        }
        #endregion
    }
    public enum eLogLevel
    {
        LV_DEBUG = 1,
        LV_INFO,
        LV_WARNING,
        LV_ERROR,
        LV_EXCEPTION,
        LV_MAX,
    }
}
