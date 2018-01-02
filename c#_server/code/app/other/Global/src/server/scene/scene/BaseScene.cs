using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 场景基类
    /// @author hannibal
    /// @time 2017-8-14
    /// </summary>
    public class BaseScene
    {
        protected uint m_SceneGUID = 0;     //场景唯一id
        protected List<Unit> m_ListUnits;   //当前场景单位列表

        public BaseScene()
        {
            m_ListUnits = new List<Unit>();
        }

        public virtual void Setup(ushort type)
        {

        }
        public virtual void Destroy()
        {
            foreach (var obj in m_ListUnits)
            {
                obj.Destroy();
            }
            m_ListUnits.Clear();
        }
        public virtual void Update()
        {
        }

        public bool AddUnit(Unit unit)
        {
            if (unit == null) return false;
            if (m_ListUnits.Contains(unit)) return false;

            m_ListUnits.Add(unit);
            unit.EnterScene();
            return true;
        }
        public void RemoveUnit(Unit unit)
        {
            if (unit == null) return;
            unit.LeaveScene();
            m_ListUnits.Remove(unit);
        }

        public uint SceneGUID
        {
            get { return m_SceneGUID; }
            set { m_SceneGUID = value; }
        }
    }
}
