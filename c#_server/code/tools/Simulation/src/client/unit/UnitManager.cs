using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 管理服务器单位
    /// @author hannibal
    /// @time 2016-8-14
    /// </summary>
    public class UnitManager : Singleton<UnitManager>
    {
        private Dictionary<long, Unit> m_units;
        private Player m_main_player = null;

        public UnitManager()
        {
            m_units = new Dictionary<long, Unit>();
        }

        public void Setup()
        {

        }
        public void Destroy()
        {
            this.RemoveAll();
        }
        public void Tick()
        {
            foreach (var obj in m_units)
            {
                obj.Value.Update();
            }
        }

        public bool AddUnit(Unit unit)
        {
            if (unit == null) return false;
            if (m_units.ContainsKey(unit.obj_idx)) return false;

            if (unit.obj_idx == PlayerDataMgr.Instance.main_player_id)
                m_main_player = unit as Player;

            m_units.Add(unit.obj_idx, unit);
            return true;
        }
        public void RemoveUnit(long unit_idx)
        {
            Unit unit = null;
            if(m_units.TryGetValue(unit_idx, out unit))
            {
                unit.Destroy();
                CommonObjectPools.Despawn(unit);
            }
            if (m_main_player != null && m_main_player.obj_idx == unit_idx)
                m_main_player = null;

            m_units.Remove(unit_idx);
        }
        public void RemoveAll()
        {
            foreach(var obj in m_units)
            {
                obj.Value.Destroy();
                CommonObjectPools.Despawn(obj.Value);
            }
            m_units.Clear();
            m_main_player = null;
        }
        public Dictionary<long, Unit> units
        {
            get { return m_units; }
        }
        public Unit GetUnitByIdx(long id)
        {
            Unit unit = null;
            if (m_units.TryGetValue(id, out unit))
                return unit;
            return null;
        }
        public Player main_player
        {
            get { return m_main_player; }
        }
    }
}
