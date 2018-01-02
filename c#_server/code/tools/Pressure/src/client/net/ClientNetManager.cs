using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 网络管理器
    /// @author hannibal
    /// @time 2016-8-22
    /// </summary>
    public class ClientNetManager : Singleton<ClientNetManager>
    {
        private int m_send_msg_count = 0;
        private int m_recv_msg_count = 0;
        private int m_send_msg_size = 0;
        private int m_recv_msg_size = 0;
        private ByteArray m_send_by = null;
        private object m_sync_lock = new object();
        private List<long> m_sockets = new List<long>();

        public ClientNetManager()
        {
            m_send_by = NetUtils.AllocSendPacket();
        }

        public void Setup()
        {
            ProtocolID.RegisterPools();
        }
        public void Destroy()
        {
            CloseAll();
        }
        public void Tick()
        {
        }
        /// <summary>
        /// 开启连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="count">连接数量</param>
        public void StartConnect(string ip, ushort port, int count)
        {
            string host = "ws://" + ip + ":" + port;
            for(int i = 0; i < count; ++i)
            {
                NetConnectManager.Instance.ConnectTo(ip, port, OnAcceptConnect, OnMessageReveived, OnConnectClose);
                //NetConnectManager.Instance.ConnectTo(host, OnAcceptConnect, OnMessageReveived, OnConnectClose);
                System.Threading.Thread.Sleep(5);
            }
        }
        public void CloseAll()
        {
            lock (m_sync_lock)
            {
                while (m_sockets.Count > 0)
                {
                    NetConnectManager.Instance.Disconnect(m_sockets[0]);
                    System.Threading.Thread.Sleep(5);
                }
                m_sockets.Clear();
            }
            ResetRecvSendPacket();
        }
        /// <summary>
        /// 关闭单个连接
        /// </summary>
        /// <param name="conn_idx"></param>
        public void CloseClient(long conn_idx)
        {
            NetConnectManager.Instance.Disconnect(conn_idx);
        }
        /// <summary>
        /// 发包
        /// </summary>
        /// <param name="conn_idx"></param>
        /// <param name="packet"></param>
        public void Send(long conn_idx, PacketBase packet)
        {
            if (m_sockets.Contains(conn_idx))
            {                
                //包索引
                uint packet_idx = 0;
                if (packet is PackBaseC2S)
                {
                    packet_idx = PacketEncrypt.GetPacketIndex();
                    (packet as PackBaseC2S).packet_idx = PacketEncrypt.EncrpytPacketIndex(packet_idx, GlobalID.ENCRYPT_KEY);
                }

                m_send_by.Clear();
                m_send_by.WriteUShort(0);//先写入长度占位
                packet.Write(m_send_by);
                int data_len = m_send_by.Available - NetID.PacketHeadSize;
                m_send_by.ModifyUShort((ushort)data_len, 0);

                //数据有效性
                if (packet is PackBaseC2S)
                {
                    //包长2 + 协议头2 + 包索引id4 + 有效性校验2 = 10
                    ushort data_verify = PacketEncrypt.CalcPacketDataVerify(m_send_by.Buffer, 10, m_send_by.Available - 10, packet_idx, GlobalID.ENCRYPT_KEY);
                    m_send_by.ModifyUShort((ushort)data_verify, 8);
                }

                int size = NetConnectManager.Instance.Send(conn_idx, m_send_by);
                m_send_msg_count++;
                m_send_msg_size += size;
            }
            
            PacketPools.Recover(packet);
        }
        public void ResetRecvSendPacket()
        {
            m_send_msg_count = 0;
            m_recv_msg_count = 0;
            m_send_msg_size = 0;
            m_recv_msg_size = 0;
        }
        public int send_msg_count { get { return m_send_msg_count; } }
        public int recv_msg_count { get { return m_recv_msg_count; } }
        public int send_msg_size { get { return (int)(m_send_msg_size * 0.001f); } }
        public int recv_msg_size { get { return (int)(m_recv_msg_size * 0.001f); } }
        public int connect_count { get { return m_sockets.Count; } }

        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～事件～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void OnAcceptConnect(long conn_idx)
        {
            lock (m_sync_lock)
            {
                m_sockets.Add(conn_idx);
            }
            c2gs.EncryptInfo msg = PacketPools.Get(c2gs.msg.ENCRYPT) as c2gs.EncryptInfo;
            msg.version = 1;
            this.Send(conn_idx, msg);
            EventController.TriggerEvent(ClientEventID.NET_CONNECTED_OPEN, conn_idx);
        }
        private void OnConnectClose(long conn_idx)
        {
            lock (m_sync_lock)
            {
                m_sockets.Remove(conn_idx);
            }
            EventController.TriggerEvent(ClientEventID.NET_CONNECTED_CLOSE, conn_idx);
        }
        private void OnMessageReveived(long conn_idx, ushort header, ByteArray data)
        {
            m_recv_msg_count++;
            m_recv_msg_size += data.Available + NetID.PacketHeadSize + 2;
            EventController.TriggerEvent(ClientEventID.RECV_DATA, conn_idx, header, data);
        }
    }
}
