using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading; 
using System;

namespace dc
{
    /// <summary>
    /// 网络连接管道
    /// @author hannibal
    /// @time 2016-5-24
    /// </summary>
    public class NetChannel
    {
        protected long m_conn_idx = 0;
        protected BaseNet m_net_socket = null;
        protected ByteArray m_by_buffer = null;
        //由于收到数据时，已经通过lock全局锁同步到主线程，所以可以共用一个buffer
        static protected ByteArray m_dispatcher_buffer = new ByteArray(NetID.RecvPacketMaxSize, NetID.RecvPacketMaxSize);

        public NetChannel()
        {
            m_by_buffer = new ByteArray(NetID.InitByteArraySize, NetID.MaxByteArraySize);
        }

        public virtual void Setup(BaseNet socket, long conn_idx)
        {
            m_net_socket = socket;
            m_conn_idx = conn_idx;
        }

        public virtual void Destroy()
        {
            m_net_socket = null;
        }

        public virtual void Update()
        {
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        public virtual int Send(ByteArray by)
        {
            return 0;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        public void HandleReceive(byte[] by, int count)
        {
            if (by == null || by.Length == 0) return;

            m_by_buffer.WriteBytes(by, count);
            ParsePacket();
        }

        /// <summary>
        /// 解析数据包
        /// </summary>
        private byte[] head_by = new byte[NetID.PacketHeadSize];
        protected void ParsePacket()
        {
            while (m_by_buffer.Available >= NetID.PacketHeadSize)
            {
                if (m_by_buffer.Peek(ref head_by, NetID.PacketHeadSize))
                {
                    ushort msg_length = BitConverter.ToUInt16(head_by, 0);
                    if (m_by_buffer.Available >= msg_length + NetID.PacketHeadSize)
                    {
                        //读取包数据
                        m_by_buffer.Skip(NetID.PacketHeadSize);
                        ushort header = m_by_buffer.ReadUShort();
                        m_dispatcher_buffer.Clear();
                        int len = m_by_buffer.Read(m_dispatcher_buffer, msg_length - sizeof(ushort));
                        if (len != msg_length - sizeof(ushort))
                        {
                            Log.Error("读取错误");
                            m_by_buffer.Skip(msg_length - sizeof(ushort));//跳到下一个位置
                            continue;
                        }
                        //派发数据
                        if (m_net_socket != null && m_net_socket.OnReceive != null)
                        {
                            m_net_socket.OnReceive(m_conn_idx, header, m_dispatcher_buffer);
                        }
                    }
                    else
                        break;
                }
                else
                    break;
            }
        }

        public long conn_idx
        {
            get { return m_conn_idx; }
        }
    }
}