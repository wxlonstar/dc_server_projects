using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 链接socket管理器
    /// @author hannibal
    /// @time 2016-8-17
    /// </summary>
    public class NetConnectManager : Singleton<NetConnectManager>
    {
        private long m_share_conn_idx = 0;
        private Dictionary<long, NetConnecter> m_connectedes = null;

        public NetConnectManager()
        {
            m_connectedes = new Dictionary<long, NetConnecter>();
        }

        public void Setup()
        {

        }

        public void Destroy()
        {
            foreach (var obj in m_connectedes)
                obj.Value.Destroy();
            m_connectedes.Clear();
        }

        public void Tick()
        {
            foreach (var obj in m_connectedes)
            {
                obj.Value.Update();
            } 
            foreach (var obj in m_connectedes)
            {
                if (!obj.Value.Valid)//底层已经销毁
                {
                    obj.Value.Destroy();
                    m_connectedes.Remove(obj.Key);
                    break;
                }
            }
        }
        /// <summary>
        /// 连接主机
        /// </summary>
        public long ConnectTo(string ip, ushort port, BaseNet.OnConnectedFunction connected, BaseNet.OnReceiveFunction receive, BaseNet.OnCloseFunction close)
        {
            TCPNetConnecter socket = new TCPNetConnecter();
            socket.Setup();
            socket.conn_idx = ++m_share_conn_idx;
            m_connectedes.Add(socket.conn_idx, socket);
            //之所以延迟到下帧执行，可以防止上层逻辑还没有加入到集合，就已经连接成功返回
            TimerManager.Instance.AddNextFrame((timer_id, param) => { socket.Connect(ip, port, connected, receive, close); });
            return socket.conn_idx;
        }
        /// <summary>
        /// 连接web服
        /// </summary>
        public long ConnectTo(string host, BaseNet.OnConnectedFunction connected, BaseNet.OnReceiveFunction receive, BaseNet.OnCloseFunction close)
        {
            WebNetConnecter socket = new WebNetConnecter();
            socket.Setup();
            socket.conn_idx = ++m_share_conn_idx;
            m_connectedes.Add(socket.conn_idx, socket);
            socket.Connect(host, connected, receive, close);
            return socket.conn_idx;
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect(long conn_idx)
        {
            NetConnecter socket;
            if (m_connectedes.TryGetValue(conn_idx, out socket))
            {
                socket.Destroy();
            }
            m_connectedes.Remove(conn_idx);
        }
        public int Send(long conn_idx, ByteArray by)
        {
            NetConnecter socket;
            if(m_connectedes.TryGetValue(conn_idx, out socket))
            {
                return socket.Send(conn_idx, by);
            }
            return 0;
        }
        public NetConnecter GetConnecter(long conn_idx)
        {
            NetConnecter socket;
            if (m_connectedes.TryGetValue(conn_idx, out socket))
            {
                return socket;
            }
            return null;
        }
    }
}
