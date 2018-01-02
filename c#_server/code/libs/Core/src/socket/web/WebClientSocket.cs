using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace dc
{
    /// <summary>
    /// web socket客户端
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public class WebClientSocket
    {
        private WebSocketSharp.WebSocket m_socket = null;
        private object m_sync_lock = new object();

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

        public WebClientSocket()
        {
        }
        public void Close()
        {
            lock (m_sync_lock)
            {
                if (m_socket != null)
                {
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

        public void Connect(string host)
        {
            m_socket = new WebSocketSharp.WebSocket(host);
            m_socket.OnOpen += OnConnectedOpened;
            m_socket.OnMessage += OnMessageReveived;
            m_socket.OnClose += OnConnectedClose;
            m_socket.OnError += OnConnectedError;
            m_socket.ConnectAsync();
        }
        public void Send(byte[] message)
        {
            if (m_socket == null || message == null)
                return;
            m_socket.Send(message);
        }
 
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～事件～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void OnConnectedOpened(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(100);//让连接线程等等上层主线程
            lock (m_sync_lock)
            {
                if (OnOpen != null) OnOpen(0);
            }
        }
        private void OnMessageReveived(object sender, EventArgs e)
        {
            WebSocketSharp.MessageEventArgs args = e as WebSocketSharp.MessageEventArgs;
            if (args == null || args.RawData == null) return;

            lock (m_sync_lock)
            {
                if (OnMessage != null) OnMessage(0, args.RawData, args.RawData.Length);
            }
        }
        private void OnConnectedClose(object sender, EventArgs e)
        {
            this.Close();
        }
        private void OnConnectedError(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
