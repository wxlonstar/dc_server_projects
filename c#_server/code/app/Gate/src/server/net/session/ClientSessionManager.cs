using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 会话管理器
    /// @author hannibal
    /// @time 2016-5-25
    /// </summary>
    public class ClientSessionManager : Singleton<ClientSessionManager>
    {
        private Dictionary<long, ClientSession> m_sessions = null;
        private Dictionary<long, float> m_accept_sessions = null;
        private Dictionary<long, long> m_account_sessions = null;//账号对应的session

        public ClientSessionManager()
        {
            m_sessions = new Dictionary<long, ClientSession>();
            m_accept_sessions = new Dictionary<long, float>();
            m_account_sessions = new Dictionary<long, long>();
        }

        public void Setup()
        {
        }
        public void Destroy()
        {
            CloseAllSession();
            m_sessions.Clear();
            m_accept_sessions.Clear();
        }

        public void Tick()
        {
            //update可以导致删除session，所以用这种方式遍历
            List<ClientSession> list = new List<ClientSession>(m_sessions.Values);
            for (int i = list.Count - 1; i >= 0; --i)
            {
                list[i].Update();
            }
            this.TickKickout();
            this.TickUploadCount();
        }

        /// <summary>
        /// 踢掉无效链接
        /// </summary>
        private void TickKickout()
        {
            foreach (var obj in m_accept_sessions)
            {
                if (Time.timeSinceStartup - obj.Value >= GlobalID.KICKOUT_INVALID_SESSION_TIME * 1000)//5秒后还没有发握手协议，直接踢掉，放置占线
                {
                    KickoutSession(obj.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// 上报玩家数量
        /// </summary>
        private long tmpLastUploadTime = 0;
        private void TickUploadCount()
        {
            if (Time.timeSinceStartup - tmpLastUploadTime >= GlobalID.UPLOAD_ONLINE_COUNT_TIME * 1000)
            {
                gs2ws.OnlineCount msg = PacketPools.Get(gs2ws.msg.ONLINE_COUNT) as gs2ws.OnlineCount;
                msg.count = (ushort)this.GetClientSessionCount();
                ServerNetManager.Instance.Send2WS(msg);

                tmpLastUploadTime = Time.timeSinceStartup;
            }
        }

        #region session管理
        public bool AddAcceptSession(long conn_idx)
        {
            if (m_accept_sessions.ContainsKey(conn_idx))
                return false;
            m_accept_sessions.Add(conn_idx, Time.timeSinceStartup);
            return true;
        }
        public bool CleanupAcceptSession(long conn_idx)
        {
            return m_accept_sessions.Remove(conn_idx);
        }
        public bool HasAcceptSession(long conn_idx)
        {
            if (!m_accept_sessions.ContainsKey(conn_idx))
                return false;
            return true;
        }
        public int GetAcceptSessionCount()
        {
            return m_accept_sessions.Count;
        }

        public ClientSession AddSession(long conn_idx)
        {
            if (!m_accept_sessions.ContainsKey(conn_idx))
                return null;

            ClientSession session = CommonObjectPools.Spawn<ClientSession>();
            session.Setup(conn_idx);
            m_sessions.Add(conn_idx, session);
            return session;
        }
        public bool CleanupSession(long conn_idx)
        {
            CleanupAcceptSession(conn_idx);

            ClientSession session;
            if (m_sessions.TryGetValue(conn_idx, out session))
            {
                m_account_sessions.Remove(session.account_idx);

                session.Destroy();
                CommonObjectPools.Despawn(session);
            }
            m_sessions.Remove(conn_idx);
            return true;
        }
        public void CloseAllSession()
        {
            foreach (var obj in m_sessions)
            {
                obj.Value.Destroy();
                CommonObjectPools.Despawn(obj.Value);
            }
            m_sessions.Clear();
            m_account_sessions.Clear();
        }

        public ClientSession GetSession(long conn_idx)
        {
            ClientSession session;
            if (m_sessions.TryGetValue(conn_idx, out session))
            {
                return session;
            }
            return null;
        }
        /// <summary>
        /// 有效连接数
        /// </summary>
        public int GetClientSessionCount()
        {
            return m_sessions.Count;
        }
        /// <summary>
        /// 账号对应的session
        /// </summary>
        public void AddSessionByAccount(long account_idx, long conn_idx)
        {
            m_account_sessions.Remove(account_idx);
            m_account_sessions.Add(account_idx, conn_idx);
        }
        public ClientSession GetSessionByAccount(long account_idx)
        {
            long conn_idx;
            if (!m_account_sessions.TryGetValue(account_idx, out conn_idx)) return null;

            ClientSession session;
            if (!m_sessions.TryGetValue(conn_idx, out session)) return null;

            return session;
        }
        /// <summary>
        /// 连接是否已满
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedFull()
        {
            if (GetClientSessionCount() + GetAcceptSessionCount() >= ServerConfig.net_info.max_connected)
                return true;
            return false;
        }
        /// <summary>
        /// 踢号：入口
        /// </summary>
        public void KickoutSession(long conn_idx)
        {
            ClientSession session;
            if (m_sessions.TryGetValue(conn_idx, out session))
            {
                session.Kickout();
            }
            CleanupSession(conn_idx);
            ForClientNetManager.Instance.CloseConn(conn_idx);
        }
        #endregion

        #region 广播消息
        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="packet"></param>
        public void Broadcast(PacketBase packet, eSessionStatus status)
        {
            foreach (var obj in m_sessions)
            {
                ClientSession session = obj.Value;
                if (session.session_status == status)
                {
                    ForClientNetManager.Instance.Send(session.conn_idx, packet);
                }
            }
        }
        /// <summary>
        /// 广播转发的消息
        /// </summary>
        /// <param name="header"></param>
        /// <param name="by"></param>
        /// <param name="status"></param>
        public void BroadcastProxy(ushort header, ByteArray by, eSessionStatus status)
        {
            foreach (var obj in m_sessions)
            {
                ClientSession session = obj.Value;
                if(session.session_status == status)
                {
                    ForClientNetManager.Instance.SendProxy(session.conn_idx, header, by);
                }
            }
        }
        #endregion
    }
}
