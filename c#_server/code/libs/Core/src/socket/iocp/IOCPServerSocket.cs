using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace dc
{
    /// <summary>
    /// 服务端iocp
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public sealed class IOCPServerSocket
    {
        private long m_share_conn_idx = 0;
        private Socket m_socket = null;           //监听Socket
        private object m_sync_lock = new object();
        private byte[] m_read_buffer = new byte[SocketUtils.SendRecvMaxSize];   //读缓存

        private Dictionary<long, UserToken> m_user_tokens = null;
        private List<SocketAsyncEventArgs> m_recv_args_pools = null;
        private List<SocketAsyncEventArgs> m_send_args_pools = null;

        #region 定义委托
        /// <summary>
        /// 接收连接
        /// </summary>
        /// <param name="token"></param>
        public delegate void OnAcceptConnect(long conn_idx);
        /// <summary>
        /// 接收到客户端的数据
        /// </summary>
        /// <param name="token"></param>
        /// <param name="buff"></param>
        /// <param name="count"></param>
        public delegate void OnReceiveData(long conn_idx, byte[] buff, int count);
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="token"></param>
        public delegate void OnConnectClose(long conn_idx);
        #endregion

        #region 定义事件
        /// <summary>
        /// 接收到客户端的数据事件
        /// </summary>
        public event OnAcceptConnect OnOpen;
        public event OnReceiveData OnMessage;
        public event OnConnectClose OnClose;
        #endregion

        public IOCPServerSocket()
        {
            m_user_tokens = new Dictionary<long, UserToken>();
            m_recv_args_pools = new List<SocketAsyncEventArgs>();
            m_send_args_pools = new List<SocketAsyncEventArgs>();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="port"></param>
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

                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
                m_socket.Bind(localEndPoint);
                m_socket.Listen(100);

                StartAccept(null);
                return true;
            }
            catch (Exception e)
            {
                Log.Exception(e);
                this.Close();
                return false;
            }
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
                    if (OnClose != null) OnClose(obj.Key);
                }
                m_user_tokens.Clear();

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
        }

        /// <summary>
        /// 主动关闭
        /// </summary>
        /// <param name="token"></param>
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
                }
                m_user_tokens.Remove(token.ConnId);
                if (OnClose != null) OnClose(conn_idx);
            }
        }
        /// <summary>
        /// 关闭客户端:内部出现错误时调用
        /// </summary>
        /// <param name="e"></param>
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            UserToken token = e.UserToken as UserToken;
            if (token != null)
            {
                lock (m_sync_lock)
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
                    m_user_tokens.Remove(token.ConnId);
                    DespawnAsyncArgs(m_recv_args_pools, e);
                    if (OnClose != null) OnClose(token.ConnId);
                }
            }
        }
        public Socket socket
        {
            get { return m_socket; }
        }

        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～接收连接～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        /// <summary>
        /// 开始接收连接
        /// </summary>
        /// <param name="acceptEventArg"></param>
        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (m_socket == null) return;
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
            }
            acceptEventArg.AcceptSocket = null;
            if (!m_socket.AcceptAsync(acceptEventArg))
            {
                ProcessAccept(acceptEventArg);
            }
        }
        /// <summary>
        /// 接收到一个连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (m_socket == null) return;

            if (e.SocketError != SocketError.Success)
            {
                if (e.SocketError != SocketError.OperationAborted) Log.Error("ProcessAccept Error:" + e.SocketError);
                return;
            }

            lock(m_sync_lock)
            {
                SocketAsyncEventArgs recvEventArgs = SpawnAsyncArgs(m_recv_args_pools);
                long conn_idx = ++m_share_conn_idx;
                UserToken userToken = (UserToken)recvEventArgs.UserToken;
                userToken.ConnId = conn_idx;
                userToken.Socket = e.AcceptSocket;
                userToken.Remote = e.AcceptSocket.RemoteEndPoint;
                m_user_tokens.Add(conn_idx, userToken);
                if (OnOpen != null) OnOpen(conn_idx);
                
                //连接成功后，有可能被踢出，需要再次判断是否有效
                if (m_user_tokens.ContainsKey(conn_idx))
                {
                    try
                    {
                        if (!e.AcceptSocket.ReceiveAsync(recvEventArgs))
                        {
                            ProcessReceive(recvEventArgs);
                        }
                    }
                    catch (Exception ex)
                    {
                        DespawnAsyncArgs(m_recv_args_pools, recvEventArgs);
                        Log.Exception(ex);
                    }
                }
            }
            StartAccept(e);
        }

        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～读写数据～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        /// <summary>
        /// 对数据进行打包,然后再发送
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public void Send(long conn_idx, byte[] message, int offset, int count)
        {
            UserToken token;
            if (!m_user_tokens.TryGetValue(conn_idx, out token) || token.Socket == null || !token.Socket.Connected || message == null)
                return;

            SocketAsyncEventArgs sendArg = SpawnAsyncArgs(m_send_args_pools);
            sendArg.UserToken = token;
            try
            {
                sendArg.SetBuffer(message, offset, count);
                token.Socket.SendAsync(sendArg);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        void SendReceive_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    Log.Error("The last operation completed on the socket was not a receive or send");
                    break;
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                UserToken token = (UserToken)e.UserToken;
                if (!m_user_tokens.ContainsKey(token.ConnId)) return;
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //读取数据
                    lock (m_sync_lock)
                    {
                        Array.Copy(e.Buffer, e.Offset, m_read_buffer, 0, e.BytesTransferred);
                        if (OnMessage != null)OnMessage(token.ConnId, m_read_buffer, e.BytesTransferred);

                        //派发消息的时候，有可能上层逻辑关闭了当前连接，必须再判断一次当前连接是否正常
                        if (m_user_tokens.ContainsKey(token.ConnId))
                        {
                            if (token.Socket != null && token.Socket.Connected && !token.Socket.ReceiveAsync(e))
                                this.ProcessReceive(e);
                        }
                        else
                        {
                            DespawnAsyncArgs(m_recv_args_pools, e);
                        }
                    }
                }
                else
                {
                    CloseClientSocket(e);
                }
            }
            catch (Exception xe)
            {
                Log.Exception(xe);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                DespawnAsyncArgs(m_send_args_pools, e);
            }
            else
            {
                CloseClientSocket(e);
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
            if(m_user_tokens.TryGetValue(conn_idx, out token))
            {
                return token.Remote;
            }
            return null;
        }

        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～对象池管理～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private object m_pools_lock = new object();
        private static int m_total_new_count = 0;
        private static int m_total_remove_count = 0;
        private SocketAsyncEventArgs SpawnAsyncArgs(List<SocketAsyncEventArgs> list)
        {
            SocketAsyncEventArgs obj = null;
            if (list.Count > 0)
            {
                lock (m_pools_lock)
                {
                    obj = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                }
                System.Threading.Interlocked.Decrement(ref m_total_remove_count);
                return obj;
            }
            else
            {
                System.Threading.Interlocked.Increment(ref m_total_new_count);
                UserToken token = new UserToken();
                obj = new SocketAsyncEventArgs();
                obj.Completed += new EventHandler<SocketAsyncEventArgs>(SendReceive_Completed);
                obj.UserToken = token;
                obj.SetBuffer(token.Buffer, 0, token.Buffer.Length);
                return obj;
            }
        }
        private void DespawnAsyncArgs(List<SocketAsyncEventArgs> list, SocketAsyncEventArgs obj)
        {
            if (obj == null) return;
            lock (m_pools_lock)
            {
                obj.AcceptSocket = null;
                if (!list.Contains(obj))
                {
                    list.Add(obj);
                    System.Threading.Interlocked.Increment(ref m_total_remove_count);
                }
            }
        }
        public static string ToString(bool is_print)
        {
            StringBuilder st = new StringBuilder();
            st.AppendLine("IOCPServerSocket使用情况:");
            st.AppendLine("New次数:" + m_total_new_count);
            st.AppendLine("空闲数量:" + m_total_remove_count);
            if (is_print) Console.WriteLine(st.ToString());
            return st.ToString();
        }
    }
}
