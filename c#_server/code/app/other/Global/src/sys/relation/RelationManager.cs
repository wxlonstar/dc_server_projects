using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 角色关系管理器
    /// @author hannibal
    /// @time 2016-9-27
    /// </summary>
    public class RelationManager : Singleton<RelationManager>
    {
        private Dictionary<long, MemberRelation> m_cache_members = null;

        public RelationManager()
        {
            m_cache_members = new Dictionary<long, MemberRelation>();
        }

        public void Setup()
        {
            this.RegisterEvent();
        }
        public void Destroy()
        {
            this.UnRegisterEvent();
            foreach (var member in m_cache_members)
            {
                member.Value.Destroy();
            }
            m_cache_members.Clear();
        }
        public void Tick()
        {
            foreach (var obj in m_cache_members)
            {
                obj.Value.Update();
            }

            int update_count = 0;
            MemberRelation member = null;
            foreach (var obj in m_cache_members)
            {
                member = obj.Value;
                if (member.NeedSave())
                {
                    member.Save();
                    if (++update_count > 60) break;//当次循环最大保存数量
                }
            }
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
        /// 登入处理
        /// </summary>
        private void OnPlayerLogin(long char_idx)
        {
            MemberRelation member;
            if (m_cache_members.TryGetValue(char_idx, out member))
            {
                member.SyncDataFromUnit();
                return;
            }

            Log.Debug("请求加载玩家关系数据:" + char_idx);
            member = new MemberRelation();
            m_cache_members.Add(char_idx, member);
            member.Setup(char_idx);
        }
        /// <summary>
        /// 登出处理
        /// </summary>
        private void OnPlayerLogout(long char_idx)
        {
            RemoveMember(char_idx);
        }
        #endregion
        #region 集合管理
        /// <summary>
        /// 获取成员
        /// </summary>
        /// <param name="char_idx">角色id</param>
        /// <returns></returns>
        public MemberRelation GetMember(long char_idx)
        {
            MemberRelation member;
            if (m_cache_members.TryGetValue(char_idx, out member))
                return member;
            return null;
        }
        public void RemoveMember(long char_idx)
        {
            MemberRelation member;
            if (m_cache_members.TryGetValue(char_idx, out member))
            {
                member.Destroy();
            }
            m_cache_members.Remove(char_idx);
        }
        /// <summary>
        /// 缓存数量
        /// </summary>
        public int GetMemberCount()
        {
            return m_cache_members.Count;
        }
        #endregion

        #region 属性改变，同步信息
        /// <summary>
        /// 属性改变
        /// </summary>
        public void UpdateAttribute(long char_idx, eUnitModType type, long value)
        {
            MemberRelation member = null;
            foreach (var obj in m_cache_members)
            {
                member = obj.Value;
                member.UpdateAttribute(char_idx, type, value);
            }
        }
        public void UpdateAttribute(long char_idx, eUnitModType type, string value)
        {
            MemberRelation member = null;
            foreach (var obj in m_cache_members)
            {
                member = obj.Value;
                member.UpdateAttribute(char_idx, type, value);
            }
        }
        #endregion
    }
}
