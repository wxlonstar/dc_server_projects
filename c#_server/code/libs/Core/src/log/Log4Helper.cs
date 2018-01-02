using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// log4日志输出->文件
    /// @author hannibal
    /// @time 2016-8-24
    /// </summary>
    public class Log4Helper
    {
        private static log4net.ILog m_info_log = null;
        private static log4net.ILog m_warn_log = null;
        private static log4net.ILog m_error_log = null;
        private static log4net.ILog m_exception_log = null;

        public static void Init(log4net.ILog info_log, log4net.ILog warn_log, log4net.ILog error_log, log4net.ILog exception_log)
        {
            m_info_log = info_log;
            m_warn_log = warn_log;
            m_error_log = error_log;
            m_exception_log = exception_log;
        }
        
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(string msg)
        {
            if (m_info_log == null) return;
            m_info_log.Info(msg);
        }
        /// <summary>
        /// 警告
        /// </summary>
        public static void Warning(string msg)
        {
            if (m_warn_log == null) return;
            m_warn_log.Warn(msg);
        }
        /// <summary>
        /// 错误
        /// </summary>
        public static void Error(string msg)
        {
            if (m_error_log == null) return;
            m_error_log.Error(msg);
        }
        /// <summary>
        /// 抛出异常
        /// </summary>
        public static void Exception(Exception e)
        {
            if (m_exception_log == null) return;
            m_exception_log.Fatal(e);
        }
    }
}
