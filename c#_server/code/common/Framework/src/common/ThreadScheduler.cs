using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 逻辑锁，保证多线程执行的结果，返回给逻辑层后，在同一个线程
    /// 1.网络消息
    /// 2.数据库查询结果
    /// 3.控制台cmd命令
    /// 4.其他涉及到多线程与主线程交互的
    /// @author hannibal
    /// @time 2016-7-28
    /// </summary>
    public class ThreadScheduler : Singleton<ThreadScheduler>
    {
        private Object m_LogicLock = new Object();

        public Object LogicLock
        {
            get { return m_LogicLock; }
        }
    }
}
