using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 逻辑服消息处理
    /// @author hannibal
    /// @time 2016-8-16
    /// </summary>
    public class ServerMsgProc : ConnAppProc
    {
        public ServerMsgProc()
            : base()
        {
            m_msg_begin_idx = ProtocolID.MSG_BASE_SS2GS;
        }
        public override void Setup()
        {
            base.Setup();
        }
        public override void Destroy()
        {
            base.Destroy();
        }
        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public override int Send(PacketBase packet)
        {
            if (packet is PackBaseS2S)
                ((PackBaseS2S)packet).server_uid.gs_uid = ServerNetManager.Instance.srv_uid;

            return ForServerNetManager.Instance.Send(m_conn_idx, packet);
        }
        protected override void RegisterHandle()
        {
            RegisterMsgProc(ss2gs.msg.PROXY_SS_MSG, HandleReqProxyMsgToClient);
            RegisterMsgProc(ss2gs.msg.CREATE_CHARACTER, OnCreateCharacter);
            RegisterMsgProc(ss2gs.msg.REQ_KICK_ACCOUNT, OnReqKickAccount);
            RegisterMsgProc(ss2gs.msg.ENTER_GAME, OnEnterGame);
            RegisterMsgProc(ss2gs.msg.ROBOT_TEST, OnRobotTest);
            RegisterMsgProc(ss2gs.msg.NOTIFY_SERVER, OnNotifyServer);
        }

        private void RegisterMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - m_msg_begin_idx);
            m_msg_proc[msg_id] = fun;
        }
        /// <summary>
        /// 处理转发给client的消息
        /// </summary>
        private ClientUID tmp_client_uid = new ClientUID();
        private byte[] tmp_client_uid_by = new byte[ClientUID.Size()];
        private void HandleReqProxyMsgToClient(PacketBase packet)
        {
            ProxyS2CMsg proxy_msg = packet as ProxyS2CMsg;

            //转发的消息id
            ushort header = proxy_msg.data.ReadUShort();
            //由于ws2c和ss2c有部分消息共用(如ws上的聊天用到了ss2c的聊天)，这里屏蔽id检测
            //if (header < ProtocolID.MSG_BASE_SS2C || header >= ProtocolID.MSG_BASE_SS2C + ProtocolID.MSG_APPLAYER_PER_INTERVAL)
            //{
            //    Log.Debug("收到错误的消息ID:" + header);
            //    return;
            //}
            //读clientuid
            proxy_msg.data.Peek(ref tmp_client_uid_by, tmp_client_uid_by.Length);
            tmp_client_uid.Read(tmp_client_uid_by);
            if (proxy_msg.is_broadcast)
            {
                ClientSessionManager.Instance.BroadcastProxy(header, proxy_msg.data, eSessionStatus.IN_GAMING);
            }
            else
            {
                ForClientNetManager.Instance.SendProxy(tmp_client_uid.conn_idx, header, proxy_msg.data);
            }
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        private void OnCreateCharacter(PacketBase packet)
        {
            ss2gs.CreateCharacter msg = packet as ss2gs.CreateCharacter;

            gs2c.CreateCharacter rep_msg = PacketPools.Get(gs2c.msg.CREATE_CHARACTER) as gs2c.CreateCharacter;
            rep_msg.result = msg.result;
            rep_msg.char_idx = msg.char_idx;
            ForClientNetManager.Instance.Send(msg.client_uid.conn_idx, rep_msg);
        }
        /// <summary>
        /// 踢号
        /// </summary>
        private void OnReqKickAccount(PacketBase packet)
        {
            ss2gs.ReqKickoutAccount msg = packet as ss2gs.ReqKickoutAccount;
            ClientSession session = ClientSessionManager.Instance.GetSessionByAccount(msg.account_idx);
            if (session != null)
            {
                ClientSessionManager.Instance.KickoutSession(session.conn_idx);
            }
        }
        /// <summary>
        /// 进入游戏
        /// </summary>
        private void OnEnterGame(PacketBase packet)
        {
            ss2gs.EnterGame msg = packet as ss2gs.EnterGame;

            //确定session是否还存在
            ClientSession session = ClientSessionManager.Instance.GetSession(msg.client_uid.conn_idx);
            if (session != null)
            {
                session.session_status = eSessionStatus.IN_GAMING;

                gs2c.EnterGame rep_msg = PacketPools.Get(gs2c.msg.ENTER_GAME) as gs2c.EnterGame;
                rep_msg.char_idx = msg.char_idx;
                session.Send(rep_msg);
            }
        }
        /// <summary>
        /// 压力测试
        /// </summary>
        /// <param name="packet"></param>
        private void OnRobotTest(PacketBase packet)
        {
            ss2gs.RobotTest msg = packet as ss2gs.RobotTest;

            gs2c.RobotTest rep_msg = PacketPools.Get(gs2c.msg.ROBOT_TEST) as gs2c.RobotTest;
            rep_msg.length = msg.length;
            ForClientNetManager.Instance.Send(msg.client_uid.conn_idx, rep_msg);
        }
        /// <summary>
        /// 通知网关玩家连接的服务器
        /// </summary>
        private void OnNotifyServer(PacketBase packet)
        {
            ss2gs.NotifyServer msg = packet as ss2gs.NotifyServer;

            ClientSession session = ClientSessionManager.Instance.GetSessionByAccount(msg.account_idx);
            if(session != null)
            {
                switch(msg.s_type)
                {
                    case eServerType.FIGHT: session.fs_uid = msg.fs_uid; break;
                }
            }
        }
    }
}
