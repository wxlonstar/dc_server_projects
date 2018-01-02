using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 逻辑单位
    /// @author hannibal
    /// @time 2017-8-14
    /// </summary>
    public class Unit
    {
        protected uint m_UnitGUID = 0;

        public virtual void Setup()
        {

        }
        public virtual void Destroy()
        {

        }
        public virtual void Update()
        {

        }

        public virtual void EnterScene()
        {

        }
        public virtual void LeaveScene()
        {

        }

        public uint UnitGUID
        {
            get { return m_UnitGUID; }
            set { m_UnitGUID = value; }
        }
    }
}
