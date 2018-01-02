using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 单个服务器连接
    /// @author hannibal
    /// @time 2017-8-16
    /// </summary>
    public class ConnAppProc
    {
        protected long m_conn_idx;
        protected ushort m_msg_begin_idx = 0;
        protected AppServer m_srv_info = new AppServer();

        public delegate void MsgProcFunction(PacketBase packet);
        protected MsgProcFunction[] m_msg_proc = null;

        public ConnAppProc()
        {
            m_msg_proc = new MsgProcFunction[ProtocolID.MSG_APPLAYER_PER_INTERVAL];
        }

        public virtual void Setup()
        {
            for (int i = 0; i < ProtocolID.MSG_APPLAYER_PER_INTERVAL; ++i)
            {
                m_msg_proc[i] = null;
            }
            this.RegisterHandle();
        }
        public virtual void Destroy()
        {
            for (int i = 0; i < ProtocolID.MSG_APPLAYER_PER_INTERVAL; ++i)
            {
                m_msg_proc[i] = null;
            }
        }
        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public virtual int Send(PacketBase packet)
        {
            return 0;
        }
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="conn_idx"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public bool HandleMsg(long conn_idx, PacketBase packet)
        {
            ushort header = packet.header;
            if (header < m_msg_begin_idx || header >= m_msg_begin_idx + ProtocolID.MSG_APPLAYER_PER_INTERVAL)
                return false;

            ushort msg_id = (ushort)(header - m_msg_begin_idx);
            MsgProcFunction fun = m_msg_proc[msg_id];
            if (fun == null)
            {
                Log.Warning("未处理消息:" + (m_msg_begin_idx + msg_id));
                return false;
            }

            fun(packet);
            return true;
        }
        /// <summary>
        /// 注册消息处理函数
        /// </summary>
        protected virtual void RegisterHandle()
        {
        }
        public long conn_idx
        {
            get { return m_conn_idx; }
            set { m_conn_idx = value; }
        }
        public AppServer srv_info
        {
            get { return m_srv_info; }
        }
    }
}
