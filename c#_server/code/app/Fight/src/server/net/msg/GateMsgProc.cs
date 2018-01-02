using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// client消息处理，通过网关转发
    /// @author hannibal
    /// @time 2016-8-16
    /// </summary>
    public class GateMsgProc : ConnAppProc
    {
        protected MsgProcFunction[] m_client_msg_proc = null;

        public GateMsgProc()
            : base()
        {
            m_msg_begin_idx = ProtocolID.MSG_BASE_GS2FS;
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
            //处理gate消息
            RegisterMsgProc(gs2fs.msg.PROXY_CLIENT_MSG, HandleProxyClientMsg);
            //处理客户端消息
            RegisterClientMsgProc(c2fs.msg.PING_NET, OnPingNet);
        }
        private void RegisterMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - m_msg_begin_idx);
            m_msg_proc[msg_id] = fun;
        }
        private void RegisterClientMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - ProtocolID.MSG_BASE_C2FS);
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
            if (header < ProtocolID.MSG_BASE_C2FS || header >= ProtocolID.MSG_BASE_C2FS + ProtocolID.MSG_APPLAYER_PER_INTERVAL)
            {
                Log.Debug("收到错误的消息ID:" + header);
                return;
            }

            PacketBase msg = PacketPools.Get(header);
            msg.Read(proxy_msg.data);

            //处理
            ushort msg_id = (ushort)(header - ProtocolID.MSG_BASE_C2FS);
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
        #region 处理客户端消息

        /// <summary>
        /// ping网络
        /// </summary>
        private void OnPingNet(PacketBase packet)
        {
            c2fs.PingNet msg = packet as c2fs.PingNet;

            long offset_time = Time.time - msg.tick;
            Log.Debug("收到第:" + msg.packet_id + " 时间:" + Time.time + " 延迟:" + offset_time);

            fs2c.PingNet rep_msg = PacketPools.Get(fs2c.msg.PING_NET) as fs2c.PingNet;
            rep_msg.packet_id = msg.packet_id;
            rep_msg.tick = msg.tick;
            rep_msg.flags = msg.flags;
            ServerNetManager.Instance.SendProxy(msg.client_uid, rep_msg);
        }
        #endregion
    }
}
