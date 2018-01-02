using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 消息处理
    /// @author hannibal
    /// @time 2016-5-25
    /// </summary>
    public class ClientMsgProc
    {
        public delegate void MsgProcFunction(ClientSession sessioin, PacketBase packet);
        protected Dictionary<ushort, MsgProcFunction> m_msg_proc = null;

        public ClientMsgProc()
        {
            m_msg_proc = new Dictionary<ushort, MsgProcFunction>();
        }
        public void Setup()
        {
            this.RegisterHandle();
        }
        public void Destroy()
        {
            m_msg_proc.Clear();
        }
        private void RegisterHandle()
        {
            RegisterMsgProc(c2gs.msg.PING_NET, OnPingNet);
            RegisterMsgProc(c2gs.msg.CLIENT_LOGIN, OnClientLogin);
            RegisterMsgProc(c2gs.msg.ENUM_CHAR, OnEnumCharacterList);
            RegisterMsgProc(c2gs.msg.CREATE_CHARACTER, OnCreateCharacter);
            RegisterMsgProc(c2gs.msg.ENTER_GAME, OnEnterGame);
            RegisterMsgProc(c2gs.msg.ROBOT_TEST, OnClientRobotTest);
            RegisterMsgProc(c2gs.msg.SPEED_CHECK, OnSpeedCheck);
        }
        private void RegisterMsgProc(ushort id, MsgProcFunction fun)
        {
            if(m_msg_proc.ContainsKey(id))
            {
                Log.Error("相同键已经存在:" + id);
                return;
            }
            m_msg_proc.Add(id, fun);
        }

        /// <summary>
        /// 网络事件处理
        /// </summary>
        public void OnNetworkClient(long conn_idx, ushort header, ByteArray data)
        {
            ClientSession session = ClientSessionManager.Instance.GetSession(conn_idx);
            if (session != null)
            {
                ///1.判断cd
                if (!session.CheckCooldown(header))
                {
                    //Log.Debug("协议cd不够:" + header);
                    return;
                }

                ///2.消息解密
                if(GlobalID.ENABLE_PACKET_ENCRYPT)
                {
                    //id有效性校验
                    uint packet_idx = data.ReadUInt();
                    packet_idx = PacketEncrypt.DecrpytPacketIndex(packet_idx, PacketEncrypt.Encrypt_Key);
                    if (packet_idx < session.last_packet_idx)
                    {
                        Log.Warning("协议索引校验失败conn_idx:" + conn_idx + " header:" + header);
                        ClientSessionManager.Instance.KickoutSession(conn_idx);
                        return;
                    }
                    session.last_packet_idx = packet_idx;

                    //验证数据的有效性：防止被修改
                    ushort send_verify_data = data.ReadUShort();
                    ushort verify_data = PacketEncrypt.CalcPacketDataVerify(data.Buffer, 6, data.Available, packet_idx, PacketEncrypt.Encrypt_Key);
                    if (send_verify_data != verify_data)
                    {
                        Log.Warning("数据有效性校验失败conn_idx:" + conn_idx + " header:" + header);
                        ClientSessionManager.Instance.KickoutSession(conn_idx);
                        return;
                    }
                }

                ///3.是否转发消息
                if(header >= ProtocolID.MSG_BASE_C2SS && header < ProtocolID.MSG_BASE_C2SS + ProtocolID.MSG_APPLAYER_PER_INTERVAL)
                {
                    HandleProxyMsgToSS(session, header, data);
                }
                else if (header >= ProtocolID.MSG_BASE_C2WS && header < ProtocolID.MSG_BASE_C2WS + ProtocolID.MSG_APPLAYER_PER_INTERVAL)
                {
                    HandleProxyMsgToWS(session, header, data);
                }
                else if (header >= ProtocolID.MSG_BASE_C2FS && header < ProtocolID.MSG_BASE_C2FS + ProtocolID.MSG_APPLAYER_PER_INTERVAL)
                {
                    HandleProxyMsgToFS(session, header, data);
                }
                else
                {
                    PacketBase packet = PacketPools.Get(header);
                    packet.Read(data);

                    MsgProcFunction fun;
                    if (m_msg_proc.TryGetValue(packet.header, out fun))
                    {
                        try
                        {
                            fun(session, packet);
                        }
                        catch(Exception e)
                        {
                            Log.Exception(e);
                        }
                    }
                    else
                    {
                        Log.Warning("未注册消息处理函数id：" + header);
                    }
                    PacketPools.Recover(packet);
                }
            }
            else
            {
                PacketBase packet = PacketPools.Get(header);
                packet.Read(data);
                switch (packet.header)
                {
                    case c2gs.msg.ENCRYPT:
                        OnClientEncrypt(conn_idx, packet);
                        break;
                    default://没有session，又不是握手协议，说明发错协议了，直接踢掉
                        ClientSessionManager.Instance.KickoutSession(conn_idx);
                        break;
                }
                PacketPools.Recover(packet);
            }
        }
        /// <summary>
        /// 处理由gate转发到ss的消息
        /// </summary>
        private void HandleProxyMsgToSS(ClientSession session, ushort header, ByteArray data)
        {
            if(session.ss_uid == 0)
            {
                Log.Debug("已经与ss断开连接");
                return;
            }
            if (session.session_status != eSessionStatus.IN_GAMING && session.session_status != eSessionStatus.ALREADY_LOGIN)
            {
                Log.Debug("session状态错误:" + session.session_status);
                return;
            }
            ProxyC2SMsg msg = PacketPools.Get(gs2ss.msg.PROXY_CLIENT_MSG) as ProxyC2SMsg;
            msg.Set(session.client_uid, header, data);
            ForServerNetManager.Instance.Send(session.ss_uid, msg);
        }
        /// <summary>
        /// 处理由gate转发到ws的消息
        /// </summary>
        private void HandleProxyMsgToWS(ClientSession session, ushort header, ByteArray data)
        {
            if (ServerNetManager.Instance.world_conn_idx == 0)
            {
                Log.Debug("已经与ws断开连接");
                return;
            }
            if (session.session_status != eSessionStatus.IN_GAMING && session.session_status != eSessionStatus.ALREADY_LOGIN)
            {
                Log.Debug("session状态错误:" + session.session_status);
                return;
            }
            ProxyC2SMsg msg = PacketPools.Get(gs2ws.msg.PROXY_CLIENT_MSG) as ProxyC2SMsg;
            msg.Set(session.client_uid, header, data);
            ServerNetManager.Instance.Send2WS(msg);
        }
        /// <summary>
        /// 处理由gate转发到fs的消息
        /// </summary>
        private void HandleProxyMsgToFS(ClientSession session, ushort header, ByteArray data)
        {
            if (session.fs_uid == 0)
            {
                Log.Debug("已经与fs断开连接");
                return;
            }
            if (session.session_status != eSessionStatus.IN_GAMING && session.session_status != eSessionStatus.ALREADY_LOGIN)
            {
                Log.Debug("session状态错误:" + session.session_status);
                return;
            }
            ProxyC2SMsg msg = PacketPools.Get(gs2fs.msg.PROXY_CLIENT_MSG) as ProxyC2SMsg;
            msg.Set(session.client_uid, header, data);
            ForServerNetManager.Instance.Send(session.fs_uid, msg);
        }
        /// <summary>
        /// 加密
        /// </summary>
        private void OnClientEncrypt(long conn_idx, PacketBase packet)
        {
            if (!ClientSessionManager.Instance.HasAcceptSession(conn_idx))
                return;

            c2gs.EncryptInfo msg = packet as c2gs.EncryptInfo;

            //版本验证
            byte main_version;
            byte sub_version;
            ushort revision_version;
            GlobalID.SplitVersion(msg.version, out main_version, out sub_version, out revision_version);
            if (main_version != GlobalID.VERSION_MAIN)
            {
                ClientSessionManager.Instance.KickoutSession(conn_idx);
                return;
            }
            GlobalID.SetClientVersion(msg.version);

            //收到正常的握手协议后，才加入正式session列表
            ClientSession session = ClientSessionManager.Instance.AddSession(conn_idx);
            if (session != null)
            {
                ClientSessionManager.Instance.CleanupAcceptSession(conn_idx);
                session.session_status = eSessionStatus.CREATED;

                gs2c.EncryptInfo rep_msg = PacketPools.Get(gs2c.msg.ENCRYPT) as gs2c.EncryptInfo;
                rep_msg.key = PacketEncrypt.Encrypt_Key;
                rep_msg.flags = 0;
                session.Send(rep_msg);
            }
            else
            {
                ClientSessionManager.Instance.KickoutSession(conn_idx);
            }
        }
        /// <summary>
        /// ping网络
        /// </summary>
        private void OnPingNet(ClientSession session, PacketBase packet)
        {
            c2gs.PingNet msg = packet as c2gs.PingNet;

            long offset_time = Time.time - msg.tick;
            Log.Debug("收到第:" + msg.packet_id + " 时间:" + Time.time + " 延迟:" + offset_time);

            gs2c.PingNet rep_msg = PacketPools.Get(gs2c.msg.PING_NET) as gs2c.PingNet;
            rep_msg.packet_id = msg.packet_id;
            rep_msg.tick = msg.tick;
            rep_msg.flags = msg.flags;
            session.Send(rep_msg);
        }
        /// <summary>
        /// 登陆
        /// </summary>
        private void OnClientLogin(ClientSession session, PacketBase packet)
        {
            c2gs.ClientLogin msg = packet as c2gs.ClientLogin;

            if (string.IsNullOrEmpty(msg.name) || string.IsNullOrEmpty(msg.psw))
            {
                Log.Debug("数据错误 name:" + msg.name);
                return;
            }

            //只有创建或登录错误，才能进一步验证
            if (session.session_status != eSessionStatus.CREATED && session.session_status != eSessionStatus.LOGIN_FAILED)
            {
                Log.Debug("错误的seesion状态:" + session.session_status);
                return;
            }

            //标记状态，正在验证中，防止重复验证
            session.session_status = eSessionStatus.LOGIN_DOING;

            //发给ws请求验证
            gs2ws.ClientLogin ws_msg = PacketPools.Get(gs2ws.msg.CLIENT_LOGIN) as gs2ws.ClientLogin;
            ws_msg.client_uid = session.client_uid;
            ws_msg.name = msg.name;
            ws_msg.psw = msg.psw;
            ServerNetManager.Instance.Send2WS(ws_msg);
        }
        /// <summary>
        /// 角色列表
        /// </summary>
        private void OnEnumCharacterList(ClientSession session, PacketBase packet)
        {
            gs2ss.EnumCharacter msg = PacketPools.Get(gs2ss.msg.ENUM_CHAR) as gs2ss.EnumCharacter;
            msg.client_uid = session.client_uid;
            msg.account_idx = session.account_idx;
            msg.game_db_id = session.db_id.game_id;
            ForServerNetManager.Instance.Send(session.ss_uid, msg);
            
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        private void OnCreateCharacter(ClientSession session, PacketBase packet)
        {
            c2gs.CreateCharacter msg = packet as c2gs.CreateCharacter;

            //发送到ws，分配char_idx
            gs2ws.CreateCharacter rep_msg = PacketPools.Get(gs2ws.msg.CREATE_CHARACTER) as gs2ws.CreateCharacter;
            rep_msg.client_uid = session.client_uid;
            rep_msg.account_idx = session.account_idx;
            rep_msg.db_id = session.db_id.game_id;
            rep_msg.name = msg.name;
            rep_msg.flags = msg.flags;
            ServerNetManager.Instance.Send2WS(rep_msg);
        }
        /// <summary>
        /// 进入游戏
        /// </summary>
        private void OnEnterGame(ClientSession session, PacketBase packet)
        {
            c2gs.EnterGame msg = packet as c2gs.EnterGame;

            if (session.session_status != eSessionStatus.ALREADY_LOGIN)
            {
                Log.Debug("OnEnterGame::错误的seesion状态:" + session.session_status);
                return;
            }
            //请求进入游戏
            session.session_status = eSessionStatus.ENTER_GAMING;

            gs2ss.EnterGame rep_msg = PacketPools.Get(gs2ss.msg.ENTER_GAME) as gs2ss.EnterGame;
            rep_msg.client_uid = session.client_uid;
            rep_msg.account_idx = session.account_idx;
            rep_msg.char_idx = msg.char_idx; 
            session.Send2SS(rep_msg);
        }
        /// <summary>
        /// 压力测试
        /// </summary>
        private void OnClientRobotTest(ClientSession session, PacketBase packet)
        {
            c2gs.RobotTest msg = packet as c2gs.RobotTest;
            if(Utils.HasFlag(msg.flags, (uint)eServerType.WORLD))
            {
                gs2ws.RobotTest rep_msg = PacketPools.Get(gs2ws.msg.ROBOT_TEST) as gs2ws.RobotTest;
                rep_msg.client_uid = session.client_uid;
                rep_msg.length = msg.length;
                ServerNetManager.Instance.Send2WS(rep_msg);
            }
            if (Utils.HasFlag(msg.flags, (uint)eServerType.SERVER))
            {
                gs2ss.RobotTest rep_msg = PacketPools.Get(gs2ss.msg.ROBOT_TEST) as gs2ss.RobotTest;
                rep_msg.client_uid = session.client_uid;
                rep_msg.length = msg.length;
                session.Send2SS(rep_msg);
            }
            if (msg.flags == 0 || Utils.HasFlag(msg.flags, (uint)eServerType.GATE))
            {
                gs2c.RobotTest rep_msg = PacketPools.Get(gs2c.msg.ROBOT_TEST) as gs2c.RobotTest;
                rep_msg.length = msg.length;
                session.Send(rep_msg);
            }
        }
        /// <summary>
        /// 加速检测
        /// </summary>
        private void OnSpeedCheck(ClientSession session, PacketBase packet)
        {
            c2gs.SpeedCheck msg = packet as c2gs.SpeedCheck;

            session.speed_checker.OnRecvTrapMsg(msg.check_sn);
        }
    }
}
