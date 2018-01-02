using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace dc
{
    /// <summary>
    /// 客户端tcp
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public sealed class TCPClientSocket
    {
        private Socket m_socket = null;
        private object m_sync_lock = new object();
        private byte[] m_recv_buffer = new byte[SocketUtils.SendRecvMaxSize];   //读缓存
        private byte[] m_send_buffer = new byte[SocketUtils.SendRecvMaxSize];   //写缓存
        private SendRecvBufferPools m_buffer_pools = null;

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

        public TCPClientSocket()
        {
            m_buffer_pools = new SendRecvBufferPools();
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
                    catch (Exception) { }
                    m_socket.Close();
                    m_socket = null;
                }
                if (OnClose != null)
                {
                    OnClose.Invoke(0);
                    OnClose = null;
                }
                OnOpen = null;
                OnMessage = null;
            }
        }
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

                m_socket.BeginConnect(ipEndpoint, new AsyncCallback(OnConnected), m_socket);
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
        private void OnConnected(IAsyncResult ar)
        {
            if (m_socket == null) return;

            Socket client = (Socket)ar.AsyncState;
            ar.AsyncWaitHandle.Close();
            try
            {
                lock (m_sync_lock)
                {
                    client.EndConnect(ar);
                    this.BeginReceive();
                    if (OnOpen != null) OnOpen.Invoke(0);
                }
            }
            catch (Exception e)
            {
                Log.Error("连接失败:" + e.Message);
                this.Close();
            }
        }
        private void BeginReceive()
        {
            if (m_socket == null) return;
            m_socket.BeginReceive(m_recv_buffer, 0, m_recv_buffer.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), m_recv_buffer);
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        private void OnReceive(IAsyncResult ar)
        {
            if (m_socket == null) return;
            try
            {
                ar.AsyncWaitHandle.Close();
                lock (m_sync_lock)
                {
                    if (m_socket == null) return;
                    byte[] buf = (byte[])ar.AsyncState;
                    int len = m_socket.EndReceive(ar);

                    if (len > 0)
                    {
                        if (OnMessage != null) OnMessage.Invoke(0, buf, len);
                        this.BeginReceive();
                    }
                    else
                    {
                        this.Close();
                        return;
                    }
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10054) Log.Exception(e);
                this.Close();
            }
            catch (Exception e)
            {
                Log.Exception(e);
                this.Close();
                return;
            }
        }
        public void Send(byte[] message, int offset, int count)
        {
            if (m_socket == null || !m_socket.Connected || message == null)
                return;
            SendRecvBuffer buffer = m_buffer_pools.Spawn();
            buffer.Socket = m_socket;
            System.Array.Copy(message, offset, buffer.Buffer, 0, count);
            try
            {
                buffer.Socket.BeginSend(buffer.Buffer, 0, count, 0, new AsyncCallback(OnSend), buffer);
            }
            catch (Exception e)
            {
                Log.Error("发送失败:" + e.Message);
                this.Close();
            }
        }
        private void OnSend(IAsyncResult ar)
        {
            ar.AsyncWaitHandle.Close();
            SendRecvBuffer buffer = (SendRecvBuffer)ar.AsyncState;
            //已经断开连接
            if (buffer.Socket == null || !buffer.Socket.Connected)
            {
                this.Close();
                return;
            }

            try
            {
                buffer.Socket.EndSend(ar);
                m_buffer_pools.Despawn(buffer);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                this.Close();
            }
        }
        public Socket socket
        {
            get { return m_socket; }
        }
    }
}
