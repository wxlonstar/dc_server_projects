using System;
using System.Collections.Generic;
using Fleck;

namespace dc
{
    /// <summary>
    /// web 服务端
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public sealed class WebServerSocket
    {
        private WebSocketServer m_socket = null;

        private long m_share_conn_idx = 0;
        private object m_sync_lock = new object();
        private Dictionary<long, IWebSocketConnection> m_sockets = null;
        private Dictionary<IWebSocketConnection, long> m_user_tokens = null;

        #region 定义委托
        /// <summary>
        /// 连接成功
        /// </summary>
        /// <param name="conn_idx"></param>
        public delegate void OnAcceptConnect(long conn_idx);
        /// <summary>
        /// 接收到客户端的数据
        /// </summary>
        /// <param name="conn_idx"></param>
        /// <param name="buff"></param>
        /// <param name="count"></param>
        public delegate void OnReceiveData(long conn_idx, byte[] buff, int count);
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="conn_idx"></param>
        public delegate void OnConnectClose(long conn_idx);
        #endregion

        #region 定义事件
        public event OnAcceptConnect OnOpen;
        public event OnReceiveData OnMessage;
        public event OnConnectClose OnClose;
        #endregion

        public WebServerSocket()
        {
            m_sockets = new Dictionary<long, IWebSocketConnection>();
            m_user_tokens = new Dictionary<IWebSocketConnection, long>();
        }

        public void Close()
        {
            if (m_socket != null)
            {
                m_socket.Dispose();
                m_socket = null;
            }
            lock(m_sync_lock)
            {
                foreach (var obj in m_sockets)
                {
                    obj.Value.Close();
                    if (OnClose != null) OnClose(obj.Key);
                }
                m_sockets.Clear();
                m_user_tokens.Clear();
                OnOpen = null;
                OnMessage = null;
                OnClose = null;
            }
        }
        /// <summary>
        /// 主动关闭
        /// </summary>
        /// <param name="token"></param>
        public void CloseConn(long conn_idx)
        {
            IWebSocketConnection socket = null;
            lock (m_sync_lock)
            {
                if (m_sockets.TryGetValue(conn_idx, out socket))
                {
                    socket.Close();
                    m_user_tokens.Remove(socket);
                }
                m_sockets.Remove(conn_idx);
                if (OnClose != null) OnClose(conn_idx);
            }
        }

        public bool Start(string host)
        {
            m_socket = new WebSocketServer(host);
            m_socket.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    OnConnectedOpen(socket);
                };
                socket.OnClose = () =>
                {
                    OnConnectedClose(socket);
                };
                socket.OnBinary = by =>
                {
                    OnMessageReveived(socket, by);
                };
                socket.OnError = e =>
                {
                    OnSocketError(socket, e);
                };
            });
            return true;
        }

        public int Send(long conn_idx, byte[] message)
        {
            if (m_socket == null) return 0;
            IWebSocketConnection socket;
            if (m_sockets.TryGetValue(conn_idx, out socket))
            {
                if (socket.IsAvailable)
                {
                    socket.Send(message);
                    return message.Length;
                }
            }
            return 0;
        }

        private void OnConnectedOpen(IWebSocketConnection socket)
        {
            lock (m_sync_lock)
            {
                long conn_idx = ++m_share_conn_idx;
                m_user_tokens.Add(socket, conn_idx);
                m_sockets.Add(conn_idx, socket);

                if (OnOpen != null) OnOpen(conn_idx);
            }
        }
        private void OnMessageReveived(IWebSocketConnection socket, byte[] by)
        {
            lock (m_sync_lock)
            {
                long conn_idx;
                if(m_user_tokens.TryGetValue(socket, out conn_idx))
                {
                    if (OnMessage != null) OnMessage(conn_idx, by, by.Length);
                }
            }
        }
        private void OnConnectedClose(IWebSocketConnection socket)
        {
            socket.Close();
            lock (m_sync_lock)
            {
                long conn_idx;
                if (m_user_tokens.TryGetValue(socket, out conn_idx))
                {
                    m_sockets.Remove(conn_idx);
                    if (OnClose != null) OnClose(conn_idx);
                }
                m_user_tokens.Remove(socket);
            }
        }
        private void OnSocketError(IWebSocketConnection socket, Exception e)
        {
            Log.Exception(e);
            OnConnectedClose(socket);
        }
    }
}
