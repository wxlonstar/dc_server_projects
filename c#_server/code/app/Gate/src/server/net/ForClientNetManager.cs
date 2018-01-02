using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 管理客户端连接
    /// @author hannibal
    /// @time 2016-8-18
    /// </summary>
    public class ForClientNetManager : Singleton<ForClientNetManager>
    {
        private ClientMsgProc m_msg_proc = null;
        private IOCPNetAccepter m_net_socket = null;
        private ByteArray m_send_by = null;

        public void Setup()
        {
            m_net_socket = new IOCPNetAccepter();
            m_net_socket.Setup();
            m_msg_proc = new ClientMsgProc();
            m_msg_proc.Setup();
            m_send_by = NetUtils.AllocSendPacket();
            this.RegisterEvent();
        }
        public void Destroy()
        {
            this.UnRegisterEvent();
            if (m_msg_proc != null)
            {
                m_msg_proc.Destroy();
                m_msg_proc = null;
            }
            if(m_net_socket != null)
            {
                m_net_socket.Destroy();
                m_net_socket = null;
            }
        }
        public void Tick()
        {
            if (m_net_socket != null)
                m_net_socket.Update();
        }
        public bool Start(string ip, ushort port)
        {
            bool ret = false;
            string host = "ws://" + ip + ":" + port;
            if (m_net_socket != null)
            {
                ret = m_net_socket.Listen(port, OnConnectOpen, OnMessageReveived, OnConnectClose);
            }
            if (ret)
                Log.Info("for client服务器已经启动 host:" + host);
            else
                Log.Error("for client服务器开启失败 host:" + host);
            return ret;
        }
        /// <summary>
        /// 关闭所有连接
        /// </summary>
        public void Close()
        {
            if (m_net_socket != null)
            {
                m_net_socket.Close();
                m_net_socket = null;
            }
        }
        /// <summary>
        /// 关闭单个连接
        /// </summary>
        /// <param name="conn_idx"></param>
        public void CloseConn(long conn_idx)
        {
            if (m_net_socket != null && conn_idx > 0)
                m_net_socket.CloseConn(conn_idx);
        }

        #region 网络
        private void OnConnectOpen(long conn_idx)
        {
            Log.Debug("for client open session:" + conn_idx);
            if (ClientSessionManager.Instance.IsConnectedFull())
            {
                this.CloseConn(conn_idx);
                return;
            }
            ClientSessionManager.Instance.AddAcceptSession(conn_idx);
        }
        private void OnConnectClose(long conn_idx)
        {
            Log.Debug("for client close session:" + conn_idx);
            ClientSession session = ClientSessionManager.Instance.GetSession(conn_idx);
            if(session != null)
            {
                session.Logout();
            }
            ClientSessionManager.Instance.CleanupSession(conn_idx);
        }
        private void OnMessageReveived(long conn_idx, ushort header, ByteArray data)
        {
            m_msg_proc.OnNetworkClient(conn_idx, header, data);
        }

        private void RegisterEvent()
        {
            EventController.AddEventListener(EventID.NET_CONNECTED_WORLD_SRV, OnGameEvent);
            EventController.AddEventListener(EventID.NET_DISCONNECT_WORLD_SRV, OnGameEvent);
        }
        private void UnRegisterEvent()
        {
            EventController.RemoveEventListener(EventID.NET_CONNECTED_WORLD_SRV, OnGameEvent);
            EventController.RemoveEventListener(EventID.NET_DISCONNECT_WORLD_SRV, OnGameEvent);
        }
        private void OnGameEvent(GameEvent evt)
        {
            switch(evt.type)
            {
                case EventID.NET_CONNECTED_WORLD_SRV:
                    this.Start(ServerConfig.net_info.ip_for_client,ServerConfig.net_info.port_for_client);//"ws://127.0.0.1:7000"
                    break;

                case EventID.NET_DISCONNECT_WORLD_SRV:
                    this.Close();
                    break;
            }
        }
        #endregion

        #region 发消息
        /// <summary>
        /// 发消息给client
        /// </summary>
        /// <param name="conn_idx"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public int Send(long conn_idx, PacketBase packet)
        {
            int size = 0;
            if (m_net_socket != null)
            {
                m_send_by.Clear();
                m_send_by.WriteUShort(0);//先写入长度占位
                packet.Write(m_send_by);
                size = m_net_socket.Send(conn_idx, m_send_by);
            }
            PacketPools.Recover(packet);
            return size;
        }
        /// <summary>
        /// 发送由gate转发给client的消息
        /// </summary>
        /// <param name="conn_idx"></param>
        /// <param name="header"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public int SendProxy(long conn_idx, ushort header, ByteArray by)
        {
            int size = 0;
            if (m_net_socket != null)
            {
                m_send_by.Clear();
                m_send_by.WriteUShort(0);//先写入长度占位
                m_send_by.WriteUShort(header);//协议头
                by.Read(m_send_by, by.Available);//协议内容
                size = m_net_socket.Send(conn_idx, m_send_by);
            }
            return size;
        }
        #endregion
    }
}
