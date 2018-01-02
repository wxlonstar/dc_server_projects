using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace dc
{
    /// <summary>
    /// 登陆压力测试
    /// @author hannibal
    /// @time 2016-8-22
    /// </summary>
    public class LoginPressureManager : Singleton<LoginPressureManager>
    {
        private bool m_active = false;
        private Timer m_timer = null;
        private List<long> m_connectes = null;
        private sPressureLoginInfo m_pressure_info = null;
        public LoginPressureManager()
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
            if (m_active)
            {
                lock (ThreadScheduler.Instance.LogicLock)
                {
                    this.ReloginOne();
                    this.CloseOne();
                }
            }
        }
        private void Start()
        {
            Stop();
            m_active = true;
            m_timer = new Timer();
            m_timer.Interval = (int)(1000.0f * m_pressure_info.dis_conn_time);
            m_timer.Tick += Update;
            m_timer.Start();
        }
        private void Stop()
        {
            m_active = false;
            if (m_timer != null)
            {
                m_timer.Stop();
                m_timer = null;
            }
        }
        /// <summary>
        /// 重连一个
        /// </summary>
        private void ReloginOne()
        {
            if(m_connectes.Count < ServerConfig.net_info.login_client_count)
            {
                ClientNetManager.Instance.StartConnect(ServerConfig.net_info.login_server_ip, ServerConfig.net_info.login_server_port, 1);
            }
        }
        /// <summary>
        /// 关闭一个
        /// </summary>
        private void CloseOne()
        {
            if (m_connectes.Count == 0) return;

            long conn_idx = MathUtils.RandRange_List(m_connectes);
            ClientNetManager.Instance.CloseClient(conn_idx);
        }
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～事件～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void RegisterEvent()
        {
            EventController.AddEventListener(ClientEventID.NET_CONNECTED_OPEN, OnGameEvent);
            EventController.AddEventListener(ClientEventID.NET_CONNECTED_CLOSE, OnGameEvent);
            EventController.AddEventListener(ClientEventID.SWITCH_PRESSURE, OnGameEvent);
        }
        private void UnRegisterEvent()
        {
            EventController.RemoveEventListener(ClientEventID.NET_CONNECTED_OPEN, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.NET_CONNECTED_CLOSE, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.SWITCH_PRESSURE, OnGameEvent);
        }
        private void OnGameEvent(GameEvent evt)
        {
            switch (evt.type)
            {
                case ClientEventID.NET_CONNECTED_OPEN:
                    {
                        if (!m_active) break;
                        long conn_idx = evt.Get<long>(0);
                        if (!m_connectes.Contains(conn_idx))
                        {
                            m_connectes.Add(conn_idx);
                        }
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
                        if (type == ePressureType.Login && is_start)
                        {
                            m_pressure_info = evt.Get<sPressureLoginInfo>(2);
                            this.Start();
                        }
                        else
                        {
                            this.Stop();
                        }
                    }
                    break;
            }
        }
    }
}
