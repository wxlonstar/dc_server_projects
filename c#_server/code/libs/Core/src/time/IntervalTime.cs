using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 间隔时间
    /// @author hannibal
    /// @time 2016-3-7
    /// </summary>
    public class IntervalTime
    {
        private long m_interval_time;//毫秒
        private long m_now_time;

        public IntervalTime()
        {
            m_now_time = 0;
        }
        /// <summary>
        /// 初始化定时器
        /// </summary>
        /// <param name="interval">触发间隔</param>
        /// <param name="start">是否第一帧开始执行</param>
        public void Init(long interval, bool first_frame = false)
        {
            m_interval_time = interval;
            if (first_frame) m_now_time = m_interval_time;
        }

        public void Reset()
        {
            m_now_time = 0;
        }

        public bool Update(long elapse_time)
        {
            m_now_time += elapse_time;
            if (m_now_time >= m_interval_time)
            {
                m_now_time -= m_interval_time;
                return true;
            }
            return false;
        }
    }

}
