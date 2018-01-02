using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 世界服消息处理
    /// @author hannibal
    /// @time 2016-8-16
    /// </summary>
    public class WorldMsgProc : ConnAppProc
    {
        public WorldMsgProc()
            : base()
        {
            m_msg_begin_idx = ProtocolID.MSG_BASE_WS2GS;
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

            return ServerNetManager.Instance.Send(m_conn_idx, packet);
        }
        protected override void RegisterHandle()
        {
            RegisterMsgProc(ws2gs.msg.PROXY_WS_MSG, HandleReqProxyMsgToClient);
            RegisterMsgProc(ws2gs.msg.CLIENT_LOGIN, OnClientLogin);
            RegisterMsgProc(ws2gs.msg.REQ_KICK_ACCOUNT, OnReqKickAccount);
            RegisterMsgProc(ws2gs.msg.ROBOT_TEST, OnRobotTest);
            RegisterMsgProc(ws2gs.msg.CREATE_CHARACTER, OnCreateCharacter);
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
            //if (header < ProtocolID.MSG_BASE_WS2C || header >= ProtocolID.MSG_BASE_WS2C + ProtocolID.MSG_APPLAYER_PER_INTERVAL)
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
        /// 收到广播服务器列表
        /// </summary>
        public void HandleAppServerList(PacketBase packet)
        {
            inner.AppServerList list_msg = packet as inner.AppServerList;
            for(int i = 0; i < list_msg.list.Count; ++i)
            {
                AppServerItem item = list_msg.list[i];
                Log.Info("收到广播服务器列表 type:" + item.srv_type + " sid:" + item.srv_uid);
            }
        }
        /// <summary>
        /// 新服务器加入
        /// </summary>
        public void HandleAppServerAdd(PacketBase packet)
        {
            inner.AppServerAdd add_msg = packet as inner.AppServerAdd;
            Log.Info("收到新服务器 type:" + add_msg.app_info.srv_type + " sid:" + add_msg.app_info.srv_uid);
            AppServerItem item = add_msg.app_info;
        }
        /// <summary>
        /// 移除服务器
        /// </summary>
        public void HandleAppServerRemove(PacketBase packet)
        {
            inner.AppServerRemove re_msg = packet as inner.AppServerRemove;
            Log.Info("收到服务器关闭 sid:" + re_msg.srv_uid);
        }
        /// <summary>
        /// 登录
        /// </summary>
        private void OnClientLogin(PacketBase packet)
        {
            ws2gs.ClientLogin msg = packet as ws2gs.ClientLogin;

            ///1.查询完成，确认这个过程中是否已经退出
            ClientSession session = ClientSessionManager.Instance.GetSession(msg.client_uid.conn_idx);
            if (session == null) return;

            ///2.修改状态
            if (msg.login_result == eLoginResult.E_SUCCESS)
            {
                //更新数据
                session.account_idx = msg.account_idx;
                session.spid = msg.spid;
                session.ss_uid = msg.ss_uid;
                session.session_status = eSessionStatus.ALREADY_LOGIN;
                ClientSessionManager.Instance.AddSessionByAccount(session.account_idx, session.conn_idx);
            }
            else
            {
                session.session_status = eSessionStatus.LOGIN_FAILED;
                //是否超过验证次数：是的话直接踢号
                session.login_error_count += 1;
                if (session.login_error_count >= 5)
                {
                    ClientSessionManager.Instance.KickoutSession(session.conn_idx);
                    return;
                }
            }

            ///3.告诉客户端
            gs2c.ClientLogin rep_msg = PacketPools.Get(gs2c.msg.CLIENT_LOGIN) as gs2c.ClientLogin;
            rep_msg.login_result = msg.login_result;
            session.Send(rep_msg);
        }
        /// <summary>
        /// 踢号
        /// </summary>
        private void OnReqKickAccount(PacketBase packet)
        {
            ws2gs.ReqKickoutAccount msg = packet as ws2gs.ReqKickoutAccount;
            ClientSession session = ClientSessionManager.Instance.GetSessionByAccount(msg.account_idx);
            if (session != null)
            {
                ClientSessionManager.Instance.KickoutSession(session.conn_idx);
            }
        }
        /// <summary>
        /// 压力测试
        /// </summary>
        private void OnRobotTest(PacketBase packet)
        {
            ws2gs.RobotTest msg = packet as ws2gs.RobotTest;

            gs2c.RobotTest rep_msg = PacketPools.Get(gs2c.msg.ROBOT_TEST) as gs2c.RobotTest;
            rep_msg.length = msg.length;
            ForClientNetManager.Instance.Send(msg.client_uid.conn_idx, rep_msg);
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        private void OnCreateCharacter(PacketBase packet)
        {
            ws2gs.CreateCharacter msg = packet as ws2gs.CreateCharacter;

            ClientSession session = ClientSessionManager.Instance.GetSessionByAccount(msg.account_idx);
            if (session == null) return;

            gs2c.CreateCharacter rep_msg = PacketPools.Get(gs2c.msg.CREATE_CHARACTER) as gs2c.CreateCharacter;
            rep_msg.result = msg.result;
            rep_msg.char_idx = msg.char_idx;
            session.Send(rep_msg);
        }
    }
}
