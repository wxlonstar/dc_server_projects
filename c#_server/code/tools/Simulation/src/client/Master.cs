using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace dc
{
    /// <summary>
    /// 管理器
    /// @author hannibal
    /// @time 2016-7-28
    /// </summary>
    public class Master : Singleton<Master>
    {
        private Timer m_timer;
        public void Setup()
        {
            ServerConfig.Read();
            Framework.Instance.Setup(Tick);

            ClientNetManager.Instance.Setup();
            UnitManager.Instance.Setup();
            this.RegisterEvent();
        }
        public void Destroy()
        {
            this.UnRegisterEvent();
            ClientNetManager.Instance.Destroy();
            UnitManager.Instance.Destroy();
        }
        public void Tick()
        {
            ClientNetManager.Instance.Tick();
            UnitManager.Instance.Tick();
        }

        public void Start()
        {
            m_timer = new Timer();
            m_timer.Interval = 10;
            m_timer.Tick += Update;
            m_timer.Start();
        }
        private long tmpLastTime = Time.time;
        private void Update(object sender, EventArgs e)
        {
            Framework.Instance.Update(Time.time - tmpLastTime);
            tmpLastTime = Time.time;
        }

        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～事件～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void RegisterEvent()
        {
            EventController.AddEventListener(ClientEventID.NET_CONNECTED_OPEN, OnGameEvent);
            EventController.AddEventListener(ClientEventID.NET_CONNECTED_CLOSE, OnGameEvent);
        }
        private void UnRegisterEvent()
        {
            EventController.RemoveEventListener(ClientEventID.NET_CONNECTED_OPEN, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.NET_CONNECTED_CLOSE, OnGameEvent);
        }
        private void OnGameEvent(GameEvent evt)
        {
            switch (evt.type)
            {
                case ClientEventID.NET_CONNECTED_OPEN:
                    {
                        DataManager.Instance.Setup();
                    }
                    break;

                case ClientEventID.NET_CONNECTED_CLOSE:
                    {
                        DataManager.Instance.Destroy();
                    }
                    break;
            }
        }
    }
}
