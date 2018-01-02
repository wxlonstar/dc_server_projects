﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// web 客户端socket
    /// @author hannibal
    /// @time 2016-7-30
    /// </summary>
    public sealed class WebNetConnecter : NetConnecter
    {
        private long m_conn_idx = 0;
        private WebClientSocket m_socket = null;
        private NetChannel m_channel = null;
        private MemoryStream m_send_stream = null;

        public WebNetConnecter()
        {
            m_send_stream = new MemoryStream();
        }

        public override void Setup()
        {
            base.Setup();
        }

        public override void Destroy()
        {
            if (m_socket != null)
            {//socket只有外部调用时才主动关闭，否则底层会先自己关闭
                m_socket.Close();
                m_socket = null;
            }
            base.Destroy();
        }
        public override void Close()
        {
            HanldeCloseConnect();
            base.Close();
        }

        public override void Update()
        {
            if (m_channel != null)
            {
                m_channel.Update();
            }
            base.Update();
        }

        public long Connect(string host, BaseNet.OnConnectedFunction connected, BaseNet.OnReceiveFunction receive, BaseNet.OnCloseFunction close)
        {
            OnConnected = connected;
            OnReceive = receive;
            OnClose = close;

            m_socket = new WebClientSocket();
            m_socket.OnOpen += OnAcceptConnect;
            m_socket.OnMessage += OnMessageReveived;
            m_socket.OnClose += OnConnectClose;
            m_socket.Connect(host);

            return m_conn_idx;
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

            m_socket.Send(m_send_stream.ToArray());
            return (int)m_send_stream.Length;
        }
        private void OnAcceptConnect(long conn_idx)
        {
            lock (ThreadScheduler.Instance.LogicLock)
            {
                m_channel = NetChannelPools.Spawn();
                m_channel.Setup(this, m_conn_idx);

                if (OnConnected != null) OnConnected(m_channel.conn_idx);
            }
        }
        private void OnMessageReveived(long conn_idx, byte[] by, int count)
        {
            lock (ThreadScheduler.Instance.LogicLock)
            {
                m_channel.HandleReceive(by, count);
            }
        }
        private void OnConnectClose(long conn_idx)
        {
            lock (ThreadScheduler.Instance.LogicLock)
            {
                this.Close();
            }
        }
        /// <summary>
        /// 关闭链接：底层通知
        /// </summary>
        private void HanldeCloseConnect()
        {
            if (m_channel != null)
            {
                m_channel.Destroy();
                NetChannelPools.Despawn(m_channel);
                m_channel = null;
            }
            if (OnClose != null)
            {
                OnClose(conn_idx);
                OnClose = null;
            }
        }

        public override bool Valid
        {
            get
            {
                if (m_socket == null) return false;
                return true;
            }
        }
        public long conn_idx
        {
            get { return m_conn_idx; }
            set { m_conn_idx = value; }
        }
    }
}
