using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace dc
{
    /// <summary>
    /// 时间
    /// @author hannibal
    /// @time 2016-7-28
    /// </summary>
    public class Time
    {
        private static long m_delta_time = 0;
        private static int m_total_frame = 0;
        private static int m_fps = 20;

        private static Stopwatch m_st;

        public static void Start()
        {
            m_st = new Stopwatch();
            m_st.Start();
        }

        /// <summary>
        /// 两帧之间的时间间隔,单位毫秒
        /// </summary>
        public static long deltaTime
        {
            get { return m_delta_time; }
            set { m_delta_time = value; }
        }

        /// <summary>
        /// 当前时间，相对2009年开始经过的毫秒数
        /// </summary>
        public static long time 
        {
            get{return TimeUtils.TimeSince2009; }
        }
        /// <summary>
        /// 当前时间，相对2009年开始经过的秒数
        /// </summary>
        public static long second_time
        {
            get { return TimeUtils.SecondSince2009; }
        }
        /// <summary>
        /// 游戏启动到现在的时间,单位毫秒
        /// </summary>
        public static long timeSinceStartup
        {
            get { return m_st.ElapsedMilliseconds; }
        }

        /// <summary>
        /// 游戏启动后，经过的帧数
        /// </summary>
        public static int frameCount
        {
            get { return m_total_frame; }
            set { m_total_frame = value; }   
        }
        public static int FPS
        {
            get { return m_fps; }
            set 
            { 
                if(value >= 1 && value <= 100) m_fps = value; 
            }
        }
    }
}
