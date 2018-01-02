using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 定时器管理器
    /// @author hannibal
    /// @time 2016-9-19
    /// </summary>
    public class TimerManager : Singleton<TimerManager>
    {
        private int m_idCounter = 0;
        private List<int> m_RemovalPending = new List<int>();
        private List<TimerEntity> m_Timers = new List<TimerEntity>();
        private List<TimerEntity> m_EntityPools = new List<TimerEntity>();

        public void Setup()
        {
            m_idCounter = 0;
        }

        public void Destroy()
        {
            for (int i = 0; i < m_Timers.Count; i++)
            {
                this.Despawn(m_Timers[i]);
            }
            m_Timers.Clear();
            m_RemovalPending.Clear();
        }

        public void Tick()
        {
            Remove();

            for (int i = 0; i < m_Timers.Count; i++)
            {
                m_Timers[i].Update();
            }
        }

        /// <summary>
        /// 增加定时器，执行一次后销毁
        /// </summary>
        /// <param name="rate">触发频率(单位毫秒)</param>
        /// <param name="callBack">触发回调函数</param>
        /// <returns>新定时器id</returns>
        public int AddOnce(long rate, Action<int, string> callBack, string param="")
        {
            return AddLoop(rate, 1, callBack, param);
        }
        /// <summary>
        /// 增加定时器，可以指定循环次数
        /// </summary>
        /// <param name="rate">触发频率(单位毫秒)</param>
        /// <param name="ticks">循环次数，如果是0则不会自动删除</param>
        /// <param name="callBack">触发回调函数</param>
        /// <returns>新定时器id</returns>
        public int AddLoop(long rate, int ticks, Action<int, string> callBack, string param = "")
        {
            if (ticks <= 0) ticks = 0;
            TimerEntity newTimer = this.Spawn();
            newTimer.Init(++m_idCounter, rate, ticks, callBack, param);
            m_Timers.Add(newTimer);
            return newTimer.id;
        }
        /// <summary>
        /// 下一帧执行
        /// </summary>
        /// <param name="callBack">触发回调函数</param>
        public void AddNextFrame(Action<int, string> callBack, string param = "")
        {
            AddOnce(0, callBack, param);
        }
        /// <summary>
        /// 移除定时器
        /// </summary>
        /// <param name="timerId">Timer GUID</param>
        public void RemoveTimer(int timerId)
        {
            m_RemovalPending.Add(timerId);
        }

        /// <summary>
        /// 移除过期定时器
        /// </summary>
        private void Remove()
        {
            if (m_RemovalPending.Count > 0)
            {
                foreach (int id in m_RemovalPending)
                {
                    for (int i = 0; i < m_Timers.Count; i++)
                    {
                        if (m_Timers[i].id == id)
                        {
                            this.Despawn(m_Timers[i]);
                            m_Timers.RemoveAt(i);
                            break;
                        }
                    }
                }

                m_RemovalPending.Clear();
            }
        }

        private TimerEntity Spawn()
        {
            if(m_EntityPools.Count > 0)
            {
                TimerEntity t = m_EntityPools[m_EntityPools.Count - 1];
                m_EntityPools.RemoveAt(m_EntityPools.Count - 1);
                return t;
            }
            return new TimerEntity();
        }
        private void Despawn(TimerEntity t)
        {
            m_EntityPools.Add(t);
        }
    }

    /// <summary>
    /// 定时器
    /// </summary>
    class TimerEntity
    {
        public int  id;
        public bool isActive;

        public long     mRate;
        public int      mTicks;
        public int      mTicksElapsed;
        public Action<int, string> mCallBack;
        public string   mParam;

        public IntervalTime mTime = new IntervalTime();

        public void Init(int id_, long rate_, int ticks_, Action<int, string> callback_, string param_)
        {
            id = id_;
            mRate = rate_ < 0 ? 0 : rate_;
            mTicks = ticks_ < 0 ? 0 : ticks_;
            mCallBack = callback_;
            mParam = param_;
            mTicksElapsed = 0;
            isActive = true;

            mTime.Init(rate_);
        }

        public void Update()
        {
            if (isActive && mTime.Update(Time.deltaTime))
            {
                mCallBack.Invoke(id, mParam);
                mTicksElapsed++;

                if (mTicks > 0 && mTicks == mTicksElapsed)
                {
                    isActive = false;
                    TimerManager.Instance.RemoveTimer(id);
                }
            }
        }
    }
}
