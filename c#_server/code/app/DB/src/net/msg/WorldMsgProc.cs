using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 世界服消息处理
    /// @author hannibal
    /// @time 2017-8-16
    /// </summary>
    public class WorldMsgProc : ConnAppProc
    {
        public WorldMsgProc()
            : base()
        {
            m_msg_begin_idx = ProtocolID.MSG_BASE_WS2DB;
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
                ((PackBaseS2S)packet).server_uid.db_uid = ServerNetManager.Instance.srv_uid;

            return ServerNetManager.Instance.Send(m_conn_idx, packet);
        }
        protected override void RegisterHandle()
        {
            RegisterMsgProc(ws2db.msg.CHARACTER_INFO, OnCharacterInfo);
            RegisterMsgProc(ws2db.msg.LOGOUT_CLIENT, OnLogoutClient);
        }

        private void RegisterMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - m_msg_begin_idx);
            m_msg_proc[msg_id] = fun;
        }
        /// <summary>
        /// 收到广播服务器列表
        /// </summary>
        /// <param name="packet"></param>
        public void HandleAppServerList(PacketBase packet)
        {
        }
        /// <summary>
        /// 新服务器加入
        /// </summary>
        /// <param name="packet"></param>
        public void HandleAppServerAdd(PacketBase packet)
        {
        }
        /// <summary>
        /// 移除服务器
        /// </summary>
        /// <param name="packet"></param>
        public void HandleAppServerRemove(PacketBase packet)
        {
        }
        private void OnCharacterInfo(PacketBase packet)
        {

        }
        private void OnLogoutClient(PacketBase packet)
        {

        }
    }
}
