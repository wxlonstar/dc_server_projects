using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 服务器网络管理
    /// @author hannibal
    /// @time 2016-7-27
    /// </summary>
    public class ServerNetManager : Singleton<ServerNetManager>
    {
        private ByteArray m_send_by = null;
        private ushort m_srv_uid = 0;
        private ushort m_srv_realm_idx = 0;
        private long m_world_conn_idx = 0;
        private long m_global_conn_idx = 0;
        private Dictionary<long, ConnAppProc> m_app_servers = null;//conn_idx->server
        private Dictionary<ushort, ConnAppProc> m_srv_servers = null;//srv->server

        public ServerNetManager()
        {
            m_app_servers = new Dictionary<long, ConnAppProc>();
            m_srv_servers = new Dictionary<ushort, ConnAppProc>();
            m_send_by = NetUtils.AllocSendPacket();
        }

        public void Setup()
        {
            m_world_conn_idx = 0;
            m_global_conn_idx = 0;
            m_srv_uid = ServerConfig.net_info.server_uid;
            ProtocolID.RegisterPools();
        }
        public void Destroy()
        {
            foreach (var obj in m_app_servers)
            {
                obj.Value.Destroy();
            }
            m_app_servers.Clear();
        }
        public void Tick()
        {
            if (m_world_conn_idx == 0)
            {
                Reconnect2WorldServer();
            }
            if (m_global_conn_idx == 0)
            {
                Reconnect2GlobalServer();
            }
        }
        public void Connect2WorldServer(string ip, ushort port)
        {
            if (m_world_conn_idx != 0)
            {
                Log.Warning("世界服已经连接");
                return;
            }

            Log.Info("开始连接世界服 ip:" + ip + " port:" + port);
            long conn_idx = NetConnectManager.Instance.ConnectTo(ip, port, OnConnected, OnMessageReveived, OnConnectClose);
            ConnAppProc app_server = CreateConnApp(eServerType.WORLD);
            app_server.conn_idx = conn_idx;
            app_server.srv_info.srv_type = eServerType.WORLD;
            app_server.srv_info.srv_endpoint.Set(SocketUtils.IpToInt(ip), port);
            app_server.srv_info.srv_status = eConnAppStatus.CONNECTING;
            m_app_servers.Add(conn_idx, app_server);
            m_world_conn_idx = conn_idx;
        }
        public void Connect2GlobalServer(string ip, ushort port)
        {
            //只有连接上了世界服，才连接全局服
            if (!IsConnectedWorldServer()) return;

            if (m_global_conn_idx != 0)
            {
                Log.Warning("全局服已经连接");
                return;
            }

            Log.Info("开始连接全局服 ip:" + ip + " port:" + port);
            long conn_idx = NetConnectManager.Instance.ConnectTo(ip, port, OnConnected, OnMessageReveived, OnConnectClose);
            ConnAppProc app_server = CreateConnApp(eServerType.GLOBAL);
            app_server.conn_idx = conn_idx;
            app_server.srv_info.srv_type = eServerType.GLOBAL;
            app_server.srv_info.srv_endpoint.Set(SocketUtils.IpToInt(ip), port);
            app_server.srv_info.srv_status = eConnAppStatus.CONNECTING;
            m_app_servers.Add(conn_idx, app_server);
            m_global_conn_idx = conn_idx;
        }
        private float tmpReconnectWSTime = 0;
        public void Reconnect2WorldServer()
        {
            if (tmpReconnectWSTime < Time.timeSinceStartup)
            {
                Connect2WorldServer(ServerConfig.net_info.ws_ip, ServerConfig.net_info.ws_port);
                tmpReconnectWSTime = Time.timeSinceStartup + 5000;
            }
        }
        private float tmpReconnectGLTime = 0;
        public void Reconnect2GlobalServer()
        {
            if (tmpReconnectGLTime < Time.timeSinceStartup)
            {
                Connect2GlobalServer(ServerConfig.net_info.gl_ip, ServerConfig.net_info.gl_port);
                tmpReconnectGLTime = Time.timeSinceStartup + 10000;
            }
        }
        public void Connect2Server(string ip, ushort port, eServerType type, ushort srv_uid)
        {
            Log.Info("连接服务器 ip:" + ip + " port:" + port +" type:" + type + " uid:" + srv_uid);
            long conn_idx = NetConnectManager.Instance.ConnectTo(ip, port, OnConnected, OnMessageReveived, OnConnectClose);

            ConnAppProc app_server = CreateConnApp(type);
            app_server.conn_idx = conn_idx;
            app_server.srv_info.srv_type = type;
            app_server.srv_info.srv_uid = srv_uid;
            app_server.srv_info.srv_realm_idx = m_srv_realm_idx;
            app_server.srv_info.srv_endpoint.Set(SocketUtils.IpToInt(ip), port);
            app_server.srv_info.srv_status = eConnAppStatus.CONNECTING;
            m_app_servers.Add(conn_idx, app_server);
        }

        /// <summary>
        /// ss统一踢号入口
        /// </summary>
        public void KickAccount(long account_idx)
        {
            Player player = UnitManager.Instance.GetPlayerByAccount(account_idx);
            if (player == null) return;

            ss2gs.ReqKickoutAccount msg = PacketPools.Get(ss2gs.msg.REQ_KICK_ACCOUNT) as ss2gs.ReqKickoutAccount;
            msg.account_idx = account_idx;
            ServerNetManager.Instance.Send(player.client_uid.srv_uid, msg);
        }

        public ushort srv_uid { get { return m_srv_uid; } }
        public ushort srv_realm_idx { get { return m_srv_realm_idx; } }
        public long world_conn_idx { get { return m_world_conn_idx; } }
        public long global_conn_idx { get { return m_global_conn_idx; } }

        #region 网络
        private void OnConnected(long conn_idx)
        {
            Log.Info("Open session:" + conn_idx);

            ConnAppProc app_server;
            if(m_app_servers.TryGetValue(conn_idx, out app_server))
            {
                Log.Info("发送内部服务器连接请求:" + app_server.srv_info.srv_type);
                inner.ReqLogin msg = PacketPools.Get(inner.msg.REQ_LOGIN) as inner.ReqLogin;
                msg.srv_info.srv_type = eServerType.SERVER;
                msg.srv_info.srv_realm_idx = m_srv_realm_idx;
                msg.srv_info.srv_uid = m_srv_uid;
                msg.srv_info.srv_endpoint.ip = 0;//逻辑服只连接其他服，不需要设置
                msg.srv_info.srv_endpoint.port = 0;
                this.Send(conn_idx, msg);
            }
            else
            {
                Log.Warning("未找到连接:" + conn_idx);
            }
        }
        private void OnConnectClose(long conn_idx)
        {
            Log.Info("Close session:" + conn_idx);
            ConnAppProc app_server;
            if (m_app_servers.TryGetValue(conn_idx, out app_server))
            {
                OnConnAppLeave(app_server);
            }
            m_app_servers.Remove(conn_idx);
        }
        private void OnMessageReveived(long conn_idx, ushort header, ByteArray data)
        {
            PacketBase packet = PacketPools.Get(header);
            packet.Read(data);

            ConnAppProc app_server;
            if (m_app_servers.TryGetValue(conn_idx, out app_server))
            {
                if (app_server.srv_info.srv_status == eConnAppStatus.CONNECTED)
                {
                    if (!app_server.HandleMsg(conn_idx, packet))
                    {
                        if (app_server.srv_info.srv_type == eServerType.WORLD)
                        {
                            switch(packet.header)
                            {
                                case inner.msg.APPSERVER_LIST:
                                    (app_server as WorldMsgProc).HandleAppServerList(packet);
                                    break;
                                case inner.msg.APPSERVER_ADD:
                                    (app_server as WorldMsgProc).HandleAppServerAdd(packet);
                                    break;
                                case inner.msg.APPSERVER_REMOVE:
                                    (app_server as WorldMsgProc).HandleAppServerRemove(packet);
                                    break;
                                case inner.msg.APPSERVER_SHUTDOWN:
                                    Master.Instance.Stop();
                                    break;
                            }
                        }
                    }
                }
                else if (app_server.srv_info.srv_status == eConnAppStatus.CONNECTING)
                {
                    if (packet.header == inner.msg.REP_LOGIN)
                    {
                        inner.RepLogin msg = packet as inner.RepLogin;
                        if (msg.result == inner.RepLogin.eResult.SUCCESS)
                        {
                            app_server.srv_info.srv_status = eConnAppStatus.CONNECTED;
                            if(msg.srv_info.srv_type == eServerType.WORLD)
                            {
                                m_srv_realm_idx = msg.srv_info.srv_realm_idx;//由世界服分配的id
                                app_server.srv_info.srv_uid = msg.srv_info.srv_uid;
                                app_server.srv_info.srv_realm_idx = msg.srv_info.srv_realm_idx;
                                GameTimeManager.Instance.SetAdjustTime(msg.ws_time - Time.time);//修正服务器时间
                                Log.Info("当前时间:" + Time.time + " 当前ws时间:" + msg.ws_time);
                                Log.Info("当前服所属区服id:" + m_srv_realm_idx + ", sid:" + m_srv_uid);
                            }
                            else if (msg.srv_info.srv_type == eServerType.GLOBAL)
                            {
                                app_server.srv_info.srv_uid = msg.srv_info.srv_uid;
                            }
                            OnConnAppEnter(app_server);
                        }
                        else
                        {
                            Log.Warning("连接服务器出错 type:" + msg.srv_info.srv_type + " result:" + msg.result);
                        }
                    }
                    else
                    {
                        Log.Warning("收到无效协议:" + packet.header);
                    }
                }
            }
            PacketPools.Recover(packet);
        }
        #endregion

        #region 连接列表
        private ConnAppProc CreateConnApp(eServerType server_type)
        {
            ConnAppProc app_server = null;
            switch (server_type)
            {
                case eServerType.WORLD:
                    app_server = new WorldMsgProc();
                    break;
                case eServerType.GATE:
                    app_server = new GateMsgProc();
                    break;
                case eServerType.FIGHT:
                    app_server = new FightMsgProc();
                    break;
                case eServerType.GLOBAL:
                    app_server = new GlobalMsgProc();
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
            Log.Info("连接上服务器 type:" + app_server.srv_info.srv_type.ToString() + " sid:" + app_server.srv_info.srv_uid);
            m_srv_servers.Add(app_server.srv_info.srv_uid, app_server);
            switch (app_server.srv_info.srv_type)
            {
                case eServerType.WORLD:
                    //之所以放在连接世界服后再连接全局服，是可以告诉全局服ss所在大区id
                    Connect2GlobalServer(ServerConfig.net_info.gl_ip, ServerConfig.net_info.gl_port);
                    EventController.TriggerEvent(EventID.NET_CONNECTED_WORLD_SRV);//成功连接到世界服
                    break;
            }
        }
        private void OnConnAppLeave(ConnAppProc app_server)
        {
            Log.Info("断开与服务器的连接 type:" + app_server.srv_info.srv_type.ToString() + " sid:" + app_server.srv_info.srv_uid);
            app_server.srv_info.srv_status = eConnAppStatus.CLOSED;
            switch (app_server.srv_info.srv_type)
            {
                case eServerType.WORLD:
                    m_world_conn_idx = 0;
                    EventController.TriggerEvent(EventID.NET_DISCONNECT_WORLD_SRV);
                    break;
                case eServerType.GLOBAL:
                    m_global_conn_idx = 0;
                    break;
                case eServerType.FIGHT://如果战斗服退出，当前战斗服的玩家自动连接其他战斗服
                    EventController.TriggerEvent(EventID.NET_DISCONNECT_FIGHT_SRV, app_server.srv_info.srv_uid);
                    break;
            }
            m_srv_servers.Remove(app_server.srv_info.srv_uid);
        }
        public void GetConnAppList(List<ConnAppProc> list, eServerType type)
        {
            foreach (var obj in m_app_servers)
            {
                if (obj.Value.srv_info.srv_type == type) list.Add(obj.Value);
            }
        }
        public ConnAppProc GetConnApp(ushort srv_uid)
        {
            ConnAppProc app_server = null;
            if (m_srv_servers.TryGetValue(srv_uid, out app_server))
            {
                return app_server;
            }
            return null;
        }
        /// <summary>
        /// 时间服是否已经连接成功
        /// </summary>
        public bool IsConnectedWorldServer()
        {
            if (m_world_conn_idx == 0) return false;
            ConnAppProc app_server;
            if (m_app_servers.TryGetValue(m_world_conn_idx, out app_server))
            {
                if (app_server.srv_info.srv_status == eConnAppStatus.CONNECTED)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 分配战斗服id
        /// </summary>
        public ushort AllocFightServer()
        {
            List<ushort> list = new List<ushort>();
            foreach (var obj in m_app_servers)
            {
                ConnAppProc app_server = obj.Value;
                if (app_server.srv_info.srv_status == eConnAppStatus.CONNECTED && app_server.srv_info.srv_type == eServerType.FIGHT) 
                    list.Add(app_server.srv_info.srv_uid);
            }
            if (list.Count == 0) return 0;
            return MathUtils.RandRange_List<ushort>(list);
        }
        #endregion

        #region 发消息
        public int Send(long conn_idx, PacketBase packet)
        {
            m_send_by.Clear();
            m_send_by.WriteUShort(0);//先写入长度占位
            packet.Write(m_send_by);
            int len = NetConnectManager.Instance.Send(conn_idx, m_send_by);
            PacketPools.Recover(packet);

            return len;
        }
        /// <summary>
        /// 通过sid发送
        /// </summary>
        public int Send(ushort srv_uid, PacketBase packet)
        {
            int size = 0;
            ConnAppProc app_server = null;
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
        /// 通过gate转发给client
        /// </summary>
        public int SendProxy(ClientUID client_uid, PackBaseS2C packet, bool is_broadcast = false)
        {
            int size = 0;
            ConnAppProc app_server;
            if (m_srv_servers.TryGetValue(client_uid.srv_uid, out app_server))
            {
                ProxyS2CMsg msg = PacketPools.Get((ushort)ss2gs.msg.PROXY_SS_MSG) as ProxyS2CMsg;
                msg.is_broadcast = is_broadcast;
                msg.Set(client_uid, packet);
                size = Send(app_server.conn_idx, msg);
            }
            PacketPools.Recover(packet);//回收消息本身
            return size;
        }
        /// <summary>
        /// 发给ws
        /// </summary>
        public int Send2WS(PacketBase packet)
        {
            if (m_world_conn_idx == 0) return 0;

            ConnAppProc app_server = null;
            if (m_app_servers.TryGetValue(m_world_conn_idx, out app_server))
            {
                return app_server.Send(packet);
            }
            return 0;
        }
        /// <summary>
        /// 发给gl
        /// </summary>
        public int Send2GL(PacketBase packet)
        {
            if (m_global_conn_idx == 0) return 0;

            ConnAppProc app_server = null;
            if (m_app_servers.TryGetValue(m_global_conn_idx, out app_server))
            {
                return app_server.Send(packet);
            }
            return 0;
        }
        /// <summary>
        /// 发给fs
        /// </summary>
        public int Send2FS(ushort fs_uid, PacketBase packet)
        {
            if (fs_uid == 0) return 0;

            ConnAppProc app_server = null;
            if (m_srv_servers.TryGetValue(fs_uid, out app_server))
            {
                return app_server.Send(packet);
            }
            return 0;
        }
        /// <summary>
        /// 广播消息:通过gate转发给client
        /// </summary>
        public void BroadcastProxyMsg(PackBaseS2C packet)
        {
            foreach (var obj in m_app_servers)
            {
                if (obj.Value.srv_info.srv_type == eServerType.GATE)
                {
                    ProxyS2CMsg msg = PacketPools.Get((ushort)ss2gs.msg.PROXY_SS_MSG) as ProxyS2CMsg;
                    msg.is_broadcast = true;
                    msg.Set(packet.client_uid, packet);
                    obj.Value.Send(msg);
                }
            }
            PacketPools.Recover(packet);//回收消息本身
        }
        #endregion
    }
}
