using System;
using System.Collections.Generic;
using Fleck;
using System.IO;

namespace dc
{
    /// <summary>
    /// web 服务器socket
    /// @author hannibal
    /// @time 2016-7-30
    /// </summary>
    public sealed class WebNetAccepter : NetAccepter
    {
        private WebServerSocket m_socket = null;
        private Dictionary<long, NetChannel> m_channels = null;
        //发送是在主线程执行，只需要一块内存就行；接收需要针对每个端分配内存
        private static MemoryStream m_send_stream = new MemoryStream();

        public WebNetAccepter()
            : base()
        {
            m_channels = new Dictionary<long, NetChannel>();
        }

        public override void Setup()
        {
            base.Setup();
        }

        public override void Destroy()
        {
            base.Destroy();
        }
        public override void Close()
        {
            if (m_socket != null)
            {
                m_socket.Close();
                m_socket = null;
            }
            //需要放m_socket.Close()后，socket关闭时，内部回调的关闭事件HanldeCloseConnect不能正常执行
            foreach (var obj in m_channels)
            {
                obj.Value.Destroy();
                NetChannelPools.Despawn(obj.Value);
            }
            m_channels.Clear();
            base.Close();
        }
        public override void CloseConn(long conn_idx)
        {
            if (m_socket != null && conn_idx > 0)
            {
                m_socket.CloseConn(conn_idx);
            }
        }
        public override void Update()
        {
            foreach (var obj in m_channels)
            {
                obj.Value.Update();
            }

            base.Update();
        }

        public bool Listen(string host, BaseNet.OnAcceptFunction accept, BaseNet.OnReceiveFunction receive, BaseNet.OnCloseFunction close)
        {
            OnAccept = accept;
            OnReceive = receive;
            OnClose = close;

            m_socket = new WebServerSocket();
            m_socket.OnOpen += OnAcceptConnect;
            m_socket.OnMessage += OnMessageReveived;
            m_socket.OnClose += OnConnectClose;
            return m_socket.Start(host);
        }

        public override int Send(long conn_idx, ByteArray by)
        {
            base.Send(conn_idx, by);

            if (m_socket == null) return 0;

            if (by.Available >= NetID.SendPacketMaxSize)
            {
                by.Skip(NetID.PacketHeadSize);
                ushort header = by.ReadUShort();
                Log.Error("发送数据量过大:" + header);
                return 0;
            }
            int data_len = by.Available - NetID.PacketHeadSize;
            by.ModifyUShort((ushort)data_len, 0);
            m_send_stream.SetLength(0);
            m_send_stream.Write(by.GetBuffer(), 0, (int)by.Available);//数据内容

            m_socket.Send(conn_idx, m_send_stream.ToArray());
            return (int)m_send_stream.Length;
        }
        private void OnAcceptConnect(long conn_idx)
        {
            lock (ThreadScheduler.Instance.LogicLock)
            {
                NetChannel channel = NetChannelPools.Spawn();
                channel.Setup(this, conn_idx);
                m_channels.Add(channel.conn_idx, channel);

                if (OnAccept != null) OnAccept(conn_idx);
            }
        }
        private void OnMessageReveived(long conn_idx, byte[] by, int count)
        {
            lock (ThreadScheduler.Instance.LogicLock)
            {
                NetChannel channel;
                if (m_channels.TryGetValue(conn_idx, out channel))
                {
                    channel.HandleReceive(by, count);
                }
            }
        }
        private void OnConnectClose(long conn_idx)
        {
            lock (ThreadScheduler.Instance.LogicLock)
            {
                this.HanldeCloseConnect(conn_idx);
            }
        }
        /// <summary>
        /// 关闭链接：底层通知
        /// </summary>
        private void HanldeCloseConnect(long conn_idx)
        {
            NetChannel channel;
            if (m_channels.TryGetValue(conn_idx, out channel))
            {
                channel.Destroy();
                NetChannelPools.Despawn(channel);
            }
            m_channels.Remove(conn_idx);
            if (OnClose != null)
            {
                OnClose(conn_idx);
            }
        }
        public override string GetConnIP(long conn_idx)
        {
            return "";
        }
        public override ushort GetConnPort(long conn_idx)
        {
            return 0;
        }
        public override bool Valid
        {
            get
            {
                if (m_socket == null) return false;
                return true;
            }
        }
    }
}
