using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// db事件
    /// @author hannibal
    /// @time 2016-11-1
    /// </summary>
    public class DBEvent
    {
        private long m_char_idx = 0;
        private Dictionary<long, DBEventInfo> m_events = null;

        private long m_last_load_time = 0;      //最后加载时间

        public DBEvent()
        {
            m_events = new Dictionary<long, DBEventInfo>();
        }

        public void Setup(long char_idx)
        {
            m_char_idx = char_idx;
            Load();
        }
        public void Destroy()
        {
            m_events.Clear();
            m_char_idx = 0;
        }
        #region 加载
        /// <summary>
        /// 加载
        /// </summary>
        public void Load()
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null) return;

            //查询和自己相关的事件
            SQLDBEventHandle.QueryDBEvent(m_char_idx, player.db_id, OnLoadedEvent);

            m_last_load_time = Time.timeSinceStartup;
        }
        private void OnLoadedEvent(List<DBEventInfo> list)
        {
            if (list.Count == 0 || m_char_idx == 0) return;

            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null) return;

            for(int i = list.Count - 1; i >= 0; --i)
            {
                if (m_events.ContainsKey(list[i].event_idx))
                    list.RemoveAt(i);
            }
            if (list.Count == 0) return;

            //执行所有事件
            List<long> list_dels = new List<long>();
            foreach(var obj in list)
            {
                player.HandleDBEvent(obj);
                list_dels.Add(obj.event_idx);
            }

            //目前的做法是直接删除，后期可以根据HandleDBEvent的返回值确定是否删除
            if (list_dels.Count > 0)
            {
                SQLDBEventHandle.DeleteDBEvent(list_dels, player.db_id);
            }
        }
        /// <summary>
        /// 是否需要加载
        /// </summary>
        /// <returns></returns>
        public bool NeedLoad()
        {
            if (Time.timeSinceStartup - m_last_load_time > 1000 * db_event.DB_UPDATE_TIME_OFFSET)
                return true;
            return false;
        }
        #endregion
    }
}
