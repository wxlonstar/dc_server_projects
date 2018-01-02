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
            m_msg_begin_idx = ProtocolID.MSG_BASE_WS2FS;
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
                ((PackBaseS2S)packet).server_uid.fs_uid = ServerNetManager.Instance.srv_uid;

            return ServerNetManager.Instance.Send(m_conn_idx, packet);
        }
        protected override void RegisterHandle()
        {
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
            inner.AppServerList list_msg = packet as inner.AppServerList;
            for (int i = 0; i < list_msg.list.Count; ++i)
            {
                AppServerItem item = list_msg.list[i];
                Log.Info("收到广播服务器列表 type:" + item.srv_type + " sid:" + item.srv_uid);
                if (item.srv_type == eServerType.GATE)
                {
                    string ip = SocketUtils.IntToIp(item.srv_ip);
                    ServerNetManager.Instance.Connect2Server(ip, item.srv_port, item.srv_type, item.srv_uid);
                }
            }
        }
        /// <summary>
        /// 新服务器加入
        /// </summary>
        /// <param name="packet"></param>
        public void HandleAppServerAdd(PacketBase packet)
        {
            inner.AppServerAdd add_msg = packet as inner.AppServerAdd;
            Log.Info("收到新服务器 type:" + add_msg.app_info.srv_type + " sid:" + add_msg.app_info.srv_uid);
            AppServerItem item = add_msg.app_info;
            if (item.srv_type == eServerType.GATE)
            {
                string ip = SocketUtils.IntToIp(item.srv_ip);
                ServerNetManager.Instance.Connect2Server(ip, item.srv_port, item.srv_type, item.srv_uid);
            }
        }
        /// <summary>
        /// 移除服务器
        /// </summary>
        /// <param name="packet"></param>
        public void HandleAppServerRemove(PacketBase packet)
        {
            inner.AppServerRemove re_msg = packet as inner.AppServerRemove;
            Log.Info("收到服务器关闭 sid:" + re_msg.srv_uid);
        }
    }
}
