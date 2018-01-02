using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 场景基类
    /// @author hannibal
    /// @time 2016-8-14
    /// </summary>
    public class BaseScene
    {
        protected uint m_scene_type_idx = 0;
        protected long m_scene_obj_idx = 0;//所在场景idx
        protected List<Unit> m_units;       //当前场景单位列表

        public BaseScene()
        {
            m_units = new List<Unit>();
        }

        public virtual void Setup(uint scene_type, long scene_obj)
        {
            m_scene_type_idx = scene_type;
            m_scene_obj_idx = scene_obj;
        }
        public virtual void Destroy()
        {
            foreach (var obj in m_units)
            {
                obj.OnLeave();
            }
            m_units.Clear();
        }
        public virtual void Update()
        {
        }

        public bool AddUnit(Unit unit)
        {
            if (unit == null) return false;
            if (m_units.Contains(unit)) return false;

            m_units.Add(unit);
            unit.EnterScene(m_scene_type_idx, m_scene_obj_idx);
            AOIManager.Instance.Add(unit.obj_idx, m_scene_obj_idx, unit.pos_y, unit.pos_x);
            return true;
        }
        public void RemoveUnit(Unit unit)
        {
            if (unit == null) return;
            unit.LeaveScene();
            m_units.Remove(unit);
            AOIManager.Instance.Remove(unit.obj_idx);
        }

        public uint scene_type_idx
        {
            get { return m_scene_type_idx; }
            set { m_scene_type_idx = value; }
        }
        public long scene_obj_idx
        {
            get { return m_scene_obj_idx; }
            set { m_scene_obj_idx = value; }
        }
    }
}
