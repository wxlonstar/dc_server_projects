using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace dc
{
    /// <summary>
    /// 网络压力测试
    /// @author hannibal
    /// @time 2016-8-22
    /// </summary>
    public class NetPressureManager : Singleton<NetPressureManager>
    {
        private bool m_active = false;
        private bool m_had_recv_encrypt = false;
        private Timer m_timer = null;
        private List<long> m_connectes = null;
        private sPressureNetInfo m_pressure_info = null;
        public NetPressureManager()
        {
            m_connectes = new List<long>();
        }

        public void Setup()
        {
            m_active = false;
            this.RegisterEvent();
        }
        public void Destroy()
        {
            this.UnRegisterEvent();
            m_connectes.Clear();
        }
        public void Tick()
        {
        }
        private void Update(object sender, EventArgs e)
        {
            if (m_active && m_had_recv_encrypt)
            {
                lock(ThreadScheduler.Instance.LogicLock)
                {
                    this.SendAll();
                }
            }
        }
        private void Start()
        {
            m_active = true;
            m_had_recv_encrypt = false;
            Stop();
            m_timer = new Timer();
            m_timer.Interval = 1000/m_pressure_info.send_count_per_second;
            m_timer.Tick += Update;
            m_timer.Start();
        }
        private void Stop()
        {
            m_active = false;
            if(m_timer != null)
            {
                m_timer.Stop();
                m_timer = null;
            }
        }
        private void SendAll()
        {
            foreach(var obj in m_connectes)
            {
                c2gs.RobotTest packet = PacketPools.Get(c2gs.msg.ROBOT_TEST) as c2gs.RobotTest;
                packet.length = m_pressure_info.send_size_per_packet;
                packet.flags = (uint)eServerType.SERVER | (uint)eServerType.WORLD;
                ClientNetManager.Instance.Send(obj, packet);
            }
        }
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～事件～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void RegisterEvent()
        {
            EventController.AddEventListener(ClientEventID.NET_CONNECTED_OPEN, OnGameEvent);
            EventController.AddEventListener(ClientEventID.NET_CONNECTED_CLOSE, OnGameEvent);
            EventController.AddEventListener(ClientEventID.SWITCH_PRESSURE, OnGameEvent);
            EventController.AddEventListener(ClientEventID.RECV_DATA, OnGameEvent);
        }
        private void UnRegisterEvent()
        {
            EventController.RemoveEventListener(ClientEventID.NET_CONNECTED_OPEN, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.NET_CONNECTED_CLOSE, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.SWITCH_PRESSURE, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.RECV_DATA, OnGameEvent);
        }
        private void OnGameEvent(GameEvent evt)
        {
            switch (evt.type)
            {
                case ClientEventID.NET_CONNECTED_OPEN:
                    {
                        if (!m_active) break;
                        long conn_idx = evt.Get<long>(0);
                        if (!m_connectes.Contains(conn_idx)) m_connectes.Add(conn_idx);
                    }
                    break;
                case ClientEventID.NET_CONNECTED_CLOSE:
                    {
                        if (!m_active) break;
                        long conn_idx = evt.Get<long>(0);
                        m_connectes.Remove(conn_idx);
                    }
                    break;
                case ClientEventID.SWITCH_PRESSURE:
                    {
                        ePressureType type = evt.Get<ePressureType>(0);
                        bool is_start = evt.Get<bool>(1);
                        if (type == ePressureType.Net && is_start)
                        {
                            m_pressure_info = evt.Get<sPressureNetInfo>(2);
                            this.Start();
                        }
                        else
                        {
                            this.Stop();
                        }
                    }
                    break;
                case ClientEventID.RECV_DATA:
                    {
                        if (!m_active) break;
                        long conn_idx = evt.Get<long>(0);
                        ushort header = evt.Get<ushort>(1);
                        ByteArray data = evt.Get<ByteArray>(2);
                        if(header == gs2c.msg.ENCRYPT)
                        {
                            m_had_recv_encrypt = true;
                            PacketBase packet = PacketPools.Get(header);
                            packet.Read(data);
                            GlobalID.ENCRYPT_KEY = (packet as gs2c.EncryptInfo).key;
                            PacketPools.Recover(packet);
                        }
                    }
                    break;
            }
        }
    }
}
