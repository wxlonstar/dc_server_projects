using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 账号数据缓存
    /// @author hannibal
    /// @time 2016-7-28
    /// </summary>
    public class AccountCacheManager : Singleton<AccountCacheManager>
    {
        /// <summary>
        /// 缓冲的账号角色列表
        /// </summary>
        private Dictionary<string, CacheAccountData> m_chache_account = null;
        /// <summary>
        /// 账号->账号名
        /// </summary>
        private Dictionary<long, string> m_account_2_name = null;

        public AccountCacheManager()
        {
            m_chache_account = new Dictionary<string, CacheAccountData>();
            m_account_2_name = new Dictionary<long, string>();
        }

        public void Setup()
        {
            this.RegisterEvent();
        }
        public void Destroy()
        {
            this.UnRegisterEvent();
            foreach (var obj in m_chache_account)
            {
                CommonObjectPools.Despawn(obj.Value);
            }
            m_chache_account.Clear();
            m_account_2_name.Clear();
        }
        private long tmpLastUpdate = 0;
        public void Tick()
        {
            //释放
            if (tmpLastUpdate < Time.timeSinceStartup)
            {
                //TODO:可以考虑放到另外一个线程去执行
                FreeUnusedMemery();
                tmpLastUpdate = Time.timeSinceStartup + 5 * 60 * 1000;//每5分钟执行一次
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
        /// 玩家进入
        /// </summary>
        private void OnPlayerLogin(long char_idx)
        {
            this.ModifyLogoutTime(char_idx, 0);
        }
        /// <summary>
        /// 玩家登出，修改登出时间：这个有点绕，大致过程char_idx->account_idx->account_name
        /// </summary>
        private void OnPlayerLogout(long char_idx)
        {
            this.ModifyLogoutTime(char_idx, Time.second_time);
        }
        /// <summary>
        /// 修改登出时间：这个有点绕，大致过程char_idx->account_idx->account_name
        /// </summary>
        private void ModifyLogoutTime(long char_idx, long time)
        {
            long account_idx = UnitManager.Instance.GetAccountByCharIdx(char_idx);
            if (account_idx > 0)
            {
                string account_name;
                if (m_account_2_name.TryGetValue(account_idx, out account_name))
                {
                    CacheAccountData cache_chars;
                    if (m_chache_account.TryGetValue(account_name, out cache_chars))
                        cache_chars.logout_time = time;
                }
            }
        }
        #endregion

        #region 集合管理
        public bool AddAccountData(string account_name, AccountData account_data)
        {
            this.RemoveAccountData(account_data.account_idx);

            CacheAccountData cache_chars = CommonObjectPools.Spawn<CacheAccountData>();
            cache_chars.logout_time = Time.second_time;
            cache_chars.account_data = account_data;
            m_chache_account.Add(account_name, cache_chars);
            m_account_2_name.Add(account_data.account_idx, account_name);

            return true;
        }
        public void RemoveAccountData(long account_idx)
        {
            string account_name;
            if(m_account_2_name.TryGetValue(account_idx, out account_name))
            {
                CacheAccountData cache_chars;
                if (m_chache_account.TryGetValue(account_name, out cache_chars))
                {
                    CommonObjectPools.Despawn(cache_chars);
                }
                m_chache_account.Remove(account_name);
            }
            m_account_2_name.Remove(account_idx);
        }
        /// <summary>
        /// 获取账号数据
        /// </summary>
        public bool GetAccountData(string account_name, ref AccountData account_data)
        {
            CacheAccountData cache_chars;
            if (m_chache_account.TryGetValue(account_name, out cache_chars))
            {
                account_data = cache_chars.account_data;
                return true;
            }
            return false;
        }
        #endregion

        #region 释放离线数据
        /// <summary>
        /// 释放离线玩家数据
        /// </summary>
        private void FreeUnusedMemery()
        {
            List<CacheAccountData> list = CollectOffline();
            if (list.Count <= 0) return;

            ///7天前的数据，先释放
            int leave_count = GlobalID.TOTAL_RELEASE_CACHE_ACCOUNT_PER;
            leave_count = leave_count - ReleaseOffline(list, 7, leave_count);
            if (leave_count <= 0 || list.Count <= 0) return;

            ///如果超过总缓存，再释放最近离线玩家
            if (m_chache_account.Count <= GlobalID.MAX_CACHE_ACCOUNT_COUNT) return;

            ///3天前的数据
            leave_count = leave_count - ReleaseOffline(list, 3, leave_count);
            if (leave_count <= 0 || list.Count <= 0) return;

            ///随机释放
            ReleaseOffline(list, 0, leave_count);
        }
        /// <summary>
        /// 收集离线玩家
        /// </summary>
        /// <returns></returns>
        private List<CacheAccountData> CollectOffline()
        {
            List<CacheAccountData> list = new List<CacheAccountData>();
            foreach (var obj in m_chache_account)
            {
                if (obj.Value.logout_time > 0)
                    list.Add(obj.Value);
            }
            return list;
        }
        /// <summary>
        /// 释放超过多少天未登陆的数据
        /// </summary>
        private int ReleaseOffline(List<CacheAccountData> list, int day, int total_release)
        {
            int release_count = 0;
            for (int i = list.Count - 1; i >= 0 && release_count < total_release; --i)
            {
                CacheAccountData account_data = list[i];
                if (account_data.logout_time > 0 && (Time.second_time - account_data.logout_time >= Utils.Day2Second(day)))
                {
                    this.RemoveAccountData(account_data.account_data.account_idx);
                    list.RemoveAt(i);
                    ++release_count;
                }
            }
            return release_count;
        }
        #endregion
    }
    /// <summary>
    /// 缓存的账号数据
    /// </summary>
    public class CacheAccountData : IPoolsObject
    {
        public long logout_time = 0;    //角色登出时间，如果是0，表示还未登出
        public AccountData account_data;

        public void Init()
        {
            logout_time = 0;
        }
    }
}
