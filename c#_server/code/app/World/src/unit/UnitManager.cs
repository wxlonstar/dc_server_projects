using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 管理服务器单位
    /// @author hannibal
    /// @time 2017-8-14
    /// </summary>
    public class UnitManager : Singleton<UnitManager>
    {
        private Dictionary<uint, Unit> m_DicUnits;

        public UnitManager()
        {
            m_DicUnits = new Dictionary<uint,Unit>();
        }

        public void Setup()
        {

        }
        public void Destroy()
        {
            foreach (var obj in m_DicUnits)
            {
                obj.Value.Destroy();
            }
            m_DicUnits.Clear();
        }
        public void Tick()
        {
            foreach (var obj in m_DicUnits)
            {
                obj.Value.Update();
            }
        }

        public bool AddUnit(Unit unit)
        {
            if (unit == null) return false;
            if (m_DicUnits.ContainsKey(unit.UnitGUID)) return false;

            m_DicUnits.Add(unit.UnitGUID, unit);
            return true;
        }
        public void RemoveUnit(Unit unit)
        {
            if (unit == null) return;
            m_DicUnits.Remove(unit.UnitGUID);
        }

        public Unit GetUnitByID(uint id)
        {
            Unit unit = null;
            if (m_DicUnits.TryGetValue(id, out unit))
                return unit;
            return null;
        }
    }
}
