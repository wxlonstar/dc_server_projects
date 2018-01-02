using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 连接其他服务器，包括世界服
    /// @author hannibal
    /// @time 2017-7-27
    /// </summary>
    public class ServerNetManager : Singleton<ServerNetManager>
    {
        private ByteArray m_send_by = null;
        private ushort m_srv_uid = 0;
        private ushort m_srv_realm_idx = 0;
        private long m_world_conn_idx = 0;
        private long m_next_char_idx = 0;
        private Dictionary<long, ConnAppProc> m_app_servers = null;

        public ServerNetManager()
        {
            m_app_servers = new Dictionary<long, ConnAppProc>();
            m_send_by = NetUtils.AllocSendPacket();
        }

        public void Setup()
        {
            m_world_conn_idx = 0;
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
            app_server.srv_info.srv_status = eConnAppStatus.CONNECTING;
            app_server.srv_info.srv_endpoint.ip = SocketUtils.IpToInt(ServerConfig.net_info.ip_for_server);//告诉其他服，db的ip和端口
            app_server.srv_info.srv_endpoint.port = ServerConfig.net_info.port_for_server;
            m_app_servers.Add(conn_idx, app_server);
            m_world_conn_idx = conn_idx;
        }
        private float tmpReconnectTime = 0;
        public void Reconnect2WorldServer()
        {
            if (tmpReconnectTime < Time.timeSinceStartup)
            {
                Connect2WorldServer(ServerConfig.net_info.ws_ip, ServerConfig.net_info.ws_port);
                tmpReconnectTime = Time.timeSinceStartup + 5000;
            }
        }
        public void Connect2Server(string ip, ushort port, eServerType type, ushort srv_uid)
        {
            Log.Info("连接服务器 ip:" + ip + " port:" + port + " type:" + type + " uid:" + srv_uid);
            long conn_idx = NetConnectManager.Instance.ConnectTo(ip, port, OnConnected, OnMessageReveived, OnConnectClose);

            ConnAppProc app_server = CreateConnApp(type);
            app_server.conn_idx = conn_idx;
            app_server.srv_info.srv_type = type;
            app_server.srv_info.srv_uid = srv_uid;
            app_server.srv_info.srv_realm_idx = m_srv_realm_idx;
            app_server.srv_info.srv_status = eConnAppStatus.CONNECTING;
            m_app_servers.Add(conn_idx, app_server);
        }
        public int Send(long conn_idx, PacketBase packet)
        {
            m_send_by.Clear();
            m_send_by.WriteUShort(0);//先写入长度占位
            packet.Write(m_send_by);
            int len = NetConnectManager.Instance.Send(conn_idx, m_send_by);
            PacketPools.Recover(packet);
            return len;
        }
        public int Send2WS(PacketBase packet)
        {
            if(m_world_conn_idx == 0)return 0;
            m_send_by.Clear();
            m_send_by.WriteUShort(0);//先写入长度占位
            packet.Write(m_send_by);
            int len = NetConnectManager.Instance.Send(m_world_conn_idx, m_send_by);
            PacketPools.Recover(packet);
            return len;
        }
        public void InitNextCharIdx()
	    {
            if (m_srv_realm_idx > 0)
            {
                SQLCharHandle.QueryMaxCharIdx(m_srv_realm_idx, max_id =>
                {
                    m_next_char_idx = max_id;
                    if (m_next_char_idx == 0)
                    {//没有数据
                        m_next_char_idx = m_srv_realm_idx;
                        m_next_char_idx = m_next_char_idx * 10000000000;
                    }
                    Log.Info("next_char_idx:" + m_next_char_idx);
                });
            }
            else
            {
                Log.Warning("未连接上世界服");
            }
	    }
        public long GetNextCharIdx()
        { 
            return ++m_next_char_idx;
        }

        public ushort srv_uid { get { return m_srv_uid; } }
        public ushort srv_realm_idx { get { return m_srv_realm_idx; } }
        public long world_conn_idx { get { return m_world_conn_idx; } }

        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～事件～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void OnConnected(long conn_idx)
        {
            Log.Info("open conn session:" + conn_idx);

            ConnAppProc app_server;
            if (m_app_servers.TryGetValue(conn_idx, out app_server))
            {
                if (app_server.srv_info.srv_type == eServerType.WORLD)
                {//连接世界服
                    inner.ReqLogin msg = PacketPools.Get(inner.msg.REQ_LOGIN) as inner.ReqLogin;
                    msg.srv_info.srv_type = eServerType.DB;
                    msg.srv_info.srv_uid = m_srv_uid;
                    msg.srv_info.srv_endpoint.ip = app_server.srv_info.srv_endpoint.ip;
                    msg.srv_info.srv_endpoint.port = app_server.srv_info.srv_endpoint.port;
                    this.Send(conn_idx, msg);
                }
            }
        }
        private void OnConnectClose(long conn_idx)
        {
            Log.Info("close conn session:" + conn_idx);
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
                            switch (packet.header)
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
                            if (msg.srv_info.srv_type == eServerType.WORLD)
                            {
                                m_srv_realm_idx = msg.srv_info.srv_realm_idx;//由世界服分配的id
                                app_server.srv_info.srv_uid = msg.srv_info.srv_uid;
                                app_server.srv_info.srv_realm_idx = msg.srv_info.srv_realm_idx;
                                GameTimeManager.Instance.SetAdjustTime(msg.ws_time - Time.time);//修正服务器时间
                                Log.Info("当前时间:" + Time.time + " 当前ws时间:" + msg.ws_time);
                                Log.Info("当前服所属区服id:" + m_srv_realm_idx + ", sid:" + m_srv_uid);
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
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～连接列表～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private ConnAppProc CreateConnApp(eServerType server_type)
        {
            ConnAppProc app_server = null;
            switch (server_type)
            {
                case eServerType.WORLD:
                    app_server = new WorldMsgProc();
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
            Log.Info("连接上服务器 type:" + app_server.srv_info.srv_type.ToString() + " sid:" + app_server.srv_info.srv_uid);
            if (app_server.srv_info.srv_type == eServerType.WORLD)
            {
                EventController.TriggerEvent(EventID.NET_CONNECTED_WORLD_SRV);//成功连接到世界服

                //启动DB
                DBManager.Instance.Start(ServerConfig.net_info.db_list);
                InitNextCharIdx();
            }
        }
        private void OnConnAppLeave(ConnAppProc app_server)
        {
            Log.Info("断开与服务器的连接 type:" + app_server.srv_info.srv_type.ToString() + " sid:" + app_server.srv_info.srv_uid);

            if (app_server.srv_info.srv_type == eServerType.WORLD)
            {
                m_world_conn_idx = 0;
                EventController.TriggerEvent(EventID.NET_DISCONNECT_WORLD_SRV);
            }
        }
        public void GetConnAppList(List<ConnAppProc> list, eServerType type)
        {
            foreach (var obj in m_app_servers)
            {
                if (obj.Value.srv_info.srv_type == type) list.Add(obj.Value);
            }
        }
    }
}
