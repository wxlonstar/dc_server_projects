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
                CommonObjectPools.Despawn(member);
            }
            m_cache_members.Clear();
        }
        public void Tick()
        {
            MemberRelation member = null;
            foreach (var obj in m_cache_members)
            {
                member = obj.Value;
                member.Update();
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
        #endregion
        /// <summary>
        /// 登入处理
        /// </summary>
        private void OnPlayerLogin(long char_idx)
        {
            this.RemoveMember(char_idx);

            MemberRelation member = CommonObjectPools.Spawn<MemberRelation>();
            member.Setup(char_idx);
            m_cache_members.Add(char_idx, member);
        }
        /// <summary>
        /// 登出处理
        /// </summary>
        private void OnPlayerLogout(long char_idx)
        {
            this.RemoveMember(char_idx);
        }
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
                CommonObjectPools.Despawn(member);
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
        /// <summary>
        /// 是否存在关系
        /// </summary>
        public bool HaveRelationFlag(long char_idx, long target_char_idx, eRelationFlag flag)
        {
            MemberRelation member = this.GetMember(char_idx);
            if (member != null)
            {
                return member.HaveRelationFlag(target_char_idx, flag);
            }
            return false;
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
