﻿using System;
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
        public void Tick()
        {
            //逻辑
            foreach (var obj in m_cache_units)
            {
                obj.Value.Update();
            }
        }
        /// <summary>
        /// 登入处理
        /// </summary>
        public void HandleLogin(ClientUID client_uid, InterServerID srv_uid, PlayerInfoForFS data)
        {
            Unit unit = GetUnitByIdx(data.char_idx);
            if (unit != null)
            {//可能上次退出时，没有清除数据
                this.RemoveUnit(unit);
            }
            //创建玩家
            unit = CommonObjectPools.Spawn<Unit>();
            unit.Setup(client_uid, srv_uid, data);
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
                this.RemoveUnit(unit);
            }
        }
        #region 集合管理
        public bool AddUnit(Unit unit)
        {
            if (unit == null) return false;
            if (m_cache_units.ContainsKey(unit.char_idx))
            {
                RemoveUnit(unit);
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
        /// 缓存数量
        /// </summary>
        public int GetUnitCount()
        {
            return m_cache_units.Count;
        }
        #endregion
    }
}
