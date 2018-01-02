using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 逻辑单位
    /// @author hannibal
    /// @time 2016-8-14
    /// </summary>
    public abstract class Unit : IPoolsObject
    {
        protected eUnitType m_unit_type = eUnitType.NONE;
        protected uint m_obj_type = 0;
        protected long m_obj_idx = 0;
        protected long m_scene_obj_idx = 0;//所在场景idx
        protected uint m_scene_type_idx = 0;

        protected Position2D m_pos = new Position2D();
        protected PlayerAttribute m_unit_attr = null;

        public Unit()
        {

        }
        public virtual void Init()
        {
            m_obj_type = 0;
            m_obj_idx = 0;
            m_scene_obj_idx = 0;
            m_scene_type_idx = 0;
            m_pos.Set(0, 0);
        }
        public virtual void OnEnter()
        {

        }
        public virtual void OnLeave()
        {

        }
        public virtual void Update()
        {

        }
        /// <summary>
        /// 加载数据
        /// </summary>
        public virtual void LoadData(object info)
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
        public abstract UnitAOIInfo GetUnitAOIInfo();
        /// <summary>
        /// 修改位置
        /// </summary>
        public virtual void ModifyPos(int x, int y)
        {
            m_pos.Set(x, y);
        }
        public eUnitType unit_type
        {
            get { return m_unit_type; }
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
        public virtual int pos_x
        {
            get { return m_pos.x; }
        }
        public virtual int pos_y
        {
            get { return m_pos.y; }
        }
        public PlayerAttribute unit_attr
        {
            get { return m_unit_attr; }
        }
    }
}
