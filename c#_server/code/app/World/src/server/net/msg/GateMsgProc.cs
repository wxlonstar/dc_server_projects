using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 网关+client消息处理
    /// @author hannibal
    /// @time 2016-8-16
    /// </summary>
    public class GateMsgProc : ConnAppProc
    {
        protected MsgProcFunction[] m_client_msg_proc = null;

        public GateMsgProc()
            : base()
        {
            m_msg_begin_idx = ProtocolID.MSG_BASE_GS2WS;
            m_client_msg_proc = new MsgProcFunction[ProtocolID.MSG_APPLAYER_PER_INTERVAL];
            for (int i = 0; i < ProtocolID.MSG_APPLAYER_PER_INTERVAL; ++i)
            {
                m_client_msg_proc[i] = null;
            }
        }
        public override void Setup()
        {
            base.Setup();
        }
        public override void Destroy()
        {
            base.Destroy();
            for (int i = 0; i < ProtocolID.MSG_APPLAYER_PER_INTERVAL; ++i)
            {
                m_client_msg_proc[i] = null;
            }
        }
        protected override void RegisterHandle()
        {
            //处理客户端消息
            RegisterClientMsgProc(c2ws.msg.SERVER_TIME, OnServerTime);
            RegisterClientMsgProc(c2ws.msg.PING_NET, OnPingNet);

            //处理gate消息
            RegisterMsgProc(gs2ws.msg.PROXY_CLIENT_MSG, HandleProxyClientMsg);
            RegisterMsgProc(gs2ws.msg.CLIENT_LOGIN, OnClientLogin);
            RegisterMsgProc(gs2ws.msg.ROBOT_TEST, OnRobotTest);
            RegisterMsgProc(gs2ws.msg.CREATE_CHARACTER, OnCreateCharacter);
            RegisterMsgProc(gs2ws.msg.ONLINE_COUNT, OnPlayerCount);
        }
        private void RegisterMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - m_msg_begin_idx);
            m_msg_proc[msg_id] = fun;
        }
        private void RegisterClientMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - ProtocolID.MSG_BASE_C2WS);
            m_client_msg_proc[msg_id] = fun;
        }
        /// <summary>
        /// 处理由gate转发的client消息
        /// </summary>
        /// <param name="packet"></param>
        private void HandleProxyClientMsg(PacketBase packet)
        {
            ProxyC2SMsg proxy_msg = packet as ProxyC2SMsg;

            //转发的消息id
            ushort header = proxy_msg.data.ReadUShort();
            if (header < ProtocolID.MSG_BASE_C2WS || header >= ProtocolID.MSG_BASE_C2WS + ProtocolID.MSG_APPLAYER_PER_INTERVAL)
            {
                Log.Debug("收到错误的消息ID:" + header);
                return;
            }

            PacketBase msg = PacketPools.Get(header);
            msg.Read(proxy_msg.data);

            //处理
            ushort msg_id = (ushort)(header - ProtocolID.MSG_BASE_C2WS);
            MsgProcFunction fun = m_client_msg_proc[msg_id];
            if (fun == null)
            {
                Log.Debug("未注册消息处理函数ID:" + header);
            }
            else
            {
                fun(msg);
            }
            PacketPools.Recover(msg);
        }
        /// <summary>
        /// 登陆
        /// </summary>
        private void OnClientLogin(PacketBase packet)
        {
            gs2ws.ClientLogin msg = packet as gs2ws.ClientLogin;

            string account_name = msg.name;
            string account_psw = msg.psw;
            ClientUID client_uid = msg.client_uid;

            //查询是否在缓存
            AccountData account_data = new AccountData();
            if (AccountCacheManager.Instance.GetAccountData(account_name, ref account_data))
            {
                CheckAccountLogin(client_uid, account_name, account_psw, account_data);
            }
            else
            {
                SQLLoginHandle.QueryAccountData(account_name, (data) =>
                {
                    AccountCacheManager.Instance.AddAccountData(account_name, data);
                    CheckAccountLogin(client_uid, account_name, account_psw, data);
                }
                );
            }
        }
        /// <summary>
        /// 请求验证
        /// </summary>
        private void CheckAccountLogin(ClientUID client_uid, string account_name, string account_psw, AccountData account_data)
        {
            ///1.验证账号密码
            eLoginResult result = eLoginResult.E_FAILED_UNKNOWNERROR;
            if (account_data.account_idx > 0)
            {
                string md5_msg = StringUtils.GetMD5(account_psw);
                if (account_data.password_md5 == md5_msg)
                    result = eLoginResult.E_SUCCESS;
                else
                    result = eLoginResult.E_FAILED_INVALIDPASSWORD;
            }
            else
                result = eLoginResult.E_FAILED_INVALIDACCOUNT;

            ///2.验证结果处理
            if (result == eLoginResult.E_SUCCESS)
            {
                //处理踢号：如果存在账号索引，说明已经登录成功过一次
                Unit unit = UnitManager.Instance.GetUnitByAccount(account_data.account_idx);
                if (unit != null)
                {
                    //踢号：发给账号所在的gate
                    ServerNetManager.Instance.KickAccount(unit.account_idx);

                    //延长几秒发送，等待踢号
                    long account_idx = account_data.account_idx;
                    ushort spid = account_data.spid;
                    TimerManager.Instance.AddOnce(3000, (timer_id, param) =>
                    {
                        this.SendLoginResult(client_uid, result, account_idx, spid);
                    });
                }
                else
                {
                    this.SendLoginResult(client_uid, result, account_data.account_idx, account_data.spid);
                }
            }
            else
            {
                this.SendLoginResult(client_uid, result, 0, 0);
            }
        }
        /// <summary>
        /// 发送验证结果
        /// </summary>
        private void SendLoginResult(ClientUID client_uid, eLoginResult result, long account_idx, ushort spid)
        {
            ws2gs.ClientLogin msg = PacketPools.Get(ws2gs.msg.CLIENT_LOGIN) as ws2gs.ClientLogin;
            msg.client_uid = client_uid;
            msg.login_result = result;
            if(result == eLoginResult.E_SUCCESS)
            {
                msg.account_idx = account_idx;
                msg.spid = spid;
                msg.ss_uid = ServerNetManager.Instance.AllocSSForClient();
            }
            ServerNetManager.Instance.Send(client_uid.srv_uid, msg);
        }
        /// <summary>
        /// 压力测试
        /// </summary>
        /// <param name="packet"></param>
        private void OnRobotTest(PacketBase packet)
        {
            gs2ws.RobotTest msg = packet as gs2ws.RobotTest;

            ws2gs.RobotTest rep_msg = PacketPools.Get(ws2gs.msg.ROBOT_TEST) as ws2gs.RobotTest;
            rep_msg.client_uid = msg.client_uid;
            rep_msg.length = msg.length;
            this.Send(rep_msg);
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        private void OnCreateCharacter(PacketBase packet)
        {
            gs2ws.CreateCharacter msg = packet as gs2ws.CreateCharacter;

            CreateCharacterInfo create_info = new CreateCharacterInfo();
            create_info.spid = msg.spid;
            create_info.ws_id = ServerConfig.net_info.server_realm;
            create_info.ss_id = 0;
            create_info.fs_id = 0;
            create_info.char_idx = ServerNetManager.Instance.GetNextCharIdx(msg.db_id);
            create_info.char_name = msg.name;
            create_info.char_type = (byte)msg.flags;

            long account_idx = msg.account_idx;
            ushort gs_uid = msg.server_uid.gs_uid;
            SQLCharHandle.CreateCharacter(account_idx, new DBID(msg.db_id), create_info, (res) =>
            {
                eCreateCharResult result = eCreateCharResult.E_FAILED_COMMONERROR;
                if (create_info.char_idx == res)
                    result = eCreateCharResult.E_SUCCESS;
                else
                {
                    switch (res)
                    {
                        case 0: result = eCreateCharResult.E_FAILED_INTERNALERROR; break;
                        case 1: result = eCreateCharResult.E_FAILED_CHARCOUNTLIMIT; break;
                        case 2: result = eCreateCharResult.E_FAILED_INVALIDPARAM_REPEATEDNAME; break;
                        case 3: result = eCreateCharResult.E_FAILED_COMMONERROR; break;
                    }
                }

                ws2gs.CreateCharacter rep_msg = PacketPools.Get(ws2gs.msg.CREATE_CHARACTER) as ws2gs.CreateCharacter;
                rep_msg.result = result;
                rep_msg.account_idx = account_idx;
                rep_msg.char_idx = create_info.char_idx;
                ServerNetManager.Instance.Send(gs_uid, rep_msg);
            }
            );
        }
        /// <summary>
        /// 上报玩家数量
        /// </summary>
        private void OnPlayerCount(PacketBase packet)
        {
            gs2ws.OnlineCount msg = packet as gs2ws.OnlineCount;
            ServerInfoManager.Instance.UpdatePlayerCount(msg.server_uid.gs_uid, msg.count);
        }

        #region 处理客户端转发的消息
        /// <summary>
        /// ping网络
        /// </summary>
        private void OnPingNet(PacketBase packet)
        {
            c2ws.PingNet msg = packet as c2ws.PingNet;

            long offset_time = Time.time - msg.tick;
            Log.Debug("收到第:" + msg.packet_id + " 时间:" + Time.time + " 延迟:" + offset_time);

            ws2c.PingNet rep_msg = PacketPools.Get(ws2c.msg.PING_NET) as ws2c.PingNet;
            rep_msg.packet_id = msg.packet_id;
            rep_msg.tick = msg.tick;
            rep_msg.flags = msg.flags;
            ServerNetManager.Instance.SendProxy(msg.client_uid, rep_msg);
        }
        /// <summary>
        /// 服务器时间
        /// </summary>
        private void OnServerTime(PacketBase packet)
        {
            c2ws.ServerTime msg = packet as c2ws.ServerTime;

            //告诉客户端当前服务器时间
            ws2c.ServerTime rep_msg = PacketPools.Get(ws2c.msg.SERVER_TIME) as ws2c.ServerTime;
            rep_msg.server_time = GameTimeManager.Instance.server_time;
            ServerNetManager.Instance.SendProxy(msg.client_uid, rep_msg);
        }
        #endregion
    }
}
