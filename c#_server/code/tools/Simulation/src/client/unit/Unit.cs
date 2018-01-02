using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 逻辑单位
    /// @author hannibal
    /// @time 2016-8-14
    /// </summary>
    public class Unit
    {
        protected uint m_obj_type = 0;
        protected long m_obj_idx = 0;
        protected long m_scene_obj_idx = 0;//所在场景idx
        protected uint m_scene_type_idx = 0;

        protected Position2D m_pos = new Position2D();

        public Unit()
        {

        }
        public virtual void Init()
        {

        }
        public virtual void Setup()
        {
        }
        public virtual void Destroy()
        {

        }
        public virtual void Update()
        {

        }
        /// <summary>
        /// 加载数据
        /// </summary>
        public virtual void LoadData(UnitAOIInfo info)
        {

        }

        public virtual void EnterScene(uint scene_type, long scene_obj)
        {
            m_scene_type_idx = scene_type;
            m_scene_obj_idx = scene_obj;
        }
        public virtual void LeaveScene()
        {
            m_scene_type_idx = 0;
            m_scene_obj_idx = 0;
        }
        /// <summary>
        /// 拷贝数据到unitinfo
        /// </summary>
        /// <param name="info"></param>
        public virtual void CopyUnitInfo(UnitAOIInfo info)
        {

        }
        public void ModifyPos(int x, int y)
        {
            m_pos.Set(x, y);
        }

        public uint obj_type
        {
            get { return m_obj_type; }
            set { m_obj_type = value; }
        }
        public long obj_idx
        {
            get { return m_obj_idx; }
            set { m_obj_idx = value; }
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
        public Position2D pos
        {
            get { return m_pos; }
        }
    }
}
