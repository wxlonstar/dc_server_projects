using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 网络管理器
    /// @author hannibal
    /// @time 2016-8-22
    /// </summary>
    public class ClientNetManager : Singleton<ClientNetManager>
    {
        private long m_cur_conn_idx = 0;
        private string m_user_name = "";
        private string m_user_psw = "";

        private ByteArray m_send_by = null;
        private ServerMsgProc m_msg_proc;

        public ClientNetManager()
        {
            m_send_by = NetUtils.AllocSendPacket();
            m_msg_proc = new ServerMsgProc();
        }

        public void Setup()
        {
            ProtocolID.RegisterPools();
            m_msg_proc.Setup();
        }
        public void Destroy()
        {
            Close();
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
        public void StartConnect(string ip, ushort port, string name, string psw)
        {
            m_user_name = name;
            m_user_psw = psw;

            string host = "ws://" + ip + ":" + port;

            NetConnectManager.Instance.ConnectTo(ip, port, OnAcceptConnect, OnMessageReveived, OnConnectClose);
            //NetConnectManager.Instance.ConnectTo(host, OnAcceptConnect, OnMessageReveived, OnConnectClose);
        }
        public void Close()
        {
            if(m_cur_conn_idx > 0)
            {
                NetConnectManager.Instance.Disconnect(m_cur_conn_idx);
                m_cur_conn_idx = 0;
            }
        }
        /// <summary>
        /// 发包
        /// </summary>
        /// <param name="conn_idx"></param>
        /// <param name="packet"></param>
        public void Send(PacketBase packet)
        {
            if (m_cur_conn_idx > 0)
            {
                //包索引
                uint packet_idx = 0;
                if (packet is PackBaseC2S)
                {
                    packet_idx = PacketEncrypt.GetPacketIndex();
                    (packet as PackBaseC2S).packet_idx = PacketEncrypt.EncrpytPacketIndex(packet_idx, GlobalID.ENCRYPT_KEY);
                }

                //数据写入stream
                m_send_by.Clear();
                m_send_by.WriteUShort(0);//先写入长度占位
                packet.Write(m_send_by);
                int data_len = m_send_by.Available - NetID.PacketHeadSize;//总长度
                m_send_by.ModifyUShort((ushort)data_len, 0);

                //数据有效性
                if(packet is PackBaseC2S)
                {
                    //包长2 + 协议头2 + 包索引id4 + 有效性校验2 = 10
                    ushort data_verify = PacketEncrypt.CalcPacketDataVerify(m_send_by.Buffer, 10, m_send_by.Available - 10, packet_idx, GlobalID.ENCRYPT_KEY);
                    m_send_by.ModifyUShort((ushort)data_verify, 8);
                }

                NetConnectManager.Instance.Send(m_cur_conn_idx, m_send_by);
            }

            PacketPools.Recover(packet);
        }
        public bool is_connected { get { return m_cur_conn_idx > 0; } }
        public long cur_conn_idx { get { return m_cur_conn_idx; } }
        public string user_name { get { return m_user_name; } }
        public string user_psw { get { return m_user_psw; } }

        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～事件～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void OnAcceptConnect(long conn_idx)
        {
            m_cur_conn_idx = conn_idx;

            c2gs.EncryptInfo msg = PacketPools.Get(c2gs.msg.ENCRYPT) as c2gs.EncryptInfo;
            msg.version = GlobalID.GetVersion();
            this.Send(msg);
            EventController.TriggerEvent(ClientEventID.NET_CONNECTED_OPEN, m_cur_conn_idx);
        }
        private void OnConnectClose(long conn_idx)
        {
            if (m_cur_conn_idx == conn_idx)
                m_cur_conn_idx = 0;
            EventController.TriggerEvent(ClientEventID.NET_CONNECTED_CLOSE, conn_idx);
        }
        private void OnMessageReveived(long conn_idx, ushort header, ByteArray data)
        {
            m_msg_proc.OnNetworkServer(conn_idx, header, data);
        }
    }
}
