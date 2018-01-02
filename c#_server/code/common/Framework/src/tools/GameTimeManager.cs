using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 游戏逻辑时间
    /// @author hannibal
    /// @time 2016-10-28
    /// </summary>
    public class GameTimeManager : Singleton<GameTimeManager>
    {
        private long m_adjust_time = 0;

        private int m_cur_hour = 0;

        private Dictionary<string, List<Action>> m_day_callbacks = new Dictionary<string, List<Action>>();

        public void Setup()
        {
            this.InitHourTimer();
        }
        public void Destroy()
        {
            m_day_callbacks.Clear();
        }

        #region 整点报时
        /// <summary>
        /// 设置定时器
        /// </summary>
        private void InitHourTimer()
        {
            m_cur_hour = DateTime.Now.Hour;//获取当前时间的小时部分

            DateTime t = DateTime.Now.AddHours(1);
            DateTime zhengdian = new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 1);//设置成多一秒
            TimeSpan ts = zhengdian - DateTime.Now; //距离整点时间间隔

            long rate = (long)ts.TotalMilliseconds;
            if (ts.TotalMilliseconds >= 1000*60)//超过一分钟，定时器设置成总时长的一半，减少误差
                rate = (long)(ts.TotalMilliseconds*0.5f);
            TimerManager.Instance.AddOnce(rate, OnHourTimer); // 设置timer时间，单位是毫秒
        }
        /// <summary>
        /// 整点触发
        /// </summary>
        private void OnHourTimer(int timer_id, string param)
        {
            int h = DateTime.Now.Hour;//获取当前时间的小时部分
            if(h == m_cur_hour)
            {//有可能还差一点
                this.InitHourTimer();
                return;
            }
            else
            {//到时间了
                EventController.TriggerEvent(EventID.INTEGRAL_HOUR_TIMER, h);
                this.InitHourTimer();
                Log.Info("当前时间:" + DateTime.Now.Hour + " " + DateTime.Now.Minute + " " + DateTime.Now.Second);
            }
        }
        #endregion

        #region 指定日期报时
        /// <summary>
        /// 指定日期触发
        /// </summary>
        public void AddDayTimer(int year, int month, int day, int hour, Action callback)
        {
            if (callback == null || year <= 0 || month < 0 || day < 0 || hour < 0)
                return;
            //时间必须大于当前
            DateTime zhengdian = new DateTime(year, month, day, hour, 0, 0);
            TimeSpan ts = zhengdian - DateTime.Now;
            if (ts.TotalMilliseconds <= 0)
                return;

            List<Action> funs;
            string key = year + "_" + month + "_" + day + "_" + hour;
            if (!m_day_callbacks.TryGetValue(key, out funs))
            {
                funs = new List<Action>();
                m_day_callbacks.Add(key, funs);
                this.InitDayTimer(year, month, day, hour);
            }
            funs.Add(callback);
        }
        /// <summary>
        /// 移除
        /// </summary>
        public void RemoveDayTimer(int year, int month, int day, int hour, Action callback)
        {
            List<Action> funs;
            string key = year + "_" + month + "_" + day + "_" + hour;
            if (m_day_callbacks.TryGetValue(key, out funs))
            {
                funs.Remove(callback);
            }
        }
        private void InitDayTimer(int year, int month, int day, int hour)
        {
            DateTime zhengdian = new DateTime(year, month, day, hour, 0, 1);//设置成多一秒
            TimeSpan ts = zhengdian - DateTime.Now; //距离整点时间间隔

            long rate = (long)ts.TotalMilliseconds;
            if (ts.TotalMilliseconds >= 1000 * 60)//超过一分钟，定时器设置成总时长的一半，减少误差
                rate = (long)(ts.TotalMilliseconds * 0.5f);
            string key = year + "_" + month + "_" + day + "_" + hour;
            TimerManager.Instance.AddOnce(rate, OnDayTimer, key); // 设置timer时间，单位是毫秒
        }
        private void OnDayTimer(int timer_id, string param)
        {
            List<Action> funs;
            if (m_day_callbacks.TryGetValue(param, out funs))
            {
                string[] arr = param.Split('_');
                int year = int.Parse(arr[0]);
                int month = int.Parse(arr[1]);
                int day = int.Parse(arr[2]);
                int hour = int.Parse(arr[3]);

                DateTime zhengdian = new DateTime(year, month, day, hour, 0, 0);
                TimeSpan ts = zhengdian - DateTime.Now;
                if (ts.TotalMilliseconds <= 1000)//少于一秒
                {//到点执行
                    foreach (var fun in funs)
                        fun.Invoke();
                    m_day_callbacks.Remove(param);
                }
                else
                {
                    this.InitDayTimer(year, month, day, hour);
                }
            }

        }
        #endregion

        #region ws服务器时间
        /// <summary>
        /// ws时间，相对2009年开始经过的毫秒数
        /// 1.与客户端交互时使用
        /// 2.服务器启动后，ws会下发当前时间
        /// </summary>
        public long server_time
        {
            get { return TimeUtils.TimeSince2009 + m_adjust_time; }
        }
        /// <summary>
        /// 当前服务器相对ws的调整值
        /// </summary>
        public void SetAdjustTime(long t)
        {
            m_adjust_time = t;
        }
        #endregion
    }
}
