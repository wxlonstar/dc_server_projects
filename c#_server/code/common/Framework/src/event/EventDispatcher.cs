using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 事件控制器
    /// @author hannibal
    /// @time 2014-12-8
    /// </summary>
    public sealed class EventDispatcher
    {
        public delegate void RegistFunction(GameEvent evt);
        private Dictionary<string, RegistFunction> m_dispathcerMap;

        public EventDispatcher()
	    {
            m_dispathcerMap = new Dictionary<string, RegistFunction>();
	    }

        public void AddEventListener(string EventID, RegistFunction pFunction)
	    {
		    if (!m_dispathcerMap.ContainsKey(EventID))
		    {
			    m_dispathcerMap.Add(EventID, pFunction);
		    }
		    else
		    {
			    m_dispathcerMap[EventID] += pFunction;
		    }
	    }
        public void RemoveEventListener(string EventID, RegistFunction pFunction)
	    {
		    if (m_dispathcerMap.ContainsKey(EventID))
		    {
			    m_dispathcerMap[EventID] -= pFunction;
		    }
	    }
        public void TriggerEvent(string EventID, GameEvent info)
	    {
            info.type = EventID;
		    if (m_dispathcerMap.ContainsKey(EventID) && m_dispathcerMap[EventID] != null)
		    {
			    m_dispathcerMap[EventID](info);
		    }
	    }
        static public GameEvent m_DefaultGameEvent = new GameEvent();
        public void TriggerEvent(string EventID, params object[] list)
        {
            m_DefaultGameEvent.Init(list);
            m_DefaultGameEvent.type = EventID;
            if (m_dispathcerMap.ContainsKey(EventID) && m_dispathcerMap[EventID] != null)
            {
                m_dispathcerMap[EventID](m_DefaultGameEvent);
            }
        }
        public void Cleanup()
        {
            m_dispathcerMap.Clear();
        }
    }
}
