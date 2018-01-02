using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 管理服务器单位
    /// @author hannibal
    /// @time 2016-9-24
    /// </summary>
    public class UnitManager : Singleton<UnitManager>
    {
        private Dictionary<long, Unit> m_cache_units = null;//所有单位集合
        private Dictionary<string, long> m_name_units;//姓名->角色

        public UnitManager()
        {
            m_cache_units = new Dictionary<long, Unit>();
            m_name_units = new Dictionary<string, long>();
        }

        public void Setup()
        {

        }
        public void Destroy()
        {
            foreach (var obj in m_cache_units)
            {
                obj.Value.OnLeave();
                CommonObjectPools.Despawn(obj.Value);
            }
            m_cache_units.Clear();
            m_name_units.Clear();
        }
        private long tmpLastUpdate = 0;
        public void Tick()
        {
            //逻辑
            foreach (var obj in m_cache_units)
            {
                obj.Value.Update();
            }
            //保存
            int update_count = 0;
            Unit unit = null;
            foreach (var obj in m_cache_units)
            {
                unit = obj.Value;
                if (unit.NeedSave())
                {
                    unit.Save();
                    if (++update_count > 60) break;//当次循环最大保存数量
                }
            }
            //释放
            if(tmpLastUpdate < Time.timeSinceStartup)
            {
                //TODO:可以考虑放到另外一个线程去执行
                FreeUnusedMemery();
                tmpLastUpdate = Time.timeSinceStartup + 5 * 60 * 1000;//每5分钟执行一次
            }
        }
        /// <summary>
        /// 登入处理
        /// </summary>
        public void HandleLogin(ushort ss_uid, PlayerInfoForGL data)
        {
            Unit unit = GetUnitByIdx(data.char_idx);
            if(unit != null)
            {
                this.RemoveUnit(unit);
            }
            //创建玩家
            unit = CommonObjectPools.Spawn<Unit>();
            unit.Setup(ss_uid, data);
            UnitManager.Instance.AddUnit(unit);
            unit.OnEnter();
        }
        /// <summary>
        /// 登出处理
        /// </summary>
        public void HandleLogout(long char_idx)
        {
            Unit unit = UnitManager.Instance.GetUnitByIdx(char_idx);
            if (unit != null)
            {
                unit.OnLeave();
            }
        }
        #region 集合管理
        public bool AddUnit(Unit unit)
        {
            if (unit == null) return false;
            if (m_cache_units.ContainsKey(unit.char_idx))
            {
                this.RemoveUnit(unit);
            }
            m_cache_units.Add(unit.char_idx, unit);
            m_name_units.Remove(unit.char_name);
            m_name_units.Add(unit.char_name, unit.char_idx);

            return true;
        }
        public void RemoveUnit(Unit unit)
        {
            if (unit == null) return;
            m_cache_units.Remove(unit.char_idx);
            m_name_units.Remove(unit.char_name);
            CommonObjectPools.Despawn(unit);
        }

        public Unit GetUnitByIdx(long char_idx)
        {
            Unit unit = null;
            if (m_cache_units.TryGetValue(char_idx, out unit))
                return unit;
            return null;
        }
        public Unit GetUnitByName(string char_name)
        {
            long char_idx = 0;
            if (m_name_units.TryGetValue(char_name, out char_idx))
            {
                return GetUnitByIdx(char_idx) as Unit;
            }
            return null;
        }
        /// <summary>
        /// 查询玩家是否在线
        /// </summary>
        public bool IsOnline(long char_idx)
        {
            Unit unit = GetUnitByIdx(char_idx);
            if (unit != null && unit.is_online)
                return true;
            return false;
        }
        /// <summary>
        /// 缓存数量
        /// </summary>
        public int GetUnitCount()
        {
            return m_cache_units.Count;
        }
        /// <summary>
        /// 在线数量
        /// </summary>
        /// <returns></returns>
        public int GetOnlineUnitCount()
        {
            int count = 0;
            Unit unit = null;
            foreach (var obj in m_cache_units)
            {
                if (unit != null && unit.is_online)
                    count++;
            }
            return count;
        }
        #endregion

        #region 释放离线数据
        /// <summary>
        /// 释放离线玩家数据
        /// </summary>
        private void FreeUnusedMemery()
        {
            List<Unit> list = CollectOffline();
            if (list.Count <= 0) return;

            ///7天前的数据，先释放
            int leave_count = GlobalID.TOTAL_RELEASE_CACHE_UNIT_PER;
            leave_count = leave_count - ReleaseOffline(list, 7, leave_count);
            if (leave_count <= 0 || list.Count <= 0) return;

            ///如果超过总缓存，再释放最近离线玩家
            if (m_cache_units.Count <= GlobalID.MAX_CACHE_UNIT_COUNT) return;

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
        private List<Unit> CollectOffline()
        {
            List<Unit> list = new List<Unit>();
            foreach(var obj in m_cache_units)
            {
                if (!obj.Value.is_online)
                    list.Add(obj.Value);
            }
            return list;
        }
        /// <summary>
        /// 释放超过多少天未登陆的数据
        /// </summary>
        private int ReleaseOffline(List<Unit> list, int day, int total_release)
        {
            int release_count = 0;
            for (int i = list.Count - 1; i >= 0 && release_count < total_release; --i)
            {
                Unit unit = list[i];
                if (!unit.is_online && (Time.second_time - unit.last_access_time >= Utils.Day2Second(day)))
                {
                    this.RemoveUnit(unit);
                    list.RemoveAt(i);
                    ++release_count;
                }
            }
            return release_count;
        }
        #endregion
    }
}
