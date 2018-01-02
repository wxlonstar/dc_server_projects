using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 次数管理器
    /// @author hannibal
    /// @time 2016-10-11
    /// </summary>
    public class CounterManager : Singleton<CounterManager>
    {
        private Dictionary<long, PlayerCounter> m_counter_members = null;

        public CounterManager()
        {
            m_counter_members = new Dictionary<long, PlayerCounter>();
        }

        public void Setup()
        {
            this.RegisterEvent();
        }
        public void Destroy()
        {
            this.UnRegisterEvent();
            foreach (var member in m_counter_members)
            {
                member.Value.Destroy();
            }
            m_counter_members.Clear();
        }
        public void Tick()
        {
            int update_count = 0;
            PlayerCounter member = null;
            foreach (var obj in m_counter_members)
            {
                member = obj.Value;
                if (member.NeedSave())
                {
                    member.Save();
                    if (++update_count > 60) break;//当次循环最大更新数量
                }
            }
        }
        /// 外部修改次数
        /// </summary>
        /// <param name="char_idx"></param>
        /// <param name="type"></param>
        /// <param name="modify_value">消耗值</param>
        /// <returns>true表示可以扣除次数</returns>
        public bool ConsumeCounter(long char_idx, eCounterType type, ushort modify_value)
        {
            PlayerCounter player_counter;
            if (m_counter_members.TryGetValue(char_idx, out player_counter))
            {
                player_counter.ConsumeCounter(type, modify_value);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取次数
        /// </summary>
        public ushort GetCounter(long char_idx, eCounterType type)
        {
            PlayerCounter player_counter;
            if (m_counter_members.TryGetValue(char_idx, out player_counter))
            {
                return player_counter.GetAndModifyCounter(type);
            }
            return 0;
        }
        #region 事件
        private void RegisterEvent()
        {
            EventController.AddEventListener(EventID.PLAYER_ENTER_GAME, OnGameEvent);
            EventController.AddEventListener(EventID.PLAYER_LEAVE_GAME, OnGameEvent);
        }
        private void UnRegisterEvent()
        {
            EventController.RemoveEventListener(EventID.PLAYER_ENTER_GAME, OnGameEvent);
            EventController.RemoveEventListener(EventID.PLAYER_LEAVE_GAME, OnGameEvent);
        }
        private void OnGameEvent(GameEvent evt)
        {
            switch (evt.type)
            {
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
        /// 玩家登录
        /// </summary>
        private void OnPlayerLogin(long char_idx)
        {
            this.AddMember(char_idx);
        }
        /// <summary>
        /// 玩家登出
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
            m_counter_members.Remove(char_idx);
            PlayerCounter player_counter = new PlayerCounter();
            m_counter_members.Add(char_idx, player_counter);
            player_counter.Setup(char_idx);
        }
        public void RemoveMember(long char_idx)
        {
            PlayerCounter player_counter;
            if (m_counter_members.TryGetValue(char_idx, out player_counter))
            {
                player_counter.Save();
                player_counter.Destroy();
            }
            m_counter_members.Remove(char_idx);
        }
        #endregion
    }
}
