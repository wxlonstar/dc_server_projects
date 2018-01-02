using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 服务器网络管理:只接受其他服连接
    /// @author hannibal
    /// @time 2016-7-27
    /// </summary>
    public class ServerNetManager : Singleton<ServerNetManager>
    {
        private IOCPNetAccepter m_net_socket = null;
        private ByteArray m_send_by = null;
        private Dictionary<long, ConnAppProc> m_app_servers = null;
        private Dictionary<ushort, ConnAppProc> m_srv_servers = null;//srv->server

        private Dictionary<ushort, long> m_max_char_idx = null;//保存每个分库的最大角色id

        public ServerNetManager()
        {
            m_app_servers = new Dictionary<long, ConnAppProc>();
            m_srv_servers = new Dictionary<ushort, ConnAppProc>();
            m_max_char_idx = new Dictionary<ushort, long>();
            m_send_by = NetUtils.AllocSendPacket();
        }

        public void Setup()
        {
            ProtocolID.RegisterPools();
            m_net_socket = new IOCPNetAccepter();
            m_net_socket.Setup();
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
            m_max_char_idx.Clear();
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
                Log.Info("服务器已经启动 port:" + port);
            else
                Log.Error("服务器开启失败 port:" + port);
            return ret;
        }
        /// <summary>
        /// ws统一踢号入口
        /// </summary>
        public void KickAccount(long account_idx)
        {
            Unit unit = UnitManager.Instance.GetUnitByAccount(account_idx);
            if (unit == null) return;

            ws2gs.ReqKickoutAccount msg = PacketPools.Get(ws2gs.msg.REQ_KICK_ACCOUNT) as ws2gs.ReqKickoutAccount;
            msg.account_idx = account_idx;
            ServerNetManager.Instance.Send(unit.client_uid.srv_uid, msg);
        }

        #region ws分配角色id
        /// <summary>
        /// 初始化每个gamedb对应的最大账号id
        /// </summary>
        public void InitNextCharIdx()
        {
            foreach (var obj in ServerConfig.db_info.db_list)
            {
                if (obj.type == (ushort)eDBType.Game)
                {
                    QueryMaxCharIdx(obj.id);
                }
            }
        }
        private void QueryMaxCharIdx(ushort game_db_id)
        {
            DBID db_id = new DBID();
            db_id.game_id = game_db_id;
            SQLCharHandle.QueryMaxCharIdx(ServerConfig.net_info.server_realm, db_id, max_id =>
            {
                long next_char_idx = max_id;
                if (next_char_idx == 0)
                {//没有数据，则初始化起始值
                    next_char_idx = ServerConfig.net_info.server_realm;
                    next_char_idx = next_char_idx * GlobalID.INIT_CHAR_IDX;
                }
                m_max_char_idx.Add(game_db_id, next_char_idx);
                Log.Info("next_char_idx " + game_db_id + " :" + next_char_idx);
            });
        }
        /// <summary>
        /// 创号时，分配唯一账号id
        /// </summary>
        public long GetNextCharIdx(ushort game_db_id)
        {
            long next_char_idx = 0;
            if (m_max_char_idx.TryGetValue(game_db_id, out next_char_idx))
            {
                m_max_char_idx[game_db_id] = ++next_char_idx;
                return next_char_idx;
            }
            Log.Warning("未找到db_id:" + game_db_id);
            return 0;
        }
        #endregion

        #region 网络
        private void OnAcceptConnect(long conn_idx)
        {
            Log.Info("Open session:" + conn_idx);
        }
        private void OnConnectClose(long conn_idx)
        {
            Log.Info("Close session:" + conn_idx);
            
            ConnAppProc app_server = null;
            if(m_app_servers.TryGetValue(conn_idx, out app_server))
            {
                //广播消息
                inner.AppServerRemove msg = PacketPools.Get(inner.msg.APPSERVER_REMOVE) as inner.AppServerRemove;
                msg.srv_uid = app_server.srv_info.srv_uid;
                this.BroadcastMsgWithout(msg, app_server.conn_idx);

                OnConnAppLeave(app_server);
            }
            m_app_servers.Remove(conn_idx);
        }
        private void OnMessageReveived(long conn_idx, ushort header, ByteArray data)
        {
            PacketBase packet = PacketPools.Get(header);
            packet.Read(data);
            do
            {
                ConnAppProc app_server = null;
                if (!m_app_servers.TryGetValue(conn_idx, out app_server))
                {
                    if (packet.header == inner.msg.REQ_LOGIN)
                    {
                        if (!m_app_servers.ContainsKey(conn_idx))
                        {
                            inner.ReqLogin msg = packet as inner.ReqLogin;

                            //检测是否相同id的服务器以及连接
                            if (m_srv_servers.ContainsKey(msg.srv_info.srv_uid))
                            {
                                Log.Warning("相同服务器以及连接 sid:" + msg.srv_info.srv_uid);
                                m_net_socket.CloseConn(conn_idx);
                                break;
                            }
                            Log.Info("收到内部服务器连接请求:" + msg.srv_info.srv_type);
                            //创建新连接
                            app_server = CreateConnApp(msg.srv_info.srv_type);
                            app_server.conn_idx = conn_idx;
                            app_server.srv_info.srv_type = msg.srv_info.srv_type;
                            app_server.srv_info.srv_status = eConnAppStatus.CONNECTED;
                            app_server.srv_info.srv_realm_idx = ServerConfig.net_info.server_realm;
                            app_server.srv_info.srv_uid = msg.srv_info.srv_uid;
                            app_server.srv_info.srv_endpoint.ip = msg.srv_info.srv_endpoint.ip;
                            app_server.srv_info.srv_endpoint.port = msg.srv_info.srv_endpoint.port;
                            m_app_servers.Add(conn_idx, app_server);
                            OnConnAppEnter(app_server);

                            //响应
                            inner.RepLogin rep_msg = PacketPools.Get(inner.msg.REP_LOGIN) as inner.RepLogin;
                            rep_msg.result = inner.RepLogin.eResult.SUCCESS;
                            rep_msg.srv_info.srv_type = eServerType.WORLD;
                            rep_msg.srv_info.srv_realm_idx = ServerConfig.net_info.server_realm;
                            rep_msg.srv_info.srv_uid = ServerConfig.net_info.server_uid;
                            rep_msg.ws_time = GameTimeManager.Instance.server_time;
                            this.Send(conn_idx, rep_msg);

                            //告诉当前存在的服务器
                            List<ConnAppProc> list_app = new List<ConnAppProc>();
                            GetConnAppList(list_app, eServerType.GATE);
                            GetConnAppList(list_app, eServerType.SERVER);
                            GetConnAppList(list_app, eServerType.FIGHT);

                            //发送当前存在的服务器列表
                            while (list_app.Count > 0)
                            {
                                inner.AppServerList list_msg = PacketPools.Get(inner.msg.APPSERVER_LIST) as inner.AppServerList;
                                for (int i = 0; i < 10 && list_app.Count > 0; ++i)
                                {
                                    ConnAppProc tmp_app = list_app[list_app.Count-1];
                                    if (tmp_app.srv_info.srv_uid == app_server.srv_info.srv_uid)
                                    {//不发送自己
                                        list_app.RemoveAt(list_app.Count - 1);
                                        continue;
                                    }

                                    AppServerItem item = new AppServerItem();
                                    item.srv_uid = tmp_app.srv_info.srv_uid;
                                    item.srv_type = tmp_app.srv_info.srv_type;
                                    item.srv_ip = tmp_app.srv_info.srv_endpoint.ip;
                                    item.srv_port = tmp_app.srv_info.srv_endpoint.port;
                                    list_msg.list.Add(item);

                                    list_app.RemoveAt(list_app.Count - 1);
                                }
                                if (list_msg.list.Count > 0)
                                    app_server.Send(list_msg);
                                else
                                    PacketPools.Recover(list_msg);
                            }

                            //广播新服务器加入
                            inner.AppServerAdd add_msg = PacketPools.Get(inner.msg.APPSERVER_ADD) as inner.AppServerAdd;
                            add_msg.app_info.srv_uid = app_server.srv_info.srv_uid;
                            add_msg.app_info.srv_type = app_server.srv_info.srv_type;
                            add_msg.app_info.srv_ip = app_server.srv_info.srv_endpoint.ip;
                            add_msg.app_info.srv_port = app_server.srv_info.srv_endpoint.port;
                            this.BroadcastMsgWithout(add_msg, app_server.conn_idx);
                        }
                    }
                }
                if (app_server != null)
                {
                    if (!app_server.HandleMsg(conn_idx, packet))
                    {
                        switch (packet.header)
                        {
                            case inner.msg.APPSERVER_SHUTDOWN:
                                break;
                        }
                    }
                }
            } while (false);
            PacketPools.Recover(packet);
        }
        #endregion

        #region 连接列表
        private ConnAppProc CreateConnApp(eServerType server_type)
        {
            ConnAppProc app_server = null;
            switch(server_type)
            {
                case eServerType.GATE:
                    app_server = new GateMsgProc();
                    break;
                case eServerType.SERVER:
                    app_server = new ServerMsgProc();
                    break;
                case eServerType.FIGHT:
                    app_server = new FightMsgProc();
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
            Log.Info("服务器加入 type:" + app_server.srv_info.srv_type.ToString() + " sid:" + app_server.srv_info.srv_uid + " ip:" + ip);
            m_srv_servers.Add(app_server.srv_info.srv_uid, app_server);

            //写入服务器信息
            RemoteServerInfo server_info = new RemoteServerInfo();
            server_info.srv_uid = app_server.srv_info.srv_uid;
            server_info.type = app_server.srv_info.srv_type;
            server_info.ip = ip;
            server_info.start_time = Time.time;
            ServerInfoManager.Instance.AddRemoteServer(server_info);
        }
        private void OnConnAppLeave(ConnAppProc app_server)
        {
            Log.Info("服务器退出 type:" + app_server.srv_info.srv_type.ToString() + " sid:" + app_server.srv_info.srv_uid);
            app_server.srv_info.srv_status = eConnAppStatus.CLOSED;
            m_srv_servers.Remove(app_server.srv_info.srv_uid);
            ServerInfoManager.Instance.RemoveRemoteServer(app_server.srv_info.srv_uid);
        }
        public void GetConnAppList(List<ConnAppProc> list, eServerType type)
        {
            foreach (var obj in m_app_servers)
            {
                if (obj.Value.srv_info.srv_type == type) list.Add(obj.Value);
            }
        }
        /// <summary>
        /// 给gate分配ss
        /// </summary>
        public ushort AllocSSForClient()
        {
            List<ConnAppProc> list = new List<ConnAppProc>();
            this.GetConnAppList(list, eServerType.SERVER);
            if (list.Count > 0)
            {
                return MathUtils.RandRange_List<ConnAppProc>(list).srv_info.srv_uid;
            }
            return 0;
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
        /// <summary>
        /// 根据服务器id发
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
        /// 发给客户端
        /// </summary>
        public int SendProxy(ClientUID client_uid, PackBaseS2C packet)
        {
            int size = 0;
            ConnAppProc app_server;
            if (m_srv_servers.TryGetValue(client_uid.srv_uid, out app_server))
            {
                ProxyS2CMsg msg = PacketPools.Get((ushort)ws2gs.msg.PROXY_WS_MSG) as ProxyS2CMsg;
                msg.Set(client_uid, packet);
                size = Send(app_server.conn_idx, msg);
            }
            else
            {
                Log.Debug("未找到服务器id:" + client_uid.srv_uid);
            }

            PacketPools.Recover(packet);//回收消息本身
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
            foreach(var obj in m_app_servers)
            {
                if(obj.Value.conn_idx != conn_idx)obj.Value.Send(packet);
            }
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
                    ProxyS2CMsg msg = PacketPools.Get((ushort)ws2gs.msg.PROXY_WS_MSG) as ProxyS2CMsg;
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
