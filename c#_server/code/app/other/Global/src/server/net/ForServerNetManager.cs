using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 管理其他服连接
    /// @author hannibal
    /// @time 2016-8-18
    /// </summary>
    public class ForServerNetManager : Singleton<ForServerNetManager>
    {
        private IOCPNetAccepter m_net_socket = null;
        private ushort m_srv_uid = 0;
        private ByteArray m_send_by = null;
        private Dictionary<long, ConnAppProc> m_app_servers = null;
        private Dictionary<ushort, ConnAppProc> m_srv_servers = null;//srv->server

        public ForServerNetManager()
        {
            m_app_servers = new Dictionary<long, ConnAppProc>();
            m_srv_servers = new Dictionary<ushort, ConnAppProc>();
            m_send_by = NetUtils.AllocSendPacket();
        }

        public void Setup()
        {
            m_srv_uid = ServerConfig.net_info.server_uid;
            m_net_socket = new IOCPNetAccepter();
            m_net_socket.Setup();
            ProtocolID.RegisterPools();
        }
        public void Destroy()
        {
            foreach(var obj in m_app_servers)
            {
                obj.Value.Destroy();
            }
            m_app_servers.Clear();
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
        public bool Start(ushort port)
        {
            bool ret = false;
            if (m_net_socket != null)
            {
                ret = m_net_socket.Listen(port, OnAcceptConnect, OnMessageReveived, OnConnectClose);
            }
            if (ret)
                Log.Info("for server服务器已经启动 port:" + port);
            else
                Log.Error("for server服务器开启失败 port:" + port);
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
        public ushort srv_uid { get { return m_srv_uid; } }

        #region 网络
        private void OnAcceptConnect(long conn_idx)
        {
            Log.Info("for server open session:" + conn_idx);
        }
        private void OnConnectClose(long conn_idx)
        {
            Log.Info("for server close session:" + conn_idx);
            
            ConnAppProc app_server = null;
            if(m_app_servers.TryGetValue(conn_idx, out app_server))
            {
                OnConnAppLeave(app_server);
            }
            m_app_servers.Remove(conn_idx);
        }
        private void OnMessageReveived(long conn_idx, ushort header, ByteArray data)
        {
            PacketBase packet = PacketPools.Get(header);
            packet.Read(data);
            ConnAppProc app_server = null;
            if(!m_app_servers.TryGetValue(conn_idx, out app_server))
            {
                if (packet.header == inner.msg.REQ_LOGIN)
                {
                    if(!m_app_servers.ContainsKey(conn_idx))
                    {
                        //创建新连接
                        inner.ReqLogin msg = packet as inner.ReqLogin;
                        Log.Info("收到内部服务器连接请求:" + msg.srv_info.srv_type);

                        app_server = CreateConnApp(msg.srv_info.srv_type);
                        app_server.conn_idx = conn_idx;
                        app_server.srv_info.srv_type = msg.srv_info.srv_type;
                        app_server.srv_info.srv_status = eConnAppStatus.CONNECTED;
                        app_server.srv_info.srv_realm_idx = msg.srv_info.srv_realm_idx;
                        app_server.srv_info.srv_uid = msg.srv_info.srv_uid;
                        app_server.srv_info.srv_endpoint.ip = msg.srv_info.srv_endpoint.ip;
                        app_server.srv_info.srv_endpoint.port = msg.srv_info.srv_endpoint.port;
                        m_app_servers.Add(conn_idx, app_server);
                        OnConnAppEnter(app_server);

                        //响应
                        inner.RepLogin rep_msg = PacketPools.Get(inner.msg.REP_LOGIN) as inner.RepLogin;
                        rep_msg.result = inner.RepLogin.eResult.SUCCESS;
                        rep_msg.srv_info.srv_type = eServerType.GLOBAL;
                        rep_msg.srv_info.srv_realm_idx = app_server.srv_info.srv_realm_idx;
                        rep_msg.srv_info.srv_uid = m_srv_uid;
                        this.Send(conn_idx, rep_msg);
                    }
                }
            }
            if(app_server != null)
            {
                if (!app_server.HandleMsg(conn_idx, packet))
                {
                }
            }
            PacketPools.Recover(packet);
        }
        #endregion

        #region 连接列表
        private ConnAppProc CreateConnApp(eServerType server_type)
        {
            ConnAppProc app_server = null;
            switch(server_type)
            {
                case eServerType.SERVER:
                    app_server = new ServerMsgProc();
                    break;
                default:
                    Log.Warning("错误的服务器类型:" + server_type);
                    return null;
            }
            app_server.Setup();
            return app_server;
        }
        private void OnConnAppEnter(ConnAppProc app_server)
        {
            if (m_srv_servers.ContainsKey(app_server.srv_info.srv_uid))
            {
                Log.Warning("已经存在服务器 sid:" + app_server.srv_info.srv_uid);
                return;
            }
            string ip = m_net_socket.GetConnIP(app_server.conn_idx);
            Log.Info("服务器加入 type:" + app_server.srv_info.srv_type.ToString() + " wid:" + app_server.srv_info.srv_realm_idx + " sid:" + app_server.srv_info.srv_uid + " ip:" + ip);
            m_srv_servers.Add(app_server.srv_info.srv_uid, app_server);
        }
        private void OnConnAppLeave(ConnAppProc app_server)
        {
            Log.Info("服务器退出 type:" + app_server.srv_info.srv_type.ToString() + " wid:" + app_server.srv_info.srv_realm_idx + " sid:" + app_server.srv_info.srv_uid);
            app_server.srv_info.srv_status = eConnAppStatus.CLOSED;
            m_srv_servers.Remove(app_server.srv_info.srv_uid);
        }
        public void GetConnAppList(List<ConnAppProc> list, eServerType type)
        {
            foreach (var obj in m_app_servers)
            {
                if (obj.Value.srv_info.srv_type == type) list.Add(obj.Value);
            }
        }
        #endregion

        #region 发消息
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
        public int Send(ushort srv_uid, PacketBase packet)
        {
            int size = 0;
            ConnAppProc app_server;
            if (m_srv_servers.TryGetValue(srv_uid, out app_server))
            {
                size = app_server.Send(packet);
            }
            else
            {
                PacketPools.Recover(packet);
            }
            
            return size;
        }
        /// <summary>
        /// 广播消息
        /// </summary>
        public void BroadcastMsg(PacketBase packet)
        {
            foreach (var obj in m_app_servers)
            {
                obj.Value.Send(packet);
            }
        }
        /// <summary>
        /// 广播消息
        /// </summary>
        public void BroadcastMsg(PacketBase packet, eServerType type)
        {
            foreach (var obj in m_app_servers)
            {
                if (obj.Value.srv_info.srv_type == type) obj.Value.Send(packet);
            }
        }
        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="conn_idx">排除服务器</param>
        public void BroadcastMsgWithout(PacketBase packet, long conn_idx)
        {
            foreach (var obj in m_app_servers)
            {
                if (obj.Value.conn_idx != conn_idx) obj.Value.Send(packet);
            }
        }
        #endregion
    }
}
