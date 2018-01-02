using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 战斗服消息处理
    /// @author hannibal
    /// @time 2016-8-16
    /// </summary>
    public class FightMsgProc : ConnAppProc
    {
        public FightMsgProc()
            : base()
        {
            m_msg_begin_idx = ProtocolID.MSG_BASE_FS2GS;
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
            RegisterMsgProc(fs2gs.msg.PROXY_WS_MSG, HandleReqProxyMsgToClient);
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
            if (header < ProtocolID.MSG_BASE_FS2C || header >= ProtocolID.MSG_BASE_FS2C + ProtocolID.MSG_APPLAYER_PER_INTERVAL)
            {
                Log.Debug("收到错误的消息ID:" + header);
                return;
            }
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
    }
}
