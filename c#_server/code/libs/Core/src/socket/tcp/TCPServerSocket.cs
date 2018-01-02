using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace dc
{
    /// <summary>
    /// 服务端tcp
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public sealed class TCPServerSocket
    {
        private long m_share_conn_idx = 0;
        private Socket m_socket = null;
        private object m_sync_lock = new object();
        private byte[] m_recv_buffer = new byte[SocketUtils.SendRecvMaxSize];   //读缓存
        private byte[] m_send_buffer = new byte[SocketUtils.SendRecvMaxSize];   //写缓存
        private SendRecvBufferPools m_buffer_pools = null;
        private UserTokenPools m_user_tokens_pools = null;
        private Dictionary<long, UserToken> m_user_tokens = null;

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

        public TCPServerSocket()
        {
            m_buffer_pools = new SendRecvBufferPools();
            m_user_tokens_pools = new UserTokenPools();
            m_user_tokens = new Dictionary<long, UserToken>();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Close()
        {
            lock (m_sync_lock)
            {
                Socket _socket = null;
                foreach (var obj in m_user_tokens)
                {
                    _socket = obj.Value.Socket;
                    if (_socket != null)
                    {
                        try
                        {
                            _socket.Shutdown(SocketShutdown.Both);
                        }
                        catch (Exception) { }
                        _socket.Close();
                    }
                    m_user_tokens_pools.Despawn(obj.Value);
                    if (OnClose != null) OnClose(obj.Key);
                }
                m_user_tokens.Clear();
            }
            if (m_socket != null)
            {
                try
                {
                    m_socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception) { }
                m_socket.Close();
                m_socket = null;
            }
            OnOpen = null;
            OnMessage = null;
            OnClose = null;
        }
        /// <summary>
        /// 主动关闭
        /// </summary>
        public void CloseConn(long conn_idx)
        {
            UserToken token = null;
            lock (m_sync_lock)
            {
                if (m_user_tokens.TryGetValue(conn_idx, out token))
                {
                    if (token.Socket != null)
                    {
                        try
                        {
                            token.Socket.Shutdown(SocketShutdown.Both);
                        }
                        catch (Exception) { }
                        token.Socket.Close();
                        token.Socket = null;
                    }
                    m_user_tokens_pools.Despawn(token);
                }
                m_user_tokens.Remove(conn_idx);
                if (OnClose != null) OnClose(conn_idx);
            }
        }
        /// <summary>
        /// 关闭客户端:内部出现错误时调用
        /// </summary>
        private void CloseClientSocket(long conn_idx)
        {            
            UserToken token = null;
            lock (m_sync_lock)
            {
                if (m_user_tokens.TryGetValue(conn_idx, out token))
                {
                    if (token.Socket != null)
                    {
                        try
                        {
                            token.Socket.Shutdown(SocketShutdown.Send);
                        }
                        catch (Exception) { }
                        token.Socket.Close();
                        token.Socket = null;
                    }
                    m_user_tokens_pools.Despawn(token);
                    m_user_tokens.Remove(token.ConnId);
                    if (OnClose != null) OnClose(token.ConnId);
                }
            }
        }
        public bool Start(ushort port)
        {
            try
            {
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_socket.NoDelay = true;
                m_socket.Blocking = false;
                m_socket.SendBufferSize = SocketUtils.SendRecvMaxSize;
                m_socket.ReceiveBufferSize = SocketUtils.SendRecvMaxSize;
                m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                SocketUtils.SetKeepAlive(m_socket);

                m_socket.Bind(new IPEndPoint(IPAddress.Any, port));  //绑定IP地址：端口  
                m_socket.Listen(100);

                m_socket.BeginAccept(new AsyncCallback(OnAccept), m_socket);
                return true;
            }
            catch (Exception e)
            {
                Log.Exception(e);
                this.Close();
                return false;
            }
        }
        private void OnAccept(IAsyncResult ar)
        {
            Socket server_socket = (Socket)ar.AsyncState;
            ar.AsyncWaitHandle.Close();
            try
            {
                //初始化一个SOCKET，用于其它客户端的连接
                Socket client_socket = server_socket.EndAccept(ar);
                lock (m_sync_lock)
                {
                    long conn_idx = ++m_share_conn_idx;
                    UserToken token = m_user_tokens_pools.Spawn();
                    token.ConnId = conn_idx;
                    token.Remote = client_socket.RemoteEndPoint;
                    token.Socket = client_socket;
                    m_user_tokens.Add(conn_idx, token);

                    if (OnOpen != null) OnOpen(conn_idx);

                    //连接成功，有可能被踢出，需要再次判断是否有效
                    if (m_user_tokens.ContainsKey(conn_idx))
                    {
                        SendRecvBuffer buffer = m_buffer_pools.Spawn();
                        buffer.ConnId = token.ConnId;
                        buffer.Socket = token.Socket;
                        BeginReceive(buffer);
                    }
                    //等待新的客户端连接
                    server_socket.BeginAccept(new AsyncCallback(OnAccept), server_socket);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
                this.Close();
                return;
            }
        }
        private void BeginReceive(SendRecvBuffer buffer)
        {
            if (m_socket == null) return;
            buffer.Socket.BeginReceive(buffer.Buffer, 0, buffer.Buffer.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), buffer);
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        private void OnReceive(IAsyncResult ar)
        {
            SendRecvBuffer buffer = (SendRecvBuffer)ar.AsyncState;
            ar.AsyncWaitHandle.Close();
            try
            {
                if (buffer.Socket == null) return;
                lock (m_sync_lock)
                {
                    int len = buffer.Socket.EndReceive(ar);
                    if (!m_user_tokens.ContainsKey(buffer.ConnId)) return;
                    if (len > 0)
                    {

                        if (OnMessage != null) OnMessage(buffer.ConnId, buffer.Buffer, len);

                        //派发消息的时候，有可能上层逻辑关闭了当前连接，必须再判断一次当前连接是否正常
                        if (m_user_tokens.ContainsKey(buffer.ConnId))
                        {
                            this.BeginReceive(buffer);
                        }
                        else
                        {
                            m_buffer_pools.Despawn(buffer);
                        }
                    }
                    else
                    {
                        Log.Error("OnReceive Recv Error");
                        m_buffer_pools.Despawn(buffer);
                        this.CloseClientSocket(buffer.ConnId);
                    }
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10054) Log.Exception(e);
                m_buffer_pools.Despawn(buffer);
                this.CloseClientSocket(buffer.ConnId);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                m_buffer_pools.Despawn(buffer);
                this.CloseClientSocket(buffer.ConnId);
                return;
            }
        }
        public void Send(long conn_idx, byte[] message, int offset, int count)
        {
            UserToken token;
            if (!m_user_tokens.TryGetValue(conn_idx, out token) || token.Socket == null || !token.Socket.Connected || message == null)
                return;

            SendRecvBuffer buffer = m_buffer_pools.Spawn();
            buffer.ConnId = conn_idx;
            buffer.Socket = token.Socket;
            System.Array.Copy(message, offset, buffer.Buffer, 0, count);
            try
            {
                buffer.Socket.BeginSend(buffer.Buffer, 0, count, 0, new AsyncCallback(OnSend), buffer);
            }
            catch (Exception e)
            {
                Log.Error("发送失败:" + e.Message);
                this.CloseClientSocket(conn_idx);
                m_buffer_pools.Despawn(buffer);
            }
        }
        private void OnSend(IAsyncResult ar)
        {
            ar.AsyncWaitHandle.Close();
            SendRecvBuffer buffer = (SendRecvBuffer)ar.AsyncState;

            //已经断开连接
            if (buffer.Socket == null || !buffer.Socket.Connected)
            {
                m_buffer_pools.Despawn(buffer);
                this.CloseClientSocket(buffer.ConnId);
                return;
            }

            try
            {
                buffer.Socket.EndSend(ar);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                this.CloseClientSocket(buffer.ConnId);
            }
            finally
            {
                m_buffer_pools.Despawn(buffer);
            }
        }        
        /// <summary>
        /// 获取连接信息
        /// </summary>
        /// <param name="conn_idx"></param>
        /// <returns></returns>
        public EndPoint GetRemote(long conn_idx)
        {
            UserToken token;
            if (m_user_tokens.TryGetValue(conn_idx, out token))
            {
                return token.Remote;
            }
            return null;
        }

        public Socket socket
        {
            get { return m_socket; }
        }
    }
}
