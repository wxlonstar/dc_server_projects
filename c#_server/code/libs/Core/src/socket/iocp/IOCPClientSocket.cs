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
    public sealed class IOCPClientSocket
    {
        private Socket m_socket = null;           //监听Socket
        private object m_sync_lock = new object();
        private byte[] m_read_buffer = new byte[SocketUtils.SendRecvMaxSize];   //读缓存
        private byte[] m_write_buffer = new byte[SocketUtils.SendRecvMaxSize];   //写缓存

        private SocketAsyncEventArgs m_recv_async_args;
        private List<SocketAsyncEventArgs> m_send_args_pools = null;

        #region 定义委托
        /// <summary>
        /// 连接成功
        /// </summary>
        /// <param name="conn_idx"></param>
        public delegate void OnConnectOpen(long conn_idx);
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
        public event OnConnectOpen OnOpen;
        public event OnReceiveData OnMessage;
        public event OnConnectClose OnClose;
        #endregion

        public IOCPClientSocket()
        {
            m_send_args_pools = new List<SocketAsyncEventArgs>();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="port"></param>
        public bool Connect(string ip, ushort port)
        {
            try
            {
                IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Parse(ip), port);
                m_socket = new Socket(ipEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                m_socket.NoDelay = true;
                m_socket.Blocking = false;
                m_socket.SendBufferSize = SocketUtils.SendRecvMaxSize;
                m_socket.ReceiveBufferSize = SocketUtils.SendRecvMaxSize;
                m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                SocketUtils.SetKeepAlive(m_socket);

                SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
                connectArgs.UserToken = m_socket;
                connectArgs.RemoteEndPoint = ipEndpoint;
                connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnect);
                m_socket.ConnectAsync(connectArgs);

                return true;
            }
            catch (Exception)
            {
                if (m_socket != null)
                {
                    m_socket.Close();
                    m_socket = null;
                }
                return false;
            }
        }
        private void OnConnect(object sender, SocketAsyncEventArgs e)
        {
            System.Threading.Thread.Sleep(100);//让连接线程等等上层主线程
            if(e.SocketError == SocketError.Success)
            {
                lock(m_sync_lock)
                {
                    this.StartReceive(e);
                    if (OnOpen != null)OnOpen(0);
                }
            }
            else
            {
                this.Close();
            }
        }  
 
        /// <summary>
        /// 停止服务
        /// </summary>
        public void Close()
        {
            lock (m_sync_lock)
            {
                if (m_socket != null)
                {
                    try
                    {
                        m_socket.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception)
                    {
                    }
                    m_socket.Close();
                    m_socket = null;
                }
                if (OnClose != null)
                {
                    OnClose(0);
                    OnClose = null;
                }
                OnOpen = null;
                OnMessage = null;
            }
        }
        public Socket socket
        {
            get { return m_socket; }
        }
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～读写数据～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void StartReceive(SocketAsyncEventArgs e)
        {
            m_recv_async_args = new SocketAsyncEventArgs();
            m_recv_async_args.UserToken = null;
            m_recv_async_args.SetBuffer(m_write_buffer, 0, m_write_buffer.Length);
            m_recv_async_args.Completed += new EventHandler<SocketAsyncEventArgs>(SendReceive_Completed);
            //启动接收,不管有没有,一定得启动.否则有数据来了也不知道.
            if (!e.ConnectSocket.ReceiveAsync(m_recv_async_args))
                ProcessReceive(m_recv_async_args);
        }
        /// <summary>
        /// 对数据进行打包,然后再发送
        /// </summary>
        public void Send(byte[] message, int offset, int count)
        {
            if (m_socket == null || !m_socket.Connected || message == null)
                return;
            try
            {
                SocketAsyncEventArgs sendArg = SpawnAsyncArgs();
                sendArg.UserToken = null;
                sendArg.SetBuffer(message, offset, count);
                m_socket.SendAsync(sendArg);
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
            if (m_socket == null) return;
            try
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //读取数据
                    lock (m_sync_lock)
                    {
                        Array.Copy(e.Buffer, e.Offset, m_read_buffer, 0, e.BytesTransferred);
                        if (OnMessage != null)OnMessage(0, m_read_buffer, e.BytesTransferred);
                        
                        if (m_socket.Connected && !m_socket.ReceiveAsync(e))
                            this.ProcessReceive(e);
                    }
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception xe)
            {
                Log.Exception(xe);
                this.Close();
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                DespawnAsyncArgs(e);
            }
            else
            {
                this.Close();
            }
        }

        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～对象池管理～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private object m_poolsLock = new object();
        private static int m_total_new_count = 0;
        private static int m_total_remove_count = 0;
        private SocketAsyncEventArgs SpawnAsyncArgs()
        {
            SocketAsyncEventArgs obj = null;
            if (m_send_args_pools.Count > 0)
            {
                lock (m_poolsLock)
                {
                    obj = m_send_args_pools[m_send_args_pools.Count - 1];
                    m_send_args_pools.RemoveAt(m_send_args_pools.Count - 1);
                }
                System.Threading.Interlocked.Decrement(ref m_total_remove_count);
                return obj;
            }
            else
            {
                System.Threading.Interlocked.Increment(ref m_total_new_count);
                obj = new SocketAsyncEventArgs();
                obj.Completed += new EventHandler<SocketAsyncEventArgs>(SendReceive_Completed);
                obj.UserToken = null;
                obj.SetBuffer(m_read_buffer, 0, m_read_buffer.Length);
                return obj;
            }
        }
        private void DespawnAsyncArgs(SocketAsyncEventArgs obj)
        {
            if (obj == null) return;
            lock (m_poolsLock)
            {
                obj.AcceptSocket = null;
                if (!m_send_args_pools.Contains(obj))
                {
                    m_send_args_pools.Add(obj);
                    System.Threading.Interlocked.Increment(ref m_total_remove_count);
                }
            }
        }
        public static string ToString(bool is_print)
        {
            StringBuilder st = new StringBuilder();
            st.AppendLine("IOCPClientSocket使用情况:");
            st.AppendLine("New次数:" + m_total_new_count);
            st.AppendLine("空闲数量:" + m_total_remove_count);
            if (is_print) Console.WriteLine(st.ToString());
            return st.ToString();
        }
    }
}
