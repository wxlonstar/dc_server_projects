using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 通过db转存的游戏事件
    /// @author hannibal
    /// @time 2016-10-11
    /// </summary>
    public class DBEventManager : Singleton<DBEventManager>
    {
        private Dictionary<long, DBEvent> m_event_members = null;

        public DBEventManager()
        {
            m_event_members = new Dictionary<long, DBEvent>();
        }

        public void Setup()
        {
            this.RegisterEvent();
        }
        public void Destroy()
        {
            this.UnRegisterEvent();
            foreach (var member in m_event_members)
            {
                member.Value.Destroy();
            }
            m_event_members.Clear();
        }
        public void Tick()
        {
            int update_count = 0;
            DBEvent member = null;
            foreach (var obj in m_event_members)
            {
                member = obj.Value;
                if (member.NeedLoad())
                {
                    member.Load();
                    if (++update_count > 60) break;//当次循环最大更新数量
                }
            }
        }
        #region 事件
        private void RegisterEvent()
        {
            EventController.AddEventListener(EventID.DB_NEW_EVENT, OnGameEvent);
            EventController.AddEventListener(EventID.PLAYER_ENTER_GAME, OnGameEvent);
            EventController.AddEventListener(EventID.PLAYER_LEAVE_GAME, OnGameEvent);
        }
        private void UnRegisterEvent()
        {
            EventController.RemoveEventListener(EventID.DB_NEW_EVENT, OnGameEvent);
            EventController.RemoveEventListener(EventID.PLAYER_ENTER_GAME, OnGameEvent);
            EventController.RemoveEventListener(EventID.PLAYER_LEAVE_GAME, OnGameEvent);
        }
        private void OnGameEvent(GameEvent evt)
        {
            switch (evt.type)
            {
                case EventID.DB_NEW_EVENT:
                    {
                        long target_char_idx = evt.Get<long>(0);
                        DBEvent db_evt;
                        if (m_event_members.TryGetValue(target_char_idx, out db_evt))
                            db_evt.Load();
                    }
                    break;
                case EventID.PLAYER_ENTER_GAME:
                    {
                        long char_idx = evt.Get<long>(0);
                        this.OnPlayerLogin(char_idx);
                    }
                    break;
                case EventID.PLAYER_LEAVE_GAME:
                    {
                        long char_idx = evt.Get<long>(0);
                        this.OnPlayerLogout(char_idx);
                    }
                    break;
            }
        }
        /// <summary>
        /// 登入处理
        /// </summary>
        private void OnPlayerLogin(long char_idx)
        {
            this.AddMember(char_idx);
        }
        /// <summary>
        /// 登出处理
        /// </summary>
        private void OnPlayerLogout(long char_idx)
        {
            this.RemoveMember(char_idx);
        }
        #endregion
        #region 集合管理
        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="char_idx">角色id</param>
        /// <returns></returns>
        public void AddMember(long char_idx)
        {
            m_event_members.Remove(char_idx);
            DBEvent db_evt = new DBEvent();
            m_event_members.Add(char_idx, db_evt);
            db_evt.Setup(char_idx);
        }
        public void RemoveMember(long char_idx)
        {
            DBEvent db_evt;
            if (m_event_members.TryGetValue(char_idx, out db_evt))
                db_evt.Destroy();
            m_event_members.Remove(char_idx);
        }
        #endregion
    }
}
