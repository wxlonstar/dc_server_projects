using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 玩家db数据缓存
    /// @author hannibal
    /// @time 2017-9-7
    /// </summary>
    public class PlayerCacheManager : Singleton<PlayerCacheManager>
    {
        private uint m_capacity_cache;//最大缓存

        private Dictionary<long, PlayerCache> m_cache_members = null;//根据id查找
        private Dictionary<string, long> m_cache_name_members = null;//根据名称查找

        public PlayerCacheManager()
        {
            m_cache_members = new Dictionary<long, PlayerCache>();
            m_cache_name_members = new Dictionary<string, long>();
        }

        public void Setup()
        {
        }
        public void Destroy()
        {
            foreach (var obj in m_cache_members)
            {
                CommonObjectPools.Despawn(obj.Value);
            }
            m_cache_members.Clear();
            m_cache_name_members.Clear();
        }
        public void Tick()
        {
            int update_count = 0;
            PlayerCache member = null;
            foreach(var obj in m_cache_members)
            {
                member = obj.Value;
                if(member.NeedSave())
                {
                    member.Save();
                    if (++update_count > 60) break;//当次循环最大保存数量
                }
            }
        }
        /// <summary>
        /// 初始化缓存，会预加载部分玩家数据
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="load_max"></param>
        public void Load(uint capacity, uint load_max)
        {
            m_capacity_cache = capacity;
            if (load_max > capacity) load_max = capacity;
            PreLoadPlayer(load_max);
        }
        /// <summary>
        /// 玩家登出
        /// </summary>
        public void HanldeLogoutClient(long char_idx)
        {
            Log.Debug("玩家退出:" + char_idx);
            PlayerCache member;
            if (m_cache_members.TryGetValue(char_idx, out member))
            {
                member.UpdateAttribute(eUnitModType.UMT_time_last_logout, Time.second_time);
                member.Save();
            }
            this.RemoveMember(char_idx);
            //邮箱
            MailCacheManager.Instance.HanldeLogoutClient(char_idx);
        }
        /// <summary>
        /// 预加载数据
        /// </summary>
        /// <param name="load_max"></param>
        private void PreLoadPlayer(uint load_max)
        {
            SQLCharHandle.QueryCharForPreload(load_max, list =>
            {
                Log.Info("预加载玩家数量:" + list.Count);
                foreach (var char_idx in list)
                {
                    LoadPlayer(char_idx, null);
                }
            }
            );
        }
        /// <summary>
        /// 加载单个玩家数据
        /// </summary>
        /// <param name="char_idx"></param>
        public void LoadPlayer(long char_idx, Action<long> callback)
        {
            if (m_cache_members.ContainsKey(char_idx))
            {
                Log.Debug("玩家数据已经加载:" + char_idx);
                if (callback != null) callback(char_idx);
                return;
            }

            Log.Debug("请求加载玩家数据:" + char_idx);
            PlayerCache member = CommonObjectPools.Spawn<PlayerCache>();
            member.Load(char_idx, is_load =>
            {
                if (is_load)
                {
                    m_cache_members.Add(char_idx, member);
                    if (m_cache_name_members.ContainsKey(member.ss_data.char_name))
                    {
                        Log.Warning("存在相同玩家名:" + member.ss_data.char_name);
                    }
                    else
                    {
                        m_cache_name_members.Add(member.ss_data.char_name, char_idx);
                    }
                    if (callback != null) callback(char_idx);
                }
                else
                {
                    Log.Warning("玩家数据加载失败:" + char_idx);
                    CommonObjectPools.Despawn(member);
                    if (callback != null) callback(0);
                }
            }
            );
        }
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～集合管理器～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="char_idx">角色id</param>
        /// <returns></returns>
        public PlayerCache GetMember(long char_idx)
        {
            PlayerCache member;
            if (m_cache_members.TryGetValue(char_idx, out member))
                return member;
            return null;
        }
        /// <summary>
        /// 根据名称获取缓存数据
        /// </summary>
        /// <param name="char_name">角色名</param>
        /// <returns></returns>
        public PlayerCache GetMemberByName(string char_name)
        {
            long char_idx;
            if(m_cache_name_members.TryGetValue(char_name, out char_idx))
            {
                PlayerCache member;
                if (m_cache_members.TryGetValue(char_idx, out member))
                    return member;
            }
            return null;
        }
        /// <summary>
        /// 从集合中移除
        /// </summary>
        /// <param name="char_idx">角色id</param>
        public void RemoveMember(long char_idx)
        {
            PlayerCache member;
            if (m_cache_members.TryGetValue(char_idx, out member))
            {
                m_cache_name_members.Remove(member.ss_data.char_name);
                CommonObjectPools.Despawn(member);
            }
            m_cache_members.Remove(char_idx);
        }
        /// <summary>
        /// 缓存玩家数量
        /// </summary>
        /// <returns></returns>
        public int GetCacheCount()
        {
            return m_cache_members.Count;
        }
    }
}
