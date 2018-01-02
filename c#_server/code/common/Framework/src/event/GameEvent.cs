using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 事件参数
    /// @author hannibal
    /// @time 2016-3-26
    /// </summary>
    public struct GameEvent
    {
        public const int CAPACITY = 10;

        private string m_type;
        private int m_Length;
        private object[] m_data;

        public GameEvent(params object[] list)
        {
            m_type = "";
            m_Length = 0;
            m_data = new object[CAPACITY];
            for (int i = 0; i < list.Length; i++)
            {
                Set(i, list[i]);
            }
        }

        public void Init(params object[] list)
        {
            m_type = "";
            m_Length = 0;
            if (m_data == null) m_data = new object[CAPACITY];
            for (int i = 0; i < list.Length; i++)
            {
                Set(i, list[i]);
            }
        }

        public T Get<T>(int idx)
        {
            if (idx >= m_Length)
            {
                Log.Error("GameEvent::Get 参数错误:" + idx);
            }

            object obj = m_data[idx];
            return (T)obj;
        }

        private void Set(int idx, object val)
        {
            if (idx >= CAPACITY)
            {
                Log.Error("GameEvent::Set 参数错误:" + idx);
                return;
            }
            m_data[idx] = val;
            m_Length = idx + 1;
        }

        public string type
        {
            get { return m_type; }
            set { m_type = value; }
        }
    }
}
