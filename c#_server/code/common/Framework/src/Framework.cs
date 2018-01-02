using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace dc
{    
    /// <summary>
    /// 管理器
    /// @author hannibal
    /// @time 2016-7-28
    /// </summary>
    public class Framework : Singleton<Framework>
    {
        private bool m_IsStop = false;
        private long m_LastTickCount = 0;
        private Stopwatch m_st = null;
        private Action m_LoopCallback = null;

        public Framework()
        {
            Time.Start();
            m_st = new Stopwatch();
        }
        public void Setup(Action callback)
        {
            m_IsStop = false;
            m_LastTickCount = 0;
            m_LoopCallback = callback;
            m_st.Start();

            TimerManager.Instance.Setup();
            GameTimeManager.Instance.Setup();
            NetConnectManager.Instance.Setup();
        }
        public void Destroy()
        {
            m_IsStop = true;
            m_st.Stop();

            TimerManager.Instance.Destroy();
            GameTimeManager.Instance.Destroy();
            NetConnectManager.Instance.Destroy();
        }
        public void MainLoop()
        {
            while (!m_IsStop)
            {
                int FRAMES_PER_SECOND = Time.FPS;
		        int SKIP_TICKS = 1000 / FRAMES_PER_SECOND;
                long dwTickCount = m_st.ElapsedMilliseconds;
                if (m_LastTickCount == 0) m_LastTickCount = dwTickCount;
                if ((dwTickCount - m_LastTickCount) <= SKIP_TICKS)
		        {
                    Thread.Sleep(1);
		        }
		        else
		        {
                    Update(dwTickCount - m_LastTickCount);
                    m_LastTickCount = dwTickCount;
		        }
            }
        }
        /// <summary>
        /// 让winform执行，控制台程序不应当调用
        /// </summary>
        /// <param name="deltaTime">上下两帧相差豪秒数</param>
        public void Update(long deltaTime)
        {
            lock (ThreadScheduler.Instance.LogicLock)
            {
                Time.frameCount += 1;
                Time.deltaTime = deltaTime;
                PreTime();
                Tick();
                EndTime();
            }
        }
        public void Stop()
        {
            m_IsStop = true;
        }
        private void PreTime()
        {
            TimerManager.Instance.Tick();
            NetConnectManager.Instance.Tick();
        }
        private void Tick()
        {
            if (m_LoopCallback != null) m_LoopCallback();
        }
        private void EndTime()
        {

        }
    }
}
