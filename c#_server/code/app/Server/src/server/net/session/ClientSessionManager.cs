using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 会话管理器
    /// @author hannibal
    /// @time 2017-5-25
    /// </summary>
    public class ClientSessionManager : Singleton<ClientSessionManager>
    {
        private Dictionary<uint, float> m_DicAcceptSession = null;
        private Dictionary<uint, ClientSession> m_DicSession = null;

        public void Setup()
        {
            m_DicAcceptSession = new Dictionary<uint, float>();
            m_DicSession = new Dictionary<uint, ClientSession>();
        }
        public void Destroy()
        {
            CloseAllSession();
            if (m_DicSession != null)
            {
                m_DicSession.Clear();
                m_DicSession = null;
            }
            if (m_DicAcceptSession != null)
            {
                m_DicAcceptSession.Clear();
                m_DicAcceptSession = null;
            }
        }

        public void Tick()
        {
            foreach (var obj in m_DicSession)
            {
                obj.Value.Update();
            }
        }

        public bool AddAcceptSession(uint conn_idx)
        {
            if (m_DicAcceptSession.ContainsKey(conn_idx))
                return false;
            m_DicAcceptSession.Add(conn_idx, Time.timeSinceStartup);
            return true;
        }
        public bool CleanupAcceptSession(uint conn_idx)
        {
            return m_DicAcceptSession.Remove(conn_idx);
        }
        public bool HasAcceptSession(uint conn_idx)
        {
            if (!m_DicAcceptSession.ContainsKey(conn_idx))
                return false;
            return true;
        }
        public uint GetAcceptSessionCount()
        {
            return (uint)m_DicAcceptSession.Count;
        }

        public ClientSession AddSession(uint conn_idx)
        {
            if (!m_DicAcceptSession.ContainsKey(conn_idx))
                return null;

            ClientSession session = new ClientSession();
            session.Setup(conn_idx);
            m_DicSession.Add(conn_idx, session);
            return session;
        }
        public bool CleanupSession(uint conn_idx)
        {
            CleanupAcceptSession(conn_idx);

            ClientSession session;
            if (m_DicSession.TryGetValue(conn_idx, out session))
            {
                session.Destroy();
            }
            m_DicSession.Remove(conn_idx);
            return true;
        }
        public void CloseAllSession()
        {
            if (m_DicSession == null)return;

            foreach (var obj in m_DicSession)
            {
                obj.Value.Destroy();
            }
            m_DicSession.Clear();
        }

        public ClientSession GetSession(uint conn_idx)
        {
            ClientSession session;
            if (m_DicSession.TryGetValue(conn_idx, out session))
            {
                return session;
            }
            return null;
        }
        public uint GetClientSessionCount()
        {
            return (uint)m_DicSession.Count;
        }
    }

}
